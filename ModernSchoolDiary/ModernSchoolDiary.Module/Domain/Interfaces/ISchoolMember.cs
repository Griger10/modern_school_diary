using ModernSchoolDiary.Module.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernSchoolDiary.Module.Domain.Interfaces
{
    public interface ISchoolMember
    {
        string FirstName { get; }
        string LastName { get; }
        string FatherName { get; }
        string FullName { get; }
        string? Email { get; }
        UserRole? SchoolRole { get; }
    }
}
