using System;

namespace Nofs.Net.Annotations
{
    public interface IFolderObject
    {
        /**
	 * Tells NOFS if objects marked with DomainObject or FolderObject annotations can be added
	 * @return	True if objects can be added, false otherwise
	 */
        bool CanAdd(); // default true;

        /**
         * Tells NOFS if objects can be removed from the folder 
         * @return	True if objects can be removed, false otherwise
         */
        bool CanRemove(); // default true;

        /**
         * If CanAdd() is not sufficient, CanAddMethod points to a method that NOFS can query to tell
         * if a file or folder can or cannot be added to the folder
         * @return	True if objects can be added, false otherwise
         */
        string CanAddMethod(); // default "";

        /**
         * If CanRemove() is not sufficient, CanRemoveMethod points to a method that NOFs can query to tell
         * if a file or folder can or cannot be removed from the folder
         * @return	True if objects can be removed, false otherwise
         */

        string CanRemoveMethod(); // default "";

        /**
         * points to a method name that has signature: bool FilterChildType(Class<?> possibleType)
         * If a folder type's inner type is abstract and has more than one possible subclass, this method
         * can be called to remove the ambiguity.
         * 
         * For example:
         * 
         * \@DomainObject
         * abstract class MyBaseFile {}
         * 
         * \@DomainObject
         * class MyFile1 extends MyBaseFile {}
         * 
         * \@DomainObject
         * class MyFile2 extends MyBaseFile {}
         * 
         * \@DomainObject
         * \@FolderObject
         * class MyFolder extends LinkedList<MyBaseFile> {}
         * 
         * In this example, MyFolder could on a mknod create one of either MyFile1 or MyFile2.
         * If the \@FolderObject annotation is instead specified like the next example then the
         * ambiguity can be resolved:
         * 
         * \@DomainObject
         * \@FolderObject(ChildTypeFilterMethod="Filter")
         * class MyFolder extends LinkedList<MyBaseFile> {
         * 		bool Filter(Class<?> possibleType) {
         * 			return possibleType == MyFile1.class;
         * 		}
         * }
         * 
         * @return
         */
        string ChildTypeFilterMethod(); // default "";
    }
}
