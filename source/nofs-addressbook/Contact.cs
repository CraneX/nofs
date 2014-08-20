using System;
using Nofs.Net.AnnotationDriver;
using Nofs.Net.Common.Interfaces.Library;
using System.Xml.Serialization;

namespace Nofs.Net.nofs_addressbook
{
    [DomainObject]
    [Serializable]
    public class Contact
    {
        private String _name;
        private String _phoneNumber;

        private IDomainObjectContainer _container;

        public Contact()
        {
        }

        public Contact(String name, String phone)
        {
            _name = name;
            _phoneNumber = phone;
        }


        [ProvidesName]
        public String Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                if (_container != null)
                {
                    _container.ObjectChanged(this);
                }
            }
        }

        public String PhoneNumber
        {
            get
            {
                return _phoneNumber;
            }
            set
            {
                _phoneNumber = value;
                if (_container != null)
                {
                    _container.ObjectChanged(this);
                }
            }
        }

        [NeedsContainer]
        [XmlIgnore]
        public IDomainObjectContainer Container
        {
            get
            {
                return _container;
            }
            set
            {
                _container = value;
            }
        }
    }
}
