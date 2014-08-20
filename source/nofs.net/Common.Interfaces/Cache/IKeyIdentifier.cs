using System;

namespace Nofs.Net.Common.Interfaces.Cache
{
    public interface IKeyIdentifier
    {
        Guid Id
        {
            get;
        }

        object Reference
        {
            get;
        }
    }
}
