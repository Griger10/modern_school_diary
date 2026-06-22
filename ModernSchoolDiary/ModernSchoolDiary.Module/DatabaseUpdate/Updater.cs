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
                role.AddNavigationPermission("Application/NavigationItems/Items/Reports", SecurityPermissionState.Allow);

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

                role.AddTypePermission<TeacherSubjectClass>(SecurityOperations.Read, SecurityPermissionState.Allow);
                role.AddTypePermission<TeacherSubjectClass>(SecurityOperations.Write, SecurityPermissionState.Deny);
                role.AddTypePermission<TeacherSubjectClass>(SecurityOperations.Create, SecurityPermissionState.Deny);
                role.AddTypePermission<TeacherSubjectClass>(SecurityOperations.Delete, SecurityPermissionState.Deny);

                role.AddTypePermission<Student>(SecurityOperations.Read, SecurityPermissionState.Deny);
                role.AddTypePermission<Student>(SecurityOperations.Write, SecurityPermissionState.Deny);
                role.AddTypePermission<Student>(SecurityOperations.Create, SecurityPermissionState.Deny);
                role.AddTypePermission<Student>(SecurityOperations.Delete, SecurityPermissionState.Deny);
                role.AddObjectPermission<Student>(
                    SecurityOperations.Read,
                    "SchoolClass.Assignments[Teacher.LinkedUser.ID = CurrentUserId()]",
                    SecurityPermissionState.Allow);

                role.AddTypePermission<Grade>(SecurityOperations.Read, SecurityPermissionState.Deny);
                role.AddTypePermission<Grade>(SecurityOperations.Write, SecurityPermissionState.Deny);
                role.AddTypePermission<Grade>(SecurityOperations.Create, SecurityPermissionState.Allow);
                role.AddTypePermission<Grade>(SecurityOperations.Delete, SecurityPermissionState.Deny);
                role.AddObjectPermission<Grade>(
                    SecurityOperations.ReadWriteAccess,
                    "Teacher.LinkedUser.ID = CurrentUserId()",
                    SecurityPermissionState.Allow);
                role.AddObjectPermission<Grade>(
                    SecurityOperations.Delete,
                    "Teacher.LinkedUser.ID = CurrentUserId()",
                    SecurityPermissionState.Allow);

                role.AddTypePermission<Attendance>(SecurityOperations.Read, SecurityPermissionState.Deny);
                role.AddTypePermission<Attendance>(SecurityOperations.Write, SecurityPermissionState.Deny);
                role.AddTypePermission<Attendance>(SecurityOperations.Create, SecurityPermissionState.Allow);
                role.AddTypePermission<Attendance>(SecurityOperations.Delete, SecurityPermissionState.Deny);
                role.AddObjectPermission<Attendance>(
                    SecurityOperations.ReadWriteAccess,
                    "Subject.Assignments[Teacher.LinkedUser.ID = CurrentUserId()]",
                    SecurityPermissionState.Allow);
                role.AddObjectPermission<Attendance>(
                    SecurityOperations.Delete,
                    "Subject.Assignments[Teacher.LinkedUser.ID = CurrentUserId()]",
                    SecurityPermissionState.Allow);

                role.AddTypePermission<Homework>(SecurityOperations.Read, SecurityPermissionState.Deny);
                role.AddTypePermission<Homework>(SecurityOperations.Write, SecurityPermissionState.Deny);
                role.AddTypePermission<Homework>(SecurityOperations.Create, SecurityPermissionState.Allow);
                role.AddTypePermission<Homework>(SecurityOperations.Delete, SecurityPermissionState.Deny);
                role.AddObjectPermission<Homework>(
                    SecurityOperations.ReadWriteAccess,
                    "Teacher.LinkedUser.ID = CurrentUserId()",
                    SecurityPermissionState.Allow);
                role.AddObjectPermission<Homework>(
                    SecurityOperations.Delete,
                    "Teacher.LinkedUser.ID = CurrentUserId()",
                    SecurityPermissionState.Allow);

                role.AddTypePermission<HomeworkSubmission>(SecurityOperations.Read, SecurityPermissionState.Deny);
                role.AddTypePermission<HomeworkSubmission>(SecurityOperations.Write, SecurityPermissionState.Deny);
                role.AddTypePermission<HomeworkSubmission>(SecurityOperations.Delete, SecurityPermissionState.Deny);
                role.AddObjectPermission<HomeworkSubmission>(
                    SecurityOperations.ReadWriteAccess,
                    "Homework.Teacher.LinkedUser.ID = CurrentUserId()",
                    SecurityPermissionState.Allow);

                role.AddTypePermission<DevExpress.Persistent.BaseImpl.EF.FileData>(
                    SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                role.AddTypePermission<DevExpress.Persistent.BaseImpl.EF.FileData>(
                    SecurityOperations.Create, SecurityPermissionState.Allow);

                role.AddObjectPermission<DevExpress.Persistent.BaseImpl.EF.ReportDataV2>(
                    SecurityOperations.Read,
                    "[DisplayName] = 'Список класса'",
                    SecurityPermissionState.Allow);

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
                    "[DisplayName] = 'Ведомость успеваемости ученика'",
                    SecurityPermissionState.Allow);

                role.AddTypePermission<Subject>(SecurityOperations.Read, SecurityPermissionState.Allow);
                role.AddTypePermission<Subject>(SecurityOperations.Write, SecurityPermissionState.Deny);
                role.AddTypePermission<SchoolClass>(SecurityOperations.Read, SecurityPermissionState.Allow);
                role.AddTypePermission<SchoolClass>(SecurityOperations.Write, SecurityPermissionState.Deny);
                role.AddTypePermission<AcademicTerm>(SecurityOperations.Read, SecurityPermissionState.Allow);
                role.AddTypePermission<AcademicTerm>(SecurityOperations.Write, SecurityPermissionState.Deny);

                role.AddTypePermission<Grade>(SecurityOperations.Read, SecurityPermissionState.Deny);
                role.AddTypePermission<Grade>(SecurityOperations.Write, SecurityPermissionState.Deny);
                role.AddTypePermission<Grade>(SecurityOperations.Create, SecurityPermissionState.Deny);
                role.AddTypePermission<Grade>(SecurityOperations.Delete, SecurityPermissionState.Deny);
                role.AddObjectPermission<Grade>(
                    SecurityOperations.Read,
                    "Student.LinkedUser.ID = CurrentUserId()",
                    SecurityPermissionState.Allow);

                role.AddTypePermission<Attendance>(SecurityOperations.Read, SecurityPermissionState.Deny);
                role.AddTypePermission<Attendance>(SecurityOperations.Write, SecurityPermissionState.Deny);
                role.AddTypePermission<Attendance>(SecurityOperations.Create, SecurityPermissionState.Deny);
                role.AddTypePermission<Attendance>(SecurityOperations.Delete, SecurityPermissionState.Deny);
                role.AddObjectPermission<Attendance>(
                    SecurityOperations.Read,
                    "Student.LinkedUser.ID = CurrentUserId()",
                    SecurityPermissionState.Allow);

                role.AddTypePermission<Homework>(SecurityOperations.Read, SecurityPermissionState.Deny);
                role.AddTypePermission<Homework>(SecurityOperations.Write, SecurityPermissionState.Deny);
                role.AddTypePermission<Homework>(SecurityOperations.Create, SecurityPermissionState.Deny);
                role.AddTypePermission<Homework>(SecurityOperations.Delete, SecurityPermissionState.Deny);
                role.AddObjectPermission<Homework>(
                    SecurityOperations.Read,
                    "Submissions[Student.LinkedUser.ID = CurrentUserId()]",
                    SecurityPermissionState.Allow);

                role.AddTypePermission<HomeworkSubmission>(SecurityOperations.Read, SecurityPermissionState.Deny);
                role.AddTypePermission<HomeworkSubmission>(SecurityOperations.Write, SecurityPermissionState.Deny);
                role.AddTypePermission<HomeworkSubmission>(SecurityOperations.Delete, SecurityPermissionState.Deny);
                role.AddTypePermission<HomeworkSubmission>(SecurityOperations.Create, SecurityPermissionState.Allow);
                role.AddTypePermission<DevExpress.Persistent.BaseImpl.EF.FileData>(
                    SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                role.AddTypePermission<DevExpress.Persistent.BaseImpl.EF.FileData>(
                    SecurityOperations.Create, SecurityPermissionState.Allow);
                role.AddObjectPermission<HomeworkSubmission>(
                    SecurityOperations.ReadWriteAccess,
                    "Student.LinkedUser.ID = CurrentUserId()",
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