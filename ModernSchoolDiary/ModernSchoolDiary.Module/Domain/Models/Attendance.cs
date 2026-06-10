using DevExpress.Persistent.Base;
using ModernSchoolDiary.Module.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSchoolDiary.Module.Domain.Models
{

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
        public virtual Student Student { get; set; } = null!;

        [Browsable(false)]
        public virtual Guid SubjectId { get; set; }

        [Required]
        public virtual Subject Subject { get; set; } = null!;

        [Required]
        public virtual DateTime Date { get; set; } = DateTime.Today;

        [Required]
        public virtual AttendanceStatus Status { get; set; } = AttendanceStatus.Present;

        public virtual AbsenceReason Reason { get; set; } = AbsenceReason.None;

        public string DisplayName =>
            $"{Student?.FullName} — {Subject?.Title} — {Date:dd.MM.yyyy}";
    }
}
