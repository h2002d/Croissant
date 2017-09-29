using LaserArt.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace LaserArt.DAO
{
    public class DesignerDAO:DAO
    {
        public static void saveSales(Sales newProduct)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_SaveSales", sqlConnection))
                {
                    try
                    {
                        sqlConnection.Open();
                        command.CommandType = CommandType.StoredProcedure;

                        if (newProduct.Id == null)
                            command.Parameters.AddWithValue("@Id", DBNull.Value);
                        else
                            command.Parameters.AddWithValue("@Id", newProduct.Id);

                        command.Parameters.AddWithValue("@image", newProduct.image);
                        command.Parameters.AddWithValue("@link", newProduct.link);

                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                }
            }
        }
        public static List<Sales> getSales(int? id)
        {

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_GetSales", sqlConnection))
                {
                    try
                    {
                        sqlConnection.Open();
                        command.CommandType = CommandType.StoredProcedure;
                        if (id == null)
                            command.Parameters.AddWithValue("@Id", DBNull.Value);
                        else
                            command.Parameters.AddWithValue("@Id", id);
                        SqlDataReader rdr = command.ExecuteReader();
                        List<Sales> newProductList = new List<Sales>();
                        while (rdr.Read())
                        {
                            Sales newProduct = new Sales();
                            newProduct.Id = Convert.ToInt32(rdr["Id"]);
                            newProduct.image = rdr["image"].ToString();
                            newProduct.link = rdr["link"].ToString();

                            newProductList.Add(newProduct);
                        }
                        return newProductList;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }

            }
        }
    }
}