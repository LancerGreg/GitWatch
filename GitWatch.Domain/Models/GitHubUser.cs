using System;
using System.Collections.Generic;
using System.Text;

namespace GitWatch.Domain.Models
{
    public class GitHubUser
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
