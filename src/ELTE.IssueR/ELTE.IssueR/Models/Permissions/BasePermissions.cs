using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ELTE.IssueR.Models.Permissions
{
    public enum BasePermission
    {
        AddMember = 1,
        EditMember = 2,
        RemoveMember = 4,
        AddContent = 8,
        EditContent = 16,
        RemoveContent = 32
    }

    public static class BasicPermissions
    {
        public static Permission Administrator()
        {
            return new Permission(BasePermission.AddMember, BasePermission.EditMember, BasePermission.RemoveMember,
                BasePermission.AddContent, BasePermission.EditContent, BasePermission.RemoveContent);
        }

        public static Permission HumanResource()
        {
            return new Permission(BasePermission.AddMember, BasePermission.EditMember, BasePermission.RemoveMember);
        }

        public static Permission Leader()
        {
            return new Permission(BasePermission.EditMember, 
                BasePermission.AddContent, BasePermission.EditContent, BasePermission.RemoveContent);
        }

        public static Permission Worker()
        {
            return new Permission(BasePermission.AddContent, BasePermission.EditContent);
        }

        public static Permission New()
        {
            return new Permission();
        }
    }

    public class Permission
    {

        #region Variables

        private Int32 _perm;
        public Int32 Code
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
            _perm = 0;
        }

        public Permission(params BasePermission[] ps)
        {
            _perm = 0;
            foreach(BasePermission bp in ps)
            {
                _perm += (Int32)bp;
            }
        }

        #endregion

        #region Methods

        public List<BasePermission> Available
        {
            get
            {
                List<BasePermission> result = new List<BasePermission>();

                Int32[] vals = (Int32[])Enum.GetValues(typeof(BasePermission));
                Int32 temp = _perm;
                Int32 i = 0;
                while(temp > 0)
                {
                    if (temp % vals[i] == 0)
                    {
                        temp -= vals[i];
                        result.Add((BasePermission)vals[i]);
                    }

                    ++i;
                }


                return result;
            }
        }

        public void AddPermission(BasePermission p)
        {
            _perm += (Int32)p;
        }

        public void RemovePermission(BasePermission p)
        {
            _perm -= (Int32)p;
        }

        #endregion

        #region Statics

        public static implicit operator Int32(Permission p)
        {
            return p._perm;
        }

        #endregion

    }
}