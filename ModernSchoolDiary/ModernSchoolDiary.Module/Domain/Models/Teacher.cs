using DevExpress.Persistent.Base;
using ModernSchoolDiary.Module.BusinessObjects;
using ModernSchoolDiary.Module.Domain.Enums;
using ModernSchoolDiary.Module.Domain.Models.Base;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ModernSchoolDiary.Module.Domain.Models
{
    [DefaultClassOptions]
    [NavigationItem("Школа")]
    [DisplayName("Учителя")]
    [DefaultProperty(nameof(FullName))]
    public class Teacher : Person
    {
        public override UserRole? SchoolRole => UserRole.Teacher;

        [Display(Name = "Классное руководство")]
        public virtual ObservableCollection<SchoolClass> ManagedClasses { get; set; } = new ();

        [Display(Name = "Предметы и классы")]
        public virtual ObservableCollection<TeacherSubjectClass> Assignments { get; set; } = new();

        [Browsable(false)]
        public virtual Guid? LinkedUserId { get; set; }

        [Display(Name = "Пользователь")]
        public virtual ApplicationUser? LinkedUser { get; set; }
    }
}