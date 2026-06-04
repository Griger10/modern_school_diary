using DevExpress.Persistent.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSchoolDiary.Module.Domain.Models
{
    [NavigationItem("Журнал")]
    [DisplayName("Ответы на домашние задания")]
    [DefaultProperty(nameof(DisplayName))]
    public class HomeworkSubmission
    {
        [Key]
        [Browsable(false)]
        public virtual Guid Id { get; protected set; }

        [Browsable(false)]
        public virtual Guid HomeworkId { get; set; }

        [Required]
        [Display(Name = "Задание")]
        public virtual Homework Homework { get; set; } = null!;

        [Browsable(false)]
        public virtual Guid StudentId { get; set; }

        [Required]
        [Display(Name = "Ученик")]
        public virtual Student Student { get; set; } = null!;

        [Display(Name = "Выполнено")]
        public virtual bool IsCompleted { get; set; }

        [MaxLength(500)]
        [Display(Name = "Комментарий учителя")]
        public virtual string? TeacherComment { get; set; }

        [Display(Name = "Статус")]
        public string DisplayName =>
            $"{Student?.FullName} — {(IsCompleted ? "Принято" : "Отправлено на доработку")}";
    }
}
