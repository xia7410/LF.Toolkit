using LF.Toolkit.Data.Map;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace LF.Toolkit.Data.Map
{
    public class XmlDapperProvider : IDapperProvider
    {
        IDictionary<string, IDapperMapping> dict;

        public XmlDapperProvider()
        {
            string path = ConfigurationManager.AppSettings["XmlMapPath"];
            if (string.IsNullOrEmpty(path)) path = "./maps";
            if (HttpContext.Current != null)
            {
                path = HttpContext.Current.Server.MapPath(path);
            }

            if (Directory.Exists(path))
            {
                string[] files = Directory.GetFiles(path, "*.xml");
                if (files.Length > 0)
                {
                    dict = new Dictionary<string, IDapperMapping>();

                    foreach (var file in files)
                    {
                        var xd = new XmlDocument();
                        var ms = new MemoryStream();

                        try
                        {
                            xd.Load(file);
                            xd.Save(ms);
                            ms.Position = 0;

                            //声明一个XML序列化器
                            XmlSerializer serializer = new XmlSerializer(typeof(DapperMapping));
                            //反序列化
                            var mapping = (DapperMapping)serializer.Deserialize(ms);

                            if (mapping != null)
                            {
                                mapping.CommandDictionary = mapping.Commands.ToDictionary(i => i.CommandKey, i => i as IDapperCommand);
                                dict.Add(mapping.Type, mapping);
                            }
                        }
                        catch
                        {
                            throw;
                        }
                        finally
                        {
                            if (ms != null)
                            {
                                ms.Close();
                            }
                        }
                    }
                }
            }
            else
            {
                throw new Exception("未找到映射目录");
            }
        }

        public IDictionary<string, IDapperMapping> DapperMappings
        {
            get { return dict; }
        }
    }
}
