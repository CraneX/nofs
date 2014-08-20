using System;

namespace Nofs.Net.Annotations
{
    public interface IDomainObject
    {

        /**
         * Tells NOFS if the domain object can be written to
         * @return	True if the domain object can be altered by NOFS, false if not
         */
        bool CanWrite(); //default true;

        /**
         * Tells NOFS the type of the class that manages block layout if such a class exists
         * @return	By default, this resolves to object.class which acts as a NULL value. It can be set to an object that is marked as a layout object.
         */
        Type LayoutObject();// default object.class;
    }




}
