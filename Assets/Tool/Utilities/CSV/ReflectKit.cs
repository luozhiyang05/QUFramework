using System;
using System.Reflection;

namespace Tool.Utilities.CSV
{
    public static class ReflectKit
    {
        /// <summary>
        /// 读取CSV后赋值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fieldInfo"></param>
        /// <param name="obj"></param>
        /// <exception cref="Exception"></exception>
        public static void SetValue(string value, FieldInfo fieldInfo, object obj,bool isEnum = false)
        {
            try
            {
                //判断该字段是否是布尔类型
                if (fieldInfo.FieldType == typeof(bool))
                {
                    if (bool.TryParse(value, out bool boolValue))
                    {
                        fieldInfo.SetValue(obj, boolValue);
                    }
                    else if (byte.TryParse(value, out byte byteValue))
                    {
                        fieldInfo.SetValue(obj, byteValue == 1);
                    }
                }
                else
                {
                    //处理枚举
                    if (isEnum)
                    {
                        fieldInfo.SetValue(obj, Convert.ChangeType(Int32.Parse(value), fieldInfo.FieldType));
                    }
                    else
                    {
                        fieldInfo.SetValue(obj, Convert.ChangeType(value, fieldInfo.FieldType));
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        
        public static void SetValue(object value, FieldInfo fieldInfo, object obj)
        {
            try
            {
                fieldInfo.SetValue(obj, value);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// 深拷贝
        /// </summary>
        /// <param name="obj">要拷贝的对象</param>
        /// <typeparam name="T">要拷贝的对象的类型</typeparam>
        /// <returns></returns>
        public static T DeepCopyByReflection<T>(T obj)
        {
            if (obj is string || obj.GetType().IsValueType)
                return obj;

            object newObj = Activator.CreateInstance(obj.GetType());
            FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Public |
                                                         BindingFlags.NonPublic |
                                                         BindingFlags.Static |
                                                         BindingFlags.Instance);
            foreach (var field in fields)
            {
                try
                {
                    field.SetValue(newObj, DeepCopyByReflection(field.GetValue(obj)));
                }
                catch
                {
                }
            }

            return (T)newObj;
        }


        /// <summary>
        /// 读取配置类的信息到数据类中
        /// </summary>
        /// <param name="info"></param>
        /// <param name="config"></param>
        /// <param name="flags"></param>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="T"></typeparam>
        public static void SetValue<V, T>(V info, T config, BindingFlags flags)
        {
            Type infoType = typeof(V);
            Type configType = typeof(T);

            FieldInfo[] infoFieldInfos = infoType.GetFields(flags);
            FieldInfo[] configFieldInfos = configType.GetFields(flags);

            for (var i = 0; i < configFieldInfos.Length; i++)
            {
                for (var j = 0; j < infoFieldInfos.Length; j++)
                {
                    if (infoFieldInfos[j].Name == configFieldInfos[i].Name)
                    {
                        object obj = configFieldInfos[i].GetValue(config);
                        infoFieldInfos[j].SetValue(info, obj);
                        break;
                    }
                }
            }
        }
    }
}