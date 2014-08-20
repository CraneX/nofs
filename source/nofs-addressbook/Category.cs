using System;
using System.Collections.Generic;
using Nofs.Net.AnnotationDriver;

namespace Nofs.Net.nofs_addressbook
{
    [DomainObject]
    [FolderObject(FolderOperatorObject.CanAdd | FolderOperatorObject.CanRemove)]
    public class Category : List<Contact>
    {
        [ProvidesName]
        public String Name
        {
            set;
            get;
        }
    }
}
