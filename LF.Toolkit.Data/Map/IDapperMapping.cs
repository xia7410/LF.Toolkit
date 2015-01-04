using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LF.Toolkit.Data.Map
{
    public interface IDapperMapping
    {
        string Type { get; set; }

        string ConnectionKey { get; set; }

        IDictionary<string, IDapperCommand> CommandDictionary { get; set; }
    }
}
