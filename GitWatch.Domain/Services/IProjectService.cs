using GitWatch.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GitWatch.Domain.Services
{
    public interface IProjectService
    {
        Task<IndexViewModel> ProjectPages(string login, string password, DateTime startDate, int page = 1);
    }
}
