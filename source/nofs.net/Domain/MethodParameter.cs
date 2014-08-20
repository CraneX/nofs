using System;
using Nofs.Net.Common.Interfaces.Domain;
using System.Text;

namespace Nofs.Net.Domain.Impl
{
    public class MethodParameter : IMethodParameter
    {
        private string _parameter;
        private Type _type;
        private object _value;
        private bool _hasValue = false;

        public MethodParameter(string value, string type, int index)// throws Exception 
        {
            _value = value;
            _type = GetType(type);
            //_parameter = "$" + index;
            _parameter = value;
        }

        public MethodParameter(string xml) 
        {
            _hasValue = true;
            int firstQuote = xml.IndexOf('\"', 0);
            if (firstQuote == (-1))
            {
                throw new System.Exception("invalid method invocation format");
            }
            int secondQuote = xml.IndexOf('\"', firstQuote + 1);
            if (secondQuote == (-1))
            {
                throw new System.Exception("invalid method invocation format");
            }
            int thirdQuote = xml.IndexOf('\"', secondQuote + 1);
            if (thirdQuote == (-1))
            {
                throw new System.Exception("invalid method invocation format");
            }
            int fourthQuote = xml.IndexOf('\"', thirdQuote + 1);
            if (fourthQuote == (-1))
            {
                throw new System.Exception("invalid method invocation format");
            }
            _parameter = xml.Substring(firstQuote + 1, secondQuote);
            string typeStr = xml.Substring(thirdQuote + 1, fourthQuote);
            _type = GetType(typeStr);
        }

        private static Type GetType(string className)  
        {
            Type type;
            if ("Complex".CompareTo(className) == 0)
            {
                type = null;
            }
            else if (typeof(long).Name.CompareTo(className) == 0)
            {
                type = typeof(long);
            }
            else if (typeof(byte).Name.CompareTo(className) == 0)
            {
                type = typeof(byte);
            }
            else if (typeof(short).Name.CompareTo(className) == 0)
            {
                type = typeof(short);
            }
            else if (typeof(int).Name.CompareTo(className) == 0)
            {
                type = typeof(int);
            }
            else if (typeof(float).Name.CompareTo(className) == 0)
            {
                type = typeof(float);
            }
            else if (typeof(double).Name.CompareTo(className) == 0)
            {
                type = typeof(double);
            }
            else if (typeof(bool).Name.CompareTo(className) == 0)
            {
                type = typeof(bool);
            }
            else if (typeof(char).Name.CompareTo(className) == 0)
            {
                type = typeof(char);
            }
            else if (typeof(string).Name.CompareTo(className) == 0)
            {
                type = typeof(string);
            }
            else if (typeof(Byte).Name.CompareTo(className) == 0)
            {
                type = typeof(Byte);
            }
            else if (typeof(Double).Name.CompareTo(className) == 0)
            {
                type = typeof(Double);
            }
            else if (typeof(Boolean).Name.CompareTo(className) == 0)
            {
                type = typeof(Boolean);
            }
            else
            {
                throw new System.Exception("could not figure out type: " + className);
            }
            return type;
        }

        public MethodParameter(Type type, int index)
        {
            _type = GetTypeValue(type);
            _parameter = "$" + index;
            _value = null;
        }

        private static Type GetTypeValue(Type type)
        {
            Type objType;
            if (typeof(long) == type)
            {
                objType = type;
            }
            else if (typeof(Byte) == type || typeof(byte) == type)
            {
                objType = type;
            }
            else if (typeof(short) == type)
            {
                objType = type;
            }
            else if (typeof(Int32) == type || typeof(int) == type)
            {
                objType = type;
            }
            else if (typeof(float) == type)
            {
                objType = type;
            }
            else if (typeof(Double) == type || typeof(double) == type)
            {
                objType = type;
            }
            else if (typeof(Boolean) == type || typeof(bool) == type)
            {
                objType = type;
            }
            else if (typeof(char) == type)
            {
                objType = type;
            }
            else if (typeof(string) == type)
            {
                objType = type;
            }
            else
            {
                objType = null;
            }
            return objType;
        }

        
        public object ConvertToValue() // throws Exception 
        {
            if (!_hasValue || IsComplex())
            {
                return null;
            }
            else if (_value == null)
            {
                if (typeof(long) == _type)
                {
                    _value = long.Parse(_parameter);
                }
                else if (typeof(Byte) == _type || typeof(byte) == _type)
                {
                    _value = byte.Parse(_parameter);
                }
                else if (typeof(short) == _type)
                {
                    _value = short.Parse(_parameter);
                }
                else if (typeof(Int32) == _type || typeof(int) == _type)
                {
                    _value = int.Parse(_parameter);
                }
                else if (typeof(float) == _type)
                {
                    _value = float.Parse(_parameter);
                }
                else if (typeof(Double) == _type || typeof(double) == _type)
                {
                    _value = double.Parse(_parameter);
                }
                else if (typeof(Boolean) == _type || typeof(bool) == _type)
                {
                    _value = bool.Parse(_parameter);
                }
                else if (typeof(char) == _type)
                {
                    _value = _parameter.ToCharArray()[0];
                }
                else if (typeof(string) == _type)
                {
                    _value = _parameter;
                }
                else
                {
                    throw new System.Exception("unknown type: " + _type.Name);
                }
            }
            return _value;
        }

        
        public Type GetParameterType()
        {
            return _type;
        }

        
        public string GetValue()
        {
            return _parameter;
        }

        
        public bool IsComplex()
        {
            return _type == null;
        }

        
        public string ToXML(int argumentNumber)
        {
            StringBuilder buffer = new StringBuilder();
            buffer.Append("<Parameter Value=\"");
            if (IsComplex())
            {
                buffer.Append("$" + argumentNumber);
                buffer.Append("\" Type=\"Complex\"/>");
            }
            else
            {
                buffer.Append(_parameter);
                buffer.Append("\" Type=\"");
                buffer.Append(_type.Name);
                buffer.Append("\"/>");
            }
            return buffer.ToString();
        }

    }
}
