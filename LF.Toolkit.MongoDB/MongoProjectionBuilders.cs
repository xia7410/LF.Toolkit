using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Reflection;
using MongoDB.Bson.Serialization.Attributes;

namespace LF.Toolkit.MongoDB
{
    public static class MongoProjectionBuilders
    {
        static readonly ConcurrentDictionary<Type, ProjectionDefinition<BsonDocument>> m_ProjectionCollections = new ConcurrentDictionary<Type, ProjectionDefinition<BsonDocument>>();

        /// <summary>
        /// 包含类型中基础字段的定义
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ProjectionDefinition<BsonDocument> IncludeBasicProperties<T>()
        {
            var type = typeof(T);
            var p = m_ProjectionCollections.GetOrAdd(type, (t) =>
            {
                var list = new List<ProjectionDefinition<BsonDocument>>();
                var projection = Builders<BsonDocument>.Projection;

                foreach (var pi in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    //bsonId
                    var idAttr = pi.GetCustomAttribute<BsonIdAttribute>();
                    if (idAttr == null)
                    {
                        //判断是否忽略自动
                        var ignoreAttr = pi.GetCustomAttribute<BsonIgnoreAttribute>();
                        if (ignoreAttr == null)
                        {
                            string eleName = "";
                            //字段别名
                            var eleAttr = pi.GetCustomAttribute<BsonElementAttribute>();
                            if (eleAttr != null)
                            {
                                eleName = eleAttr.ElementName;
                            }
                            else
                            {
                                eleName = pi.Name;
                            }
                            //添加基本类型
                            if (pi.PropertyType == typeof(string) || pi.PropertyType == typeof(decimal) || pi.PropertyType == typeof(DateTime)
                                || pi.PropertyType.IsPrimitive)
                            {
                                list.Add(projection.Include(eleName));
                            }
                        }
                    }
                }

                return projection.Combine(list);
            });

            return p;
        }

        /// <summary>
        /// 包含指定类型的基础字段与指定数组名称的构造
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="elementNames"></param>
        /// <returns></returns>
        public static ProjectionDefinition<BsonDocument> Include<T>(params string[] elementNames)
        {
            var projection = Builders<BsonDocument>.Projection;
            var basic = IncludeBasicProperties<T>();
            if (basic != null)
            {
                return projection.Combine(basic, Include(elementNames));
            }

            return Include(elementNames);
        }

        /// <summary>
        /// 包含指定名称的构造
        /// </summary>
        /// <param name="elementNames"></param>
        /// <returns></returns>
        public static ProjectionDefinition<BsonDocument> Include(params string[] elementNames)
        {
            var projection = Builders<BsonDocument>.Projection;

            return projection.Combine(elementNames.Select(e => projection.Include(e)));
        }

        /// <summary>
        /// 不包含指定名称的构造
        /// </summary>
        /// <param name="elementNames"></param>
        /// <returns></returns>
        public static ProjectionDefinition<BsonDocument> Exclude(params string[] elementNames)
        {
            var projection = Builders<BsonDocument>.Projection;

            return projection.Combine(elementNames.Select(e => projection.Exclude(e)));
        }
    }
}
