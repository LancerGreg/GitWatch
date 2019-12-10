using Octokit;
using System;
using System.Collections.Generic;
using System.Text;

namespace GitWatch.Domain.Models
{
    public class ProjectModel
    {
        public GitHubClient _client;
        public long _repositoryId;
        public string _name { get; set; }
        public int _subscribersCount { get; set; }

        public ProjectModel() { }

        public ProjectModel(GitHubClient client, long repositoryId, string name)
        {
            _client = client;
            _repositoryId = repositoryId;
            _name = name;
        }

        public ProjectModel(GitHubClient client, long repositoryId, string name, int subscribersCount)
        {
            _client = client;
            _repositoryId = repositoryId;
            _name = name;
            _subscribersCount = subscribersCount;
        }
    }
}
