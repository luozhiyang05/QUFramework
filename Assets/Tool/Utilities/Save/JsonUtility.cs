using System.IO;
using UnityEngine;

namespace Tool.Utilities.Save
{
    public static class JsonUtility
    {
        private static readonly string Path = Application.persistentDataPath;

        public static void DeleteFile(string fileName)
        {
            //打开文件流
            string fileUrl = Path + "\\" + fileName;
            if(!File.Exists(fileUrl)) return;
            File.Delete(fileUrl);
        }
        
        public static void WriteStrToFile(string str, string fileName)
        {
            //打开文件流
            string fileUrl = Path + "\\" + fileName;
            using (StreamWriter sw = new StreamWriter(fileUrl))
            {
                //写入
                sw.WriteLine(str);
                sw.Close();
            }
        }

        public static string ReadStrFromFile(string fileName)
        {
            string fileUrl = Path + "\\" + fileName;
            string readStr = "";
            //打开文件流
            bool hasExist = File.Exists(fileUrl);
            if (!hasExist) return default;
            using (StreamReader sw = File.OpenText(fileUrl))
            {
                //读取
                readStr = sw.ReadToEnd();
                sw.Close();
            } 
            return readStr;
        }
        
        public static void WriteFile(object obj, string fileName)
        {
            //转为Json
            string jsonStr = UnityEngine.JsonUtility.ToJson(obj, true);
            //打开文件流
            string fileUrl = Path + "\\" + fileName;
            using (StreamWriter sw = new StreamWriter(fileUrl))
            {
                //写入
                sw.WriteLine(jsonStr);
                sw.Close();
            }
        }

        public static F ReadFile<F>(string fileName)
        {
            string readStr = null;
            string fileUrl = Path + "\\" + fileName;
            
            //打开文件流
            bool hasExist = File.Exists(fileUrl);
            if (!hasExist) return default;
            using (StreamReader sw = File.OpenText(fileUrl))
            {
                //读取
                readStr = sw.ReadToEnd();
                sw.Close();
            } 
            //转为file
            return UnityEngine.JsonUtility.FromJson<F>(readStr);
        }
    }
}