using LaserArt.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace LaserArt.DAO
{
    public class OrderDAO:DAO
    {
        public static List<Order> getOrdersById(int? id )
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                List<Order> newProductList = new List<Order>();

                using (SqlCommand command = new SqlCommand("sp_GetOrderById", sqlConnection))
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
                        while (rdr.Read())
                        {
                           
                           
                            Order newOrder = new Order();
                            newOrder.Id = Convert.ToInt32(rdr["Id"]);
                            newOrder.Address = rdr["Address"].ToString();
                           
                            newOrder.PhoneNumber = rdr["PhoneNumber"].ToString();
                            newOrder.Name = rdr["Name"].ToString();
                          
                            newOrder.OrderDate = Convert.ToDateTime(rdr["OrderDate"]);
                            newOrder.isCompleted = Convert.ToBoolean(rdr["isCompleted"]==DBNull.Value);
                            
                            newProductList.Add(newOrder);
                        }

                        rdr.Close();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }

              
                return newProductList;
            }

        }
        public static int saveOrder(Order newProduct)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_SaveOrder", sqlConnection))
                {
                    try
                    {
                        sqlConnection.Open();
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@Name", newProduct.Name);
                        command.Parameters.AddWithValue("@PhoneNumber", newProduct.PhoneNumber);
                        command.Parameters.AddWithValue("@SurName", "");
                        command.Parameters.AddWithValue("@Address", newProduct.Address);
                        command.Parameters.AddWithValue("@Latitude", "");
                        command.Parameters.AddWithValue("@Longitude", "");
                        //if(newProduct.isCompleted)
                        //    command.Parameters.AddWithValue("@isCompleted", 1);
                        //else
                        //    command.Parameters.AddWithValue("@isCompleted", Boolean.FalseString);
                        
                        var id = command.ExecuteScalar();
                        foreach (var item in newProduct.Products)
                        {
                            SqlCommand cmd = new SqlCommand("sp_SaveOrderProducts",sqlConnection);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@OrderId", Convert.ToInt32(id));
                            cmd.Parameters.AddWithValue("@ProductId", item.ProductId);
                            cmd.Parameters.AddWithValue("@Quantity", item.ProductQuantity);

                            cmd.ExecuteNonQuery();
                        }
                        return Convert.ToInt32(id);
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