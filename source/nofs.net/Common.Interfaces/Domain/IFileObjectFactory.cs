using System;
using System.Collections.Generic;
using System.Reflection;

namespace Nofs.Net.Common.Interfaces.Domain
{
    public interface IFileObjectFactory
    {
        IFileObject BuildFileObject(string parentPath, object obj); 
        IFileObject BuildFileObject(string parentPath, object obj, MethodInfo method); 
        IFileObjectStat BuildStat(object obj); 
        IFileObjectStat BuildStat(object obj, MethodInfo method); 

        IEnumerable<string> GetChildNames(object parent); 
        IEnumerable<string> GetChildNames(object methodObj, MethodInfo parentMethod);
        object GetChildWithName(object methodObj, MethodInfo parentMethod, string name); 
        object GetChildWithName(object parent, string name); 
    }
}
