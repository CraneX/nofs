using System;
using Nofs.Net.Annotations;

namespace Nofs.Net.AnnotationDriver
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ProvidesNameAttribute : Attribute, IProvidesName
    {
        public ProvidesNameAttribute()
            :base()
        {
        }

        public static bool IsProvidesName()
        {
            return false;
        }
    }
}
