using System;
using System.Collections.Generic;
using System.Text;

namespace LinkForge.Application.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message)
        {

        }
    }
}
