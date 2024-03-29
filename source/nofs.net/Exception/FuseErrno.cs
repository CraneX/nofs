﻿using System;
using Mono.Unix.Native;

namespace Nofs.Net.Fuse
{
    public sealed class FuseErrno
    {
        public const int NOERROR = 0;

        // extended attributes support needs these...
        public const int EPERM = 1;    /* Operation not permitted */
        public const int ENOENT = 2;    /* No such file or directory */
        public const int ESRCH = 3;    /* No such process */
        public const int EINTR = 4;    /* Interrupted system call */
        public const int EIO = 5;    /* I/O error */
        public const int ENXIO = 6;    /* No such device or address */
        public const int E2BIG = 7;    /* Arg list too long */
        public const int ENOEXEC = 8;    /* Exec format error */
        public const int EBADF = 9;    /* Bad file number */
        public const int ECHILD = 10;    /* No child processes */
        public const int EAGAIN = 11;    /* Try again */
        public const int ENOMEM = 12;    /* Out of memory */
        public const int EACCES = 13;    /* Permission denied */
        public const int EFAULT = 14;    /* Bad address */
        public const int ENOTBLK = 15;    /* Block device required */
        public const int EBUSY = 16;    /* Device or resource busy */
        public const int EEXIST = 17;    /* File exists */
        public const int EXDEV = 18;    /* Cross-device link */
        public const int ENODEV = 19;    /* No such device */
        public const int ENOTDIR = 20;    /* Not a directory */
        public const int EISDIR = 21;    /* Is a directory */
        public const int EINVAL = 22;    /* Invalid argument */
        public const int ENFILE = 23;    /* File table overflow */
        public const int EMFILE = 24;    /* Too many open files */
        public const int ENOTTY = 25;    /* Not a typewriter */
        public const int ETXTBSY = 26;    /* Text file busy */
        public const int EFBIG = 27;    /* File too large */
        public const int ENOSPC = 28;    /* No space left on device */
        public const int ESPIPE = 29;    /* Illegal seek */
        public const int EROFS = 30;    /* Read-only file system */
        public const int EMLINK = 31;    /* Too many links */
        public const int EPIPE = 32;    /* Broken pipe */
        public const int EDOM = 33;    /* Math argument out of domain of func */
        public const int ERANGE = 34;    /* Math result not representable */
        public const int EDEADLK = 35;    /* Resource deadlock would occur */
        public const int ENAMETOOLONG = 36;    /* File name too long */
        public const int ENOLCK = 37;    /* No record locks available */
        public const int ENOSYS = 38;    /* Function not implemented */
        public const int ENOTEMPTY = 39;    /* Directory not empty */
        public const int ELOOP = 40;    /* Too many symbolic links encountered */
        public const int EWOULDBLOCK = EAGAIN;    /* Operation would block */
        public const int ENOMSG = 42;    /* No message of desired type */
        public const int EIDRM = 43;    /* Identifier removed */
        public const int ECHRNG = 44;    /* Channel number out of range */
        public const int EL2NSYNC = 45;    /* Level 2 not synchronized */
        public const int EL3HLT = 46;    /* Level 3 halted */
        public const int EL3RST = 47;    /* Level 3 reset */
        public const int ELNRNG = 48;    /* Link number out of range */
        public const int EUNATCH = 49;    /* Protocol driver not attached */
        public const int ENOCSI = 50;    /* No CSI structure available */
        public const int EL2HLT = 51;    /* Level 2 halted */
        public const int EBADE = 52;    /* Invalid exchange */
        public const int EBADR = 53;    /* Invalid request descriptor */
        public const int EXFULL = 54;    /* Exchange full */
        public const int ENOANO = 55;    /* No anode */
        public const int EBADRQC = 56;    /* Invalid request code */
        public const int EBADSLT = 57;    /* Invalid slot */
        public const int EDEADLOCK = EDEADLK;
        public const int EBFONT = 59;    /* Bad font file format */
        public const int ENOSTR = 60;    /* Device not a stream */
        public const int ENODATA = 61;    /* No data available */
        public const int ETIME = 62;    /* Timer expired */
        public const int ENOSR = 63;    /* Out of streams resources */
        public const int ENONET = 64;    /* Machine is not on the network */
        public const int ENOPKG = 65;    /* Package not installed */
        public const int EREMOTE = 66;    /* object is remote */
        public const int ENOLINK = 67;    /* Link has been severed */
        public const int EADV = 68;    /* Advertise error */
        public const int ESRMNT = 69;    /* Srmount error */
        public const int ECOMM = 70;    /* Communication error on send */
        public const int EPROTO = 71;    /* Protocol error */
        public const int EMULTIHOP = 72;    /* Multihop attempted */
        public const int EDOTDOT = 73;    /* RFS specific error */
        public const int EBADMSG = 74;    /* Not a data message */
        public const int EOVERFLOW = 75;    /* Value too large for defined data type */
        public const int ENOTUNIQ = 76;    /* Name not unique on network */
        public const int EBADFD = 77;    /* File descriptor in bad state */
        public const int EREMCHG = 78;    /* Remote address changed */
        public const int ELIBACC = 79;    /* Can not access a needed shared library */
        public const int ELIBBAD = 80;    /* Accessing a corrupted shared library */
        public const int ELIBSCN = 81;    /* .lib section in a.out corrupted */
        public const int ELIBMAX = 82;    /* Attempting to link in too many shared libraries */
        public const int ELIBEXEC = 83;    /* Cannot exec a shared library directly */
        public const int EILSEQ = 84;    /* Illegal byte sequence */
        public const int ERESTART = 85;    /* Interrupted system call should be restarted */
        public const int ESTRPIPE = 86;    /* Streams pipe error */
        public const int EUSERS = 87;    /* Too many users */
        public const int ENOTSOCK = 88;    /* Socket operation on non-socket */
        public const int EDESTADDRREQ = 89;    /* Destination address required */
        public const int EMSGSIZE = 90;    /* Message too long */
        public const int EPROTOTYPE = 91;    /* Protocol wrong type for socket */
        public const int ENOPROTOOPT = 92;    /* Protocol not available */
        public const int EPROTONOSUPPORT = 93;    /* Protocol not supported */
        public const int ESOCKTNOSUPPORT = 94;    /* Socket type not supported */
        public const int EOPNOTSUPP = 95;    /* Operation not supported on transport endpoint */
        public const int EPFNOSUPPORT = 96;    /* Protocol family not supported */
        public const int EAFNOSUPPORT = 97;    /* Address family not supported by protocol */
        public const int EADDRINUSE = 98;    /* Address already in use */
        public const int EADDRNOTAVAIL = 99;    /* Cannot assign requested address */
        public const int ENETDOWN = 100;    /* Network is down */
        public const int ENETUNREACH = 101;    /* Network is unreachable */
        public const int ENETRESET = 102;    /* Network dropped connection because of reset */
        public const int ECONNABORTED = 103;    /* Software caused connection abort */
        public const int ECONNRESET = 104;    /* Connection reset by peer */
        public const int ENOBUFS = 105;    /* No buffer space available */
        public const int EISCONN = 106;    /* Transport endpoint is already connected */
        public const int ENOTCONN = 107;    /* Transport endpoint is not connected */
        public const int ESHUTDOWN = 108;    /* Cannot send after transport endpoint shutdown */
        public const int ETOOMANYREFS = 109;    /* Too many references: cannot splice */
        public const int ETIMEDOUT = 110;    /* Connection timed out */
        public const int ECONNREFUSED = 111;    /* Connection refused */
        public const int EHOSTDOWN = 112;    /* Host is down */
        public const int EHOSTUNREACH = 113;    /* No route to host */
        public const int EALREADY = 114;    /* Operation already in progress */
        public const int EINPROGRESS = 115;    /* Operation now in progress */
        public const int ESTALE = 116;    /* Stale NFS file handle */
        public const int EUCLEAN = 117;    /* Structure needs cleaning */
        public const int ENOTNAM = 118;    /* Not a XENIX named type file */
        public const int ENAVAIL = 119;    /* No XENIX semaphores available */
        public const int EISNAM = 120;    /* Is a named type file */
        public const int EREMOTEIO = 121;    /* Remote I/O error */
        public const int EDQUOT = 122;    /* Quota exceeded */
        public const int ENOMEDIUM = 123;    /* No medium found */
        public const int EMEDIUMTYPE = 124;    /* Wrong medium type */

        public const int ENOATTR = ENODATA;   /* No such attribute */
        public const int ENOTSUPP = 524;      /* Operation is not supported*/

        public int ErrorNumber
        {
            private set;
            get;
        }

        public FuseErrno(Errno error)
        {
            ErrorNumber = (int)error;
        }

        public static implicit operator int(FuseErrno en)
        {
            return en.ErrorNumber;
        }

    }
}
