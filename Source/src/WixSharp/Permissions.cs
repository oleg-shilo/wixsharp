using System;
using System.Xml.Linq;

namespace WixSharp
{
    /// <summary>
    /// Enumeration representing Generic* attributes of PermissionEx element
    /// </summary>
    [Flags]
    public enum GenericPermission
    {
        /// <summary>
        /// None does not map to a valid WiX representation
        /// </summary>
        None = 0,

        /// <summary>
        /// Maps to GenericExecute='yes' of PermissionEx
        /// </summary>
        Execute = 0x001,

        /// <summary>
        /// Maps to GenericWrite='yes' of PermissionEx
        /// </summary>
        Write = 0x010,

        /// <summary>
        /// Maps to GenericRead='yes' of PermissionEx
        /// </summary>
        Read = 0x100,

        /// <summary>
        /// Maps to GenericAll='yes' of PermissionEx
        /// </summary>
        All = Execute | Write | Read
    }

    /// <summary>
    /// Equivalent of https://wixtoolset.org/documentation/manual/v3/xsd/wix/permission.html
    /// </summary>
    public class Permission : WixObject
    {
        [Xml]
        public bool? Append;

        [Xml]
        public bool? ChangePermission;

        /// <summary>
        /// For a directory, the right to create a subdirectory. Only valid under a 'CreateFolder' parent.
        /// </summary>
        [Xml]
        public bool? CreateChild;

        /// <summary>
        /// For a directory, the right to create a file in the directory. Only valid under a 'CreateFolder' parent.
        /// </summary>
        [Xml]
        public bool? CreateFile;

        [Xml]
        public bool? CreateLink;

        [Xml]
        public bool? CreateSubkeys;

        [Xml]
        public bool? Delete;

        /// <summary>
        /// For a directory, the right to delete a directory and all the files it contains, including read-only files. Only valid under a 'CreateFolder' parent.
        /// </summary>
        [Xml]
        public bool? DeleteChild;

        [Xml]
        public string Domain;

        [Xml]
        public bool? EnumerateSubkeys;

        [Xml]
        public bool? Execute;

        /// <summary>
        /// Bit mask for FILE_ALL_ACCESS from WinNT.h (0x001F01FF).
        /// </summary>
        [Xml]
        public bool? FileAllRights;

        [Xml]
        public bool? GenericAll;

        public bool? GenericExecute;

        /// <summary>
        /// specifying this will fail to grant read access
        /// </summary>
        [Xml]
        public bool? GenericRead;

        [Xml]
        public bool? GenericWrite;

        [Xml]
        public bool? Notify;

        [Xml]
        public bool? Read;

        [Xml]
        public bool? ReadAttributes;

        [Xml]
        public bool? ReadExtendedAttributes;

        [Xml]
        public bool? ReadPermission;

        /// <summary>
        /// Bit mask for SPECIFIC_RIGHTS_ALL from WinNT.h (0x0000FFFF).
        /// </summary>
        [Xml]
        public bool? SpecificRightsAll;

        [Xml]
        public bool? Synchronize;

        [Xml]
        public bool? TakeOwnership;

        /// <summary>
        /// For a directory, the right to traverse the directory. By default, users are assigned the BYPASS_TRAVERSE_CHECKING privilege, which ignores the FILE_TRAVERSE access right. Only valid under a 'CreateFolder' parent.
        /// </summary>
        [Xml]
        public bool? Traverse;

        [Xml]
        public string User;

        [Xml]
        public bool? Write;

        [Xml]
        public bool? WriteAttributes;

        [Xml]
        public bool? WriteExtendedAttributes;
    }

    /// <summary>
    /// Represents applying permission(s) to the containing File entity
    /// </summary>
    /// <remarks>
    /// DirPermission is a Wix# representation of WiX Util:PermissionEx
    /// </remarks>
    public class DirPermission : WixEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DirPermission"/> class.
        /// </summary>
        public DirPermission()
        {
        }

        /// <summary>
        /// Creates a FilePermission instance for <paramref name="user"/>.
        /// <para>Note that <see cref="DirPermission"/> inherits its parent <see cref="Dir"/> features unless
        /// it has it own features specified.</para>
        /// </summary>
        /// <param name="user"></param>
        public DirPermission(string user)
        {
            if (string.IsNullOrEmpty(user))
                throw new ArgumentNullException("user", "User is a required value for Permission");
            User = user;
        }

        /// <summary>
        /// Creates a FilePermission instance for <paramref name="user"/>@<paramref name="domain"/>
        /// <para>Note that <see cref="DirPermission"/> inherits its parent <see cref="Dir"/> features unless
        /// it has it own features specified.</para>
        /// </summary>
        /// <param name="user"></param>
        /// <param name="domain"></param>
        public DirPermission(string user, string domain)
        {
            if (string.IsNullOrEmpty(user))
                throw new ArgumentNullException("user", "User is a required value for Permission");

            User = user;
            Domain = domain;
        }

        /// <summary>
        /// Creates a FilePermission instance for <paramref name="user"/> with generic permissions described by <paramref name="permission"/>
        /// <para>Note that <see cref="DirPermission"/> inherits its parent <see cref="Dir"/> features unless
        /// it has it own features specified.</para>
        /// </summary>
        /// <param name="user"></param>
        /// <param name="permission"></param>
        public DirPermission(string user, GenericPermission permission)
        {
            if (string.IsNullOrEmpty(user))
                throw new ArgumentNullException("user", "User is a required value for Permission");

            User = user;

            SetGenericPermission(permission);
        }

        /// <summary>
        /// Creates a FilePermission instance for <paramref name="user"/>@<paramref name="domain"/> with generic permissions described by <paramref name="permission"/>
        /// <para>Note that <see cref="DirPermission"/> inherits its parent <see cref="Dir"/> features unless
        /// it has it own features specified.</para>
        /// </summary>
        /// <param name="user"></param>
        /// <param name="domain"></param>
        /// <param name="permission"></param>
        public DirPermission(string user, string domain, GenericPermission permission)
        {
            if (string.IsNullOrEmpty(user))
                throw new ArgumentNullException("user", "User is a required value for Permission");

            User = user;
            Domain = domain;

            SetGenericPermission(permission);
        }

        private void SetGenericPermission(GenericPermission permission)
        {
            if (permission == GenericPermission.All)
            {
                GenericAll = true;
                return;
            }

            if ((permission & GenericPermission.Execute) == GenericPermission.Execute)
                GenericExecute = true;

            if ((permission & GenericPermission.Write) == GenericPermission.Write)
                GenericWrite = true;

            if ((permission & GenericPermission.Read) == GenericPermission.Read)
                GenericRead = true;
        }

        /// <summary>
        /// Maps to the User property of PermissionEx
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// Maps to the Domain property of PermissionEx
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// Maps to the Append property of PermissionEx
        /// </summary>
        public bool? Append { get; set; }

        /// <summary>
        /// Maps to the ChangePermission property of PermissionEx
        /// </summary>
        public bool? ChangePermission { get; set; }

        /// <summary>
        /// Maps to the CreateChild property of PermissionEx
        /// </summary>
        public bool? CreateChild { get; set; }

        /// <summary>
        /// Maps to the CreateFile property of PermissionEx
        /// </summary>
        public bool? CreateFile { get; set; }

        /// <summary>
        /// Maps to the CreateLink property of PermissionEx
        /// </summary>
        public bool? CreateLink { get; set; }

        /// <summary>
        /// Maps to the CreateSubkeys property of PermissionEx
        /// </summary>
        public bool? CreateSubkeys { get; set; }

        /// <summary>
        /// Maps to the Delete property of PermissionEx
        /// </summary>
        public bool? Delete { get; set; }

        /// <summary>
        /// Maps to the DeleteChild property of PermissionEx
        /// </summary>
        public bool? DeleteChild { get; set; }

        /// <summary>
        /// Maps to the EnumerateSubkeys property of PermissionEx
        /// </summary>
        public bool? EnumerateSubkeys { get; set; }

        /// <summary>
        /// Maps to the Execute property of PermissionEx
        /// </summary>
        public bool? Execute { get; set; }

        /// <summary>
        /// Maps to the GenericAll property of PermissionEx
        /// </summary>
        public bool? GenericAll { get; set; }

        /// <summary>
        /// Maps to the GenericExecute property of PermissionEx
        /// </summary>
        public bool? GenericExecute { get; set; }

        /// <summary>
        /// Maps to the GenericRead property of PermissionEx
        /// </summary>
        public bool? GenericRead { get; set; }

        /// <summary>
        /// Maps to the GenericWrite property of PermissionEx
        /// </summary>
        public bool? GenericWrite { get; set; }

        /// <summary>
        /// Maps to the Notify property of PermissionEx
        /// </summary>
        public bool? Notify { get; set; }

        /// <summary>
        /// Maps to the Read property of PermissionEx
        /// </summary>
        public bool? Read { get; set; }

        /// <summary>
        /// Maps to the Readattributes property of PermissionEx
        /// </summary>
        public bool? Readattributes { get; set; }

        /// <summary>
        /// Maps to the ReadExtendedAttributes property of PermissionEx
        /// </summary>
        public bool? ReadExtendedAttributes { get; set; }

        /// <summary>
        /// Maps to the ReadPermission property of PermissionEx
        /// </summary>
        public bool? ReadPermission { get; set; }

        /// <summary>
        /// Maps to the Synchronize property of PermissionEx
        /// </summary>
        public bool? Synchronize { get; set; }

        /// <summary>
        /// Maps to the TakeOwnership property of PermissionEx
        /// </summary>
        public bool? TakeOwnership { get; set; }

        /// <summary>
        /// Maps to the Traverse property of PermissionEx
        /// </summary>
        public bool? Traverse { get; set; }

        /// <summary>
        /// Maps to the Write property of PermissionEx
        /// </summary>
        public bool? Write { get; set; }

        /// <summary>
        /// Maps to the WriteAttributes property of PermissionEx
        /// </summary>
        public bool? WriteAttributes { get; set; }

        /// <summary>
        /// Maps to the WriteExtendedAttributes property of PermissionEx
        /// </summary>
        public bool? WriteExtendedAttributes { get; set; }
    }

    /// <summary>
    /// Represents applying permission(s) to the containing File entity
    /// </summary>
    /// <remarks>
    /// FilePermission is a Wix# representation of WiX Util:PermissionEx
    /// </remarks>
    public class FilePermission : WixEntity
    {
        /// <summary>
        /// Creates a FilePermission instance for <paramref name="user"/>
        /// </summary>
        /// <param name="user"></param>
        public FilePermission(string user)
        {
            if (string.IsNullOrEmpty(user))
                throw new ArgumentNullException("user", "User is a required value for Permission");
            User = user;
        }

        /// <summary>
        /// Creates a FilePermission instance for <paramref name="user"/>@<paramref name="domain"/>
        /// </summary>
        /// <param name="user"></param>
        /// <param name="domain"></param>
        public FilePermission(string user, string domain)
        {
            if (string.IsNullOrEmpty(user))
                throw new ArgumentNullException("user", "User is a required value for Permission");

            User = user;
            Domain = domain;
        }

        /// <summary>
        /// Creates a FilePermission instance for <paramref name="user"/> with generic permissions described by <paramref name="permission"/>
        /// </summary>
        /// <param name="user"></param>
        /// <param name="permission"></param>
        public FilePermission(string user, GenericPermission permission)
        {
            if (string.IsNullOrEmpty(user))
                throw new ArgumentNullException("user", "User is a required value for Permission");

            User = user;

            SetGenericPermission(permission);
        }

        /// <summary>
        /// Creates a FilePermission instance for <paramref name="user"/>@<paramref name="domain"/> with generic permissions described by <paramref name="permission"/>
        /// </summary>
        /// <param name="user"></param>
        /// <param name="domain"></param>
        /// <param name="permission"></param>
        public FilePermission(string user, string domain, GenericPermission permission)
        {
            if (string.IsNullOrEmpty(user))
                throw new ArgumentNullException("user", "User is a required value for Permission");

            User = user;
            Domain = domain;

            SetGenericPermission(permission);
        }

        private void SetGenericPermission(GenericPermission permission)
        {
            if (permission == GenericPermission.All)
            {
                GenericAll = true;
                return;
            }

            if ((permission & GenericPermission.Execute) == GenericPermission.Execute)
                GenericExecute = true;

            if ((permission & GenericPermission.Write) == GenericPermission.Write)
                GenericWrite = true;

            if ((permission & GenericPermission.Read) == GenericPermission.Read)
                GenericRead = true;
        }

        /// <summary>
        /// Maps to the User property of PermissionEx
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// Maps to the Domain property of PermissionEx
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// Maps to the Append property of PermissionEx
        /// </summary>
        public bool? Append { get; set; }

        /// <summary>
        /// Maps to the ChangePermission property of PermissionEx
        /// </summary>
        public bool? ChangePermission { get; set; }

        /// <summary>
        /// Maps to the CreateLink property of PermissionEx
        /// </summary>
        public bool? CreateLink { get; set; }

        /// <summary>
        /// Maps to the CreateSubkeys property of PermissionEx
        /// </summary>
        public bool? CreateSubkeys { get; set; }

        /// <summary>
        /// Maps to the Delete property of PermissionEx
        /// </summary>
        public bool? Delete { get; set; }

        /// <summary>
        /// Maps to the EnumerateSubkeys property of PermissionEx
        /// </summary>
        public bool? EnumerateSubkeys { get; set; }

        /// <summary>
        /// Maps to the Execute property of PermissionEx
        /// </summary>
        public bool? Execute { get; set; }

        /// <summary>
        /// Maps to the GenericAll property of PermissionEx
        /// </summary>
        public bool? GenericAll { get; set; }

        /// <summary>
        /// Maps to the GenericExecute property of PermissionEx
        /// </summary>
        public bool? GenericExecute { get; set; }

        /// <summary>
        /// Maps to the GenericRead property of PermissionEx
        /// </summary>
        public bool? GenericRead { get; set; }

        /// <summary>
        /// Maps to the GenericWrite property of PermissionEx
        /// </summary>
        public bool? GenericWrite { get; set; }

        /// <summary>
        /// Maps to the Notify property of PermissionEx
        /// </summary>
        public bool? Notify { get; set; }

        /// <summary>
        /// Maps to the Read property of PermissionEx
        /// </summary>
        public bool? Read { get; set; }

        /// <summary>
        /// Maps to the Readattributes property of PermissionEx
        /// </summary>
        public bool? Readattributes { get; set; }

        /// <summary>
        /// Maps to the ReadExtendedAttributes property of PermissionEx
        /// </summary>
        public bool? ReadExtendedAttributes { get; set; }

        /// <summary>
        /// Maps to the ReadPermission property of PermissionEx
        /// </summary>
        public bool? ReadPermission { get; set; }

        /// <summary>
        /// Maps to the Synchronize property of PermissionEx
        /// </summary>
        public bool? Synchronize { get; set; }

        /// <summary>
        /// Maps to the TakeOwnership property of PermissionEx
        /// </summary>
        public bool? TakeOwnership { get; set; }

        /// <summary>
        /// Maps to the Write property of PermissionEx
        /// </summary>
        public bool? Write { get; set; }

        /// <summary>
        /// Maps to the WriteAttributes property of PermissionEx
        /// </summary>
        public bool? WriteAttributes { get; set; }

        /// <summary>
        /// Maps to the WriteExtendedAttributes property of PermissionEx
        /// </summary>
        public bool? WriteExtendedAttributes { get; set; }
    }

    internal static class PermissionExt
    {
        static void Do<T>(this T? nullable, Action<T> action) where T : struct
        {
            if (!nullable.HasValue) return;
            action(nullable.Value);
        }

        /// <summary>
        /// Adds attributes to <paramref name="permissionElement"/> representing the state of <paramref name="dirPermission"/>
        /// </summary>
        /// <param name="dirPermission"></param>
        /// <param name="permissionElement"></param>
        public static void EmitAttributes(this DirPermission dirPermission, XElement permissionElement)
        {
            //required
            permissionElement.SetAttributeValue("User", dirPermission.User);
            //optional
            if (dirPermission.Domain.IsNotEmpty()) permissionElement.SetAttributeValue("Domain", dirPermission.Domain);

            //optional
            dirPermission.Append.Do(b => permissionElement.SetAttributeValue("Append", b.ToYesNo()));
            dirPermission.ChangePermission.Do(b => permissionElement.SetAttributeValue("ChangePermission", b.ToYesNo()));
            dirPermission.CreateLink.Do(b => permissionElement.SetAttributeValue("CreateLink", b.ToYesNo()));
            dirPermission.CreateChild.Do(b => permissionElement.SetAttribute("CreateChild", b.ToYesNo()));
            dirPermission.CreateFile.Do(b => permissionElement.SetAttribute("CreateFile", b.ToYesNo()));
            dirPermission.CreateSubkeys.Do(b => permissionElement.SetAttributeValue("CreateSubkeys", b.ToYesNo()));
            dirPermission.Delete.Do(b => permissionElement.SetAttributeValue("Delete", b.ToYesNo()));
            dirPermission.DeleteChild.Do(b => permissionElement.SetAttribute("DeleteChild", b.ToYesNo()));
            dirPermission.EnumerateSubkeys.Do(b => permissionElement.SetAttributeValue("EnumerateSubkeys", b.ToYesNo()));
            dirPermission.Execute.Do(b => permissionElement.SetAttributeValue("Execute", b.ToYesNo()));
            dirPermission.GenericAll.Do(b => permissionElement.SetAttributeValue("GenericAll", b.ToYesNo()));
            dirPermission.GenericExecute.Do(b => permissionElement.SetAttributeValue("GenericExecute", b.ToYesNo()));
            dirPermission.GenericRead.Do(b => permissionElement.SetAttributeValue("GenericRead", b.ToYesNo()));
            dirPermission.GenericWrite.Do(b => permissionElement.SetAttributeValue("GenericWrite", b.ToYesNo()));
            dirPermission.Notify.Do(b => permissionElement.SetAttributeValue("Notify", b.ToYesNo()));
            dirPermission.Read.Do(b => permissionElement.SetAttributeValue("Read", b.ToYesNo()));
            dirPermission.Readattributes.Do(b => permissionElement.SetAttributeValue("Readattributes", b.ToYesNo()));
            dirPermission.ReadExtendedAttributes.Do(b => permissionElement.SetAttributeValue("ReadExtendedAttributes", b.ToYesNo()));
            dirPermission.ReadPermission.Do(b => permissionElement.SetAttributeValue("ReadPermission", b.ToYesNo()));
            dirPermission.Synchronize.Do(b => permissionElement.SetAttributeValue("Synchronize", b.ToYesNo()));
            dirPermission.TakeOwnership.Do(b => permissionElement.SetAttributeValue("TakeOwnership", b.ToYesNo()));
            dirPermission.Traverse.Do(b => permissionElement.SetAttribute("Traverse", b.ToYesNo()));
            dirPermission.Write.Do(b => permissionElement.SetAttributeValue("Write", b.ToYesNo()));
            dirPermission.WriteAttributes.Do(b => permissionElement.SetAttributeValue("WriteAttributes", b.ToYesNo()));
            dirPermission.WriteExtendedAttributes.Do(b => permissionElement.SetAttributeValue("WriteExtendedAttributes", b.ToYesNo()));
        }

        /// <summary>
        /// Adds attributes to <paramref name="permissionElement"/> representing the state of <paramref name="filePermission"/>
        /// </summary>
        /// <param name="filePermission"></param>
        /// <param name="permissionElement"></param>
        public static void EmitAttributes(this FilePermission filePermission, XElement permissionElement)
        {
            //required
            permissionElement.SetAttributeValue("User", filePermission.User);
            //optional
            if (filePermission.Domain.IsNotEmpty()) permissionElement.SetAttributeValue("Domain", filePermission.Domain);

            //optional
            filePermission.Append.Do(b => permissionElement.SetAttributeValue("Append", b.ToYesNo()));
            filePermission.ChangePermission.Do(b => permissionElement.SetAttributeValue("ChangePermission", b.ToYesNo()));
            filePermission.CreateLink.Do(b => permissionElement.SetAttributeValue("CreateLink", b.ToYesNo()));
            filePermission.CreateSubkeys.Do(b => permissionElement.SetAttributeValue("CreateSubkeys", b.ToYesNo()));
            filePermission.Delete.Do(b => permissionElement.SetAttributeValue("Delete", b.ToYesNo()));
            filePermission.EnumerateSubkeys.Do(b => permissionElement.SetAttributeValue("EnumerateSubkeys", b.ToYesNo()));
            filePermission.Execute.Do(b => permissionElement.SetAttributeValue("Execute", b.ToYesNo()));
            filePermission.GenericAll.Do(b => permissionElement.SetAttributeValue("GenericAll", b.ToYesNo()));
            filePermission.GenericExecute.Do(b => permissionElement.SetAttributeValue("GenericExecute", b.ToYesNo()));
            filePermission.GenericRead.Do(b => permissionElement.SetAttributeValue("GenericRead", b.ToYesNo()));
            filePermission.GenericWrite.Do(b => permissionElement.SetAttributeValue("GenericWrite", b.ToYesNo()));
            filePermission.Notify.Do(b => permissionElement.SetAttributeValue("Notify", b.ToYesNo()));
            filePermission.Read.Do(b => permissionElement.SetAttributeValue("Read", b.ToYesNo()));
            filePermission.Readattributes.Do(b => permissionElement.SetAttributeValue("Readattributes", b.ToYesNo()));
            filePermission.ReadExtendedAttributes.Do(b => permissionElement.SetAttributeValue("ReadExtendedAttributes", b.ToYesNo()));
            filePermission.ReadPermission.Do(b => permissionElement.SetAttributeValue("ReadPermission", b.ToYesNo()));
            filePermission.Synchronize.Do(b => permissionElement.SetAttributeValue("Synchronize", b.ToYesNo()));
            filePermission.TakeOwnership.Do(b => permissionElement.SetAttributeValue("TakeOwnership", b.ToYesNo()));
            filePermission.Write.Do(b => permissionElement.SetAttributeValue("Write", b.ToYesNo()));
            filePermission.WriteAttributes.Do(b => permissionElement.SetAttributeValue("WriteAttributes", b.ToYesNo()));
            filePermission.WriteExtendedAttributes.Do(b => permissionElement.SetAttributeValue("WriteExtendedAttributes", b.ToYesNo()));
        }
    }
}