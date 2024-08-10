using System;
using Tool.ResourceMgr;
using Tool.Utilities;
using UnityEngine;

namespace Tool.OpenGL
{
    public enum GL_Mode
    {
        Screen,
        ViewPort,
        World
    }

    public class GLDraw : MonoBehaviour
    {
        [SerializeField] private Material _material;
        public Color Color;
        public int SideCount;
        public float Radius;
        public bool IsRender;

        public void SetValue(Color color, int sideCount, float radius)
        {
            Color = color;
            SideCount = sideCount;
            Radius = radius;
            ResMgr.GetInstance().AsyncLoad<Material>("Material/GLMaterial", (value) =>
            {
                _material = value;
                IsRender = true;
            });
        }


        /// <summary>
        /// 生成正多边形坐标数组
        /// </summary>
        /// <param name="centerPos"></param>
        /// <param name="sidesCount"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        private Vector2[] GenerateRegularPolygons(Vector2 centerPos, int sidesCount, float radius)
        {
            var rad = 2 * Mathf.PI / sidesCount;
            Vector2[] arr = new Vector2[sidesCount];
            for (int i = 0; i < sidesCount; i++)
            {
                var f = -rad * i; //符号顺时针画点
                float y = Mathf.Cos(f) * radius;
                float x = Mathf.Sin(f) * radius;
                arr[i] = new Vector2(x, y) + centerPos;
            }

            return arr;
        }

        /// <summary>
        /// 开始画线
        /// </summary>
        /// <param name="points">坐标数组</param>
        /// <param name="isClose">是否闭合</param>
        /// <param name="z"></param>
        private void DrawLines(Vector2[] points, bool isClose = true, float z = 0)
        {
            GL.Begin(GL.LINE_STRIP);
            foreach (Vector2 point in points)
            {
                GL.Vertex3(point.x, point.y, z);
            }

            if (isClose) GL.Vertex3(points[0].x, points[0].y, z);
            GL.End();
        }

        //设置颜色
        private void SetColor(Color color)
        {
            //1:先设置材质颜色和渲染管道
            _material.color = color;
            _material.SetPass(0);
        }

        /// <summary>
        /// 渲染准备
        /// </summary>
        /// <param name="glMode">渲染坐标模式</param>
        /// <param name="isSetParent">是否以当前Go为父对象</param>
        /// <param name="call">具体绘画函数</param>
        private void Render(GL_Mode glMode, bool isSetParent, Action call)
        {
            if (call == null) return;

            GL.PushMatrix();
            //设置世界坐标
            switch (glMode)
            {
                case GL_Mode.Screen:
                    //屏幕坐标
                    GL.LoadPixelMatrix();
                    break;
                case GL_Mode.ViewPort:
                    //视口坐标
                    GL.LoadOrtho();
                    break;
                //什么都不设置就是世界坐标
            }

            //以当前gameObject对象为父对象,或者用当前gameObject为原点画线
            if (isSetParent)
            {
                GL.MultMatrix(transform.localToWorldMatrix);
            }

            //具体绘画
            call.Invoke();

            GL.PopMatrix();
        }


        /// <summary>
        /// 绘画函数
        /// </summary>
        private void OnDraw()
        {
            //返回正多边形坐标数组
            var arr = GenerateRegularPolygons(transform.position, SideCount, Radius);
            if (arr.Length >= 3)
            {
                SetColor(Color);
                DrawLines(arr, true);
            }
        }

        //在完成所有常规场景渲染后调用此函数。此时，可使用GL类或Graphics.DrawMeshNow绘制自定义几何图形。
        private void OnRenderObject()
        {
            if(!IsRender) return;
            Render(GL_Mode.World, false, OnDraw);
        }
    }
}