using System;
using Nofs.Net.Annotations;

namespace Nofs.Net.AnnotationDriver
{
    [Flags]
    public enum FolderOperatorObject
    {
        None = 0x0,
        CanAdd = 0x1,
        CanModify = 0x2,
        CanRemove = 0x4,
      //  ContentPage = 0x8
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false)]
    public class FolderObjectAttribute : Attribute, IFolderObject
    {
        public FolderObjectAttribute()
            :this(FolderOperatorObject.CanAdd | FolderOperatorObject.CanRemove)
        {
        }

        public FolderObjectAttribute(FolderOperatorObject operateor)
            :base()
        {
            Operator = operateor;
        }

        public FolderOperatorObject Operator
        {
            get;
            set;
        }

        /// <summary>
        ///  Tells NOFS if objects marked with DomainObject or FolderObject annotations can be added
        /// @return	True if objects can be added, false otherwise
        /// </summary>
        /// <returns></returns>
        public bool CanAdd()
        {
            return (Operator & FolderOperatorObject.CanAdd) == FolderOperatorObject.CanAdd;
        }

        /// <summary>
        ///  
        ///  Tells NOFS if objects can be removed from the folder 
        ///  @return True if objects can be removed, false otherwise
        ///
        /// </summary>
        /// <returns></returns>
        public bool CanRemove()
        {
            return (Operator & FolderOperatorObject.CanRemove) == FolderOperatorObject.CanRemove;
        }

        public string CanAddMethod()
        {
            // default "";
            return string.Empty;
        }

        public string CanRemoveMethod() 
        {
            // default "";
            return string.Empty; 
        }

        public string ChildTypeFilterMethod()
        {
            // default "";
            return string.Empty;
        }
    }
}
