using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;

namespace ModernSchoolDiary.Module.Domain.Models
{
    [DefaultClassOptions]
    [NavigationItem("Журнал")]
    [DisplayName("Оценки")]
    [DefaultProperty(nameof(DisplayName))]
    [RuleCombinationOfPropertiesIsUnique(
    "Grade_NoDuplicate",
    DefaultContexts.Save,
    "Student,Subject,Date",
    CustomMessageTemplate = "Оценка этому ученику по этому предмету на эту дату уже выставлена")]
    public class Grade
    {
        [Key]
        [Browsable(false)]
        public virtual Guid Id { get; protected set; }

        [Browsable(false)]
        public virtual Guid StudentId { get; set; }

        [Required]
        public virtual Student Student { get; set; } = null!;

        [Browsable(false)]
        public virtual Guid SubjectId { get; set; }

        [Required]
        public virtual Subject Subject { get; set; } = null!;

        [Browsable(false)]
        public virtual Guid? TeacherId { get; set; }

        public virtual Teacher? Teacher { get; set; }

        [Browsable(false)]
        public virtual Guid? PeriodId { get; set; }

        public virtual AcademicTerm? Period { get; set; }

        [Required]
        [RuleValueComparison("Grade_Min", DefaultContexts.Save,
            ValueComparisonType.GreaterThanOrEqual, 1,
            CustomMessageTemplate = "Оценка не может быть меньше 1")]
        [RuleValueComparison("Grade_Max", DefaultContexts.Save,
            ValueComparisonType.LessThanOrEqual, 5,
            CustomMessageTemplate = "Оценка не может быть больше 5")]
        public virtual int Value { get; set; }

        [Required]
        public virtual DateTime Date { get; set; } = DateTime.Today;

        [MaxLength(500)]
        public virtual string? Comment { get; set; }

        public string DisplayName =>
            $"{Student?.FullName} — {Subject?.Title} — {Value} ({Date:dd.MM.yyyy})";
    }
}
