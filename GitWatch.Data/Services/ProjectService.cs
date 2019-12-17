using GitWatch.Core;
using GitWatch.Core.ExceptionError;
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
        private async Task<ITry<IEnumerable<ProjectRepository>>> GetRepositories(string login, string password, DateTime startDate)
        {
            List<ProjectRepository> result = new List<ProjectRepository>();

            var clientId = "dfe59b98523cdd044f45";
            var clientSecret = "d9770ec38691199d7c550ad11cdca15ffc9994f4";
            var client = new GitHubClient(new ProductHeaderValue("GitWatch"));

            var request = new OauthLoginRequest(clientId)
            {
                // other parameters
                RedirectUri = new Uri("https://localhost:44332/swagger/index.html")
            };

            var credentials = new Credentials(login, password);
            var connection = new Connection(new ProductHeaderValue("Whatever"))
            {
                Credentials = credentials
            };
            var gitClient = new GitHubClient(connection);
            var repositories = await gitClient.AsOption().MatchAsync<ITry<SearchRepositoryResult>>(async r => await Try.EncloseAsync(
                async () => await r.Search.SearchRepo(new SearchRepositoriesRequest($"pushed:>={startDate.ToString("yyyy-MM-dd")}"))),
                    async () => await Task.FromResult(new Failure<SearchRepositoryResult>(new SearchRepoFailure())));

            var miscellaneousRateLimit = await gitClient.Miscellaneous.GetRateLimits();

            //  The "core" object provides your rate limit status except for the Search API.
            var coreRateLimit = miscellaneousRateLimit.Resources.Core;

            var howManyCoreRequestsCanIMakePerHour = coreRateLimit.Limit;
            var howManyCoreRequestsDoIHaveLeft = coreRateLimit.Remaining;
            var whenDoesTheCoreLimitReset = coreRateLimit.Reset; // UTC time

            // the "search" object provides your rate limit status for the Search API.
            var searchRateLimit = miscellaneousRateLimit.Resources.Search;

            var howManySearchRequestsCanIMakePerMinute = searchRateLimit.Limit;
            var howManySearchRequestsDoIHaveLeft = searchRateLimit.Remaining;
            var whenDoesTheSearchLimitReset = searchRateLimit.Reset; // UTC time

            try
            {
                foreach (var repository in repositories.Get.Items)
                {
                    result.Add(new ProjectRepository() { project = new ProjectModel(gitClient, repository.Id, repository.Name, repository.StargazersCount) });
                }
            }
            catch (Exception e)
            {
                return await Task.FromResult(new Failure<IEnumerable<ProjectRepository>>(new LoginOrPasswordFailure()));
            }

            return Try.Enclose(() => result);
        }

        // hard cod, would replace about parameter of method
        private const int PageSize = 8;

        private async Task<ITry<IEnumerable<ProjectViewModel>>> InitRepositoryViewModels(string login, string password, DateTime startDate)
        {
            List<ProjectViewModel> _repositoryViewModels = new List<ProjectViewModel>();
            var repositories = await GetRepositories(login, password, startDate);
            
            try
            {
                foreach (var repo in repositories.Get)
                {
                    _repositoryViewModels.Add(GetInitRepository(repo, startDate).Result.Get);
                }
            }
            catch (Exception e)
            {
                return await Task.FromResult(new Failure<IEnumerable<ProjectViewModel>>(new LoginOrPasswordFailure()));
            }            

            return Try.Enclose(() => _repositoryViewModels);
        }

        private async Task<ITry<ProjectViewModel>> GetInitRepository(ProjectRepository repo, DateTime startDate)
        {
            int[] asyncRes = await Task.WhenAll(repo.GetCommitCount(startDate),
                    repo.GetContributorsCount(startDate));

            return await new ProjectViewModel().AsOption()
                .MatchAsync<ITry<ProjectViewModel>>(async r => await Try.EncloseAsync(async () =>
                {
                    r.Name = repo.project._name;
                    r.StargazerCount = repo.project._subscribersCount;
                    r.CommitCount = asyncRes[0];
                    r.ContributorCount = asyncRes[1];
                    return r;
                }),
                  async () => await Task.FromResult(new Failure<ProjectViewModel>(new DataRepositoryFailure())));
        }

        public async Task<ITry<IndexViewModel>> ProjectPages(string login, string password, DateTime startDate, int page = 1)
        {
            List<ProjectViewModel> _repositoryViewModels = new List<ProjectViewModel>();

            try
            {
                _repositoryViewModels = (await InitRepositoryViewModels(login, password, startDate)).Get.ToList();
            }
            catch (Exception e)
            {
                return await Task.FromResult(new Failure<IndexViewModel>(new LoginOrPasswordFailure()));
            }

            PageInfo pageInfo = new PageInfo
            {
                PageNumber = page,
                PageSize = PageSize,
                TotalItems = _repositoryViewModels.Count
            };

            return Try.Enclose(() =>
            {
                var index = new IndexViewModel
                {
                    PageInfo = pageInfo,
                    Repositories = _repositoryViewModels.Skip((page - 1) * PageSize).Take(PageSize)
                };
                return index;
            });
        }
    }
}
