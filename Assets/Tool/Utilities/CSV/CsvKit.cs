using System;
using System.Reflection;
using UnityEngine;

namespace Tool.Utilities.CSV
{
    public static class CsvKit
    {
        public static void Read<T>(TextAsset readAsset, int rowIndex, out T obj, BindingFlags flags,
            Action<T> callBack = null) where T : class
        {
            obj = default;
            int readRowIndex = 0;
            Type objType = typeof(T);
            //读取Csv
            string csvInfo = readAsset.ToString();
            //分割行
            string[] rows = csvInfo.Split('\n');
            Debug.LogWarning(rows.Length);
            //获取字段名的行，第一行为存字段名的
            string[] fields = rows[0].Split(',');
            //寻找要读取的行
            for (int i = 1; i < rows.Length; i++)
            {
                if (int.Parse(rows[i].Split(',')[0]) == rowIndex)
                {
                    readRowIndex = i;
                    break;
                }
            }

            //动态创建T
            obj = Activator.CreateInstance<T>();

            //遍历读取的行数据
            string[] colSplit = rows[readRowIndex].Split(',');
            for (var i = 0; i < colSplit.Length; i++)
            {
                //读取根据该数据所在的列索引值获取 字段名
                string fieldName = fields[i].Trim();
                FieldInfo fieldInfo = objType.GetField(fieldName, flags);

                //获取数据
                string data = colSplit[i];

                //写入数据类中
                ReflectKit.SetValue(data, fieldInfo, obj);
            }

            //执行回调函数
            callBack?.Invoke(obj);
        }

        public static void Read<T>(TextAsset readAsset, BindingFlags flags, Action<T> callBack = null) where T : class
        {
            int readRowIndex = 0;
            Type objType = typeof(T);
            //读取Csv
            string csvInfo = readAsset.ToString();
            //分割行
            string[] rows = csvInfo.Split('\n');
            //获取字段名的行，第一行为存字段名的
            string[] fieldNames = rows[0].Split(',');
            string[] fieldTypes = rows[1].Split(',');
            //寻找要读取的行
            for (int i = 2; i < rows.Length - 1; i++)
            {
                readRowIndex = i;

                //动态创建T
                T obj = Activator.CreateInstance<T>();

                //遍历读取的行数据
                string[] valueSplit = rows[readRowIndex].Split(',');
                for (var j = 0; j < valueSplit.Length; j++)
                {
                    //获取该列的字段名和类型
                    string fieldName = fieldNames[j].Trim();
                    string typeName = fieldTypes[j].Trim();

                    FieldInfo fieldInfo = objType.GetField(fieldName, flags);
                    if (fieldInfo == null) continue;

                    //根据CSV字段设置类型转换后赋值
                    switch (typeName)
                    {
                        case "Int":
                            ReflectKit.SetValue(Convert.ToInt32(valueSplit[j]), fieldInfo, obj);
                            break;
                        case "String":
                            ReflectKit.SetValue(valueSplit[j], fieldInfo, obj);
                            break;
                        case "Bool":
                            ReflectKit.SetValue(Convert.ToInt32(valueSplit[j]) == 1, fieldInfo, obj);
                            break;
                        case "Enum":
                            ReflectKit.SetValue(Convert.ToInt32(valueSplit[j]), fieldInfo, obj);
                            break;
                    }
                }

                //执行回调函数
                callBack?.Invoke(obj);
            }
        }
    }
}