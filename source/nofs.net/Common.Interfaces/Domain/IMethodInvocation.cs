using System;
using System.Collections.Generic;

namespace Nofs.Net.Common.Interfaces.Domain
{
    public interface IMethodInvocation
    {
        string GetPwd();
        string GetPath();
        string MethodName();
        List<IMethodParameter> GetParameters();
        string ToXML();
    }
}
