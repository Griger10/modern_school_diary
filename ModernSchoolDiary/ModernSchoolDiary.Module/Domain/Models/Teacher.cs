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
    [DefaultClassOptions]
    [NavigationItem("Школа")]
    [DefaultProperty(nameof(FullName))]
    [DisplayName("Учителя")]
    public class Teacher
    {
        [Key]
        [Browsable(false)]
        public virtual Guid Id { get; protected set; }

        [Required]
        [MaxLength(150)]
        [Display(Name = "Фамилия")]
        public virtual string LastName { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        [Display(Name = "Имя")]
        public virtual string FirstName { get; set; } = string.Empty;

        [MaxLength(150)]
        [Display(Name = "Отчество")]
        public virtual string FatherName { get; set; } = "Отсутствует";

        [Display(Name = "ФИО")]
        public string FullName => FatherName == "Отсутствует"
            ? $"{LastName} {FirstName}"
            : $"{LastName} {FirstName} {FatherName}";

        [MaxLength(200)]
        [Display(Name = "Email")]
        public virtual string? Email { get; set; }

        [Display(Name = "Классное руководство")]
        public virtual IList<SchoolClass> ManagedClasses { get; set; } = new List<SchoolClass>();

        [Display(Name = "Предметы и классы")]
        public virtual IList<TeacherSubjectClass> Assignments { get; set; } = new List<TeacherSubjectClass>();
    }
}
