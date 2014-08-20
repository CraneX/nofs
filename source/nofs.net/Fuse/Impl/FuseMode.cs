using System;

namespace Nofs.Net.Fuse.Impl
{
    public sealed class FuseMode
    {
        public const int __S_ISUID = 04000;
        public const int __S_ISGID = 02000;
        public const int __S_ISVTX = 01000;
        public const int __S_IREAD = 0400;
        public const int __S_IWRITE = 0200;
        public const int __S_IEXEC = 0100;

        public const int S_IRUSR = __S_IREAD;
        public const int S_IWUSR = __S_IWRITE;
        public const int S_IXUSR = __S_IEXEC;
        public const int S_IRWXU = (__S_IREAD | __S_IWRITE | __S_IEXEC);

        public const int S_IRGRP = (S_IRUSR >> 3);
        public const int S_IWGRP = (S_IWUSR >> 3);
        public const int S_IXGRP = (S_IXUSR >> 3);
        public const int S_IRWXG = (S_IRWXU >> 3);

        public const int S_IROTH = (S_IRGRP >> 3);
        public const int S_IWOTH = (S_IWGRP >> 3);
        public const int S_IXOTH = (S_IXGRP >> 3);
        public const int S_IRWXO = (S_IRWXG >> 3);
    }

}
