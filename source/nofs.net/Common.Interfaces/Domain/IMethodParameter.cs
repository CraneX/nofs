using System;

namespace Nofs.Net.Common.Interfaces.Domain
{
    public interface IMethodParameter
    {
        object ConvertToValue();
        string GetValue();
        Type GetParameterType();
        bool IsComplex();
        string ToXML(int argumentNumber);
    }
}
