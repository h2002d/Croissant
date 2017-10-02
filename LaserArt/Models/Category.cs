using LaserArt.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LaserArt.Models
{
    public class Category
    {
        public int? Id { get; set; }
        public string CategoryName { get; set; }
        public string CategoryImage { get; set; }
        public int ParentCategoryId { get; set; }
        public static List<Category> GetCategories(int? id)
        {
            return CategoryDAO.getProducts(id);
        }

        public static List<Category> GetCategoriesByParentId(int? id)
        {
            return CategoryDAO.getCategoryByParentId(id);
        }

        public Category SaveCategory()
        {
            return CategoryDAO.saveProduct(this);
        }

        public static void DeleteCategory(int id)
        {
            CategoryDAO.DeleteCategoryByID(id);
        }
        public static void SetCategoryParent(int categoryId, int parentId)
        {
            CategoryDAO.setCategoryParent(categoryId, parentId);
        }

    }
}