using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseService
{
    public class DBManager
    {
        public readonly IConfigurationRoot configuration;
        private readonly DBComponent_1 dbComponent_1;
        private readonly DBComponent_1 dbComponent_1W;
        private readonly DBComponent_2 dbComponent_2;
        private readonly DBComponent_2 dbComponent_2W;

        private readonly DBComponentDapper_1 dbComponentDapper_1;
        private readonly DBComponentDapper_1 dbComponentDapper_1W;
        private readonly DBComponentDapper_2 dbComponentDapper_2;
        private readonly DBComponentDapper_2 dbComponentDapper_2W;

        private bool dThreadProcessing;
        private Thread dThread;
        private TimerTriggers lstTriggers;
        private static int db_ActionIntervals = 30000;
        public string client;
        private int dbConnectRetry = 0;
        public TTriggerType requestType;
        public long readTime = 0;
        public long readTime_1 = 0;
        public long writeTime = 0;
        public long writeTime_1 = 0;
        public long updateTime = 0;
        public delegate void QueriesExecutedHandler(string name, string connectionString);
        public event QueriesExecutedHandler QueriesExecuted;
        private string routine = "DBM";
        private bool pooling = false;
        private bool dapper = false;
        private int reqPerClient = 0;
        private int counter = 0;
        private string connectionStrings;
        private int servedClients = 0;
        public DBManager(IConfigurationRoot _configuration, string _client, TTriggerType _requestType)
        {
            client = _client;
            requestType = _requestType;
            configuration = _configuration;
            connectionStrings = _configuration.GetConnectionString("DefaultConnection");
            db_ActionIntervals = Convert.ToInt32(_configuration.GetSection("DBServiceConfig")["TimeIntervals"]);
            pooling = configuration.GetSection("DBServiceConfig")["Pooling"] == "true" ? true : false;
            dapper = configuration.GetSection("DBServiceConfig")["Dapper"] == "true" ? true : false;

            if (dapper)
            {
                if (pooling)
                {
                    dbComponentDapper_2 = new DBComponentDapper_2(_configuration);
                    dbComponentDapper_2.QueriesExecuted += onQuriesExecuted;
                }
                    
                else
                {
                    dbComponentDapper_1 = new DBComponentDapper_1(_configuration);
                    dbComponentDapper_1.QueriesExecuted += onQuriesExecuted;
                }
                    
            }
            else
            {
                if (pooling)
                {
                    dbComponent_2 = new DBComponent_2(_configuration);
                    dbComponent_2.QueriesExecuted += onQuriesExecuted;
                }
                  
                else
                {
                    dbComponent_1 = new DBComponent_1(_configuration);
                    dbComponent_1.QueriesExecuted += onQuriesExecuted;
                }
                    
            }


            //dbComponent_W = new DBComponent(_configuration);
            lstTriggers = new TimerTriggers();

            //createInstance("DBTest_1");

            StartDatabaseManager();
        }

        private void onQuriesExecuted(string name)
        {
            Interlocked.Increment(ref reqPerClient);
            if(reqPerClient == 2)
            {
                dThreadProcessing = false;
                QueriesExecuted(client, connectionStrings);
            }
           
            //Interlocked.Increment(ref servedClients);
            //if (servedClients == 2)
            //    dThreadProcessing = false;
        }

        public void StartDatabaseManager()
        {

            try
            {
                //_dbComponent.onDBConnectionState += DBConnectionStateEventHandler;
                lstTriggers.ClearAllTriggers();
                if (pooling)
                {
                    //for (int i = 0; i < 99; i++)
                    //{
                    //    Thread.Sleep(10);
                    //    AddTimerTrigger(requestType, db_ActionIntervals);
                    //}

                    AddTimerTrigger(requestType, db_ActionIntervals);
                    //Thread.Sleep(500);
                    AddTimerTrigger(requestType, db_ActionIntervals);
                    //Thread.Sleep(500);
                }
                else
                    AddTimerTrigger(TTriggerType.CONNECT_DB, db_ActionIntervals);
                //lstTriggers.Count();
                //DateTime dt = DateTime.Now;
                //Console.WriteLine(lstTriggers.Count);
                //TimerTrigger timerTrigger = lstTriggers.GetPendingTriggerByTime(dt);
                //Thread.Sleep(50);
                //Console.WriteLine(lstTriggers.Count);
                dThreadProcessing = true;
                dThread = new Thread(ThreadProcessing);
                dThread.IsBackground = true;
                dThread.Start();


            }

            catch (Exception ex)
            {
                Log.Error("routine:" + routine + " StartDatabaseManager:" + ex.Message);
            }

        }

        private void ThreadProcessing()
        {
            try
            {
                
                while (dThreadProcessing)
                {
                    Thread.Sleep(db_ActionIntervals);
                    int counter = lstTriggers.Count;
                    DateTime dt = DateTime.Now;
                    if (counter > 0)
                    {
                        dt = DateTime.Now;
                        TimerTrigger timerTrigger = lstTriggers.GetPendingTriggerByTime(dt);
                        Task.Run(() => ProcessTriggers(timerTrigger, counter));
                        Console.WriteLine("ProcessTriggers()");
                        

                        Thread.Sleep(db_ActionIntervals);
                        Interlocked.Decrement(ref counter);
                    }
                    else
                    {
                        if(pooling)
                        dThreadProcessing = false;
                    }
                }
            }

            catch (Exception ex)
            {
                Log.Error("routine:" + routine + " ThreadProcessing:" + ex.Message, -1);
            }
        }




        private void ProcessTriggers(TimerTrigger timerTrigger, int C)
        {
            DateTime dt = DateTime.Now;


            try
            {
                //Thread.Sleep(5);

                dt = DateTime.Now;

                //TimerTrigger timerTrigger = lstTriggers.GetPendingTriggerByTime(dt);
                if (timerTrigger != null)
                {
                    Log.Information("routine:" + routine + " ProcessTriggers: triggerType:" + timerTrigger.TriggerType.ToString());
                    //DeleteTimerTrigger(TTriggerType.CONNECT_DB);
                    client = "DBTest_Read_" + counter;
                    Interlocked.Increment(ref counter);
                    if (dapper)
                    {
                        if (pooling)
                        {
                            switch (timerTrigger.TriggerType)
                            {
                                case TTriggerType.READ:
                                    {
                                        //first request
                                        if (C > 1)
                                        {
                                            if (!dbComponentDapper_2.Read(client, out readTime))
                                            {
                                                Log.Information($"routine:{routine} retrying...!");
                                                AddTimerTrigger(requestType, db_ActionIntervals);
                                            }
                                            else
                                            {
                                                //addding subsiquent request
                                                //Interlocked.Increment(ref reqPerClient);
                                                //AddTimerTrigger(requestType, db_ActionIntervals);
                                                
                                            }

                                        }

                                        //subsequent request
                                        else
                                        {
                                            if (!dbComponentDapper_2.Read(client, out readTime_1))
                                            {
                                                Log.Information($"routine:{routine} retrying...!");
                                                AddTimerTrigger(requestType, db_ActionIntervals);
                                            }

                                            //Interlocked.Increment(ref reqPerClient);

                                            //if (reqPerClient < 20)
                                            //{
                                            //    AddTimerTrigger(requestType, db_ActionIntervals);
                                            //}

                                        }
                                        //Log.Information($"(ElapsedTime in reading = {ReadTime})");
                                        //Console.WriteLine($"(ElapsedTime in reading = {ReadTime})");

                                        //Interlocked.Increment(ref reqPerClient);
                                        //if (reqPerClient < 2)
                                        //    AddTimerTrigger(requestType, db_ActionIntervals);
                                        break;
                                    }
                                case TTriggerType.WRITE:
                                    {
                                        ContactSummaryDetails contactSummaryDetails = new ContactSummaryDetails(client, "L_" + client);
                                        dbComponentDapper_2.InsertToDatabase(contactSummaryDetails, out writeTime);
                                        //Log.Information($"(ElapsedTime in inserting = {WriteTime})");
                                        //Console.WriteLine($"(ElapsedTime in inserting = {WriteTime})");
                                        break;
                                    }
                                case TTriggerType.UPDATE:
                                    {
                                        //long UpdateTime = 0;
                                        ContactSummaryDetails contactSummaryDetails = new ContactSummaryDetails(client, "L_" + client);
                                        dbComponentDapper_2.UpdateInDatabase(contactSummaryDetails, out updateTime);
                                        //Log.Information($"(ElapsedTime in inserting = {UpdateTime})");
                                        //Console.WriteLine($"(ElapsedTime in inserting = {UpdateTime})");
                                        break;
                                    }

                                case TTriggerType.DELETE:
                                    {
                                        //DisconnectDatabase(timerTrigger.DBConnMode);
                                        break;
                                    }
                                case TTriggerType.DELETE_ALL:
                                    {
                                        //DisconnectDatabase(timerTrigger.DBConnMode);
                                        break;
                                    }
                                case TTriggerType.SEARCH:
                                    {
                                        //DisconnectDatabase(timerTrigger.DBConnMode);
                                        break;
                                    }

                                default:
                                    {
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            switch (timerTrigger.TriggerType)
                            {
                                case TTriggerType.CONNECT_DB:
                                    {
                                        //if conn is not opened successfully 
                                        if (!dbComponentDapper_1.ConnectionOpen(client))
                                        {
                                            dbConnectRetry++;
                                            //for retring to open connection
                                            AddTimerTrigger(TTriggerType.CONNECT_DB, db_ActionIntervals * dbConnectRetry);
                                            Log.Information($"routine:" + routine + " RetryNumber: {dbConnectRetry} _client: {client} trying to reconnect after {db_ActionIntervals * dbConnectRetry} ");

                                            //if(dbConnectRetry < 4)
                                            //    AddTimerTrigger(TTriggerType.CONNECT_DB, db_ActionIntervals * dbConnectRetry);
                                            //else
                                            //    AddTimerTrigger(TTriggerType.DISCONNECT_DB, db_ActionIntervals);
                                        }
                                        //if connection open successfully
                                        else
                                        {
                                            AddTimerTrigger(requestType, db_ActionIntervals);
                                            //if (dbConnectRetry > 0)
                                            //{
                                            //    //after sucessfull retry
                                            //    AddTimerTrigger(requestType, db_ActionIntervals);
                                            //}
                                            //else
                                            //{
                                            //    AddTimerTrigger(requestType, db_ActionIntervals);
                                            //}     
                                        }

                                        break;
                                    }
                                case TTriggerType.READ:
                                    {

                                        if (reqPerClient == 0)
                                        {
                                            //if not seccessfull retry
                                            if (!dbComponentDapper_1.Read(client, out readTime))
                                            {
                                                Log.Information($"routine:{routine} retrying...!");
                                                AddTimerTrigger(requestType, db_ActionIntervals);
                                                Log.Information($"routine:{routine} ConnectionState is: {dbComponentDapper_1.connection.State}");
                                                if (dbComponentDapper_1.connection.State != System.Data.ConnectionState.Open)
                                                    dbComponentDapper_1.ConnectionOpen(client);
                                            }
                                            else
                                            {
                                                //Interlocked.Increment(ref reqPerClient);
                                                AddTimerTrigger(requestType, db_ActionIntervals);
                                            }

                                        }

                                        else
                                        {
                                            //if not seccessfull retry
                                            if (!dbComponentDapper_1.Read(client, out readTime_1))
                                            {
                                                Log.Information($"routine:{routine} retrying...!");
                                                AddTimerTrigger(requestType, db_ActionIntervals);
                                                Log.Information($"routine:{routine} ConnectionState is: {dbComponentDapper_1.connection.State}");
                                                if (dbComponentDapper_1.connection.State != System.Data.ConnectionState.Open)
                                                    dbComponentDapper_1.ConnectionOpen(client);
                                            }

                                        }

                                        //Log.Information($"(ElapsedTime in reading = {ReadTime})");
                                        //Console.WriteLine($"(ElapsedTime in reading = {ReadTime})");

                                        //Interlocked.Increment(ref reqPerClient);
                                        //if (reqPerClient <= 1)
                                        //    AddTimerTrigger(requestType, db_ActionIntervals);
                                        break;
                                    }
                                case TTriggerType.WRITE:
                                    {
                                        ContactSummaryDetails contactSummaryDetails = new ContactSummaryDetails(client, "L_" + client);
                                        dbComponentDapper_1.InsertToDatabase(contactSummaryDetails, out writeTime);
                                        //Log.Information($"(ElapsedTime in inserting = {WriteTime})");
                                        //Console.WriteLine($"(ElapsedTime in inserting = {WriteTime})");
                                        break;
                                    }
                                case TTriggerType.UPDATE:
                                    {
                                        //long UpdateTime = 0;
                                        ContactSummaryDetails contactSummaryDetails = new ContactSummaryDetails(client, "L_" + client);
                                        dbComponentDapper_1.UpdateInDatabase(contactSummaryDetails, out updateTime);
                                        //Log.Information($"(ElapsedTime in inserting = {UpdateTime})");
                                        //Console.WriteLine($"(ElapsedTime in inserting = {UpdateTime})");
                                        break;
                                    }

                                case TTriggerType.DELETE:
                                    {
                                        //DisconnectDatabase(timerTrigger.DBConnMode);
                                        break;
                                    }
                                case TTriggerType.DELETE_ALL:
                                    {
                                        //DisconnectDatabase(timerTrigger.DBConnMode);
                                        break;
                                    }
                                case TTriggerType.SEARCH:
                                    {
                                        //DisconnectDatabase(timerTrigger.DBConnMode);
                                        break;
                                    }

                                default:
                                    {
                                        break;
                                    }
                            }
                        }
                    }
                    else
                    {
                        if (pooling)
                        {
                            switch (timerTrigger.TriggerType)
                            {
                                case TTriggerType.READ:
                                    {

                                        //if (reqPerClient == 0)
                                        //first request
                                        if (C > 1)
                                        {
                                            if (!dbComponent_2.Read(client, out readTime))
                                            {
                                                Log.Information($"routine:{routine} retrying...!");
                                                AddTimerTrigger(requestType, db_ActionIntervals);
                                            }
                                            else
                                            {
                                                //Interlocked.Increment(ref reqPerClient);
                                                //AddTimerTrigger(requestType, db_ActionIntervals);
                                            }

                                        }

                                        else
                                        {
                                            if (!dbComponent_2.Read(client, out readTime_1))
                                            {
                                                Log.Information($"routine:{routine} retrying...!");
                                                AddTimerTrigger(requestType, db_ActionIntervals);
                                            }

                                        }
                                        //Log.Information($"(ElapsedTime in reading = {ReadTime})");
                                        //Console.WriteLine($"(ElapsedTime in reading = {ReadTime})");

                                        //Interlocked.Increment(ref reqPerClient);
                                        //if (reqPerClient < 2)
                                        //    AddTimerTrigger(requestType, db_ActionIntervals);
                                        break;
                                    }
                                case TTriggerType.WRITE:
                                    {
                                        ContactSummaryDetails contactSummaryDetails = new ContactSummaryDetails(client, "L_" + client);
                                        dbComponent_2.InsertToDatabase(contactSummaryDetails, out writeTime);
                                        //Log.Information($"(ElapsedTime in inserting = {WriteTime})");
                                        //Console.WriteLine($"(ElapsedTime in inserting = {WriteTime})");
                                        break;
                                    }
                                case TTriggerType.UPDATE:
                                    {
                                        //long UpdateTime = 0;
                                        ContactSummaryDetails contactSummaryDetails = new ContactSummaryDetails(client, "L_" + client);
                                        dbComponent_2.UpdateInDatabase(contactSummaryDetails, out updateTime);
                                        //Log.Information($"(ElapsedTime in inserting = {UpdateTime})");
                                        //Console.WriteLine($"(ElapsedTime in inserting = {UpdateTime})");
                                        break;
                                    }

                                case TTriggerType.DELETE:
                                    {
                                        //DisconnectDatabase(timerTrigger.DBConnMode);
                                        break;
                                    }
                                case TTriggerType.DELETE_ALL:
                                    {
                                        //DisconnectDatabase(timerTrigger.DBConnMode);
                                        break;
                                    }
                                case TTriggerType.SEARCH:
                                    {
                                        //DisconnectDatabase(timerTrigger.DBConnMode);
                                        break;
                                    }

                                default:
                                    {
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            switch (timerTrigger.TriggerType)
                            {
                                case TTriggerType.CONNECT_DB:
                                    {
                                        //if conn is not opened successfully 
                                        if (!dbComponent_1.ConnectionOpen(client))
                                        {
                                            dbConnectRetry++;
                                            //for retring to open connection
                                            AddTimerTrigger(TTriggerType.CONNECT_DB, db_ActionIntervals * dbConnectRetry);
                                            Log.Information($"routine:" + routine + " RetryNumber: {dbConnectRetry} _client: {client} trying to reconnect after {db_ActionIntervals * dbConnectRetry} ");

                                            //if(dbConnectRetry < 4)
                                            //    AddTimerTrigger(TTriggerType.CONNECT_DB, db_ActionIntervals * dbConnectRetry);
                                            //else
                                            //    AddTimerTrigger(TTriggerType.DISCONNECT_DB, db_ActionIntervals);
                                        }
                                        //if connection open successfully
                                        else
                                        {
                                            AddTimerTrigger(requestType, db_ActionIntervals);
                                            //if (dbConnectRetry > 0)
                                            //{
                                            //    //after sucessfull retry
                                            //    AddTimerTrigger(requestType, db_ActionIntervals);
                                            //}
                                            //else
                                            //{
                                            //    AddTimerTrigger(requestType, db_ActionIntervals);
                                            //}     
                                        }

                                        break;
                                    }
                                case TTriggerType.READ:
                                    {

                                        if (reqPerClient == 0)
                                        {
                                            //if not seccessfull retry
                                            if (!dbComponent_1.Read(client, out readTime))
                                            {
                                                Log.Information($"routine:{routine} retrying...!");
                                                AddTimerTrigger(requestType, db_ActionIntervals);
                                                Log.Information($"routine:{routine} ConnectionState is: {dbComponent_1.connection.State}");
                                                if (dbComponent_1.connection.State != System.Data.ConnectionState.Open)
                                                    dbComponent_1.ConnectionOpen(client);
                                            }
                                            else
                                            {
                                                //Interlocked.Increment(ref reqPerClient);
                                                AddTimerTrigger(requestType, db_ActionIntervals);
                                            }

                                        }

                                        else
                                        {
                                            //if not seccessfull retry
                                            if (!dbComponent_1.Read(client, out readTime_1))
                                            {
                                                Log.Information($"routine:{routine} retrying...!");
                                                AddTimerTrigger(requestType, db_ActionIntervals);
                                                Log.Information($"routine:{routine} ConnectionState is: {dbComponent_1.connection.State}");
                                                if (dbComponent_1.connection.State != System.Data.ConnectionState.Open)
                                                    dbComponent_1.ConnectionOpen(client);
                                            }

                                        }

                                        //Log.Information($"(ElapsedTime in reading = {ReadTime})");
                                        //Console.WriteLine($"(ElapsedTime in reading = {ReadTime})");

                                        //Interlocked.Increment(ref reqPerClient);
                                        //if (reqPerClient <= 1)
                                        //    AddTimerTrigger(requestType, db_ActionIntervals);
                                        break;
                                    }
                                case TTriggerType.WRITE:
                                    {
                                        ContactSummaryDetails contactSummaryDetails = new ContactSummaryDetails(client, "L_" + client);
                                        dbComponent_1.InsertToDatabase(contactSummaryDetails, out writeTime);
                                        //Log.Information($"(ElapsedTime in inserting = {WriteTime})");
                                        //Console.WriteLine($"(ElapsedTime in inserting = {WriteTime})");
                                        break;
                                    }
                                case TTriggerType.UPDATE:
                                    {
                                        //long UpdateTime = 0;
                                        ContactSummaryDetails contactSummaryDetails = new ContactSummaryDetails(client, "L_" + client);
                                        dbComponent_1.UpdateInDatabase(contactSummaryDetails, out updateTime);
                                        //Log.Information($"(ElapsedTime in inserting = {UpdateTime})");
                                        //Console.WriteLine($"(ElapsedTime in inserting = {UpdateTime})");
                                        break;
                                    }

                                case TTriggerType.DELETE:
                                    {
                                        //DisconnectDatabase(timerTrigger.DBConnMode);
                                        break;
                                    }
                                case TTriggerType.DELETE_ALL:
                                    {
                                        //DisconnectDatabase(timerTrigger.DBConnMode);
                                        break;
                                    }
                                case TTriggerType.SEARCH:
                                    {
                                        //DisconnectDatabase(timerTrigger.DBConnMode);
                                        break;
                                    }

                                default:
                                    {
                                        break;
                                    }
                            }
                        }
                    }


                }

            }

            catch (Exception ex)
            {
                Log.Error("routine:" + routine + " ProcessTriggers:" + ex.Message);

            }
        }


        private void AddTimerTrigger(TTriggerType triggerType, int milliSeconds)
        {

            try
            {
                DateTime dt = DateTime.Now;
                dt = dt.AddMilliseconds(milliSeconds);

                if (lstTriggers.AddTimerTrigger(triggerType, dt) == null)
                {
                    Log.Debug("routine:" + routine + " AddTimerTrigger(Failed to Add Trigger): triggerType:" + triggerType.ToString());
                }
                else
                {
                    Log.Debug("routine:" + routine + " AddTimerTrigger: triggerType:" + triggerType.ToString() + ", ms:" + milliSeconds.ToString() + ", DT:" + dt.ToUniversalTime().ToString(), -1);
                }
            }

            catch (Exception ex)
            {
                Log.Error("routine:" + routine + " AddTimerTrigger:" + ex.Message);
            }

        }

        private void DeleteTimerTrigger(TTriggerType triggerType)
        {

            try
            {
                if (lstTriggers.DeleteTrigger(triggerType))
                {
                    Log.Debug("routine:" + routine + " DeleteTimerTrigger.Not found: triggerType:" + triggerType.ToString());
                }
                else
                {
                    Log.Debug("routine:" + routine + " DeleteTimerTrigger.Deleted: triggerType:" + triggerType.ToString());
                }
            }

            catch (Exception ex)
            {
                Log.Error("routine:" + routine + " DeleteTimerTrigger:" + ex.Message);
            }

        }

        public void createInstance(string client)
        {
            DBComponent_1 dbComponent = new DBComponent_1(configuration);
            long ReadTime = 0;
            if (dbComponent.ConnectionOpen(client))
            {
                dbComponent.Read(client, out ReadTime);
                Log.Information($"routine:" + routine + " (ElapsedTime in reading = {ReadTime})");
                Console.WriteLine($"routine:" + routine + " (ElapsedTime in reading = {ReadTime})");
            }
            else
            {
                Log.Information($"routine:" + routine + " retrying......");
            }

            DBComponent_1 dbComponent_W = new DBComponent_1(configuration);
            long WriteTime = 0;
            if (dbComponent_W.ConnectionOpen(client))
            {
                ContactSummaryDetails contactSummaryDetails = new ContactSummaryDetails(client, client);
                dbComponent_W.InsertToDatabase(contactSummaryDetails, out WriteTime);
                Log.Information($"routine:" + routine + " (ElapsedTime in inserting = {WriteTime})");
                Console.WriteLine($"routine:" + routine + " (ElapsedTime in inserting = {WriteTime})");
            }
            else
            {
                Log.Information($"routine:" + routine + " retrying......");
            }
        }
        private class ClientInfo
        {
            public DBManager client { get; set; }
        }
    }
}
