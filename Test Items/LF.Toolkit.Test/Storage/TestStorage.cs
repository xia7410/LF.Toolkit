using LF.Toolkit.Data.Map;
using LF.Toolkit.Data.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LF.Toolkit.Test.Storage
{
    public class TestStorage : StorageBase<XmlDapperProvider>
    {
        public Task<string> Get()
        {
            return base.ExecuteScalarAsync<string>("Get");
        }
    }
}
