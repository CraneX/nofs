using System;
using Nofs.Net.Common.Interfaces.Domain;

namespace Nofs.Net.Domain.Impl
{
    public class BaseDomainObject : IDomainObject
    {
        private Guid _id;
        private bool _isNew;

        protected BaseDomainObject()
        {
            _id = Guid.Empty;
            _isNew = false;
        }

        public Guid Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }


        public bool IsNew
        {
            get
            {
                return _isNew || _id == Guid.Empty;
            }
            set
            {
                _isNew = value;
            }
        }

        public void SetIsNotNew()
        {
            _isNew = false;
            if (IsNew)
            {
                throw new Exception("ID was not set!!!");
            }
        }

    }
}
