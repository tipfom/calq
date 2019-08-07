using System;
using System.Collections.Generic;
using System.Text;

namespace Calq.Core
{
    public class InvalidParameterCountException : Exception
    {
        public InvalidParameterCountException(string message) : base(message)
        {

        }
    }

    public class MissingArgumentException : Exception
    {
        public MissingArgumentException(string message) : base(message)
        {

        }
    }

    public class NotEvaluateableException : Exception
    {
        public NotEvaluateableException(string message) : base(message)
        {

        }
    }
}
