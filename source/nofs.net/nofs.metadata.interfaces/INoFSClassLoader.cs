using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nofs.Net.nofs.metadata.interfaces
{
    public interface INoFSClassLoader
    {
        Type LoadClass(string className);
        IEnumerable<Type> LoadClassesWithAnnotation(MarkerTypes markerType);
        object Create(string className);
    }
}
