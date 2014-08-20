using System;
using Nofs.Net.Common.Interfaces.Domain;

namespace Nofs.Net.Common.Interfaces.Cache
{
    public interface ITranslatorStrategy
    {
        string Serialize(IFileObject sender);
        void DeserializeInto(string data, IFileObject sender);
    }

}
