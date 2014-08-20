using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nofs.Net.Annotations;

namespace Nofs.Net.AnnotationDriver
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
    public class NeedsContainerManagerAttribute : Attribute
    {
        public NeedsContainerManagerAttribute()
            : base()
        {
        }

        public static bool IsNeedsContainerManager()
        {
            return false;
        }

    }
}
