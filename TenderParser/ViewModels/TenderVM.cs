using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TenderParser.Models;

namespace TenderParser.ViewModels
{
    public class TenderVM
    {
        public Tender tender { get; set; }
        public Exception exception { get; set; }
    }
}
