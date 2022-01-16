using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TenderParser.Models
{
    public class TenderLot
    {
        public string Name { get; set; }
        public string UnitOfMeasurement { get; set; }
        public string Count { get; set; }
        public string UnitPrice { get; set; }
    }
}
