using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Configuration;
using System.Data;
using System.Xml.Linq;

namespace DatabaseSample1
{
    class Program
    {
        static void Main(string[] args)
        {
            WriteData();
            GetData();
        }

        static void WriteData()
        {
            XDocument doc = XDocument.Load("Sample1.xml");

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["MR"].ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "InsertFailedConversion";

                    SqlParameter parm = new SqlParameter("@XmlDoc", SqlDbType.NVarChar);
                    parm.Value = doc.ToString();
                    parm.Direction = ParameterDirection.Input;
                    cmd.Parameters.Add(parm);

                    parm = new SqlParameter("@UserID", SqlDbType.Int);
                    parm.Value = 1;
                    parm.Direction = ParameterDirection.Input;
                    cmd.Parameters.Add(parm);

                    parm = new SqlParameter("@retval", SqlDbType.Int);
                    parm.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(parm);

                    cmd.ExecuteNonQuery();

                    Console.WriteLine("New ID: " + parm.Value);
                }

                conn.Close();
            }
        }

        static void GetData()
        {
            XDocument doc = null;

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["MR"].ConnectionString))
            {
                XmlReader reader;
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "GetLatestFailedConversion";
                    cmd.Parameters.AddWithValue("@UserID", 1);

                    reader = cmd.ExecuteXmlReader();
                }

                if (reader.Read())
                    doc = XDocument.Load(reader);

                conn.Close();
            }

            if (doc != null)
                Console.WriteLine(doc);
        }
    }
}
