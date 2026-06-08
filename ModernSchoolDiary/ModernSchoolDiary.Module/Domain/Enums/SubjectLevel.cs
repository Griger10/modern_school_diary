using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSchoolDiary.Module.Domain.Enums
{
    public enum SubjectLevel
    {
        [Display(Name = "Базовый")] Base,
        [Display(Name = "Профильный")] Advanced
    }
}
