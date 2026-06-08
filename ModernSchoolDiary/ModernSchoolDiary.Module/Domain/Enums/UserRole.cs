using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSchoolDiary.Module.Domain.Enums
{
    public enum UserRole
    {
        [Display(Name = "Администратор")] Administrator,
        [Display(Name = "Учитель")] Teacher,
        [Display(Name = "Ученик")] Student
    }
}
