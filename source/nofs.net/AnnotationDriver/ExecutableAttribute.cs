using System;
using Nofs.Net.Annotations;

namespace Nofs.Net.AnnotationDriver
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ExecutableAttribute : Attribute
    {
        public ExecutableAttribute()
            : base()
        {
        }

        public static bool IsExecutable()
        {
            return false;
        }
    }
}
