using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LF.Toolkit.MongoDB
{
    /// <summary>
    /// 简单BsonDocument放序列化类
    /// </summary>
    public sealed class BsonConvert
    {
        static MethodInfo deserializeObject; //序列化对象函数
        static MethodInfo deserializeToArray; //序列化数组对象函数
        static MethodInfo deserializeToList;//序列化为列表函数
        static MethodInfo deserialize;//序列化基本值函数

        static BsonConvert()
        {
            var thisType = typeof(BsonConvert);
            deserializeObject = thisType.GetMethod("DeserializeObject", new Type[] { typeof(BsonDocument) });
            deserializeToArray = thisType.GetMethod("DeserializeToArray", BindingFlags.NonPublic | BindingFlags.Static);
            deserializeToList = thisType.GetMethod("DeserializeToList", BindingFlags.NonPublic | BindingFlags.Static);
            deserialize = thisType.GetMethod("Deserialize", BindingFlags.NonPublic | BindingFlags.Static);
        }

        /// <summary>
        /// 反序列化BsonArray为对象列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        static List<T> DeserializeToList<T>(BsonArray array)
        {
            List<T> dstList = null;

            if (array != null && array.Count > 0)
            {
                var t = typeof(T);
                dstList = new List<T>();

                foreach (var value in array)
                {
                    object pValue = null;
                    if (value.IsBsonDocument)
                    {
                        pValue = deserializeObject.MakeGenericMethod(t).Invoke(null, new object[] { value });
                    }
                    else if (value.IsBsonArray)
                    {
                        //只序列化一维数组对象或IEnumerable、IList、List泛型对象
                        if (t.IsArray && t.GetArrayRank() == 1)
                        {
                            pValue = deserializeToArray.MakeGenericMethod(t.GetElementType()).Invoke(null, new object[] { value });
                        }
                        else if (t.IsGenericType && t.GetGenericArguments().Length <= 1)
                        {
                            var defType = t.GetGenericTypeDefinition();
                            if (defType == typeof(IEnumerable<>) || defType == typeof(IList<>)
                                || defType == typeof(List<>))
                            {
                                pValue = deserializeToList.MakeGenericMethod(t.GetGenericArguments()[0])
                                    .Invoke(null, new object[] { value });
                            }
                        }
                    }
                    else
                    {
                        pValue = deserialize.MakeGenericMethod(t).Invoke(null, new object[] { value });
                    }

                    if (pValue != null)
                    {
                        dstList.Add((T)pValue);
                    }
                }
            }

            return dstList;
        }

        /// <summary>
        /// 反序列化BsonArray为对象数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        static T[] DeserializeToArray<T>(BsonArray array)
        {
            T[] dstArray = null;
            if (array != null && array.Count > 0)
            {
                var t = typeof(T);
                dstArray = new T[array.Count];

                for (int i = 0; i < array.Count; i++)
                {
                    var value = array[i];
                    object pValue = null;
                    if (value.IsBsonDocument)
                    {
                        pValue = deserializeObject.MakeGenericMethod(t).Invoke(null, new object[] { value });
                    }
                    else if (value.IsBsonArray)
                    {
                        //只序列化一维数组对象或IEnumerable、IList、List泛型对象
                        if (t.IsArray && t.GetArrayRank() == 1)
                        {
                            pValue = deserializeToArray.MakeGenericMethod(t.GetElementType()).Invoke(null, new object[] { value });
                        }
                        else if (t.IsGenericType && t.GetGenericArguments().Length <= 1)
                        {
                            var defType = t.GetGenericTypeDefinition();
                            if (defType == typeof(IEnumerable<>) || defType == typeof(IList<>)
                                || defType == typeof(List<>))
                            {
                                pValue = deserializeToList.MakeGenericMethod(t.GetGenericArguments()[0])
                                    .Invoke(null, new object[] { value });
                            }
                        }
                    }
                    else
                    {
                        pValue = deserialize.MakeGenericMethod(t).Invoke(null, new object[] { value });
                    }

                    if (pValue != null)
                    {
                        dstArray[i] = (T)pValue;
                    }
                }
            }

            return dstArray;
        }

        /// <summary>
        /// 反序列化BsonValue为指定类型的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns> 
        static T Deserialize<T>(BsonValue value)
        {
            object obj = null;

            if (value.IsBsonBinaryData)
            {
                obj = value.AsByteArray;
            }
            else if (value.IsBsonDateTime)
            {
                obj = value.ToUniversalTime();
            }
            else if (value.IsObjectId)
            {
                obj = value.AsObjectId;
            }
            else if (value.IsString)
            {
                obj = value.AsString;
            }
            else if (value.IsInt32)
            {
                obj = value.AsInt32;
            }
            else if (value.IsInt64)
            {
                obj = value.IsInt64;
            }
            else if (value.IsDouble)
            {
                obj = value.AsDouble;
                if (typeof(T) != typeof(double))
                {
                    obj = Convert.ChangeType(obj, typeof(T));
                }
            }
            else if (value.IsNumeric)
            {
                obj = (decimal)value;
            }
            else if (value.IsGuid)
            {
                obj = value.AsGuid;
            }

            return (T)obj;
        }

        /// <summary>
        /// 反序列化BsonDocument为指定对象
        /// 支持反序列化的类型为：T[]、IEnumerable<T>、IList<T>、List<T> , 若需要转换其他类型对象请手动转换
        /// </summary>
        /// <typeparam name="T">反序列化的类型</typeparam>
        /// <param name="bson">Bson文档对象</param>
        /// <returns></returns>
        public static T DeserializeObject<T>(BsonDocument bson) where T : new()
        {
            if (bson != null)
            {
                var t = typeof(T);
                //搜索需要序列化的字段
                var props = t.GetProperties().Where(i => !i.IsDefined(typeof(BsonIgnoreAttribute), true)).ToArray();
                if (props.Length > 0)
                {
                    var obj = t.Assembly.CreateInstance(t.FullName);
                    foreach (var p in props)
                    {
                        if (bson.Contains(p.Name))
                        {
                            var value = bson.GetValue(p.Name);
                            if (!value.IsBsonNull)
                            {
                                object pValue = null;
                                if (value.IsBsonDocument)
                                {
                                    pValue = deserializeObject.MakeGenericMethod(p.PropertyType).Invoke(null, new object[] { value });
                                }
                                else if (value.IsBsonArray)
                                {
                                    var pt = p.PropertyType;
                                    //只序列化一维数组对象或IEnumerable、IList、List泛型对象
                                    if (pt.IsArray && pt.GetArrayRank() == 1)
                                    {
                                        pValue = deserializeToArray.MakeGenericMethod(pt.GetElementType()).Invoke(null, new object[] { value });
                                    }
                                    else if (pt.IsGenericType && pt.GetGenericArguments().Length <= 1)
                                    {
                                        var defType = pt.GetGenericTypeDefinition();
                                        if (defType == typeof(IEnumerable<>) || defType == typeof(IList<>)
                                            || defType == typeof(List<>))
                                        {
                                            pValue = deserializeToList.MakeGenericMethod(pt.GetGenericArguments()[0])
                                                .Invoke(null, new object[] { value });
                                        }
                                    }
                                }
                                else
                                {
                                    pValue = deserialize.MakeGenericMethod(p.PropertyType).Invoke(null, new object[] { value });
                                }

                                p.SetValue(obj, pValue, null);
                            }
                        }
                    }

                    return (T)obj;
                }
            }

            return default(T);
        }

        /// <summary>
        /// 序列化MongoCursor为对象集合
        /// </summary>
        /// <typeparam name="T">反序列化的类型</typeparam>
        /// <param name="cursor">查询游标</param>
        /// <returns></returns>
        public static IEnumerable<T> DeserializeObject<T>(MongoCursor<BsonDocument> cursor) where T : new()
        {
            IEnumerable<T> ie = null;

            if (cursor != null)
            {
                var t = typeof(T);
                var method = deserializeObject.MakeGenericMethod(t);
                var list = new List<T>();
                foreach (BsonDocument bson in cursor)
                {
                    var obj = method.Invoke(null, new object[] { bson });
                    if (obj != null)
                    {
                        list.Add((T)obj);
                    }
                }

                ie = list;
            }

            return ie;
        }
    }
}
