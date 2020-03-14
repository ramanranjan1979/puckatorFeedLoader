using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace puckatorFeedLoader
{
    class DataAccess
    {
        private static string connectionString = System.Configuration.ConfigurationSettings.AppSettings["DbConnection"];



        public void UpsertCategory(int categoryId,int parentCategoryId,string description,bool active)
        {
            SqlConnection con = new System.Data.SqlClient.SqlConnection("data source=DESKTOP-1CQE15U;initial catalog=PUCKSOURCE;integrated security=True;MultipleActiveResultSets=True;");
            con.Open();

            SqlCommand cmd = new SqlCommand("UpsertCategory", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("categoryId", categoryId);
            cmd.Parameters.AddWithValue("parentcategoryId", parentCategoryId);
            cmd.Parameters.AddWithValue("description", description);
            cmd.Parameters.AddWithValue("active", active);

            cmd.ExecuteNonQuery();

            con.Close();

        }



        public void UpsertProduct(int productId,string model,string ean,string name,string description,string dimension,string price,string deliveryCode,string quantity,string categories,string options,string moq,string imageUrl)
        {
            SqlConnection con = new System.Data.SqlClient.SqlConnection("data source=DESKTOP-1CQE15U;initial catalog=PUCKSOURCE;integrated security=True;MultipleActiveResultSets=True;");
            con.Open();

            SqlCommand cmd = new SqlCommand("UpsertProduct", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
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
            SqlConnection con = new System.Data.SqlClient.SqlConnection("data source=DESKTOP-1CQE15U;initial catalog=PUCKSOURCE;integrated security=True;MultipleActiveResultSets=True;");
            con.Open();

            SqlCommand cmd = new SqlCommand("UpsertProductCode", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("model", model);
            cmd.Parameters.AddWithValue("ean", code);
            cmd.Parameters.AddWithValue("active", active);

            cmd.ExecuteNonQuery();

            con.Close();

        }

        public void UpsertProductImage(string model,string filename, int number, bool ismain, bool active)
        {
            SqlConnection con = new System.Data.SqlClient.SqlConnection("data source=DESKTOP-1CQE15U;initial catalog=PUCKSOURCE;integrated security=True;MultipleActiveResultSets=True;");
            con.Open();

            SqlCommand cmd = new SqlCommand("UpsertProductImage", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("model", model);
            cmd.Parameters.AddWithValue("filename", filename);
            cmd.Parameters.AddWithValue("number", number);
            cmd.Parameters.AddWithValue("ismain", ismain);
            cmd.Parameters.AddWithValue("active", active);

            cmd.ExecuteNonQuery();

            con.Close();

        }
    }
}
