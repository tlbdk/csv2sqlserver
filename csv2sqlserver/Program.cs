using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using DataAccess;
using System.IO;

namespace csv2sqlserver
{
    class Program
    {
        public static void Main(string[] args)
        {
            using (var rs = File.Open(@"C:\repos\csv2sqlserver\csv2sqlserver\testworking.csv", FileMode.Open))
            {
                var headers = new String[] { "First Name", "Last Name", "Comment" };

                var csvdataset = new System.Data.DataTable();

                foreach (var header in headers) {
                    System.Data.DataColumn datecolumn = new System.Data.DataColumn(header);
                    datecolumn.AllowDBNull = true;
                    csvdataset.Columns.Add(datecolumn);    
                }

                var csvreader = new DataAccess.DataTableBuilder().ReadLazy(rs, headers);
                foreach (var row in csvreader.Rows)
                {
                    csvdataset.Rows.Add(row.Values.ToArray<string>());
                }

                InsertDataIntoSQLServerUsingSQLBulkCopy(csvdataset);
            }
        }

        public static void InsertDataIntoSQLServerUsingSQLBulkCopy(System.Data.DataTable csvFileData)
        {
            using (SqlConnection dbConnection = new SqlConnection(@"Data Source=(localdb)\MSSQLLocaldb;Initial Catalog=Test;Integrated Security=SSPI;"))
            {
                dbConnection.Open();
                using (SqlBulkCopy sbc = new SqlBulkCopy(dbConnection))
                {
                    sbc.DestinationTableName = "Test";

                    foreach (var column in csvFileData.Columns)
                    {
                        sbc.ColumnMappings.Add(column.ToString(), column.ToString());
                    }
                    sbc.WriteToServer(csvFileData);
                }
            }
        }

    }
}
