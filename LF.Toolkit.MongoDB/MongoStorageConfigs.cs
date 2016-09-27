using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace LF.Toolkit.MongoDB
{
    public interface IMongoStorageConfig
    {
        /// <summary>
        /// 获取MongoDB存储配置名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 获取或设置配置名称
        /// </summary>
        string ReplicaSetName { get; }

        /// <summary>
        /// 获取MongoDB服务器地址集合
        /// </summary>
        IReadOnlyList<MongoServerAddress> ServerAddress { get; }

        /// <summary>
        /// 获取连接池连接最大个数
        /// </summary>
        int MaxConnectionPoolSize { get; }

        /// <summary>
        /// 获取MongoDB数据库配置集合
        /// </summary>
        IReadOnlyDictionary<string, MongoDatabaseConfig> DatabaseConfigs { get; }
    }

    /// <summary>
    /// 表示数存储配置中数据库相关配置
    /// </summary>
    public class MongoDatabaseConfig
    {
        /// <summary>
        /// 获取或设置数据库配置名称
        /// </summary>
        [XmlAttribute("Name")]
        public string Name { get; set; }

        /// <summary>
        /// 获取或设置数据库名称
        /// </summary>
        [XmlAttribute("DatabaseName")]
        public string DatabaseName { get; set; }

        /// <summary>
        /// 获取或设置用户名称
        /// </summary>
        [XmlAttribute("UserName")]
        public string Username { get; set; }

        /// <summary>
        /// 获取或设置密码
        /// </summary>
        [XmlAttribute("Password")]
        public string Password { get; set; }
    }

    public class MongoStorageConfig : IMongoStorageConfig
    {
        /// <summary>
        /// 获取或设置配置名称
        /// </summary>
        [XmlAttribute("Name")]
        public string Name { get; set; }

        /// <summary>
        /// 获取或设置副本集名称
        /// </summary>
        [XmlAttribute("ReplicaSetName")]
        public string ReplicaSetName { get; set; }

        /// <summary>
        /// 获取或设置连接池连接最大个数
        /// </summary>
        [XmlAttribute("MaxConnectionPoolSize")]
        public int MaxConnectionPoolSize { get; set; }

        /// <summary>
        /// 获取或设置服务器地址列表
        /// </summary>
        [XmlElement("ServerAddress")]
        public List<string> ServerAddressList { get; set; }

        /// <summary>
        /// 获取或设置数据库配置列表
        /// </summary>
        [XmlElement("DatabaseConfig")]
        public List<MongoDatabaseConfig> DatabaseConfigList { get; set; }

        /// <summary>
        /// 获取MongoDB服务器地址集合
        /// </summary>
        [XmlIgnore]
        public IReadOnlyList<MongoServerAddress> ServerAddress { get; internal set; }

        /// <summary>
        /// 获取MongoDB数据库配置集合
        /// </summary>
        [XmlIgnore]
        public IReadOnlyDictionary<string, MongoDatabaseConfig> DatabaseConfigs { get; internal set; }

        public MongoStorageConfig()
        {
            ServerAddressList = new List<string>();
            DatabaseConfigList = new List<MongoDatabaseConfig>();
        }
    }

    [Serializable, XmlRoot("MongoStorageConfigs", Namespace = "https://github.com/bigsupersniper")]
    public class MongoStorageConfigs
    {
        /// <summary>
        /// 获取或设置MongoDB数据存储配置列表
        /// </summary>
        [XmlElement("MongoStorageConfig")]
        public List<MongoStorageConfig> MongoStorageConfigList { get; set; }

        /// <summary>
        /// 获取MongoDB数据存储配置集合
        /// </summary>
        [XmlIgnore]
        public IReadOnlyDictionary<string, IMongoStorageConfig> StorageConfigs { get; internal set; }

        /// <summary>
        /// 从指定路径目录载入Mongo存储配置
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static MongoStorageConfigs LoadFrom(string fileName)
        {
            MongoStorageConfigs configs = null;

            if (File.Exists(fileName))
            {
                var xd = new XmlDocument();
                var ms = new MemoryStream();

                try
                {
                    xd.Load(fileName);
                    if (xd.DocumentElement != null)
                    {
                        xd.Save(ms);
                        ms.Position = 0;
                        XmlSerializer serializer = new XmlSerializer(typeof(MongoStorageConfigs));
                        configs = (MongoStorageConfigs)serializer.Deserialize(ms);
                        if (configs != null)
                        {
                            configs.StorageConfigs = configs.MongoStorageConfigList.ToDictionary(i => i.Name, config =>
                            {
                                config.ServerAddress = config.ServerAddressList.Select(i => MongoServerAddress.Parse(i)).ToList();
                                config.DatabaseConfigs = config.DatabaseConfigList.ToDictionary(i => i.Name, i => i);

                                return config as IMongoStorageConfig;
                            });
                        }
                    }
                }
                finally
                {
                    if (ms != null)
                    {
                        ms.Close();
                    }
                }
            }
            else
            {
                throw new Exception("未找到Mongo存储配置文件");
            }

            return configs;
        }
    }
}
