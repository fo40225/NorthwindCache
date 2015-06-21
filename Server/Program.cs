using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            PipeStream pipeQuery =
                    new AnonymousPipeClientStream(PipeDirection.In, args[0]);

            PipeStream pipeAnswer =
                    new AnonymousPipeClientStream(PipeDirection.Out, args[1]);

            SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder();
            sb.DataSource = ".";
            sb.IntegratedSecurity = true;
            sb.InitialCatalog = "Northwind";

            string strConn = sb.ToString();

            string strCmd = @"SELECT [CategoryID]
                                   , [CategoryName]
                                   , [Description]
                                   , [Picture]
                                FROM [Northwind].[dbo].[Categories];";

            DataTable localDataCache = new DataTable();

            using (SqlConnection conn = new SqlConnection(strConn))
            {
                using (SqlDataAdapter da = new SqlDataAdapter(strCmd, conn))
                {
                    da.Fill(localDataCache);
                }
            }

            DataView cacheIndexed = new DataView(localDataCache);
            cacheIndexed.Sort = "CategoryID";

            StreamReader sr = new StreamReader(pipeQuery);

            BinaryFormatter bf = new BinaryFormatter();

            for (; ; )
            {
                int CategoryID;
                int.TryParse(sr.ReadLine(), out CategoryID);
                var results = cacheIndexed.FindRows(CategoryID);
                var Category = results.SingleOrDefault();
                if (Category == null)
                {
                    bf.Serialize(pipeAnswer, new Dictionary<string, object>());
                    pipeAnswer.Flush();
                }
                else
                {
                    Dictionary<string, object> rtn = new Dictionary<string, object>();
                    rtn.Add("CategoryID", Category["CategoryID"]);
                    rtn.Add("CategoryName", Category["CategoryName"]);
                    rtn.Add("Description", Category["Description"]);
                    rtn.Add("Picture", Category["Picture"]);
                    bf.Serialize(pipeAnswer, rtn);
                    pipeAnswer.Flush();
                }
            }
        }
    }
}