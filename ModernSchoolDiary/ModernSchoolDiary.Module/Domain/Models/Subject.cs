using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using ModernSchoolDiary.Module.Domain.Enums;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ModernSchoolDiary.Module.Domain.Models
{
    [DefaultClassOptions]
    [NavigationItem("Школа")]
    [DisplayName("Предметы")]
    public class Subject
    {
        [Key]
        [Browsable(false)]
        public virtual Guid Id { get; protected set; }

        [Required]
        [MaxLength(100)]
        public virtual string Title { get; set; }
        public virtual SubjectLevel Level { get; set; } = SubjectLevel.Base;

        [Browsable(false)]
        public virtual ObservableCollection<TeacherSubjectClass> Assignments { get; set; } = new ObservableCollection<TeacherSubjectClass>();
    }
}
