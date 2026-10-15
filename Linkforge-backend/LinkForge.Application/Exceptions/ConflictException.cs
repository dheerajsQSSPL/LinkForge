using System;
using System.Collections.Generic;
using System.Text;

namespace LinkForge.Application.Exceptions
{
    public class ConflictException : Exception
    {
        public ConflictException(string msg) : base(msg)
        {

        }
    }
}
