using System;
using System.Collections.Generic;
using System.Reflection;
using Nofs.Net.Common.Interfaces.Cache;
using Nofs.Net.Common.Interfaces.Domain;
using Nofs.Net.nofs.metadata.interfaces;


namespace Nofs.Net.Cache.Impl
{
    public class SerializerBuilder : ITranslatorStrategy
    {
        private IRepresentationBuilder _builder;
        private IMethodFilter _filter;

        public SerializerBuilder(IRepresentationBuilder builder, IMethodFilter methodFilter)
        {
            _builder = builder;
            _filter = methodFilter;
        }


        public void DeserializeInto(string data, IFileObject obj)
        {
            object value = obj.GetValue();
            _builder.PopulateWith(data);
            FromData(_builder.FindChildByName(_builder.GetRoot(), value.GetType().Name), value);
        }


        public string Serialize(IFileObject obj)
        {
            object value = obj.GetValue();
            ToData(_builder.GetRoot(), value, value.GetType().Name);
            return _builder.TranslateToString();
        }

        //@SuppressWarnings("unchecked")
        private void ToData(IFolderReference parent, object obj, string name)
        {
            IFolderReference element = _builder.AddFolder(parent, name);

            foreach (MethodInfo method in obj.GetType().GetMethods())
            {
                if (_filter.UseMethod(obj, method)
                    && method.Name.StartsWith("get")
                    && method.Name.Length > 3
                    && method.GetParameters().Length == 0)
                {
                    object returnValue = method.Invoke(obj, new object[0]);
                    string methodName = method.Name.Substring(3);
                    if (returnValue == null)
                    {
                        _builder.AddFolder(element, methodName);
                    }
                    else if (isPrimitive(returnValue))
                    {
                        IFolderReference child = _builder.AddFolder(element, methodName);
                        _builder.SetFolderValue(child, returnValue.ToString());
                    }
                    else if (isCollection(returnValue))
                    {
                        ICollection<object> collection = (ICollection<object>)returnValue;
                        IFolderReference child = _builder.AddFolder(element, methodName);
                        foreach (object childObj in collection)
                        {
                            ToData(child, childObj, childObj.GetType().Name);
                        }
                    }
                    else
                    {
                        ToData(element, returnValue, methodName);
                    }
                }
            }
        }

        private void FromData(IFolderReference node, object obj)
        {
            foreach (MethodInfo method in obj.GetType().GetMethods())
            {
                if (_filter.UseMethod(obj, method)
                    && method.Name.StartsWith("set")
                    && method.Name.Length > 3
                    && method.GetGenericArguments().Length == 1)
                {
                    string methodName = method.Name.Substring(3);
                    IFolderReference targetNode = _builder.FindChildByName(node, methodName);
                    if (method.GetParameters().Length == 1 && isPrimitive(method.GetGenericArguments()[0]))
                    {
                        object o = primitiveFromString(_builder.GetFolderValue(targetNode), method.GetGenericArguments()[0]);
                        method.Invoke(obj, new object[1] { o });
                    }
                    else if (method.GetGenericArguments().Length == 1 && isCollection(method.GetParameters()[0]))
                    {
                        List<object> collection = new List<object>();
                        FromCollection(targetNode, collection);
                        method.Invoke(obj, collection.ToArray());
                    }
                    else
                    {
                        object child = constructObjectFromClassName(method.Name.Substring(3));
                        FromData(targetNode, child);
                        method.Invoke(obj, new object[1] { child });
                    }
                }
            }
        }

        private void FromCollection(IFolderReference node, ICollection<object> collection)
        {
            foreach (IFolderReference childElement in _builder.GetChildren(node))
            {
                object childObject = constructObjectFromClassName(childElement.Name);
                collection.Add(childObject);
                FromData(childElement, childObject);
            }
        }

        //@SuppressWarnings("unchecked")
        private static bool isCollection(object obj)
        {
            return obj is ICollection<object> ||
                   obj is IList<object>;
        }

        private static bool isCollection(Type c)
        {
            return
                c.IsAssignableFrom(typeof(ICollection<object>).GetType()) ||
                c.IsAssignableFrom(typeof(IList<object>).GetType());
        }

        private static bool isPrimitive(Type c)
        {
            return
                c.GetType().IsPrimitive ||
                c.IsAssignableFrom(typeof(string).GetType()) ||
                c.IsAssignableFrom(typeof(int).GetType()) ||
                c.IsAssignableFrom(typeof(DateTime).GetType()) ||
                c.IsAssignableFrom(typeof(float).GetType()) ||
                c.IsAssignableFrom(typeof(double).GetType()) ||
                c.IsAssignableFrom(typeof(long).GetType());
        }

        private static object primitiveFromString(string value, Type c)
        {
            if (c.IsAssignableFrom(typeof(string).GetType()))
            {
                return value;
            }
            else if (c.IsAssignableFrom(typeof(int).GetType()))
            {
                return int.Parse(value);
            }
            else if (c.IsAssignableFrom(typeof(float).GetType()))
            {
                return float.Parse(value);
            }
            else if (c.IsAssignableFrom(typeof(double).GetType()))
            {
                return double.Parse(value);
            }
            else if (c.IsAssignableFrom(typeof(long).GetType()))
            {
                return long.Parse(value);
            }
            else if (c.IsAssignableFrom(typeof(Double).GetType()))
            {
                return double.Parse(value);
            }
            else
            {
                throw new Exception("unknown primitive type: " + c.Name);
            }
        }

        private static Type getTypeFromName(string className)
        {
            return Type.GetType(className);
        }

        private static Type getClassFromName(string className) //throws ClassNotFoundException 
        {
            return Type.GetType(className).GetType();
        }

        private static object constructObjectFromClassName(string className)
        {
            System.Reflection.ConstructorInfo constructor = null;
            Type c = getTypeFromName(className);

            foreach (System.Reflection.ConstructorInfo cons in c.GetConstructors())
            {
                if (cons.GetParameters().Length == 0)
                {
                    constructor = cons;
                    break;
                }
            }

            if (constructor == null)
            {
                throw new Exception("could not find default constructor for " + c.GetType().Name);
            }
            return Activator.CreateInstance(constructor.GetType());
        }

        private static bool isPrimitive(object obj)
        {
            return
                obj.GetType().IsPrimitive ||
                obj is string ||
                obj is int ||
                obj is DateTime ||
                obj is float ||
                obj is double ||
                obj is long;
        }
    }

}
