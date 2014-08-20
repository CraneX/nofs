using System;
using Nofs.Net.Annotations;

namespace Nofs.Net.AnnotationDriver
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class RootFolderObjectAttribute : Attribute, IRootFolderObject
    {
        public RootFolderObjectAttribute()
            : base()
        {
        }

        public static bool IsRootFolderObject()
        {
            //TODO:
            return false;
        }
    }
}
