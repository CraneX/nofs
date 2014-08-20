using System;
using System.Collections.Generic;
using System.Reflection;
using Nofs.Net.Common.Interfaces.Domain;
using System.Text;
using Nofs.Net.Utils;

namespace Nofs.Net.Domain.Impl
{
    public class MethodInvocation : IMethodInvocation
    {
        private List<IMethodParameter> _parameters;
        private string _pwd;
        private string _path;
        private string _method;

        public MethodInvocation(string method, string pwd, string path, List<IMethodParameter> parameters)
        {
            _method = method;
            _pwd = pwd;
            _path = path;
            _parameters = parameters;
        }

        public MethodInvocation(string xml)
        {
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
            int fifthQuote = xml.IndexOf('\"', fourthQuote + 1);
            if (fifthQuote == (-1))
            {
                throw new System.Exception("invalid method invocation format");
            }
            int sixthQuote = xml.IndexOf('\"', fifthQuote + 1);
            if (sixthQuote == (-1))
            {
                throw new System.Exception("invalid method invocation format");
            }
            _pwd = xml.Substring(firstQuote + 1, secondQuote);
            _path = xml.Substring(thirdQuote + 1, fourthQuote);
            _method = xml.Substring(fifthQuote + 1, sixthQuote);
            _parameters = new List<IMethodParameter>();
            for (int i = xml.IndexOf("<Parameter", 0); i > 0 && i < xml.Length; i = xml.IndexOf("<Parameter", i + 1))
            {
                int end = xml.IndexOf("/>", i);
                string parameterXML = xml.Substring(i, end + 2);
                _parameters.Add(new MethodParameter(parameterXML));
            }
        }

        public MethodInvocation(string path, MethodInfo method)
        {
            _parameters = new List<IMethodParameter>();
            _path = path;
            _method = method.Name;
            int index = (0);

            foreach (ParameterInfo item in method.GetParameters())
            {
                _parameters.Add(new MethodParameter(item.ParameterType, index));
            }
        }

        public List<IMethodParameter> GetParameters()
        {
            return _parameters;
        }

        public string GetPwd()
        {
            return _pwd;
        }


        public string GetPath()
        {
            return _path;
        }


        public string MethodName()
        {
            return _method;
        }


        public string ToXML()
        {
            StringBuilder buffer = new StringBuilder();
            buffer.Append("<MethodInvocation Pwd=\"");
            buffer.Append(_pwd == null ? "" : _pwd);
            buffer.Append("\" Path=\"");
            buffer.Append(XmlUtil.EncodeXml(_path));
            buffer.Append("\" Name=\"");
            buffer.Append(_method);
            buffer.Append("\">");

            int index = 0;
            foreach (IMethodParameter parameter in _parameters)
            {
                buffer.Append(parameter.ToXML(++index));
            }
            buffer.Append("</MethodInvocation>");
            return buffer.ToString();
        }

    }
}
