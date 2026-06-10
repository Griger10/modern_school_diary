using DevExpress.ExpressApp.DC;
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
        [XafDisplayName("Администратор")] Administrator,
        [XafDisplayName("Учитель")] Teacher,
        [XafDisplayName("Ученик")] Student
    }
}
