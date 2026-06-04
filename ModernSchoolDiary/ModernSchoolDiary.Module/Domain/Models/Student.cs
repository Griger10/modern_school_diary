using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DevExpress.Persistent.Base;

namespace ModernSchoolDiary.Module.Domain.Models
{
    [DefaultClassOptions]
    [NavigationItem("Школа")]
    [DisplayName("Ученик")]
    [DefaultProperty(nameof(FullName))]
    public class Student
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

        [Browsable(false)]
        public virtual Guid? SchoolClassId { get; set; }

        [Browsable(false)]
        public virtual Guid? LinkedUserId { get; set; }

        [Display(Name = "Учебный класс")]
        public virtual SchoolClass? SchoolClass { get; set; }
    }
}
