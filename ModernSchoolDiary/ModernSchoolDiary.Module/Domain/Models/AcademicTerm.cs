using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;

namespace ModernSchoolDiary.Module.Domain.Models
{
    [DefaultClassOptions]
    [NavigationItem("Школа")]
    [DisplayName("Учебный период")]
    [DefaultProperty(nameof(Name))]
    public class AcademicTerm
    {
        [Key]
        [Browsable(false)]
        public virtual Guid Id { get; protected set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Название")]
        [RuleUniqueValue]
        public virtual string Name { get; set; }

        [Required]
        [Display(Name = "Начало")]
        public virtual DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "Окончание")]
        public virtual DateTime EndDate { get; set; }
    }
}
