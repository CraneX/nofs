using System;
using System.Collections.Generic;
using System.Linq;
using Nofs.Net.AnnotationDriver;
using Nofs.Net.Common.Interfaces.Library;

namespace Nofs.Net.nofs_addressbook
{
    [RootFolderObject]
    [Serializable]
    public class Book
    {
        private IDomainObjectContainer _bookContainer;
        private IDomainObjectContainerManager _containerManager;

        public Book()
        {
#if (DEBUG)
            _names.Add(Guid.NewGuid().ToString());
#endif
            Information = new BookInfo();
        }

        #region only for test

        private List<string> _names = new List<string>();

        [DomainObject]
        public string[] Names
        {
            get
            {
                return _names.ToArray();
            }
        }

        public IList<string> AliasName
        {
            get
            {
                return _names;
            }
        }

        #endregion

        [NeedsContainer]
        public IDomainObjectContainer Container
        {
            get
            {
                return _bookContainer;
            }
            set
            {
                _bookContainer = value;
            }
        }

        [NeedsContainerManager]
        public IDomainObjectContainerManager ContainerManager
        {
            get
            {
                if (_containerManager == null)
                {
                    throw new System.Exception("container manager is null");
                }
                return _containerManager;
            }
            set
            {
                _containerManager = value;
            }
        }

        private IDomainObjectContainer CategoryDomainObjectContainer
        {
            get
            {
                return ContainerManager.GetContainer(typeof(Category));
            }
        }

        public IEnumerable<Category> Categories
        {
            [FolderObject]
            get
            {
                foreach (Category item in CategoryDomainObjectContainer.GetAllInstances<Category>())
                {
                    yield return item;
                }
            }
        }

        private IDomainObjectContainer ContactDomainObjectContainer
        {
            get
            {
                return ContainerManager.GetContainer(typeof(Contact));
            }
        }

        [FolderObject]
        public IEnumerable<Contact> Contacts 
        {
            get
            {
                foreach (Contact item in ContactDomainObjectContainer.GetAllInstances<Contact>())
                {
                    yield return item;
                }
            }
        }

        [DomainObject]
        public BookInfo Information
        {
            private set;
            get;
        }


        [FolderObject(FolderOperatorObject.CanAdd | FolderOperatorObject.CanRemove)] 
        public IEnumerable<Contact> getFirstTenContacts() 
        {
            return ContactDomainObjectContainer.GetAllInstances<Contact>().Take<Contact>(10);
        }

        public Contact SearchbyName(String name)
        {
            return Contacts.FirstOrDefault(item => item.Name == name);
        }

        [Executable]
        public Contact AddAContact(String name, String phone) 
        {
            Contact contact = ContactDomainObjectContainer.NewPersistentInstance<Contact>();
            contact.Name = name;
            contact.PhoneNumber = phone;
            _bookContainer.ObjectChanged(this);
            return contact;
        }

        [Executable]
        public void RemoveAContact(Contact contact) 
        {
            if (contact != null)
            {
                ContactDomainObjectContainer.Remove(contact);
                _bookContainer.ObjectChanged(this);
            }
            else
            {
                Console.WriteLine("Remove A Contact: failed!");
            }
        }

        [Executable]
        public Contact RenameAContact(Contact contact, String newName) 
        {
            if (contact != null)
            {
                String oldName = contact.Name;
                contact.Name = newName;
                ContactDomainObjectContainer.ObjectRenamed(contact, oldName, newName);
                ContactDomainObjectContainer.ObjectChanged(contact);
            }
            else
            {
                Console.WriteLine("Rename A Contact: failed!");
            }
            return contact;
        }
    }

}
