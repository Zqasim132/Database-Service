using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using Microsoft.Data.SqlClient;
using Serilog;
using System.Diagnostics;

namespace DatabaseService
{
    class Program
    {
        private static List<DBManager> clients = new List<DBManager>();
        private static string longestReadTime_client = "";
        private static string longestWriteTime_client = "";
        private static string longestReadTime_1_client = "";
        private static string longestWriteTime_1_client = "";
        private static string longestUpdateTime_client = "";
        private static long longestReadTime = 0;
        private static long longestWriteTime = 0;
        private static long longestReadTime_1 = 0;
        private static long longestWriteTime_1 = 0;
        private static long longestUpdateTime = 0;
        private static long averageReadTime = 0;
        private static long averageWriteTime = 0;
        private static long averageReadTime_1 = 0;
        private static long averageWriteTime_1 = 0;
        private static long averageUpdateTime = 0;
        private static int totalClients = 0;
        private static int totalReadClients = 0;
        private static int totalWriteClients = 0;
        private static int totalUpdateClients = 0;
        private static string connectionString;

        private static int C = 0;
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                                .AddJsonFile("appsettings.json", false, true)
                                .Build();

            Serilog.Log.Logger = new Serilog.LoggerConfiguration()
                                  .Enrich.FromLogContext()
                                  //.WriteTo.Console() I prefer plugging through the config file
                                  .ReadFrom.Configuration(configuration)
                                  .CreateLogger();




            totalReadClients = Convert.ToInt32(configuration.GetSection("DBServiceConfig")["ReadClients"]);
            totalWriteClients = Convert.ToInt32(configuration.GetSection("DBServiceConfig")["WriteClients"]);
            totalUpdateClients = Convert.ToInt32(configuration.GetSection("DBServiceConfig")["UpdateClients"]);
            totalClients = totalReadClients + totalWriteClients + totalUpdateClients;



            //// Create a new event listener
            //using (var listener = new EventCounterListener())
            //{

            //}
            //connectionString = configuration.GetConnectionString("DefaultConnection");

            //for (int i = 0; i < 50; i++)
            //{
            //    // Open a connection
            //    SqlConnection cnn = new SqlConnection(connectionString);

            //    open(cnn, i);

            //}



            //reading clients
            if (totalReadClients > 0)
            {
                int numberOfClients_R = totalReadClients;
                int counter_R = 0;
                while (numberOfClients_R > 0)
                {
                    //Console.WriteLine(counter_R);
                    DBManager dBManager = new DBManager(configuration, $"DBTest_Read_{counter_R}", TTriggerType.READ);
                    dBManager.QueriesExecuted += onQuriesExecuted;
                    counter_R++;
                    numberOfClients_R--;
                    clients.Add(dBManager);
                }
            }


            //writing clients
            if (totalWriteClients > 0)
            {
                int numberOfClients_W = totalWriteClients;
                int counter_W = 0;
                while (numberOfClients_W > 0)
                {
                    DBManager dBManager = new DBManager(configuration, $"DBTest_Write_{counter_W}", TTriggerType.WRITE);
                    dBManager.QueriesExecuted += onQuriesExecuted;
                    counter_W++;
                    numberOfClients_W--;
                    clients.Add(dBManager);
                }
            }


            //updating clients
            if (totalUpdateClients > 0)
            {
                int numberOfClients_U = totalUpdateClients;
                int counter_U = 0;
                while (numberOfClients_U > 0)
                {
                    DBManager dBManager = new DBManager(configuration, $"DBTest_Update_{counter_U}", TTriggerType.UPDATE);
                    dBManager.QueriesExecuted += onQuriesExecuted;
                    counter_U++;
                    numberOfClients_U--;
                    clients.Add(dBManager);
                }
            }







            Console.ReadLine();
        }

        private static void open(SqlConnection cnn, int count)
        {
            try
            {
                if(count > 47)
                {
                    using (var listener = new EventCounterListener())
                    {
                        cnn.Open();
                        //cnn.StateChange += new StateChangeEventHandler(stateChanged);
                        // wait for sampling interval happens
                        System.Threading.Thread.Sleep(500);

                        //cnn.Close();
                    }
                }
                else
                {
                    //using (var listener = new EventCounterListener())
                    //{
                    //    cnn.Open();
                    //    //cnn.StateChange += new StateChangeEventHandler(stateChanged);
                    //    // wait for sampling interval happens
                    //    System.Threading.Thread.Sleep(500);

                    //    //cnn.Close();
                    //}

                    cnn.Open();
                }
                
            }
            catch (Exception ex) { }
            
        }

        private static void stateChanged(object sender, StateChangeEventArgs args)
        {
            Console.WriteLine("State changed");
        }
            
/// <summary>
/// event Handler for query execution completion
/// </summary>
/// <param name="source"></param>
/// <param name="dBManager"></param>
private static void onQuriesExecuted(string client, string connectionString)
        {
            //Console.WriteLine();
            //Console.WriteLine($"__Client: {client} Thread working is Finished..!");
            //Console.WriteLine();
            //if (totalClients <= 2)
            //{
            //    using (var listener = new EventCounterListener())
            //    {
            //        SqlConnection cnn = new SqlConnection("data source = PK-ZEEQASIM\\SQLEXPRESS; database = ArcPubCfg; integrated security=SSPI; TrustServerCertificate=True; Max Pool Size=1000;");
            //        cnn.Open();
            //        //cnn.StateChange += new StateChangeEventHandler(stateChanged);
            //        // wait for sampling interval happens
            //        //System.Threading.Thread.Sleep(500);

            //        //cnn.Close();
            //    }
            //}
            int c = Interlocked.Decrement(ref totalClients);
            //totalClients--;
            if(totalClients == 0)
            {
                getTimeInfo();

                for (int i = 0; i < 1; i++)
                {
                    using (var listener = new EventCounterListener())
                    {
                        SqlConnection cnn = new SqlConnection(connectionString);
                        cnn.Open();
                        //cnn.StateChange += new StateChangeEventHandler(stateChanged);
                        // wait for sampling interval happens
                        System.Threading.Thread.Sleep(1500);

                        //cnn.Close();
                    }
                }
               
            }
           
        }
        /// <summary>
        /// //geting average and longest read and write time
        /// </summary>
        private static void getTimeInfo()
        {
            foreach (DBManager dBManager in clients)
            {
                switch (dBManager.requestType)
                {
                    case TTriggerType.READ:
                        {
                            averageReadTime += dBManager.readTime;
                            averageReadTime_1 += dBManager.readTime_1;
                            if (longestReadTime < dBManager.readTime)
                            {
                                longestReadTime = dBManager.readTime;
                                longestReadTime_client = dBManager.client;
                            }

                            if (longestReadTime_1 < dBManager.readTime_1)
                            {
                                longestReadTime_1 = dBManager.readTime_1;
                                longestReadTime_1_client = dBManager.client;
                            }
                            break;
                        }
                    case TTriggerType.WRITE:
                        {
                            averageWriteTime += dBManager.writeTime;
                            if(longestWriteTime < dBManager.writeTime)
                            {
                                longestWriteTime = dBManager.writeTime;
                                longestWriteTime_client = dBManager.client;
                            }
                            break;
                        }
                    case TTriggerType.UPDATE:
                        {
                            averageWriteTime += dBManager.updateTime;
                            if (longestUpdateTime < dBManager.updateTime)
                            {
                                longestUpdateTime = dBManager.updateTime;
                                longestUpdateTime_client = dBManager.client;
                            }
                            break;
                        }
                    default: return;
                }
            }

            if (totalReadClients > 0)
            {
                Console.WriteLine();
                Console.WriteLine($"longest read time of client: {longestReadTime_client} is: {longestReadTime} milliseconds ");
                Console.WriteLine($"longest read time of subsequent client: {longestReadTime_1_client} is: {longestReadTime_1} milliseconds ");
                Console.WriteLine($"average read time of {totalReadClients} clients is: {averageReadTime / totalReadClients} milliseconds ");
                Console.WriteLine($"average read time of {totalReadClients} subsequent clients is: {averageReadTime_1 / totalReadClients} milliseconds ");
                Console.WriteLine();
            }
            if (totalWriteClients > 0)
            {
                Console.WriteLine($"longest write time of client: {longestWriteTime_client} is: {longestWriteTime} milliseconds ");
                Console.WriteLine($"average write time of {totalWriteClients} clients is: {averageWriteTime / totalWriteClients} milliseconds ");
                Console.WriteLine();
            }
            if (totalUpdateClients > 0)
            {
                Console.WriteLine($"longest update time of client: {longestUpdateTime_client} is: {longestUpdateTime} milliseconds ");
                Console.WriteLine($"average update time of {totalUpdateClients} clients is: {averageUpdateTime / totalUpdateClients} milliseconds ");
                Console.WriteLine();
            }

        }
    }
}





















        //SqlConnection conn = new SqlConnection("data source = PK-ZEEQASIM\\SQLEXPRESS; database = Sample; integrated security=SSPI");
        //SqlCommand cmd = new SqlCommand("select * from Info", conn);
        //conn.Open();
        //using (SqlDataReader reader = cmd.ExecuteReader())
        //{
        //    Console.WriteLine("FirstColumn\tSecond Column\t\tThird Column\t");

        //    while (reader.Read())
        //    {
        //        Console.WriteLine(String.Format("{0} \t | {1} \t | {2}",
        //        reader[0], reader[1], reader[2]));
        //    }

        //    //Console.WriteLine($"{reader.GetString(0)}.{reader.GetString(1)} - {reader.GetString(2)}");
        //}

        

        //var adapter = new SqlDataAdapter(cmd);
        //var dbSet = new DataSet();
        //adapter.Fill(dbSet, "Info");

        //Console.Clear();
        //foreach (DataColumn column in dbSet.Tables["Info"].Columns)
        //    Console.Write("\t{0}", column.ColumnName);
        //Console.WriteLine();
        //foreach (DataRow row in dbSet.Tables["Info"].Rows)
        //{
        //    var cells = row.ItemArray;
        //    foreach (object cell in cells)
        //        Console.Write("\t{0}", cell);
        //    Console.WriteLine();
        //}

//        Console.ReadLine();
//Console.WriteLine("Hello, World!");
