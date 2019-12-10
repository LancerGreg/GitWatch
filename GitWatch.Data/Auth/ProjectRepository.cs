﻿using GitWatch.Domain.Models;
using GitWatch.Domain.Repositories;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitWatch.Data.Auth
{
    public class ProjectRepository : IProjectRepository
    {
        public const string GitHubUri = "https://api.github.com";
        public ProjectModel project = new ProjectModel();
        public DateTime dateStart = DateTime.Now.AddDays(-2);

        public async Task<Repository> GetStargazersCount()
        {
            var repositories = await project._client.Search.SearchRepo(new SearchRepositoriesRequest($"id:={project._repositoryId}"));
            return repositories.Items.First();
        }

        public async Task<int> GetCommitCount()
        {
            var request = new CommitRequest { Since = dateStart, Until = DateTime.Now };
            var commits = await project._client.Repository.Commit.GetAll(project._repositoryId, request);
            return commits.Count();
        }

        public async Task<int> GetContributorsCount()
        {
            var request = new CommitRequest { Since = dateStart, Until = DateTime.Now };
            var commits = await project._client.Repository.Commit.GetAll(project._repositoryId, request);

            List<string> contributors = new List<string>();

            foreach (var commit in commits)
            {
                if (commit.Author != null)
                    contributors.Add(commit.Author.Login);
            }

            return contributors.Distinct().Count();
        }
    }
}
