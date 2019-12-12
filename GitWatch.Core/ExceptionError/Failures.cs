using System;
using System.Collections.Generic;
using System.Text;

namespace GitWatch.Core.ExceptionError
{
    public class UserNotFoundFailure : ExceptionTermFailure
    {
        public UserNotFoundFailure() : base(ExceptionTerm.UserNotFoundFailure)
        {
        }
    }
}
