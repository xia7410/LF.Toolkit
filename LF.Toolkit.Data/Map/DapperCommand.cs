using LF.Toolkit.Data.Map;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace LF.Toolkit.Data.Map
{
    [Serializable]
    public class DapperCommand : IDapperCommand
    {
        [XmlAttribute("key")]
        public string CommandKey { get; set; }

        [XmlAttribute("commandType")]
        public CommandType CommandType { get; set; }

        [XmlText]
        public string CommandText { get; set; }
    }
}
