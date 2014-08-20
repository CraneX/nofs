using System;
using Nofs.Net.Annotations;

namespace Nofs.Net.AnnotationDriver
{
    [Flags]
    public enum DomainOperatorObject
    {
        None = 0x0,
        CanRead = 0x1,
        CanWrite = 0x2,
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false)]
    public class DomainObjectAttribute : Attribute, IDomainObject
    {
        public DomainObjectAttribute()
            : this(DomainOperatorObject.CanRead | DomainOperatorObject.CanWrite)
        {
        }

        public DomainObjectAttribute(DomainOperatorObject operateor)
            : base()
        {
            Operator = operateor;
        }

        public DomainOperatorObject Operator
        {
            get;
            set;
        }

        /// <summary>
        /// Tells NOFS if the domain object can be written to
        ///@return	True if the domain object can be altered by NOFS, false if not
        /// </summary>
        /// <returns></returns>
        public bool CanWrite()
        {
            return (Operator & DomainOperatorObject.CanWrite) == DomainOperatorObject.CanWrite;
        }

        /**
         * Tells NOFS the type of the class that manages block layout if such a class exists
         * @return	By default, this resolves to object.class which acts as a NULL value. It can be set to an object that is marked as a layout object.
         */
        public Type LayoutObject()
        {
            return typeof(object);
        }

        public Type LayoutType()
        {
            return typeof(object);
        }
    }
}
