using Octokit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GitWatch.Domain.Repositories
{
    public interface IProjectRepository
    {
        Task<Repository> GetStargazersCount();
        Task<int> GetCommitCount(DateTime dateStart);
        Task<int> GetContributorsCount(DateTime dateStart);
    }
}
