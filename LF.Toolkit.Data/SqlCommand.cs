using LF.Toolkit.DataEngine;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace LF.Toolkit.Data
{
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
