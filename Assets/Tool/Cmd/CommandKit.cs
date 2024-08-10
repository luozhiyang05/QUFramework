using System;
using System.Collections.Generic;
using Tool.Single;
using UnityEngine;

namespace Tool.Cmd
{
    public interface ICommand
    {
        void Do();
        void UnDo();
    }

    public abstract class AbsCommand : ICommand
    {
        public virtual void Do()
        {
        }

        public virtual void UnDo()
        {
        }
    }

    public class CommandKit : Singleton<CommandKit>
    {
        private AbsCommand[] _absCommands;

        private int _maxSize;
        private int _maxCount;
        private int _tailIndex;
        private int _headIndex;

        protected override void OnInit()
        {
            _maxSize = 10;
            _maxCount = 5;
            _tailIndex = 0;
            _headIndex = 0;
            _absCommands = new AbsCommand[_maxSize];
        }

        public void SendCmd<T>() where T : AbsCommand, new()
        {
            var absCommand = new T();
            absCommand.Do();

            //判断是否需要扩容
            if (_headIndex + 1 == _maxSize)
            {
                //扩容
                var length = _headIndex - _tailIndex + 1;
                Array.Copy(_absCommands, _tailIndex, _absCommands, 0, length);
                _tailIndex = 0;
                _headIndex = length - 1;
            }
            
            //加入命令队列
            _absCommands[_headIndex] = absCommand;
            if (_headIndex - _tailIndex == _maxCount) _tailIndex++;
            _headIndex++;
        }

        public void RevokeCmd()
        {
            if(_headIndex>_tailIndex)
                _absCommands[--_headIndex].UnDo();
        }
    }
}