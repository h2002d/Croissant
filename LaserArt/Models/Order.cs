using LaserArt.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LaserArt.Models
{
    public class Order
    {
        public int Id { get; set; }
        public List<int> ProductId { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
       public int Quantity { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public bool isCompleted { get; set; }
        public List<CardModel> Products{ get; set; }
        public Order()
        {
           // Products = new List<CardModel>();
        }

        public int saveOrder()
        {
          return  OrderDAO.saveOrder(this);
        }

        public static List<Order> GetOrderById(int? id)
        {
            return OrderDAO.getOrdersById(id);
        }
    }
}