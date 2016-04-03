using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LF.Toolkit.MongoDB
{
    public class MongoBsonId<T>
    {
        /// <summary>
        /// 获取或设置ObjectId
        /// </summary>
        [BsonId]
        public T Id { get; set; }
    }
}
