using DevExpress.Persistent.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        public virtual Homework Homework { get; set; } = null!;

        [Browsable(false)]
        public virtual Guid StudentId { get; set; }

        [Required]
        public virtual Student Student { get; set; } = null!;

        public virtual bool IsCompleted { get; set; }

        [MaxLength(500)]
        public virtual string? TeacherComment { get; set; }

        public string DisplayName
        {
            get
            {
                string statusText;
                if (IsCompleted)
                    statusText = "Принято";
                else if (Homework != null && Homework.DueDate.Date < DateTime.Today)
                    statusText = "Просрочено";
                else
                    statusText = "Отправлено на доработку";

                return $"{Student?.FullName} — {statusText}";
            }
        }
    }
}
