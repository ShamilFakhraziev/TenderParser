using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TenderParser.Models
{
    public class TenderBaseInfo
    {

        public string totalPages { get; set; }
        public string currentPage { get; set; }
        public string totalRecords { get; set; }
        public List<InvData> invData { get; set; }
    }
}
