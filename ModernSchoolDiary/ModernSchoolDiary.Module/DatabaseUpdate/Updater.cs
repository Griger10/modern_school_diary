using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Updating;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EF;
using DevExpress.Persistent.BaseImpl.EF.PermissionPolicy;
using DevExpress.Persistent.BaseImpl.EFCore.AuditTrail;
using ModernSchoolDiary.Module.BusinessObjects;
using ModernSchoolDiary.Module.Domain.Models;

namespace ModernSchoolDiary.Module.DatabaseUpdate
{
    public class Updater : ModuleUpdater
    {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) :
            base(objectSpace, currentDBVersion)
        { }

        public override void UpdateDatabaseAfterUpdateSchema()
        {
            base.UpdateDatabaseAfterUpdateSchema();

            var adminRole = CreateAdminRole();

            CreateTeacherRole();
            CreateStudentRole();
            CreateDefaultRole();


            ApplicationUser adminUser = ObjectSpace.FirstOrDefault<ApplicationUser>(u => u.UserName == "Admin");
            if (adminUser == null)
            {
                adminUser = ObjectSpace.CreateObject<ApplicationUser>();
                adminUser.UserName = "Admin";
                adminUser.SetPassword("");
                adminUser.Roles.Add(adminRole);

                ObjectSpace.CommitChanges();

                ((ISecurityUserWithLoginInfo)adminUser).CreateUserLoginInfo(SecurityDefaults.PasswordAuthentication, ObjectSpace.GetKeyValueAsString(adminUser));
            }

            ObjectSpace.CommitChanges();
        }

        public override void UpdateDatabaseBeforeUpdateSchema()
        {
            base.UpdateDatabaseBeforeUpdateSchema();
        }

        PermissionPolicyRole CreateAdminRole()
        {
            var role = ObjectSpace.FirstOrDefault<PermissionPolicyRole>(r => r.Name == "Администраторы");
            if (role == null)
            {
                role = ObjectSpace.CreateObject<PermissionPolicyRole>();
                role.Name = "Администраторы";
                role.IsAdministrative = true;
            }
            return role;
        }

        PermissionPolicyRole CreateTeacherRole()
        {
            var role = ObjectSpace.FirstOrDefault<PermissionPolicyRole>(r => r.Name == "Учителя");
            if (role == null)
            {
                role = ObjectSpace.CreateObject<PermissionPolicyRole>();
                role.Name = "Учителя";

                role.AddNavigationPermission("Application/NavigationItems/Items/Журнал", SecurityPermissionState.Allow);
                role.AddNavigationPermission("Application/NavigationItems/Items/Школа", SecurityPermissionState.Allow);

                role.AddTypePermission<Student>(SecurityOperations.Read, SecurityPermissionState.Allow);
                role.AddTypePermission<Student>(SecurityOperations.Write, SecurityPermissionState.Deny);
                role.AddTypePermission<Student>(SecurityOperations.Create, SecurityPermissionState.Deny);
                role.AddTypePermission<Student>(SecurityOperations.Delete, SecurityPermissionState.Deny);

                role.AddTypePermission<SchoolClass>(SecurityOperations.Read, SecurityPermissionState.Allow);
                role.AddTypePermission<SchoolClass>(SecurityOperations.Write, SecurityPermissionState.Deny);
                role.AddTypePermission<SchoolClass>(SecurityOperations.Create, SecurityPermissionState.Deny);
                role.AddTypePermission<SchoolClass>(SecurityOperations.Delete, SecurityPermissionState.Deny);

                role.AddTypePermission<Subject>(SecurityOperations.Read, SecurityPermissionState.Allow);
                role.AddTypePermission<Subject>(SecurityOperations.Write, SecurityPermissionState.Deny);
                role.AddTypePermission<Subject>(SecurityOperations.Create, SecurityPermissionState.Deny);
                role.AddTypePermission<Subject>(SecurityOperations.Delete, SecurityPermissionState.Deny);

                role.AddTypePermission<Teacher>(SecurityOperations.Read, SecurityPermissionState.Allow);
                role.AddTypePermission<Teacher>(SecurityOperations.Write, SecurityPermissionState.Deny);
                role.AddTypePermission<Teacher>(SecurityOperations.Create, SecurityPermissionState.Deny);
                role.AddTypePermission<Teacher>(SecurityOperations.Delete, SecurityPermissionState.Deny);

                role.AddTypePermission<AcademicTerm>(SecurityOperations.Read, SecurityPermissionState.Allow);
                role.AddTypePermission<AcademicTerm>(SecurityOperations.Write, SecurityPermissionState.Deny);
                role.AddTypePermission<AcademicTerm>(SecurityOperations.Create, SecurityPermissionState.Deny);
                role.AddTypePermission<AcademicTerm>(SecurityOperations.Delete, SecurityPermissionState.Deny);

                role.AddTypePermission<Grade>(SecurityOperations.Create, SecurityPermissionState.Allow);
                role.AddObjectPermissionFromLambda<Grade>(
                    SecurityOperations.ReadWriteAccess,
                    g => g.Teacher.LinkedUser.ID == (Guid)CurrentUserIdOperator.CurrentUserId(),
                    SecurityPermissionState.Allow);
                role.AddObjectPermissionFromLambda<Grade>(
                    SecurityOperations.Delete,
                    g => g.Teacher.LinkedUser.ID == (Guid)CurrentUserIdOperator.CurrentUserId(),
                    SecurityPermissionState.Allow);

                role.AddTypePermissionsRecursively<Attendance>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                role.AddTypePermissionsRecursively<Attendance>(SecurityOperations.Create, SecurityPermissionState.Allow);
                role.AddTypePermissionsRecursively<Attendance>(SecurityOperations.Delete, SecurityPermissionState.Allow);

                role.AddTypePermissionsRecursively<Homework>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                role.AddTypePermissionsRecursively<Homework>(SecurityOperations.Create, SecurityPermissionState.Allow);
                role.AddTypePermissionsRecursively<Homework>(SecurityOperations.Delete, SecurityPermissionState.Allow);

                role.AddTypePermission<HomeworkSubmission>(SecurityOperations.Read, SecurityPermissionState.Allow);
                role.AddTypePermission<HomeworkSubmission>(SecurityOperations.Write, SecurityPermissionState.Allow);

                AddUserProfilePermissions(role);
            }
            return role;
        }

        PermissionPolicyRole CreateStudentRole()
        {
            var role = ObjectSpace.FirstOrDefault<PermissionPolicyRole>(r => r.Name == "Ученики");
            if (role == null)
            {
                role = ObjectSpace.CreateObject<PermissionPolicyRole>();
                role.Name = "Ученики";

                role.AddNavigationPermission("Application/NavigationItems/Items/Журнал", SecurityPermissionState.Allow);
                role.AddNavigationPermission("Application/NavigationItems/Items/Reports", SecurityPermissionState.Allow);

                role.AddTypePermission<DevExpress.Persistent.BaseImpl.EF.ReportDataV2>(SecurityOperations.Read, SecurityPermissionState.Deny);
                role.AddTypePermission<DevExpress.Persistent.BaseImpl.EF.ReportDataV2>(SecurityOperations.Write, SecurityPermissionState.Deny);
                role.AddTypePermission<DevExpress.Persistent.BaseImpl.EF.ReportDataV2>(SecurityOperations.Create, SecurityPermissionState.Deny);
                role.AddTypePermission<DevExpress.Persistent.BaseImpl.EF.ReportDataV2>(SecurityOperations.Delete, SecurityPermissionState.Deny);

                role.AddObjectPermission<DevExpress.Persistent.BaseImpl.EF.ReportDataV2>(
                    SecurityOperations.Read,
                    "[PredefinedReportType] = 'ModernSchoolDiary.Module.Reports.GradesReport'",
                    SecurityPermissionState.Allow);

                role.AddTypePermission<Subject>(SecurityOperations.Read, SecurityPermissionState.Allow);
                role.AddTypePermission<Subject>(SecurityOperations.Write, SecurityPermissionState.Deny);
                role.AddTypePermission<AcademicTerm>(SecurityOperations.Read, SecurityPermissionState.Allow);
                role.AddTypePermission<AcademicTerm>(SecurityOperations.Write, SecurityPermissionState.Deny);

                role.AddTypePermission<Grade>(SecurityOperations.Read, SecurityPermissionState.Deny);
                role.AddTypePermission<Grade>(SecurityOperations.Write, SecurityPermissionState.Deny);
                role.AddTypePermission<Grade>(SecurityOperations.Create, SecurityPermissionState.Deny);
                role.AddTypePermission<Grade>(SecurityOperations.Delete, SecurityPermissionState.Deny);
                role.AddObjectPermissionFromLambda<Grade>(
                    SecurityOperations.Read,
                    g => g.Student.LinkedUser.ID == (Guid)CurrentUserIdOperator.CurrentUserId(),
                    SecurityPermissionState.Allow);

                role.AddTypePermission<Attendance>(SecurityOperations.Read, SecurityPermissionState.Deny);
                role.AddTypePermission<Attendance>(SecurityOperations.Write, SecurityPermissionState.Deny);
                role.AddTypePermission<Attendance>(SecurityOperations.Create, SecurityPermissionState.Deny);
                role.AddTypePermission<Attendance>(SecurityOperations.Delete, SecurityPermissionState.Deny);
                role.AddObjectPermissionFromLambda<Attendance>(
                    SecurityOperations.Read,
                    a => a.Student.LinkedUser.ID == (Guid)CurrentUserIdOperator.CurrentUserId(),
                    SecurityPermissionState.Allow);

                role.AddTypePermission<Homework>(SecurityOperations.Read, SecurityPermissionState.Allow);
                role.AddTypePermission<Homework>(SecurityOperations.Write, SecurityPermissionState.Deny);
                role.AddTypePermission<Homework>(SecurityOperations.Create, SecurityPermissionState.Deny);
                role.AddTypePermission<Homework>(SecurityOperations.Delete, SecurityPermissionState.Deny);

                role.AddTypePermission<HomeworkSubmission>(SecurityOperations.Create, SecurityPermissionState.Allow);
                role.AddObjectPermissionFromLambda<HomeworkSubmission>(
                    SecurityOperations.ReadWriteAccess,
                    s => s.Student.LinkedUser.ID == (Guid)CurrentUserIdOperator.CurrentUserId(),
                    SecurityPermissionState.Allow);

                AddUserProfilePermissions(role);
            }
            return role;
        }

        PermissionPolicyRole CreateDefaultRole()
        {
            var role = ObjectSpace.FirstOrDefault<PermissionPolicyRole>(r => r.Name == "Default");
            if (role == null)
            {
                role = ObjectSpace.CreateObject<PermissionPolicyRole>();
                role.Name = "Default";
                AddUserProfilePermissions(role);
            }
            return role;
        }

        void AddUserProfilePermissions(PermissionPolicyRole role)
        {
            role.AddObjectPermissionFromLambda<ApplicationUser>(
                SecurityOperations.Read,
                u => u.ID == (Guid)CurrentUserIdOperator.CurrentUserId(),
                SecurityPermissionState.Allow);
            role.AddMemberPermissionFromLambda<ApplicationUser>(
                SecurityOperations.Write, "ChangePasswordOnFirstLogon",
                u => u.ID == (Guid)CurrentUserIdOperator.CurrentUserId(),
                SecurityPermissionState.Allow);
            role.AddMemberPermissionFromLambda<ApplicationUser>(
                SecurityOperations.Write, "StoredPassword",
                u => u.ID == (Guid)CurrentUserIdOperator.CurrentUserId(),
                SecurityPermissionState.Allow);
            role.AddTypePermissionsRecursively<PermissionPolicyRole>(SecurityOperations.Read, SecurityPermissionState.Deny);
            role.AddObjectPermission<ModelDifference>(SecurityOperations.ReadWriteAccess, "UserId = ToStr(CurrentUserId())", SecurityPermissionState.Allow);
            role.AddObjectPermission<ModelDifferenceAspect>(SecurityOperations.ReadWriteAccess, "Owner.UserId = ToStr(CurrentUserId())", SecurityPermissionState.Allow);
            role.AddTypePermissionsRecursively<ModelDifference>(SecurityOperations.Create, SecurityPermissionState.Allow);
            role.AddTypePermissionsRecursively<ModelDifferenceAspect>(SecurityOperations.Create, SecurityPermissionState.Allow);
            role.AddTypePermission<AuditDataItemPersistent>(SecurityOperations.Read, SecurityPermissionState.Deny);
            role.AddObjectPermissionFromLambda<AuditDataItemPersistent>(
                SecurityOperations.Read,
                a => a.UserObject.Key == CurrentUserIdOperator.CurrentUserId().ToString(),
                SecurityPermissionState.Allow);
            role.AddTypePermission<AuditEFCoreWeakReference>(SecurityOperations.Read, SecurityPermissionState.Allow);
        }
    }
}