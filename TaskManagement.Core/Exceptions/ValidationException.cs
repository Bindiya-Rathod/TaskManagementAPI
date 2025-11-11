using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.Exceptions
{
    public class ValidationException : Exception
    {
        public List<string> Errors { get; }

        public ValidationException(string message, List<string> errors)
            : base(message)
        {
            Errors = errors ?? new List<string>();
        }

        public ValidationException(string message)
            : base(message)
        {
            Errors = new List<string> { message };
        }
    }
}
