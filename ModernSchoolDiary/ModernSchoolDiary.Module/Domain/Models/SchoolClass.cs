using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;


namespace ModernSchoolDiary.Module.Domain.Models
{
    [DefaultClassOptions]
    [NavigationItem("Школа")]
    [DisplayName("Классы")]
    [DefaultProperty(nameof(Name))]
    public class SchoolClass
    {
        [Key]
        [Browsable(false)]
        public virtual Guid Id { get; protected set; }

        [Required]
        [MaxLength(10)]
        [RuleUniqueValue]
        public virtual string Name { get; set; } = String.Empty;
        public virtual ObservableCollection<Student> Students { get; set; } = new();

        [Browsable(false)]
        public virtual Guid? ClassTeacherId { get; set; }

        [Required]
        public virtual Teacher? ClassTeacher { get; set; }

    }
}
