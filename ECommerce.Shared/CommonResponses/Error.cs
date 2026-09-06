using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared.CommonResponses
{
    public class Error
    {
        public string Code { get; set; } = default!;
        public string Description { get; set; } = default!;
        public ErrorTypes Type { get; set; }

        private Error(string code, string description, ErrorTypes ErrorTypes)
        {
            Code = code;
            Description = description;
            Type = ErrorTypes;
        }

        public static Error Faliure(
            string code = "Genaral.Faliure",
            string description = "A general Faliure Has Occured"
        )
        {
            return new Error(code, description, ErrorTypes.Faliure);
        }

        public static Error Validation(
            string code = "Genaral.Validation",
            string description = "Validation Error Has Occured"
        )
        {
            return new Error(code, description, ErrorTypes.Validation);
        }

        public static Error NotFound(
            string code = "Genaral.NotFound",
            string description = "The Requested Resource was not found"
        )
        {
            return new Error(code, description, ErrorTypes.NotFound);
        }

        public static Error UnAuthorized(
            string code = "Genaral.UnAuthorized",
            string description = "You are Not Authorized to perform this action"
        )
        {
            return new Error(code, description, ErrorTypes.UnAuthorized);
        }

        public static Error Forbidden(
            string code = "Genaral.Forbidden",
            string description = "You don't have the access to this resource,Access denied"
        )
        {
            return new Error(code, description, ErrorTypes.Forbidden);
        }

        public static Error InvalidCredintals(
            string code = "Genaral.InvalidCredintals",
            string description = "Your Credintals is not valid to reach this resource"
        )
        {
            return new Error(code, description, ErrorTypes.InvalidCredintials);
        }
    }
}
