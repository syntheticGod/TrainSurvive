/*
 * 描述：数据层的持久化进出口
 * 作者：项叶盛
 * 创建时间：2018/11/22 0:03:26
 * 版本：v0.1
 */

using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace WorldMap.Model
{
    public class DataPersistence
    {
        public static DataPersistence Instance { get; } = new DataPersistence();
        private string StorageFloder
        {
            get
            {
                return Application.persistentDataPath;
            }
        }
        private string filePath;
        private DataSerialization ds;
        private DataPersistence()
        {
            ds = DataSerialization.Instance;
            filePath = StorageFloder + "/TOWN_PERSON_INFO.dat";
        }
        public void SetFileName(string fileName)
        {
            filePath = StorageFloder +"/"+ fileName;
        }
        public void Save()
        {
            FileStream fileStream = new FileStream(filePath, FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fileStream, ds);
            fileStream.Close();
        }
        public bool LoadData()
        {
            FileStream fileStream;
            try
            {
                fileStream = new FileStream(filePath, FileMode.Open);
                BinaryFormatter bf = new BinaryFormatter();
                ds.Init(bf.Deserialize(fileStream) as DataSerialization);
                fileStream.Close();
            }
            catch (FileNotFoundException e)
            {
                Debug.LogError(e.ToString());
                return false;
            }
            return true;
        }
        public void Clean()
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}