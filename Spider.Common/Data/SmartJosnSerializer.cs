using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web.Script.Serialization;
namespace Spider.Data
{
    public class SmartJosnSerializer
    {
        private class SmartObjectJsonConverter : JavaScriptConverter
        {
            public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
            {
                if (dictionary == null)
                    throw new ArgumentNullException("dictionary");
                if (type == typeof(SmartObject))
                {
                    return new SmartObject(dictionary);
                }
                return null;
            }
            public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
            {

                dynamic o = obj as SmartObject;
                if (o != null)
                {
                    return o.AsDictionary();
                }
                return obj as IDictionary<string,object>;
            }
            public override IEnumerable<Type> SupportedTypes
            {
                get { return new ReadOnlyCollection<Type>(new List<Type>(new Type[] { typeof(SmartObject) })); }
            }
        }
        private static JavaScriptSerializer _jss;
        static SmartJosnSerializer()
        {
            _jss = new JavaScriptSerializer();
            _jss.RegisterConverters(new JavaScriptConverter[] { new SmartObjectJsonConverter() });
        }
        public static dynamic Deserialize<T>(string json)
        {
            return _jss.Deserialize(json, typeof(T));
        }
        public static string Serialize<T>(T obj)
        {
            return _jss.Serialize(obj);
        }
    }
}
