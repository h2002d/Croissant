using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LaserArt.Models
{
    public class CardModel
    {
        public int ProductId { get; set; }
        public int ProductQuantity { get; set; }
        public Product product { get; set; }
        public CardModel()
        {
            product = new Product();
        }
    }
}