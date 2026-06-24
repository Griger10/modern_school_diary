using System;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using ModernSchoolDiary.Module.BusinessObjects;
using ModernSchoolDiary.Module.Domain.Models;

namespace ModernSchoolDiary.Module.Controllers
{
    public class HomeworkSubmissionController : ObjectViewController<DetailView, HomeworkSubmission>
    {
        protected override void OnActivated()
        {
            base.OnActivated();

            if (View.CurrentObject is HomeworkSubmission submission && ObjectSpace.IsNewObject(submission))
            {
                if (submission.Student == null)
                {
                    var userId = (Guid)SecuritySystem.CurrentUserId;
                    var student = ObjectSpace.FirstOrDefault<Student>(s => s.LinkedUserId == userId);
                    if (student != null)
                    {
                        submission.Student = student;
                    }
                }
            }
        }
    }
}