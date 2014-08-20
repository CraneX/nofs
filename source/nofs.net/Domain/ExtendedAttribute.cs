using System;
using Nofs.Net.Common.Interfaces.Domain;

namespace Nofs.Net.Domain.Impl
{
    public class ExtendedAttribute : BaseDomainObject, IExtendedAttribute
    {
        private string _name;
        private byte[] _value;

        public ExtendedAttribute(Guid parentID)
            : base()
        {
            _name = "";
            _value = null;
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public byte[] Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

    }
}
