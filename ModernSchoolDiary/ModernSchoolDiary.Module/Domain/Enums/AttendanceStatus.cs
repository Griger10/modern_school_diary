using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSchoolDiary.Module.Domain.Enums
{
    public enum AttendanceStatus
    {
        [Display(Name = "Присутствовал")] Present,
        [Display(Name = "Отсутствовал")] Absent,
        [Display(Name = "Опоздал")] Late
    }
}
