using System;
using Nofs.Net.Common.Interfaces.Cache;
using Nofs.Net.Common.Interfaces.Domain;
using Nofs.Net.nofs.metadata.interfaces;

namespace Nofs.Net.Cache.Impl
{
    public class TranslatorFactory {
	private IMethodFilter _methodFilter;
	
	public TranslatorFactory(IMethodFilter methodFilter) {
		_methodFilter = methodFilter;
	}
	
	public ITranslatorStrategy CreateTranslator(IFileObject fileObject) 
    {
		if(fileObject.GetGenerationType() == GenerationType.DATA_FILE) {
			return new SerializerBuilder(new XmlRepresentationBuilder(), _methodFilter);
		} else if(fileObject.GetGenerationType() == GenerationType.EXECUTABLE) {
			return new ExecutableBuilder();
		} else {
			throw new Exception("not supported");
		}
	}
}

}
