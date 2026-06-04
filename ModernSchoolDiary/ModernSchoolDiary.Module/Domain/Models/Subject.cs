using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;

namespace ModernSchoolDiary.Module.Domain.Models
{
    public enum SubjectLevel
    {
        [Display(Name = "Базовый")] Base,
        [Display(Name = "Профильный")] Advanced
    }

    [DefaultClassOptions]
    [NavigationItem("Школа")]
    [DisplayName("Предметы")]
    public class Subject
    {
        [Key]
        [Browsable(false)]
        public virtual Guid Id { get; protected set; }

        [Display(Name = "Название")]
        [Required]
        [MaxLength(100)]
        public virtual string Title { get; set; }

        [Display(Name = "Уровень")]
        public virtual SubjectLevel Level { get; set; } = SubjectLevel.Base;
    }
}
