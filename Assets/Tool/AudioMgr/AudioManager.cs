using System;
using System.Collections.Generic;
using Tool.ResourceMgr;
using Tool.Utilities;
using Tool.Utilities.Events;
using UnityEngine;
using JsonUtility = Tool.Utilities.Save.JsonUtility;
using Object = UnityEngine.Object;

namespace Tool.AudioMgr
{
    public enum EAudioType
    {
        Effect,
        Bgm
    }

    public class AudioManager : Single.Singleton<AudioManager>
    {
        private AudioClip _audioClip = null;
        
        private GameObject _audioMgr;
        private GameObject AudioMgr
        {
            get
            {
                if (!_audioMgr) Init();
                return _audioMgr;
            }
            set => _audioMgr = value;
        }

        private const string AudioFileName = "audioInfo.txt";
        private const string AudioClipPath = "Audio/";

        private readonly Queue<AudioSource> _closeSourcesQueue = new Queue<AudioSource>();
        private readonly List<AudioSource> _openSourcesList = new List<AudioSource>();
        private AudioSource _bgmAudioSource;

        private readonly ValueBindery<float> _effectAudioVolume = new ValueBindery<float>(1f);
        private readonly ValueBindery<float> _bgmAudioVolume = new ValueBindery<float>(1f);
        private readonly ValueBindery<bool> _isMute = new ValueBindery<bool>(false);

        protected override void OnInit()
        {
            //初始化
            Init();

            //绑定音量修改事件
            _effectAudioVolume.OnRegister((value) => SetupVolume());
            _bgmAudioVolume.OnRegister((value) => SetupVolume());
            _isMute.OnRegister((value) => SetupVolume());

            //加载音量
            LoadAudioInfo();
        }

        private void Init()
        {
            //生成挂载的GameObject，过场景不销毁
            AudioMgr = new GameObject("AudioManage");
            Object.DontDestroyOnLoad(AudioMgr);
        }

        private void LoadAudioInfo()
        {
            string readStr = JsonUtility.ReadStrFromFile(AudioFileName);
            if (readStr == null) return;
            
            var audioInfos = readStr.Split('_');
            //设置音效音量
            var volumeInfos = audioInfos[0].Split(':');
            var effectVolume = float.Parse(volumeInfos[1]);
            ModifyVolume(EAudioType.Effect, effectVolume);

            //设置背景音量
            volumeInfos = audioInfos[1].Split(':');
            var bgmVolume = float.Parse(volumeInfos[1]);
            ModifyVolume(EAudioType.Bgm, bgmVolume);
        }

        private void SaveAudioInfo()
        {
            //拼接字符串
            string saveStr = "";
            saveStr = "effect:" + _effectAudioVolume + "_" + "bgm:" + _bgmAudioVolume;
            //存入文件
            JsonUtility.WriteStrToFile(saveStr, AudioFileName);
        }

        public void PlayAudio(string name, EAudioType eAudioType = EAudioType.Effect, bool isLoop = false)
        {
            ResMgr.GetInstance().AsyncLoad<AudioClip>(AudioClipPath + name, (value) =>
            {
                _audioClip = value;
                if (!_audioClip) throw new Exception("找不到" + name + "的音频");

                AudioSource audioSource;
                if (eAudioType == EAudioType.Bgm)
                {
                    if (_bgmAudioSource)
                    {
                        if (_bgmAudioSource.clip.name.Equals(_audioClip.name) && _bgmAudioSource.isPlaying) return;
                        audioSource = _bgmAudioSource;
                    }
                    else
                    {
                        audioSource = AudioMgr.AddComponent<AudioSource>();
                        _bgmAudioSource = audioSource;
                    }

                    audioSource.clip = _audioClip;
                    audioSource.volume = _bgmAudioVolume.Value;
                    audioSource.loop = isLoop;
                    audioSource.Play();
                }
                else
                {
                    //查询空队列有无可用Source
                    if (_closeSourcesQueue.Count == 0)
                    {
                        //遍历开始队列，将已经播放完的Source返回空队列
                        for (var i = 0; i < _openSourcesList.Count; i++)
                        {
                            if (_openSourcesList[i].isPlaying) continue;
                            _closeSourcesQueue.Enqueue(_openSourcesList[i]);
                            _openSourcesList.Remove(_openSourcesList[i]);
                        }

                        //如果空队列依然为空，则新建一个Source
                        if (_closeSourcesQueue.Count == 0)
                        {
                            audioSource = AudioMgr.AddComponent<AudioSource>();
                            audioSource.clip = _audioClip;
                            audioSource.volume = _effectAudioVolume.Value;
                            audioSource.loop = isLoop;
                            audioSource.Play();

                            //加入开始队列
                            _openSourcesList.Add(audioSource);
                        }
                        else
                        {
                            audioSource = _closeSourcesQueue.Dequeue();
                            audioSource.clip = _audioClip;
                            audioSource.volume = _effectAudioVolume.Value;
                            audioSource.loop = isLoop;
                            audioSource.Play();

                            //加入开始队列
                            _openSourcesList.Add(audioSource);
                        }
                    }
                    //空队列有Source的话，就直接获取
                    else
                    {
                        audioSource = _closeSourcesQueue.Dequeue();
                        audioSource.clip = _audioClip;
                        audioSource.volume = _effectAudioVolume.Value;
                        audioSource.loop = isLoop;
                        audioSource.Play();

                        //加入开始队列
                        _openSourcesList.Add(audioSource);
                    }
                }
            });
        }

        public void StopAudio(string name)
        {
            var audioSource = _openSourcesList.Find(item => item.name == name);
            if (audioSource)
            {
                audioSource.Stop();
                return;
            }

            if (_bgmAudioSource.clip.name == name) _bgmAudioSource.Stop();
        }

        public void IsMute(bool value)
        {
            foreach (var effectAudioSource in _openSourcesList)
            {
                if (value) effectAudioSource.volume = 0;
                else effectAudioSource.volume = _effectAudioVolume.Value;
            }

            if (value && _bgmAudioSource) _bgmAudioSource.volume = 0;
            else if (!value && _bgmAudioSource) _bgmAudioSource.volume = _bgmAudioVolume.Value;
        }

        public void ModifyVolume(EAudioType eAudioType, float value)
        {
            switch (eAudioType)
            {
                case EAudioType.Effect:
                    _effectAudioVolume.Value = value;
                    break;
                case EAudioType.Bgm:
                    _bgmAudioVolume.Value = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(eAudioType), eAudioType, null);
            }
        }

        private void SetupVolume()
        {
            //设置音量值
            foreach (var effectAudioSource in _openSourcesList)
                effectAudioSource.volume = _effectAudioVolume.Value;
            if (_bgmAudioSource) _bgmAudioSource.volume = _bgmAudioVolume.Value;

            //保存
            SaveAudioInfo();
        }

        public float GetVolume(EAudioType eAudioType)
        {
            return eAudioType switch
            {
                EAudioType.Effect => _effectAudioVolume.Value,
                EAudioType.Bgm => _bgmAudioVolume.Value,
                _ => throw new ArgumentOutOfRangeException(nameof(eAudioType), eAudioType, null)
            };
        }
    }
}