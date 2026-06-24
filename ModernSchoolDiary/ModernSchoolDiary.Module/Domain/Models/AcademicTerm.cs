using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;

namespace ModernSchoolDiary.Module.Domain.Models
{
    [DefaultClassOptions]
    [NavigationItem("Школа")]
    [DisplayName("Учебные периоды")]
    [DefaultProperty(nameof(Name))]
    public class AcademicTerm
    {
        [Key]
        [Browsable(false)]
        public virtual Guid Id { get; protected set; }

        [Required]
        [MaxLength(100)]
        [RuleUniqueValue]
        public virtual string Name { get; set; }

        [Required]
        public virtual DateTime StartDate { get; set; }

        [Required]
        [RuleValueComparison("TermEndAfterStart", DefaultContexts.Save,
            ValueComparisonType.GreaterThan, "StartDate",
            ParametersMode.Expression,
            CustomMessageTemplate = "Дата окончания периода должна быть позже даты начала")]
        public virtual DateTime EndDate { get; set; }
    }
}
