using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace LF.Toolkit.Common
{
    public class XmlProvider
    {
        /// <summary>
        /// 反序列化xml文件为指定类型对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string fileName, Action<T> action = null)
        {
            T obj = default(T);

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
                        XmlSerializer serializer = new XmlSerializer(typeof(T));
                        obj = (T)serializer.Deserialize(ms);
                        if(action != null)
                        {
                            action(obj);
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
                throw new FileNotFoundException(fileName);
            }

            return obj;
        }

        /// <summary>
        /// 序列化指定对象到xml文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="fileName"></param>
        public static void Serialize<T>(T obj, string fileName)
        {
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            using (var fs = new FileStream(fileName, FileMode.CreateNew))
            {
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(fs, obj);
            }
        }
    }
}
