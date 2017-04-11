using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
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
        internal ConcurrentDictionary<string, ISqlMapping> SqlMappings { get; set; }

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
                    throw new Exception(string.Format("未找到指定类型 '{0}' 的映射文件", key));

                return mapping;
            }
        }

        /// <summary>
        /// 从指定路径目录载入Sql映射文件集合
        /// </summary>
        /// <param name="path"></param>
        public static ISqlMappingCollection LoadFrom(string path)
        {
            SqlMappingCollection collection = null;

            if (Directory.Exists(path))
            {
                string[] files = Directory.GetFiles(path, "*.xml");
                if (files.Length > 0)
                {
                    collection = new SqlMappingCollection();
                    var attr = (typeof(SqlMapping).GetCustomAttributes(typeof(XmlRootAttribute), false)
                        [0] as XmlRootAttribute);
                    //获取默认的命名空间
                    string defaultElementName = attr.ElementName;
                    //获取默认的命名空间
                    string defaultNamespace = attr.Namespace;
                    //并发加载
                    files.AsParallel().ForAll(file =>
                    {
                        var xd = new XmlDocument();
                        var ms = new MemoryStream();

                        try
                        {
                            xd.Load(file);
                            if (xd.DocumentElement != null)
                            {
                                //排除非sql-mapping架构xml文件
                                if (xd.DocumentElement.Name != defaultElementName)
                                {
                                    return;
                                }
                                //若序列化的xml不带命名空间则补上默认的命名空间
                                if (xd.DocumentElement.NamespaceURI == "")
                                {
                                    xd.DocumentElement.SetAttribute("xmlns", defaultNamespace);
                                }
                                xd.Save(ms);
                                ms.Position = 0;
                                XmlSerializer serializer = new XmlSerializer(typeof(SqlMapping));
                                var mapping = (SqlMapping)serializer.Deserialize(ms);
                                if (mapping != null)
                                {
                                    mapping.Type = mapping.Type.Trim();
                                    mapping.ConnectionKey = mapping.ConnectionKey.Trim();
                                    mapping.CommandDictionary = mapping.Commands.ToDictionary(i => i.CommandKey, i =>
                                    {
                                        //去除空格与换行符
                                        i.CommandText = i.CommandText.Trim().Trim(new char[] { '\r', '\n' });
                                        return i as ISqlCommand;
                                    });
                                    //添加到映射集合中
                                    collection.SqlMappings.AddOrUpdate(mapping.Type, mapping, (k, v) => mapping);
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
                    });
                }
                else
                {
                    throw new Exception("未找到映射文件");
                }
            }
            else
            {
                throw new Exception("未找到映射文件目录");
            }

            return collection;
        }
    }
}
