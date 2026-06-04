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
        [Display(Name = "Название")]
        [RuleUniqueValue]
        public virtual string Name { get; set; } = String.Empty;

        [Display(Name = "Ученики")]
        public virtual IList<Student> Students { get; set; } = new List<Student>();

    }
}
