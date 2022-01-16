using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TenderParser.Models
{
    public class Tender
    {
        public int Id { get; set; }
        public string TradeName { get; set; }
        public string TradeStateName { get; set; }
        public string CustomerFullName { get; set; }
        public double InitialPrice { get; set; }
        public DateTime PublicationDate { get; set; }
        public DateTime FillingApplicationEndDate { get; set; }
        public string DeliveryPlace { get; set; }
        public List<TenderLot> LotPositionsList { get; set; }
        public List<TenderDocument> DocumentsList { get; set; }
    }
}
