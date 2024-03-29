﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace PuckatorService
{
    class DataAccess
    {
        private string _con = string.Empty;

        public DataAccess(string connectionString)
        {
            _con = connectionString; ;
        }

        public void UpsertCategory(int categoryId, int parentCategoryId, string description, bool active)
        {
            SqlConnection con = new SqlConnection(_con);
            con.Open();

            SqlCommand cmd = new SqlCommand("UpsertCategory", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("categoryId", categoryId);
            cmd.Parameters.AddWithValue("parentcategoryId", parentCategoryId);
            cmd.Parameters.AddWithValue("description", description);
            cmd.Parameters.AddWithValue("active", active);

            cmd.ExecuteNonQuery();

            con.Close();

        }


        public void UpsertProduct(int productId, string model, string ean, string name, string description, string dimension, string price, string deliveryCode, string quantity, string categories, string options, string moq, string imageUrl)
        {
            SqlConnection con = new SqlConnection(_con);
            con.Open();

            SqlCommand cmd = new SqlCommand("UpsertProduct", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("productId", productId);
            cmd.Parameters.AddWithValue("model", model);
            cmd.Parameters.AddWithValue("ean", ean);

            cmd.Parameters.AddWithValue("name", name);
            cmd.Parameters.AddWithValue("description", description);
            cmd.Parameters.AddWithValue("dimension", dimension);
            cmd.Parameters.AddWithValue("price", price);
            cmd.Parameters.AddWithValue("deliveryCode", deliveryCode);
            cmd.Parameters.AddWithValue("quantity", quantity);
            cmd.Parameters.AddWithValue("categories", categories);
            cmd.Parameters.AddWithValue("options", options);
            cmd.Parameters.AddWithValue("moq", moq);
            cmd.Parameters.AddWithValue("imageUrl", imageUrl);

            cmd.ExecuteNonQuery();

            con.Close();

        }

        public void UpsertProductCode(string model, string code, bool active)
        {
            SqlConnection con = new SqlConnection(_con);
            con.Open();

            SqlCommand cmd = new SqlCommand("UpsertProductCode", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("model", model);
            cmd.Parameters.AddWithValue("ean", code);
            cmd.Parameters.AddWithValue("active", active);

            cmd.ExecuteNonQuery();

            con.Close();

        }

        public void UpsertProductImage(string model, string filename, int number, bool ismain, bool active)
        {
            SqlConnection con = new SqlConnection(_con);
            con.Open();

            SqlCommand cmd = new SqlCommand("UpsertProductImage", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("model", model);
            cmd.Parameters.AddWithValue("filename", filename);
            cmd.Parameters.AddWithValue("number", number);
            cmd.Parameters.AddWithValue("ismain", ismain);
            cmd.Parameters.AddWithValue("active", active);

            cmd.ExecuteNonQuery();

            con.Close();

        }

        public DataSet GetCategoryByParentCategoryId(int parentCategoryId)
        {
            SqlConnection con = new SqlConnection(_con);
            con.Open();

            SqlCommand cmd = new SqlCommand("GetCategoryByParentCategoryId", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("parentCategoryId", parentCategoryId);
            cmd.ExecuteNonQuery();

            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            sda.Fill(ds);
            con.Close();

            return ds;

        }

        public DataSet GetCategoryById(int categoryId)
        {
            SqlConnection con = new SqlConnection(_con);
            con.Open();

            SqlCommand cmd = new SqlCommand("GetCategoryByParentCategoryId", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("CategoryId", categoryId);
            cmd.ExecuteNonQuery();

            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            sda.Fill(ds);
            con.Close();

            return ds;
        }

        public int UpsertLog(int logId, int logTypeId, string requestData, string responseData, string errorCode)
        {
            SqlConnection con = new SqlConnection(_con);
            con.Open();

            SqlCommand cmd = new SqlCommand("UpsertLog", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("Id", logId);
            cmd.Parameters.AddWithValue("Logtype", logTypeId);
            cmd.Parameters.AddWithValue("RequestData", requestData);
            cmd.Parameters.AddWithValue("ResponseData", responseData);
            cmd.Parameters.AddWithValue("ErrorCode", errorCode);

            int modified = (int)cmd.ExecuteScalar();

            if (con.State == System.Data.ConnectionState.Open)
                con.Close();

            return modified;

        }

        public DataSet GetSetting()
        {
            SqlConnection con = new SqlConnection(_con);
            con.Open();

            SqlCommand cmd = new SqlCommand("GetSetting", con)
            {
                CommandType = CommandType.StoredProcedure
            };
            
            cmd.ExecuteNonQuery();

            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            sda.Fill(ds);
            con.Close();

            return ds;
        }
    }
}
