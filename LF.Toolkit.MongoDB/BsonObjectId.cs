using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LF.Toolkit.MongoDB
{
    public class BsonObjectId
    {
        /// <summary>
        /// 获取或设置Bson文档对象Id
        /// </summary>
        public ObjectId _id { get; set; } 
    }
}
