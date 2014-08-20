using System;
using Nofs.Net.Common.Interfaces.Domain;
using Nofs.Net.Common.Interfaces.Library;

namespace Nofs.Net.Domain.Impl
{
    public class StatLazyLoader
    {
        private IFileObjectStat _stat;
        private FileObject _file;
        private IStatMapper _statMapper;

        public StatLazyLoader(IStatMapper statMapper, FileObject file)
        {
            _stat = null;
            _file = file;
            _statMapper = statMapper;
        }

        public IFileObjectStat GetStat()  
        {
            if (_stat == null)
            {
                _stat = _statMapper.Load(_file.Id, _file.GetName());
            }
            return _stat;
        }
    }

}
