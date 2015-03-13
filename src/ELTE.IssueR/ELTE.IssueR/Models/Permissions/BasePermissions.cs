using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ELTE.IssueR.Models.Permissions
{
    [Flags]
    public enum BasePermission
    {
        Normal = 0x0000,
        AddMember = 0x0001,
        EditMember = 0x0002,
        RemoveMember = 0x0004,
        AddContent = 0x0008,
        EditContent = 0x0016,
        RemoveContent = 0x0032
    }

    public static class BasicPermissions
    {
        public static Permission Administrator
        {
            get
            {
                return new Permission(BasePermission.AddMember, BasePermission.EditMember, BasePermission.RemoveMember,
                    BasePermission.AddContent, BasePermission.EditContent, BasePermission.RemoveContent);
            }
        }

        public static Permission HumanResource
        {
            get
            {
                return new Permission(BasePermission.AddMember, BasePermission.EditMember, BasePermission.RemoveMember);
            }
        }

        public static Permission Leader
        {
            get
            {
                return new Permission(BasePermission.EditMember, 
                    BasePermission.AddContent, BasePermission.EditContent, BasePermission.RemoveContent);
            }
        }

        public static Permission Worker
        {
            get
            {
                return new Permission(BasePermission.AddContent, BasePermission.EditContent);
            }
        }

        public static Permission New
        {
            get
            {
                return new Permission();
            }
        }
    }

    public class Permission
    {

        #region Variables

        private BasePermission _perm;
        public Int16 Code
        {
            get
            {
                return Convert.ToInt16(_perm);
            }
        }

        #endregion

        #region Ctors

        public Permission()
        {
            _perm = 0;
        }

        public Permission(params BasePermission[] ps)
        {
            _perm = BasePermission.Normal;
            foreach(BasePermission bp in ps)
            {
                _perm |= bp;
            }
        }

        #endregion

        #region Methods

        public List<BasePermission> Available
        {
            get
            {
                List<BasePermission> result = new List<BasePermission>();

                String[] names = Enum.GetNames(typeof(BasePermission));
                foreach(String s in names)
                {
                    BasePermission temp = (BasePermission)Enum.Parse(typeof(BasePermission), s);
                    if(_perm.HasFlag(temp))
                        result.Add(temp);
                }

                return result;
            }
        }

        public void AddPermission(BasePermission p)
        {
            _perm |= p;
        }

        public void RemovePermission(BasePermission p)
        {
            _perm &= p;
        }

        public Boolean HasPermission(BasePermission p)
        {
            return _perm.HasFlag(p);
        }

        public override String ToString()
        {
            return Available.ConvertAll(p => DisplayName(p)).Aggregate((p1, p2) => p1 + ", " + p2);
        }

        #endregion

        #region Statics

        public static String DisplayName(BasePermission p)
        {
            switch(p)
            {
                case BasePermission.Normal:
                    return "";
                case BasePermission.AddMember:
                    return "Tagok felvétele";
                case BasePermission.EditMember:
                    return "Tagok kezelése";
                case BasePermission.RemoveMember:
                    return "Tagok eltávolítása";
                case BasePermission.AddContent:
                    return "Tartalom létrehozása";
                case BasePermission.EditContent:
                    return "Tartalom kezelése";
                case BasePermission.RemoveContent:
                    return "Tartalom eltávolítása";
                default:
                    return "";
            }
        }

        public static implicit operator Int16(Permission p)
        {
            return p.Code;
        }

        public static implicit operator Permission(Int16 i)
        {
            return new Permission{ _perm = (BasePermission)i };
        }

        #endregion

    }
}