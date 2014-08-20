using System;
using System.Text;
using Nofs.Net.Common.Interfaces.Cache;
using Nofs.Net.Common.Interfaces.Domain;
using Nofs.Net.Domain.Impl;
using System.Collections.Generic;

namespace Nofs.Net.Cache.Impl
{
    public class ExecutableBuilder : ITranslatorStrategy
    {
        public void DeserializeInto(string data, IFileObject obj)
        {
            throw new NotSupportedException();
        }

        private static void WriteLine(StringBuilder buffer, string line)
        {
            buffer.AppendLine(line);
        }

        public IMethodInvocation BuildMethodObject(IFileObject obj)
        {
            if (!obj.HasMethod())
            {
                throw new System.Exception("File object does not have a method");
            }
            else if (obj.GetMethod() == null)
            {
                throw new System.Exception("File object's method is null");
            }
            return new MethodInvocation(obj.Folder, obj.GetMethod());
        }

        public string Serialize(IFileObject sender)
        {
            throw new NotImplementedException();
        }
    }

}
