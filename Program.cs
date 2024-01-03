using Microsoft.VisualBasic.FileIO;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace CSVFile
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string filePath = @"files/customer.csv";
            DataTable fileData = GetDataTable(filePath);

            //InsertIntoSqlTableUsingBulkCopy(fileData);

            InsertIntoSqlTableUsingEFCore(filePath);
        }

      
        private static void InsertIntoSqlTableUsingEFCore(string filePath)
        {
            var dataT = new DataTable();
            dataT.Columns.Add("Id");
            dataT.Columns.Add("Name");
            string readData = File.ReadAllText(filePath);
          
            foreach (var s in readData.Split('\n'))
            {
                if (!string.IsNullOrEmpty(s))
                {
                    dataT.Rows.Add();
                    var x = dataT.Rows.Count;
                    int count = 0;
                    foreach (var t in s.Split(','))
                    {
                        dataT.Rows[dataT.Rows.Count - 1][count] = t;
                        count++;
                    }
                }
            }
            string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=station;Integrated Security=True";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlBulkCopy objbulk = new SqlBulkCopy(con);
                objbulk.DestinationTableName = "userCsv";
                //foreach (DataColumn item in dataT.Columns)
                //{
                //    objbulk.ColumnMappings.Add(item.ColumnName, item.ColumnName);
                //    //objbulk.ColumnMappings.Add("Name", item.ToString());
                //}
                //Mapping Table column    
                //objbulk.ColumnMappings.Add(Convert.ToInt32("Id"), Convert.ToInt32("Id"));
                objbulk.ColumnMappings.Add("Name", "Name");
                //inserting Datatable Records to DataBase    
                objbulk.WriteToServer(dataT);
            }

        }
        private static void InsertIntoSqlTableUsingBulkCopy(DataTable fileData)
        {
            string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=station;Integrated Security=True";
            using (SqlConnection con =  new SqlConnection(connectionString))
            {
                con.Open();
                using(SqlBulkCopy copyTable = new SqlBulkCopy(con))
                {
                    copyTable.DestinationTableName = "userCsv";

                    foreach (var item in fileData.Columns)
                    {
                        copyTable.ColumnMappings.Add(item.ToString(), item.ToString());

                        copyTable.WriteToServer(fileData);
                    }
                }
            }
        }

        private static DataTable GetDataTable(string filePath)
        {
            DataTable csvTable = new DataTable();
            try
            {
                using (TextFieldParser csvReader = new TextFieldParser(filePath))
                {
                    csvReader.SetDelimiters(new string[] { "," });
                    csvReader.HasFieldsEnclosedInQuotes = true;
                    string[] colField = csvReader.ReadFields();
                    foreach (var column in colField)
                    {
                        DataColumn dataCol = new DataColumn();
                        dataCol.AllowDBNull = true;
                        csvTable.Columns.Add(dataCol);
                    }
                    while (!csvReader.EndOfData)
                    {
                        string[] dataField = csvReader.ReadFields();

                        for(int i=0; i<dataField.Length; i++)
                        {
                            if (dataField[i] == "")
                                dataField[i] = null;
                        }
                        csvTable.Rows.Add(dataField);
                    }
                }
            }
            catch(Exception x)
            {
                Console.WriteLine(x.Message);
            }
           return csvTable;
        }
    }
}