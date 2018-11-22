/*
 * 描述：数据层的持久化进出口
 * 作者：项叶盛
 * 创建时间：2018/11/22 0:03:26
 * 版本：v0.1
 */
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using WorldMap.Model;

namespace WorldMap
{
    public class DataPersistence
    {
        private static DataPersistence instance;
        public static DataPersistence Instance
        {
            get
            {
                if (instance == null)
                    instance = new DataPersistence();
                return instance;
            }
        }
        private DataPersistence()
        {
            ds = DataSerialization.Instance;
            filePath = StorageFloder + "/TOWN_PERSON_INFO.dat";
        }
        private string StorageFloder
        {
            get
            {
                return Application.persistentDataPath;
            }
        }
        private string filePath;
        private DataSerialization ds;
        public void Save()
        {
            FileStream fileStream = new FileStream(filePath, FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();
            ds.BeforeSerialize();
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
                ds.AfterDeserialize(bf.Deserialize(fileStream));
            }
            catch (FileNotFoundException e)
            {
                Debug.LogError(e.ToString());
                return false;
            }
            return true;
        }
    }
    public class RandomData
    {

    }
}