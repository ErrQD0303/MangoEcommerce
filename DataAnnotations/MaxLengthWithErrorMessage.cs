using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.DataAnnotations
{
    public class MaxLengthWithErrorMessageAttribute : MaxLengthAttribute
    {
        private readonly string _errorMessageTemplate = "{0} cannot be over {1} characters";
        public MaxLengthWithErrorMessageAttribute(int length) : base(length)
        {
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(_errorMessageTemplate, name, Length);
        }
    }
}