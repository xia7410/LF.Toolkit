using LF.Toolkit.DataEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace LF.Toolkit.Data
{
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
        internal IDictionary<string, ISqlCommand> CommandDictionary { get; set; }

        ISqlCommand ISqlMapping.this[string key]
        {
            get
            {
                ISqlCommand cmd = null;
                if (!CommandDictionary.TryGetValue(key, out cmd)) throw new Exception(string.Format("Could not find the '{0}' SqlCommand", key));

                return cmd;
            }
        }

    }
}
