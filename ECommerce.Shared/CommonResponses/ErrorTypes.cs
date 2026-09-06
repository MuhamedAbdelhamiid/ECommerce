using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared.CommonResponses
{
    public enum ErrorTypes
    {
        Faliure = 0,
        Validation = 1,
        NotFound = 2,
        UnAuthorized = 3,
        Forbidden = 4,
        InvalidCredintials = 5,
    }
}
