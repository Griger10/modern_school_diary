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
    public class Grade
    {
        [Key]
        [Browsable(false)]
        public virtual Guid Id { get; protected set; }

        [Browsable(false)]
        public virtual Guid StudentId { get; set; }

        [Required]
        [Display(Name = "Ученик")]
        public virtual Student Student { get; set; } = null!;

        [Browsable(false)]
        public virtual Guid SubjectId { get; set; }

        [Required]
        [Display(Name = "Предмет")]
        public virtual Subject Subject { get; set; } = null!;

        [Browsable(false)]
        public virtual Guid? TeacherId { get; set; }

        [Display(Name = "Учитель")]
        public virtual Teacher? Teacher { get; set; }

        [Browsable(false)]
        public virtual Guid? PeriodId { get; set; }

        [Display(Name = "Период")]
        public virtual AcademicTerm? Period { get; set; }

        [Required]
        [Display(Name = "Оценка")]
        [RuleValueComparison("Grade_Min", DefaultContexts.Save,
            ValueComparisonType.GreaterThanOrEqual, 1,
            CustomMessageTemplate = "Оценка не может быть меньше 1")]
        [RuleValueComparison("Grade_Max", DefaultContexts.Save,
            ValueComparisonType.LessThanOrEqual, 5,
            CustomMessageTemplate = "Оценка не может быть больше 5")]
        public virtual int Value { get; set; }

        [Required]
        [Display(Name = "Дата")]
        public virtual DateTime Date { get; set; } = DateTime.Today;

        [MaxLength(500)]
        [Display(Name = "Комментарий")]
        public virtual string? Comment { get; set; }

        [Display(Name = "Запись")]
        public string DisplayName =>
            $"{Student?.FullName} — {Subject?.Title} — {Value} ({Date:dd.MM.yyyy})";
    }
}
