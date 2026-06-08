using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using ModernSchoolDiary.Module.BusinessObjects;
using ModernSchoolDiary.Module.Domain.Models;
using ModernSchoolDiary.Module.Domain.Enums;

namespace ModernSchoolDiary.Module.Controllers
{
    public class GradeController : ObjectViewController<DetailView, Grade>
    {
        protected override void OnActivated()
        {
            base.OnActivated();
            ObjectSpace.ObjectChanged += ObjectSpace_ObjectChanged;
            ObjectSpace.Committing += ObjectSpace_Committing;
        }
        private void ObjectSpace_ObjectChanged(object sender, ObjectChangedEventArgs e)
        {
            if (View?.CurrentObject is Grade grade && grade.Teacher == null)
            {
                var user = SecuritySystem.CurrentUser as ApplicationUser;
                if (user?.LinkedTeacher != null)
                    grade.Teacher = ObjectSpace.GetObject(user.LinkedTeacher);
            }
        }
        private void ObjectSpace_Committing(object sender, CancelEventArgs e)
        {
            var user = SecuritySystem.CurrentUser as ApplicationUser;
            if (user == null || user.SchoolRole != UserRole.Teacher)
                return;

            foreach (var grade in ObjectSpace.ModifiedObjects.OfType<Grade>())
            {
                if (grade.Subject == null || grade.Student?.SchoolClass == null)
                    continue;

                bool teaches = ObjectSpace.GetObjects<TeacherSubjectClass>().Any(a =>
                    a.Teacher != null &&
                    a.Teacher.LinkedUser != null &&
                    a.Teacher.LinkedUser.ID == user.ID &&
                    a.Subject.Id == grade.Subject.Id &&
                    a.SchoolClass.Id == grade.Student.SchoolClass.Id);

                if (!teaches)
                    throw new UserFriendlyException(
                        $"Вы не ведёте предмет «{grade.Subject.Title}» в классе «{grade.Student.SchoolClass.Name}», поэтому не можете выставить здесь оценку.");
            }
        }

        protected override void OnDeactivated()
        {
            ObjectSpace.ObjectChanged -= ObjectSpace_ObjectChanged;
            ObjectSpace.Committing -= ObjectSpace_Committing;
            base.OnDeactivated();
        }
    }
}