using GitWatch.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GitWatch.Data.Services
{
    public class UserService
    {
        public GitHubUser GetUSerByCredentials(string emeil, string password)
        {
            if (emeil != "vrv2323@gmail.com" || password != "R19980104v")
            {
                return null;
            }
            GitHubUser user = new GitHubUser() { Id = "1", Email = emeil, Name = "Roman", Password = password };
            return user;
        }
    }
}
