using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace LF.Toolkit.Data
{
    public interface ISqlMapping
    {
        string Type { get; set; }

        string ConnectionKey { get; set; }

        IDictionary<string, ISqlCommand> CommandDictionary { get; set; }
    }

    [XmlRoot("sql-mapping")]
    [Serializable]
    public class SqlMapping : ISqlMapping
    {
        [XmlElement("type")]
        public string Type { get; set; }

        [XmlElement("connectionKey")]
        public string ConnectionKey { get; set; }

        [XmlElement("command")]
        public List<SqlCommand> Commands { get; set; }

        [XmlIgnore]
        public IDictionary<string, ISqlCommand> CommandDictionary { get; set; }
    }
}
