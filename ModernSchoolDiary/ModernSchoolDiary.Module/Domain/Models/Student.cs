using DevExpress.Persistent.Base;
using ModernSchoolDiary.Module.BusinessObjects;
using ModernSchoolDiary.Module.Domain.Enums;
using ModernSchoolDiary.Module.Domain.Models.Base;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModernSchoolDiary.Module.Domain.Models
{
    [DefaultClassOptions]
    [NavigationItem("Школа")]
    [DisplayName("Ученики")]
    [DefaultProperty(nameof(FullName))]
    public class Student : Person
    {
        public override UserRole? SchoolRole => UserRole.Student;

        [Browsable(false)]
        public virtual Guid? SchoolClassId { get; set; }
        public virtual SchoolClass? SchoolClass { get; set; }

        [Browsable(false)]
        public virtual Guid? LinkedUserId { get; set; }
        public virtual ApplicationUser? LinkedUser { get; set; }
        public virtual ObservableCollection<Grade> Grades { get; set; }

        [NotMapped]
        public double AverageGrade => Grades.Any()
            ? Math.Round(Grades.Average(g => g.Value), 2)
            : 0;
    }
}