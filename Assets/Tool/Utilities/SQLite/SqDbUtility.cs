// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Reflection;
// using System.Text;
// using Mono.Data.Sqlite;
// using Tool.CustomAttribute;
// using Unity.VisualScripting;
// using UnityEngine;
//
// namespace Tool.Utilities.SQLite
// {
//     public interface IDbTable
//     {
//         public int Id { get; set; }
//     }
//
//     public static class SqDbUtility
//     {
//         private static string DbPath = Application.dataPath + "/../Save/SaveData.db";
//         private static SqliteConnection _sqlConn;
//         private static SqliteCommand _sqlCmd;
//
//         #region 数据库连接
//
//         /// <summary>
//         /// 连接数据库
//         /// </summary>
//         public static void Connect()
//         {
//             if (_sqlConn != null) return;
//
//             try
//             {
//                 //寻找有无数据库文件，无则创建（先判断save文件是否存在）
//                 FileInfo fileInfo = new FileInfo(DbPath);
//                 if (!Directory.Exists(fileInfo.Directory.FullName))
//                 {
//                     //创建Save文件夹
//                     Directory.CreateDirectory(fileInfo.Directory.FullName);
//                     if (!File.Exists(DbPath))
//                     {
//                         SqliteConnection.CreateFile(DbPath);
//                     }
//                 }
//             }
//             catch (Exception e)
//             {
//                 Debug.LogError($"数据库文件创建失败{e.Message}");
//                 return;
//             }
//
//             try
//             {
//                 //连接数据库
//                 _sqlConn = new SqliteConnection(new SqliteConnectionStringBuilder() { DataSource = DbPath }.ToString());
//                 _sqlConn.Open();
//             }
//             catch (Exception e)
//             {
//                 Debug.LogError($"数据库连接创建失败{e.Message}");
//                 return;
//             }
//         }
//
//         /// <summary>
//         /// 释放资源
//         /// </summary>
//         public static void Dispose()
//         {
//             if (_sqlConn == null) return;
//             _sqlConn.Dispose();
//             _sqlConn = null;
//         }
//
//         #endregion
//
//         #region 数据库常规操作
//
//         /// <summary>
//         /// 动态生成表
//         /// </summary>
//         /// <typeparam name="T"></typeparam>
//         /// <returns></returns>
//         public static int CreateTable<T>() where T : IDbTable
//         {
//             //通过反射T类型
//             var classType = typeof(T);
//
//             //拼接sql
//             var sql = new StringBuilder();
//             sql.Append($"create table if not exists {classType.Name}(");
//
//             //获取T属性
//             var propertyInfos = classType.GetProperties();
//             var interfaces = classType.GetFields();
//
//             //通过特性获取属性信息拼接sql
//             foreach (var propertyInfo in propertyInfos)
//             {
//                 //获取特性，无特性的跳过不存入数据库表
//                 var cab = propertyInfo.GetCustomAttribute<DbHelp>();
//                 if (cab == null) continue;
//
//                 //拼接字段名和属性
//                 sql.Append($"{cab.PropertyName} {cab.PropertyType.Name}");
//
//                 //是否为主键
//                 if (cab.IsKey) sql.Append($" primary key ");
//
//                 //是否能为空
//                 sql.Append(cab.CanBeNull ? " null " : " not null ");
//
//                 sql.Append(",");
//             }
//
//             sql.Remove(sql.Length - 1, 1);
//             sql.Append(")");
//
//             return ExecuteNonQuery(sql);
//         }
//
//         /// <summary>
//         /// 删除表
//         /// </summary>
//         /// <typeparam name="T"></typeparam>
//         /// <returns></returns>
//         public static int DeleteTable<T>() where T : IDbTable
//         {
//             var tableName = typeof(T).Name;
//             //判断表是否存在
//             if (!IsExistTable<T>())
//             {
//                 Debug.LogError($"表{tableName}不存在。");
//                 return -1;
//             }
//             var sql = new StringBuilder();
//             sql.Append($"Drop table {tableName}");
//             return ExecuteNonQuery(sql);
//         }
//
//         /// <summary>
//         /// 判断表是否存在
//         /// </summary>
//         /// <typeparam name="T"></typeparam>
//         /// <returns></returns>
//         public static bool IsExistTable<T>() where T : IDbTable
//         {
//             //查询表是否存在
//             var tableName = typeof(T).Name;
//             var sql = new StringBuilder();
//             sql.Append($"SELECT name FROM sqlite_master WHERE name='{tableName}'");
//             var sdr = ExecuteReader(sql);
//             return Convert.ToString(sdr.GetValue(0)) != "";
//         }
//
//         /// <summary>
//         /// 读取数据库表
//         /// </summary>
//         /// <typeparam name="T"></typeparam>
//         public static T LoadDbTableData<T>(int id) where T : IDbTable
//         {
//             //反射获取类型名作为表名
//             var type = typeof(T);
//             var tableName = type.Name;
//             //实例化T
//             var obj = Activator.CreateInstance<T>();
//             //拼接sql
//             var sql = new StringBuilder();
//             sql.Append($"SELECT * FROM {tableName} WHERE Id = {id}");
//             //执行
//             var sdr = ExecuteReader(sql);
//             //获取数据库字段名列表
//             for (int i = 0; i < sdr.FieldCount; i++)
//             {
//                 //通过sdr的字段名获取该字段下的值
//                 var fieldName = sdr.GetName(i);
//                 var value = sdr[fieldName];
//                 //根据字段名反射获取值
//                 var propertyInfo = type.GetProperty(fieldName);
//                 if (propertyInfo == null)
//                 {
//                     Debug.LogError($"{tableName}找不到目标字段{fieldName}");
//                     return default;
//                 }
//
//                 //获取属性类型，特殊处理Int32
//                 var cab = propertyInfo.GetCustomAttribute<DbHelp>();
//                 propertyInfo.SetValue(obj, cab.PropertyType == typeof(Int32) ? Convert.ToInt32(value) : value);
//             }
//
//             return obj;
//         }
//
//         /// <summary>
//         /// 给表插入数据
//         /// </summary>
//         /// <param name="t"></param>
//         /// <typeparam name="T"></typeparam>
//         /// <returns></returns>
//         public static int Insert<T>(T t) where T : IDbTable
//         {
//             //获取T的属性
//             var type = typeof(T);
//             var tableName = type.Name;
//             var propertyInfos = type.GetProperties();
//             //根据属性名拼接sql
//             var sql = new StringBuilder();
//             sql.Append($"INSERT INTO {tableName} (");
//             foreach (var propertyInfo in propertyInfos)
//             {
//                 //获取特性
//                 var cab = propertyInfo.GetCustomAttribute<DbHelp>();
//                 if (cab == null) continue;
//
//                 sql.Append($"{cab.PropertyName},");
//             }
//
//             sql.Remove(sql.Length - 1, 1);
//             sql.Append(") VALUES (");
//             //根据属性值拼接sql
//             foreach (var propertyInfo in propertyInfos)
//             {
//                 //获取特性
//                 var cab = propertyInfo.GetCustomAttribute<DbHelp>();
//                 if (cab == null) continue;
//
//                 //获取该属性的类型
//                 var propertyType = cab.PropertyType;
//
//                 //获取该属性的值
//                 var value = propertyInfo.GetValue(t);
//
//                 //根据特性指定的类型，将值类型转换
//                 if (propertyType == typeof(Int32))
//                     value = Convert.ToInt32(value);
//                 else if (propertyType == typeof(string))
//                     value = Convert.ToString(value);
//
//                 //拼接sql
//                 sql.Append(propertyType == typeof(string)
//                     ? $"'{value}',"
//                     : $"{value},");
//             }
//
//             sql.Remove(sql.Length - 1, 1);
//             sql.Append($")");
//             //执行
//             var rowCount = ExecuteNonQuery(sql);
//             return rowCount;
//         }
//
//         /// <summary>
//         /// 修改表数据
//         /// </summary>
//         /// <param name="t"></param>
//         /// <typeparam name="T"></typeparam>
//         /// <returns></returns>
//         public static int Update<T>(T t) where T : IDbTable
//         {
//             var type = typeof(T);
//             var tableName = type.Name;
//
//             //判断表是否存在
//             if (!IsExistTable<T>())
//             {
//                 Debug.LogError($"表{tableName}不存在。");
//                 return -1;
//             }
//
//             //拼接修改sql
//             var sql = new StringBuilder();
//             sql.Append($"UPDATE {tableName} SET ");
//             var propertyInfos = type.GetProperties();
//             foreach (var propertyInfo in propertyInfos)
//             {
//                 //获取特性
//                 var cab = propertyInfo.GetCustomAttribute<DbHelp>();
//                 if (cab == null) continue;
//
//                 //特殊处理字符串
//                 sql.Append(cab.PropertyType == typeof(string)
//                     ? $"{cab.PropertyName} = '{propertyInfo.GetValue(t)}',"
//                     : $"{cab.PropertyName} = {propertyInfo.GetValue(t)},");
//             }
//
//             sql.Remove(sql.Length - 1, 1);
//             sql.Append($" WHERE Id = {t.Id}");
//
//             return ExecuteNonQuery(sql);
//         }
//
//         #endregion
//
//         #region 执行语句
//
//         private static int ExecuteNonQuery(StringBuilder sql)
//         {
//             _sqlCmd = new SqliteCommand(_sqlConn);
//             _sqlCmd.CommandText = sql.ToString();
//             var row = _sqlCmd.ExecuteNonQuery();
//             _sqlCmd.Dispose();
//             _sqlCmd = null;
//             return row;
//         }
//
//         private static SqliteDataReader ExecuteReader(StringBuilder sql)
//         {
//             _sqlCmd = new SqliteCommand(_sqlConn);
//             _sqlCmd.CommandText = sql.ToString();
//             var executeReader = _sqlCmd.ExecuteReader();
//             _sqlCmd.Dispose();
//             _sqlCmd = null;
//             return executeReader;
//         }
//
//         #endregion
//     }
// }