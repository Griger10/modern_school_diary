using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using ModernSchoolDiary.Module.BusinessObjects;
using ModernSchoolDiary.Module.Domain.Models;
using ModernSchoolDiary.Module.Domain.Enums;

namespace ModernSchoolDiary.Module.Controllers
{
    public class AttendanceController : ObjectViewController<DetailView, Attendance>
    {
        protected override void OnActivated()
        {
            base.OnActivated();
            ObjectSpace.Committing += ObjectSpace_Committing;
        }

        private void ObjectSpace_Committing(object sender, CancelEventArgs e)
        {
            var user = SecuritySystem.CurrentUser as ApplicationUser;
            if (user == null || user.SchoolRole != UserRole.Teacher)
                return;

            foreach (var att in ObjectSpace.ModifiedObjects.OfType<Attendance>())
            {
                if (att.Subject == null || att.Student?.SchoolClass == null)
                    continue;

                bool teaches = ObjectSpace.GetObjects<TeacherSubjectClass>().Any(a =>
                    a.Teacher != null &&
                    a.Teacher.LinkedUser != null &&
                    a.Teacher.LinkedUser.ID == user.ID &&
                    a.Subject.Id == att.Subject.Id &&
                    a.SchoolClass.Id == att.Student.SchoolClass.Id);

                if (!teaches)
                    throw new UserFriendlyException(
                        $"Вы не ведёте предмет «{att.Subject.Title}» в классе «{att.Student.SchoolClass.Name}», поэтому не можете отметить здесь посещаемость.");
            }
        }

        protected override void OnDeactivated()
        {
            ObjectSpace.Committing -= ObjectSpace_Committing;
            base.OnDeactivated();
        }
    }
}