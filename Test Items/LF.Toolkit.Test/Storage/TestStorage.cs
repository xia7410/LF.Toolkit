using LF.Toolkit.Data.Map;
using LF.Toolkit.Data.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LF.Toolkit.Test.Storage
{
    public class TestStorage : StorageBase<XmlDapperProvider>
    {
        public int Get()
        {
            return base.Query<int>("Get").First();
        }
    }
}
