using LF.Toolkit.Data;
using LF.Toolkit.Data.Map;
using LF.Toolkit.Data.Storage;
using LF.Toolkit.Test.Storage;
using LF.Toolkit.Util;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

namespace LF.Toolkit.Test
{
    class Program
    {
        static void SetupLazyClassInstance()
        {
            var dapperFactory = DapperProvider<XmlDapperProvider>.Factory;
            var storageTypes = typeof(TestStorage).Assembly.GetTypes();
            var baseType = typeof(StorageBase<XmlDapperProvider>);
            var storageProvider = typeof(StorageProvider<>);
            foreach (var t in storageTypes)
            {
                if (t.IsSubclassOf(baseType))
                {
                    var provider = storageProvider.MakeGenericType(t);
                    var factory = provider.GetProperty("Factory", BindingFlags.Static);
                    if (factory != null)
                    {
                        factory.GetValue(null, null);
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            string filepath = @"C:\Users\sniper\Resource\1.jpg";
            string url = "http://localhost:8080/api/file";

            var ufs = new UploadFileStream()
            {
                FileName = "1.jpg",
                ContentType = "image/jpg",
                Name = "file",
                FileStream = File.OpenRead(filepath)
            };

            var ufs1 = new UploadFileStream()
            {
                FileName = "2.jpg",
                ContentType = "image/jpg",
                Name = "file",
                FileStream = File.OpenRead(@"C:\Users\sniper\Resource\2.jpg")
            };

            var files = new UploadFileStream[] { ufs, ufs1 };

            HttpProvider.Upload(url, new object(), files, (s) => Console.WriteLine(s), (e) => Console.WriteLine(e));

            Console.ReadKey();
        }
    }
}
