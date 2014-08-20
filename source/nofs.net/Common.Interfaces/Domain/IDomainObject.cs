using System;

namespace Nofs.Net.Common.Interfaces.Domain
{
    public interface IDomainObject
    {
        Guid Id
        {
            get;
            set;
        }

        bool IsNew
        {
            get;
            set;
        }

        void SetIsNotNew();
    }

}
