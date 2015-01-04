using LF.Toolkit.Data.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace LF.Toolkit.Data.Map
{
    [XmlRoot("dapper-mapping")]
    [Serializable]
    public class DapperMapping : IDapperMapping
    {
        [XmlElement("type")]
        public string Type { get; set; }

        [XmlElement("connectionKey")]
        public string ConnectionKey { get; set; }

        [XmlElement("command")]
        public List<DapperCommand> Commands { get; set; }

        [XmlIgnore]
        public IDictionary<string, IDapperCommand> CommandDictionary { get; set; }
    }
}
