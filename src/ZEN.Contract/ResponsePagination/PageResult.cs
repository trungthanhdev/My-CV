using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZEN.Contract.ResponsePagination
{
    public class PageResultResponse<T>
    {
        public int total_item { get; set; }
        public IEnumerable<T> data { get; set; } = [];
    }
}