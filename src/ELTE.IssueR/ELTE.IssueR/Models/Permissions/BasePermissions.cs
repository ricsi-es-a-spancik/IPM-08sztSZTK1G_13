using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ELTE.IssueR.Models.Permissions
{
    [Flags]
    public enum BasePermission
    {
        None = 0,
        AddMember = 1,
        EditMember = 2,
        RemoveMember = 4,
        AddContent = 8,
        EditContent = 16,
        RemoveContent = 32
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
        public BasePermission Code
        {
            get
            {
                return _perm;
            }
        }

        #endregion

        #region Ctors

        public Permission()
        {
            _perm = BasePermission.None;
        }

        public Permission(params BasePermission[] ps)
        {
            _perm = BasePermission.None;
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
            if (_perm.HasFlag(p))
                _perm &= ~p;
        }

        public Boolean HasPermission(BasePermission p)
        {
            return _perm.HasFlag(p);
        }

        public override String ToString()
        {
            return _perm.ToString();
        }

        #endregion

        #region Statics

        public static String DisplayName(BasePermission p)
        {
            switch(p)
            {
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

        public static explicit operator Permission(BasePermission bp)
        {
            return new Permission(bp);
        }

        #endregion

    }
}