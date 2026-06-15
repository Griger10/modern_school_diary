using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using ModernSchoolDiary.Module.BusinessObjects;
using ModernSchoolDiary.Module.Domain.Models;

namespace ModernSchoolDiary.Module.Controllers
{
    public class HomeworkSubmissionController : ObjectViewController<ObjectView, HomeworkSubmission>
    {
        protected override void OnActivated()
        {
            base.OnActivated();
            ObjectSpace.ObjectChanged += ObjectSpace_ObjectChanged;
        }

        private void ObjectSpace_ObjectChanged(object sender, ObjectChangedEventArgs e)
        {
            if (e.Object is HomeworkSubmission submission && submission.Student == null)
            {
                var userId = (Guid)SecuritySystem.CurrentUserId;
                var student = ObjectSpace.FirstOrDefault<Student>(
                    s => s.LinkedUser.ID == userId);
                if (student != null)
                    submission.Student = student;
            }
        }

        protected override void OnDeactivated()
        {
            ObjectSpace.ObjectChanged -= ObjectSpace_ObjectChanged;
            base.OnDeactivated();
        }
    }
}