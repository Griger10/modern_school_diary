using ModernSchoolDiary.Module.Domain.Enums;
using ModernSchoolDiary.Module.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSchoolDiary.Module.Domain.Models.Base
{
    public abstract class Person : ISchoolMember
    {
        [Key]
        [Browsable(false)]
        public virtual Guid Id { get; protected set; }

        [Required]
        [MaxLength(150)]
        public virtual string LastName { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public virtual string FirstName { get; set; } = string.Empty;

        [MaxLength(150)]
        public virtual string? FatherName { get; set; } = "Отсутствует";

        [MaxLength(200)]
        public virtual string? Email { get; set; }

        public string FullName => FatherName == "Отсутствует"
            ? $"{LastName} {FirstName}"
            : $"{LastName} {FirstName} {FatherName}";

        public abstract UserRole? SchoolRole { get; }
    }
}
