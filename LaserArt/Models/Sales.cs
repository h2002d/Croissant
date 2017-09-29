using LaserArt.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LaserArt.Models
{
    public class Sales
    {
        public int? Id { get; set; }
        public string image { get; set; }
        public string link  { get; set; }

        public static List<Sales> GetSalesById(int? id)
        {
            return DesignerDAO.getSales(id);
        }
        public void SaveSales()
        {
            DesignerDAO.saveSales(this);
        }
    }
}