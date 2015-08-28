using LF.Toolkit.DataEngine;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace LF.Toolkit.Data
{
    /// <summary>
    /// 表示Sql映射集合实现类
    /// </summary>
    public class SqlMappingCollection : ISqlMappingCollection
    {
        ConcurrentDictionary<string, ISqlMapping> SqlMappings { get; set; }

        public SqlMappingCollection()
        {
            SqlMappings = new ConcurrentDictionary<string, ISqlMapping>();
        }

        /// <summary>
        /// 获取指定关键字的Sql映射
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        ISqlMapping ISqlMappingCollection.this[string key]
        {
            get
            {
                ISqlMapping mapping = null;
                if (!SqlMappings.TryGetValue(key, out mapping))
                    throw new Exception(string.Format("Could not find the '{0}' mapping file", key));

                return mapping;
            }
        }

        /// <summary>
        /// 从指定路径目录载入Sql映射文件集合
        /// </summary>
        /// <param name="path"></param>
        void ISqlMappingCollection.LoadFrom(string path)
        {
            if (Directory.Exists(path))
            {
                string[] files = Directory.GetFiles(path, "*.xml");
                if (files.Length > 0)
                {
                    //并发字典
                    var condict = new ConcurrentDictionary<string, ISqlMapping>();
                    //并发加载
                    files.AsParallel().ForAll(file =>
                    {
                        var xd = new XmlDocument();
                        var ms = new MemoryStream();

                        try
                        {
                            xd.Load(file);
                            xd.Save(ms);
                            ms.Position = 0;

                            //声明一个XML序列化器
                            XmlSerializer serializer = new XmlSerializer(typeof(SqlMapping));
                            //反序列化
                            var mapping = (SqlMapping)serializer.Deserialize(ms);
                            if (mapping != null)
                            {
                                mapping.CommandDictionary = mapping.Commands.ToDictionary(i => i.CommandKey, i =>
                                {
                                    //去除空格与换行符
                                    i.CommandText = i.CommandText.Trim().Trim(new char[] { '\r', '\n' });
                                    return i as ISqlCommand;
                                });
                                //添加到映射集合中
                                SqlMappings.AddOrUpdate(mapping.Type, mapping, (k, v) => mapping);
                            }
                        }
                        finally
                        {
                            if (ms != null)
                            {
                                ms.Close();
                            }
                        }
                    });
                }
                else
                {
                    throw new Exception("Could not find Sql Xml mapping files");
                }
            }
            else
            {
                throw new Exception("Could not find Sql Xml mapping folder");
            }
        }
    }
}
