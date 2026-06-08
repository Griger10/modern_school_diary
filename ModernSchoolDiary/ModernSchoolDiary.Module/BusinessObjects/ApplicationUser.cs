using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EF.PermissionPolicy;
using ModernSchoolDiary.Module.Domain.Enums;
using ModernSchoolDiary.Module.Domain.Models;

namespace ModernSchoolDiary.Module.BusinessObjects
{
    [DefaultProperty(nameof(UserName))]
    public class ApplicationUser : PermissionPolicyUser, ISecurityUserWithLoginInfo, ISecurityUserLockout
    {
        [Browsable(false)]
        public virtual int AccessFailedCount { get; set; }

        [Browsable(false)]
        public virtual DateTime LockoutEnd { get; set; }

        [Browsable(false)]
        [NonCloneable]
        [DevExpress.ExpressApp.DC.Aggregated]
        public virtual IList<ApplicationUserLoginInfo> UserLogins { get; set; } = new ObservableCollection<ApplicationUserLoginInfo>();

        [Display(Name = "Роль в системе")]
        public virtual UserRole? SchoolRole { get; set; }

        [Browsable(false)]
        public virtual Guid? LinkedTeacherId { get; set; }

        [Display(Name = "Учитель")]
        public virtual Teacher? LinkedTeacher { get; set; }

        [Browsable(false)]
        public virtual Guid? LinkedStudentId { get; set; }

        [Display(Name = "Ученик")]
        public virtual Student? LinkedStudent { get; set; }

        IEnumerable<ISecurityUserLoginInfo> IOAuthSecurityUser.UserLogins => UserLogins.OfType<ISecurityUserLoginInfo>();

        ISecurityUserLoginInfo ISecurityUserWithLoginInfo.CreateUserLoginInfo(string loginProviderName, string providerUserKey)
        {
            ApplicationUserLoginInfo result = ((IObjectSpaceLink)this).ObjectSpace.CreateObject<ApplicationUserLoginInfo>();
            result.LoginProviderName = loginProviderName;
            result.ProviderUserKey = providerUserKey;
            result.User = this;
            return result;
        }
    }
}