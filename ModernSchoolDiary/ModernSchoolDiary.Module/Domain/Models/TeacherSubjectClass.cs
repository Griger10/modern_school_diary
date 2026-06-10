using DevExpress.Persistent.Base;
using DevExpress.XtraPrinting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSchoolDiary.Module.Domain.Models
{
    [NavigationItem("Школа")]
    [DefaultProperty(nameof(DisplayName))]
    [DisplayName("Учителя по классам")]
    public class TeacherSubjectClass
    {
        [Key]
        [Browsable(false)]
        public virtual Guid Id { get; protected set; }

        [Browsable(false)]
        public virtual Guid TeacherId { get; set; }

        [Required]
        public virtual Teacher Teacher { get; set; } = null!;

        [Browsable(false)]
        public virtual Guid SubjectId { get; set; }

        [Required]
        public virtual Subject Subject { get; set; } = null!;

        [Browsable(false)]
        public virtual Guid SchoolClassId { get; set; }

        [Required]
        public virtual SchoolClass SchoolClass { get; set; } = null!;
        public string DisplayName =>
            $"{Teacher?.FullName} — {Subject?.Title} ({SchoolClass?.Name})";
    }
}
