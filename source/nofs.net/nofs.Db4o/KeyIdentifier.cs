using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nofs.Net.Common.Interfaces.Cache;

namespace Nofs.Net.nofs.Db4o
{
    public class KeyIdentifier : IKeyIdentifier
    {
        private Guid _id;
        private object _ref;

        public KeyIdentifier(object reference)
            :this(Guid.NewGuid(), reference)

        {
        }

        public KeyIdentifier(Guid id, object reference)
        {
            _ref = reference;
            _id = id;
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

        public object Reference
        {
            get
            {
                return _ref as object;
            }
            set
            {
                _ref = value;
            }
        }

    }
}
