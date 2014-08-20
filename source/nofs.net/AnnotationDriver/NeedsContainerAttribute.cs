using System;
using Nofs.Net.Annotations;

namespace Nofs.Net.AnnotationDriver
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
    public class NeedsContainerAttribute : Attribute
    {
        public NeedsContainerAttribute()
            :base()
        {
        }

        public static bool IsNeedsContainer()
        {
            return true;
        }
    }
}
