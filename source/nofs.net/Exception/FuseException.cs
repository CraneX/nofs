using System;
using Mono.Unix.Native;

namespace Nofs.Net.Fuse
{
    public class FuseException : System.Exception
    {
        private int errno;

        public FuseException()
        {
        }

        public FuseException(string message)
            : base(message)
        {
        }


        public FuseException(string message, System.Exception cause)
            : base(message, cause)
        {

        }

        //
        public Errno ErrorNumber
        {
            get;
            set;
        }

        public FuseException initErrno(FuseErrno errno)
        {
            return initErrno(errno.ErrorNumber);
        }

        public FuseException initErrno(int errno)
        {
            this.errno = errno;

            return this;
        }

        public int getErrno()
        {
            return errno;
        }
    }
}
