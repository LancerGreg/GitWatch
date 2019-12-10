using System;
using System.Collections.Generic;
using System.Text;

namespace GitWatch.Domain.Models
{
    public class ProjectViewModel
    {
        public string Name { get; set; }
        public string UserName { get; set; }
        public int CommitCount { get; set; }
        public int StargazerCount { get; set; }
        public int ContributorCount { get; set; }
    }
}
