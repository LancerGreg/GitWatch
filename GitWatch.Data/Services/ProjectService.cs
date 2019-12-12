using GitWatch.Data.Auth;
using GitWatch.Domain.Models;
using GitWatch.Domain.Services;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GitWatch.Data.Services
{
    public class ProjectService : IProjectService
    {
        private async Task<List<ProjectRepository>> GetRepositories(string login, string password, DateTime startDate)
        {
            List<ProjectRepository> result = new List<ProjectRepository>();

            try
            {
                var credentials = new Credentials(login, password);
                var connection = new Connection(new ProductHeaderValue("Whatever"))
                {
                    Credentials = credentials
                };
                var gitClient = new GitHubClient(connection);
                var repositories = await gitClient.Search.SearchRepo(new SearchRepositoriesRequest($"pushed:>={startDate.ToString("yyyy-MM-dd")}"));

                foreach (var repository in repositories.Items)
                {
                    result.Add(new ProjectRepository() { project = new ProjectModel(gitClient, repository.Id, repository.Name, repository.StargazersCount) });
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
            return result;
        }

        // hard cod, would replace about parameter of method
        private const int PageSize = 8;

        private async Task<List<ProjectViewModel>> InitRepositoryViewModels(string login, string password, DateTime startDate)
        {
            var repositories = await GetRepositories(login, password, startDate);
            List<ProjectViewModel> _repositoryViewModels = new List<ProjectViewModel>();

            foreach (var repo in repositories)
            {
                int[] asyncRes = await Task.WhenAll(repo.GetCommitCount(),
                    repo.GetContributorsCount());
                _repositoryViewModels.Add(new ProjectViewModel
                {
                    Name = repo.project._name,
                    StargazerCount = repo.project._subscribersCount,
                    CommitCount = asyncRes[0],
                    ContributorCount = asyncRes[1]
                });
            }
            return _repositoryViewModels;
        }

        public async Task<IndexViewModel> ProjectPages(string login, string password, DateTime startDate, int page = 1)
        {
            List<ProjectViewModel> _repositoryViewModels = await InitRepositoryViewModels(login, password, startDate);

            PageInfo pageInfo = new PageInfo
            {
                PageNumber = page,
                PageSize = PageSize,
                TotalItems = _repositoryViewModels.Count
            };
            IndexViewModel ivm = new IndexViewModel
            {
                PageInfo = pageInfo,
                Repositories = _repositoryViewModels.Skip((page - 1) * PageSize).Take(PageSize)
            };
            return ivm;
        }
    }
}
