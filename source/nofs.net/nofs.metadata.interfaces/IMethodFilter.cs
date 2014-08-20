using System;
using System.Reflection;

namespace Nofs.Net.nofs.metadata.interfaces
{
    public interface IMethodFilter
    {
        bool UseMethod(object parent, MethodInfo method);// throws Exception;
    }
}
