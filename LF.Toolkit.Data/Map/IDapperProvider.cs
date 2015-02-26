using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace LF.Toolkit.Data.Map
{
    public interface IDapperProvider : ISingleton
    {
        IDictionary<string, IDapperMapping> DapperMappings { get; }
    }
}
