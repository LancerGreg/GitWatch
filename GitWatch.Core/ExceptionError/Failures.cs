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

    public class DataRepositoryFailure : ExceptionTermFailure
    {
        public DataRepositoryFailure() : base(ExceptionTerm.DataRepositoryFailure)
        {
        }
    }

    public class SearchRepoFailure : ExceptionTermFailure
    {
        public SearchRepoFailure() : base(ExceptionTerm.SearchRepoFailure)
        {
        }
    }

    public class LoginOrPasswordFailure : ExceptionTermFailure
    {
        public LoginOrPasswordFailure() : base(ExceptionTerm.LoginOrPasswordFailure)
        {
        }
    }
}
