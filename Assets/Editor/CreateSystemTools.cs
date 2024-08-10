using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class CreateSystemTools : EditorWindow
    {
        private static CreateSystemTools _panel;
        private static string _systemName;

        [MenuItem("QUFramework/生成一级模块")]
        public static void CreateSystem()
        {
            //返回当前屏幕上第一个 t 类型的 EditorWindow，utility参数为是否浮动窗口
            _panel = GetWindowWithRect<CreateSystemTools>(new Rect(0, 0, 360, 90), false, "生成系统模块");
            _panel.Show(); //默认打开
        }

        [MenuItem("QUFramework/注册模块")]
        private static void RegisterSystem()
        {
            string systemPath = Directory.GetCurrentDirectory() + @"\Assets\GameSystem\";
            string globalCSharp = Directory.GetCurrentDirectory() + @"\Assets\Framework\Global.cs";
            StringBuilder globalStr = new StringBuilder();
            StringBuilder moduleStr = new StringBuilder();
            foreach (var value in Directory.GetDirectories(systemPath))
            {
                int index = value.LastIndexOf('\\');
                var moduleName = value.Substring(index + 1);
                if (moduleName.Equals("MVCTemplate")) continue;
                globalStr.Append($"using GameSystem.{moduleName};\n");
                moduleStr.Append($"\n\t\t\tthis.RegisterModule<I{moduleName}Module>(new {moduleName}Module());");
            }

            globalStr.Append(@"namespace Framework
{
    public class Global : FrameworkMgr<Global>
    {
        protected override void OnInitModule()
        {");

            globalStr.Append(moduleStr);
            globalStr.Append(@"
        }
    }
}");
            //替换Global
            try
            {
                if (File.Exists(globalCSharp)) File.Delete(globalCSharp);
            }
            catch (Exception e)
            {
                throw new Exception("删除Global失败：" + e);
            }

            try
            {
                File.WriteAllText(globalCSharp, globalStr.ToString());
            }
            catch (Exception e)
            {
                throw new Exception("生成Global失败：" + e);
            }


            foreach (var moduleName in Directory.GetDirectories(systemPath))
            {
                if (moduleName.Contains("MVCTemplate")) continue;
                Debug.Log(moduleName);
            }

            Debug.Log("生成Global文件，注册模块成功:" + globalCSharp);
            AssetDatabase.Refresh();
        }

        private void OnGUI()
        {
            //文本框
            GUI.Label(new Rect(10, 20, 100, 20), "模块名称：");
            _systemName = GUI.TextField(new Rect(80, 20, 200, 20), _systemName);
            //确认按钮
            if (GUI.Button(new Rect(10, 50, 120, 30), "生成模块"))
                CreateFile();
        }


        private static void CreateFile()
        {
            //D:\UnityProjects\QUFrameWork
            string systemPath = Directory.GetCurrentDirectory() + @"\Assets\GameSystem\" + _systemName + @"\";

            //判断有无该目录，无则创建
            if (Directory.Exists(systemPath))
            {
                throw new Exception($"该目录已存在{systemPath}");
            }

            try
            {
                Directory.CreateDirectory(systemPath); //创建系统根目录
                Directory.CreateDirectory(systemPath + @"\Main");
                Directory.CreateDirectory(systemPath + @"\OtherView");
                Directory.CreateDirectory(systemPath + @"\Resources");
            }
            catch (Exception e)
            {
                throw new Exception("生成目录失败：" + e);
            }

            //生成View
            var viewContent = @"using Framework;
using GameSystem.MVCTemplate;
using UnityEngine.UI;

namespace GameSystem.CounterSystem.Main
{
    public class CounterSystemView : BaseView
    {
        /// <summary>
        /// 初始化UI组件
        /// </summary>
        protected override void InitUI()
        {
        }

        /// <summary>
        /// 绑定model回调事件
        /// </summary>
        protected override void SetModelCallback()
        {
        }

        protected override void OnShow()
        {
        }

        protected override void OnHide()
        {
        }
    }
}";
            var newViewContent = viewContent.Replace("CounterSystem", _systemName);
            try
            {
                File.WriteAllText(systemPath + "Main/" + _systemName + "View.cs", newViewContent);
            }
            catch (Exception e)
            {
                throw new Exception("生成View失败：" + e);
            }

            //生成Model
            string modelContent = @"using GameSystem.MVCTemplate;

namespace GameSystem.CounterSystem.Main
{
    public class CounterSystemModel : BaseModel
    {
        protected override void OnInit()
        {
        }

        /// <summary>
        /// 监听某些数据更改事件,可以通知view更新
        /// </summary>
        public override void BindListener()
        {
        }

        /// <summary>
        /// 移除事件
        /// </summary>
        public override void RemoveListener()
        {
        }
    }
}";
            var newModelContent = modelContent.Replace("CounterSystem", _systemName);
            try
            {
                File.WriteAllText(systemPath + "Main/" + _systemName + "Model.cs", newModelContent);
            }
            catch (Exception e)
            {
                throw new Exception("生成Model失败：" + e);
            }

            //生成Ctrl
            string ctrlContent = @"using GameSystem.MVCTemplate;
using Tool.UI;
using UnityEngine;

namespace GameSystem.CounterSystem.Main
{
    public class CounterSystemCtrl : BaseCtrl
    {
        public CounterSystemCtrl() : base(new CounterSystemModel(),
            UIManager.GetInstance().OpenPanelSync(""CounterSystemView"", EuiLayer.Mid).GetComponent<BaseView>(),
            EuiLayer.Mid)
        {
        }

        /// <summary>
        /// 处理部分view的业务
        /// </summary>
        protected override void InitListener()
        {
            Debug.Log(""InitListener"");
        }

        /// <summary>
        /// 展示主要view
        /// </summary>
        public void OnShowView() => OnOpen(""CounterSystemView"", EuiLayer.Mid);

        /// <summary>
        /// 关闭视图
        /// </summary>
        public void OnHideView() => OnClose();

        /// <summary>
        /// view展示完毕回调函数
        /// </summary>
        protected override void OnCompleteShow()
        {
            Debug.Log(""展示view完毕"");
        }

        /// <summary>
        /// view关闭完毕回调函数
        /// </summary>
        protected override void OnCompleteClose()
        {
            Debug.Log(""关闭view完毕"");
        }

        public CounterSystemModel GetModel() => Model as CounterSystemModel;
        public CounterSystemView GetView() => View as CounterSystemView;
    }
}";
            var newCtrlContent = ctrlContent.Replace("CounterSystem", _systemName);
            try
            {
                File.WriteAllText(systemPath + "Main/" + _systemName + "Ctrl.cs", newCtrlContent);
            }
            catch (Exception e)
            {
                throw new Exception("生成Ctrl失败：" + e);
            }

            //创建系统cs
            string systemContent = @"using Framework;
using GameSystem.CounterSystem.Main;

namespace GameSystem.CounterSystem
{
    public interface ICounterSystemModule : IModule
    {
        public void ShowView();
        public void HideView();
    }

    public class CounterSystemModule : AbsModule, ICounterSystemModule
    {
        private CounterSystemCtrl _ctrl;

        protected override void OnInit()
        {
        }

        public void ShowView()
        {
            if (_ctrl == null) _ctrl = new CounterSystemCtrl();
            else _ctrl.OnShowView();
        }

        public void HideView()
        {
            _ctrl.OnHideView();
        }
    }
}";
            var newSystemContent = systemContent.Replace("CounterSystem", _systemName);
            try
            {
                File.WriteAllText(systemPath + _systemName + "Module.cs", newSystemContent);
            }
            catch (Exception e)
            {
                throw new Exception("生成System失败：" + e);
            }

            AssetDatabase.Refresh();
            _panel.Close();
        }
    }
}