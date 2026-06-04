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
    public enum AttendanceStatus
    {
        [Display(Name = "Присутствовал")] Present,
        [Display(Name = "Отсутствовал")] Absent,
        [Display(Name = "Опоздал")] Late
    }

    public enum AbsenceReason
    {
        [Display(Name = "—")] None,
        [Display(Name = "Уважительная")] Excused,
        [Display(Name = "Неуважительная")] Unexcused
    }

    [DefaultClassOptions]
    [NavigationItem("Журнал")]
    [DisplayName("Посещаемость")]
    [DefaultProperty(nameof(DisplayName))]
    public class Attendance
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

        [Required]
        [Display(Name = "Дата")]
        public virtual DateTime Date { get; set; } = DateTime.Today;

        [Required]
        [Display(Name = "Статус")]
        public virtual AttendanceStatus Status { get; set; } = AttendanceStatus.Present;

        [Display(Name = "Причина отсутствия")]
        public virtual AbsenceReason Reason { get; set; } = AbsenceReason.None;

        [Display(Name = "Запись")]
        public string DisplayName =>
            $"{Student?.FullName} — {Subject?.Title} — {Date:dd.MM.yyyy}";
    }
}
