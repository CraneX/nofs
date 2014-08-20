using System;
using System.Collections.Generic;
using Nofs.Net.Common.Interfaces.Library;
using System.Reflection;

namespace Nofs.Net.Common.Interfaces.Domain
{
    public interface IFileObject : IDomainObject
    {
        string Folder
        {
            get;
        }

        string FileName
        {
            get;
            set;
        }

        object Content
        {
            get;
        }

        bool CanSetName(); 

        GenerationType GetGenerationType();

        IFileObjectStat GetStat();

        

        bool CanWrite(); 

        bool CanMknod(); 

        bool CanMkdir(); 

        bool CanRmdir(); 

        bool CanDeleteChildren(); 

        MethodInfo GetMethod();

        bool HasMethod();

        object GetValue(); 

        bool IsChildTypeCompatible(IFileObject possibleChild); 

        bool SupportsDirectIO(); 

        IProvidesUnstructuredData GetReadWriteInterface(); 
    }

}
