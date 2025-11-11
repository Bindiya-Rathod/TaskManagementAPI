using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public string UserId { get; }

        public UserNotFoundException(string userId)
            : base($"User with ID {userId} was not found")
        {
            UserId = userId;
        }
    }
}
