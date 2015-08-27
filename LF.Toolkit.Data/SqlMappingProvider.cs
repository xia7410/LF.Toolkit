using LF.Toolkit.Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace LF.Toolkit
{
    internal class SqlMappingProvider
    {
        IDictionary<string, ISqlMapping> DapperMappings { get; set; }

        /// <summary>
        /// 映射配置,必须在使用映射前执行
        /// </summary>
        /// <param name="path"></param>
        public SqlMappingProvider(string path)
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
                                var cmds = mapping.Commands.ToDictionary(i => i.CommandKey, i => i as ISqlCommand);
                                //去除换行符
                                foreach (var cmd in cmds)
                                {
                                    cmd.Value.CommandText = cmd.Value.CommandText.Trim().Trim(new char[] { '\r', '\n' });
                                }

                                mapping.CommandDictionary = cmds;
                                condict.AddOrUpdate(mapping.Type, mapping, (k, v) => mapping);
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

                    //转化为字典
                    DapperMappings = condict.ToDictionary(i => i.Key, i => i.Value);
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

        public ISqlMapping GetSqlMapping(string fullname)
        {
            ISqlMapping mapping = null;
            if (!DapperMappings.TryGetValue(fullname, out mapping)) throw new Exception(string.Format("Could not find the '{0}' mapping file", fullname));

            return mapping;
        }
    }
}
