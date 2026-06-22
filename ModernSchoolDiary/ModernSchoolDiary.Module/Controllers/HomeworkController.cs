using DevExpress.ExpressApp;
using ModernSchoolDiary.Module.Domain.Models;

namespace ModernSchoolDiary.Module.Controllers;

public class HomeworkController: ObjectViewController<ObjectView, Homework>
{
    protected override void OnActivated()
    {
        base.OnActivated();
        ObjectSpace.ObjectChanged += ObjectSpace_ObjectChanged;
    }

    private void ObjectSpace_ObjectChanged(object sender, ObjectChangedEventArgs e)
    {
        if (e.Object is Homework homework && homework.Teacher == null)
        {
            var userId = (Guid)SecuritySystem.CurrentUserId;
            var teacher = ObjectSpace.FirstOrDefault<Teacher>(
                t => t.LinkedUser.ID == userId);
            if (teacher != null)
                homework.Teacher = teacher;
        }
    }

    protected override void OnDeactivated()
    {
        ObjectSpace.ObjectChanged -= ObjectSpace_ObjectChanged;
        base.OnDeactivated();
    }
}