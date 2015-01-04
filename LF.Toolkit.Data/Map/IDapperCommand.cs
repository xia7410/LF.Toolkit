using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace LF.Toolkit.Data.Map
{
    public interface IDapperCommand
    {
        string CommandKey { get; set; }

        CommandType CommandType { get; set; }

        string CommandText { get; set; }
    }
}
