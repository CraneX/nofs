using System;

namespace Nofs.Net.nofs.metadata.interfaces
{
    public interface IGetterSetterPair
    {
        bool GetterExists();
        bool SetterExists();
        object Getter();
        void Setter(object value);
    }

}
