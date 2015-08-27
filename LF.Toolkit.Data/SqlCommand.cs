using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace LF.Toolkit.Data
{
    public interface ISqlCommand
    {
        string CommandKey { get; set; }

        CommandType CommandType { get; set; }

        string CommandText { get; set; }
    }

    [Serializable]
    public class SqlCommand : ISqlCommand
    {
        [XmlAttribute("key")]
        public string CommandKey { get; set; }

        [XmlAttribute("commandType")]
        public CommandType CommandType { get; set; }

        [XmlText]
        public string CommandText { get; set; }
    }
}
