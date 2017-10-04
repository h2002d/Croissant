using LaserArt.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LaserArt.Models
{
    public class ParentCategory
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string ImageSource { get; set; }

        public static List<ParentCategory> GetCategories(int? id)
        {
            return ParentCategoryDAO.getProducts(id);
        }

        public ParentCategory SaveCategory()
        {
            return ParentCategoryDAO.saveProduct(this);
        }

        public static void DeleteCategory(int id)
        {
            ParentCategoryDAO.DeleteCategoryByID(id);
        }
    }
}