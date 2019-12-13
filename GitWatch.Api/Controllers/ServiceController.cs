using GitWatch.Data.Auth;
using GitWatch.Data.Services;
using GitWatch.Domain.Models;
using GitWatch.Domain.Repositories;
using GitWatch.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GitWatch.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/auth")]
    public class ServiceController : Controller
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectService _projectService;

        public ServiceController(IProjectRepository projectRepository, IProjectService projectService)
        {
            _projectRepository = projectRepository;
            _projectService = projectService;
        }

        /// <summary>
        /// Repository pages
        /// </summary>        
        [ProducesResponseType(typeof(IndexViewModel), 200)]
        [ProducesResponseType(401)]
        [HttpGet("api/repository_from_git_hub")]
        public async Task<IActionResult> GetGitHubRepositories(string login, string password, DateTime startDate) =>
            await (await _projectService.ProjectPages(login, password, startDate))
                .MatchAsync<IActionResult>(
                    async o => { 
                        return await Task.FromResult(new OkObjectResult(o)); },
                    async ex => { 
                        return await Task.FromResult(new BadRequestObjectResult(ex.GetType().Name)); });
    }
}
