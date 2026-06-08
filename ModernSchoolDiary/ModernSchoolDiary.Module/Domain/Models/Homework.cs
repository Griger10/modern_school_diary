using DevExpress.Persistent.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSchoolDiary.Module.Domain.Models
{
    [DefaultClassOptions]
    [NavigationItem("Журнал")]
    [DisplayName("Домашние задания")]
    [DefaultProperty(nameof(DisplayName))]
    public class Homework
    {
        [Key]
        [Browsable(false)]
        public virtual Guid Id { get; protected set; }

        [Browsable(false)]
        public virtual Guid SubjectId { get; set; }

        [Required]
        [Display(Name = "Предмет")]
        public virtual Subject Subject { get; set; } = null!;

        [Browsable(false)]
        public virtual Guid SchoolClassId { get; set; }

        [Required]
        [Display(Name = "Класс")]
        public virtual SchoolClass SchoolClass { get; set; } = null!;

        [Browsable(false)]
        public virtual Guid? TeacherId { get; set; }

        [Display(Name = "Учитель")]
        public virtual Teacher? Teacher { get; set; }

        [Required]
        [MaxLength(1000)]
        [Display(Name = "Задание")]
        public virtual string Description { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Сдать до")]
        public virtual DateTime DueDate { get; set; }

        [Display(Name = "Задание")]
        public string DisplayName =>
            $"{Subject?.Title} — {SchoolClass?.Name} — до {DueDate:dd.MM.yyyy}";

        [Display(Name = "Ответы")]
        public virtual ObservableCollection<HomeworkSubmission> Submissions { get; set; } = new();

        [NotMapped]
        [Display(Name = "Статус")]
        public string Status
        {
            get
            {
                if (DueDate.Date < DateTime.Today)
                    return "Просрочено";
                return "Активно";
            }
        }
    }
}
