using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseTech.Core.DTOs.Order
{
    public record CreateOrderDTO
    (   
        List<Guid> CourseIds
    );
}