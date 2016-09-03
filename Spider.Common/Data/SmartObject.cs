/*
 * Create by zhoulq
 * kingge168@gmail.com
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Spider.Data
{
    public sealed class SmartObject : DynamicObject
    {
        private class DataUtil
        {
            public static object WrapperData(object value)
            {
                if (value is string)
                {
                    return value;
                }
                else if (value is IDictionary<string, object>)
                {
                    IDictionary<string, object> dict = value as IDictionary<string, object>;
                    return new SmartObject(dict);
                }
                else if (value is IEnumerable)
                {
                    IEnumerable enumertor = value as IEnumerable;
                    IList<dynamic> list = new LineList<dynamic>();
                    foreach (var item in enumertor)
                    {
                        if (item is IDictionary<string, object>)
                        {
                            list.Add(new SmartObject(item as IDictionary<string, object>));
                        }
                        else
                        {
                            list.Add(WrapperData(item));
                        }
                    }
                    return list;
                }
                else
                {
                    return value;
                }
            }
            public static object RecoverData(object value)
            {
                if (value is string)
                {
                    return value;
                }
                else if (value is SmartObject)
                {
                    return (value as SmartObject).AsDictionary();
                }
                else if (value is IEnumerable)
                {
                    IEnumerable enumerable = value as IEnumerable;
                    IList<dynamic> list = new LineList<dynamic>();
                    foreach (var item in enumerable)
                    {
                        if (item is SmartObject)
                        {
                            list.Add((item as SmartObject).AsDictionary());
                        }
                        else if (item is IEnumerable)
                        {
                            list.Add(RecoverData(item as IEnumerable));
                        }
                        else
                        {
                            list.Add(item);
                        }
                    }
                    return list;
                }
                else
                {
                    return value;
                }
            }
        }
        private class NameValueObject : INamedObject
        {
            private string _name;
            private object _value;
            public string Name
            {
                get { return _name; }
                set
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        throw new ArgumentException("name");
                    }
                    _name = value;
                }
            }
            public object Value
            {
                get
                {
                    return _value;
                }
                set
                {
                    if (value != null)
                    {
                        TypeCode typeCode = Type.GetTypeCode(value.GetType());
                        switch (typeCode)
                        {
                            case TypeCode.Boolean:
                            case TypeCode.Byte:
                            case TypeCode.Char:
                            case TypeCode.DateTime:
                            case TypeCode.Decimal:
                            case TypeCode.Double:
                            case TypeCode.Int16:
                            case TypeCode.Int32:
                            case TypeCode.Int64:
                            case TypeCode.SByte:
                            case TypeCode.Single:
                            case TypeCode.String:
                            case TypeCode.UInt16:
                            case TypeCode.UInt32:
                            case TypeCode.UInt64:
                                _value = value;
                                break;
                            case TypeCode.Object:
                                _value = DataUtil.WrapperData(value);
                                break;
                            case TypeCode.Empty:
                            case TypeCode.DBNull:
                            default:
                                break;
                        }
                    }
                    else
                    {
                        _value = null;
                    }
                }
            }
            
        }
        private NamedCollection<NameValueObject> _dict = null;
        private NamedCollection<NameValueObject> Dictionary
        {
            get
            {
                return _dict;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Dictionary");
                }
                _dict = value;
            }
        }
        public SmartObject()
        {
            Dictionary = new NamedCollection<NameValueObject>();
        }
        internal SmartObject(IDictionary<string, object> dict)
            : this()
        {
            if (dict != null)
            {
                foreach (KeyValuePair<string, object> kv in dict)
                {
                    TrySet(kv.Key, kv.Value);
                }
            }
        }
        public bool IsDefined(string name)
        {
            return Dictionary.Contains(name);
        }
        public IDictionary<string, object> AsDictionary()
        {
            return Dictionary.ToDictionary(o => { return o.Name; }, o => {
                return DataUtil.RecoverData(o.Value);
            });
        }        
        public IList<KeyValuePair<string, object>> AsList()
        {
            return Dictionary.Select(t => { return new KeyValuePair<string, object>(t.Name, DataUtil.RecoverData(t.Value)); }).ToList();
        }
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return Dictionary.ToList().Select<NameValueObject, string>((item) => { return item.Name; });
        }
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            return TrySet(binder.Name, value);
        }
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return TryGet(binder.Name, out result);
        }
        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            object arg = indexes[0];
            if (arg is int)
            {
                int index = (int)arg;
                return TrySet(index, value);
            }
            else if (arg is string)
            {
                string name = arg as string;
                return TrySet(name, value);
            }
            else
            {
                return false;
            }
        }
        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            object arg = indexes[0];
            if (arg is int)
            {
                int index = (int)arg;
                return TryGet(index, out result);
            }
            else if (arg is string)
            {
                string name = arg as string;
                return TryGet(name, out result);
            }
            else
            {
                result = null;
                return false;
            }
        }
        public bool TrySet(string name, object value)
        {
            NameValueObject obj = null;
            if (Dictionary.TryGet(name, out obj))
            {
                obj.Value = value;
            }
            else
            {
                try
                {
                    Dictionary.Add(new NameValueObject() { Name = name, Value = value });
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }
        public bool TrySet(int index, object value)
        {
            if (index < 0 || index > Dictionary.Count)
            {
                return false;
            }
            Dictionary[index].Value = value;
            return true;
        }
        public bool TryGet(string name, out object result)
        {
            if (Dictionary.Contains(name))
            {
                result = Dictionary[name].Value;
                return true;
            }
            result = null;
            return false;
        }
        public bool TryGet(int index, out object result)
        {
            if (index < 0 || index > Dictionary.Count)
            {
                result = null;
                return false;
            }
            result = Dictionary[index].Value;
            return true;
        }
        public static SmartObject Parse(string json)
        {
            return SmartJosnSerializer.Deserialize<SmartObject>(json);
        }
        public string ToJson()
        {
            return SmartJosnSerializer.Serialize(this);
        }
    }
}
