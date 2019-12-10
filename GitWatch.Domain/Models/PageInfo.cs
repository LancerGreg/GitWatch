using System;
using System.Collections.Generic;
using System.Text;

namespace GitWatch.Domain.Models
{
    public class PageInfo
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }

        public int TotalPages
            => (int)Math.Ceiling((decimal)TotalItems / PageSize);
    }
    public class IndexViewModel
    {
        public IEnumerable<ProjectViewModel> Repositories { get; set; }
        public PageInfo PageInfo { get; set; }
    }
}
