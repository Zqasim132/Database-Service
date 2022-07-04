using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;
using Serilog;
using Dapper;

namespace DatabaseService
{
    internal class DBComponentDapper_2
    {
        public readonly SqlConnection connection;
        private readonly string connectionStrings;
        private string routine = "DBC_Dapper_2";
        public delegate void QueriesExecutedHandler(string name);
        public event QueriesExecutedHandler QueriesExecuted;
        public DBComponentDapper_2(IConfigurationRoot _configuration)
        {
            //using (var listener = new EventCounterListener())
            //{
                
            //}
            connectionStrings = _configuration.GetConnectionString("DefaultConnection");
            connection = new SqlConnection(connectionStrings);
        }

        public bool ConnectionOpen(string client)
        {
            try
            {
                if ((connection != null) && (connection.State == ConnectionState.Open))
                {
                    Log.Information($"routine:{routine} ConnectionOpen(Already Connected) : client: {client}");
                    return true;
                }
                //using (var listener = new EventCounterListener())
                //{
                    
                //}

                connection.Open();
                Log.Information($"routine:{routine} ConnectionOpen(Connection is Opened by client: {client})");
                return true;
            }
            catch (InvalidOperationException ex)
            {
                Log.Error($"routine:{routine} Open(InvalidOperationException): clientName: {client} _ ErrorMessage: { ex.Message}");
                return false;
            }
            catch (SqlException ex)
            {
                Log.Error($"routine:{routine} Open(SqlException): clientName: {client} _ ErrorNumber:  {ex.Number} _ ErrorCode: {ex.ErrorCode} _ ErrorMessage: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Log.Error($"routine:{routine} Open(GeneralException): clientName: {client} _ErrorMessage {ex.Message}");
                return false;
            }


        }
        public bool ConnectionClose(string client)
        {
            if(connection.State == System.Data.ConnectionState.Closed)
            {
                Log.Information($"routine:{routine} ConnectionClose(Already Close) : client : {client}");
                return true;
            }

            try
            {
                connection.Close();
                Log.Information($"routine:{routine} ConnectionClose(): connection is closed by client : {client}");
                return true;
            }
            catch(Exception ex)
            {
                Log.Error($"routine:{routine} ConnectionClose(): client : {client} _ ErrorMessage: {ex.Message}");
                return false;
            }
            
        }
        public bool InsertToDatabase(ContactSummaryDetails contactSummaryDetails, out long time)
        {
            //if (connection.State != System.Data.ConnectionState.Open)
            //{
            //    Log.Error("routine:{routine} InsertToDatabase():" + "Connection is not Opened");
            //    //throw new Exception("Connection is not Opened");
            //    time = 0;
            //    return false;
            //}
            if (String.IsNullOrEmpty(contactSummaryDetails.First_Name))
            {
                Log.Error("routine:{routine} InsertToDatabase():" + "First_Name should not be null or Empty");
                //throw new ArgumentException("First_Name should not be null or Empty", nameof(contactSummaryDetails.First_Name));
                time = 0;
                return false;
            }

            if (String.IsNullOrEmpty(contactSummaryDetails.Last_Name))
            {
                Log.Error("routine:{routine} InsertToDatabase():" + "Last_Name should not be null or Empty");
                //throw new ArgumentException("Last_Name should not be null or Empty", nameof(contactSummaryDetails.Last_Name));
                time = 0;
                return false;
            }

            //if (IDChecker(contactSummaryDetails.Contact_Unique_Ref))
            //{
            //    throw new ArgumentException("contact with that id already exist ", nameof(contactSummaryDetails.Contact_Unique_Ref));
            //}

            try
            {
                using (SqlConnection con = new SqlConnection(connectionStrings))
                {
                    con.Open();
                    var query = @"insert into Contact_Summary_Details values(
                    @Contact_Unique_Ref,
                    @Contact_Type,
                    @Source_Unique_Ref,
                    @First_Name,
                    @Last_Name,
                    @Company_Name,
                    @Department,
                    @Full_Job_Title,
                    @Email,
                    @Email_2,
                    @Email_3,
                    @Extension,
                    @Extension_Unique_Ref,
                    @Business_1,
                    @Business_2,
                    @Home,
                    @Mobile,
                    @FAX,
                    @Pager,
                    @Region_Ref,
                    @Assistant_Contact_Ref,
                    @User_Profile,
                    @PIN,
                    @User_Field_1,
                    @User_Field_2,
                    @User_Field_3,
                    @Company_Section,
                    @Location,
                    @Title,
                    @Initials,
                    @Middle_Name,
                    @Room_Name,
                    @Cost_Center,
                    @Extension_RID,
                    @RRG_Pkid,
                    @Technical_Number)";

                    DynamicParameters Parameters = new DynamicParameters();

                    Parameters.Add("@Contact_Unique_Ref", contactSummaryDetails.Contact_Unique_Ref);
                    Parameters.Add("@Contact_Type", contactSummaryDetails.Contact_Type);
                    Parameters.Add("@Source_Unique_Ref", contactSummaryDetails.Source_Unique_Ref);
                    Parameters.Add("@First_Name", contactSummaryDetails.First_Name);
                    Parameters.Add("@Last_Name", contactSummaryDetails.Last_Name);
                    Parameters.Add("@Company_Name", contactSummaryDetails.Company_Name);
                    Parameters.Add("@Department", contactSummaryDetails.Department);
                    Parameters.Add("@Full_Job_Title", contactSummaryDetails.Full_Job_Title);
                    Parameters.Add("@Email", contactSummaryDetails.Email);
                    Parameters.Add("@Email_2", contactSummaryDetails.Email_2);
                    Parameters.Add("@Email_3", contactSummaryDetails.Email_3);
                    Parameters.Add("@Extension", contactSummaryDetails.Extension);
                    Parameters.Add("@Extension_Unique_Ref", contactSummaryDetails.Extension_Unique_Ref);
                    Parameters.Add("@Business_1", contactSummaryDetails.Business_1);
                    Parameters.Add("@Business_2", contactSummaryDetails.Business_2);
                    Parameters.Add("@Home", contactSummaryDetails.Home);
                    Parameters.Add("@Mobile", contactSummaryDetails.Mobile);
                    Parameters.Add("@FAX", contactSummaryDetails.FAX);
                    Parameters.Add("@Pager", contactSummaryDetails.Pager);
                    Parameters.Add("@Region_Ref", contactSummaryDetails.Region_Ref);
                    Parameters.Add("@Assistant_Contact_Ref", contactSummaryDetails.Assistant_Contact_Ref);
                    Parameters.Add("@User_Profile", contactSummaryDetails.User_Profile);
                    Parameters.Add("@PIN", contactSummaryDetails.PIN);
                    Parameters.Add("@User_Field_1", contactSummaryDetails.User_Field_1);
                    Parameters.Add("@User_Field_2", contactSummaryDetails.User_Field_2);
                    Parameters.Add("@User_Field_3", contactSummaryDetails.User_Field_3);
                    Parameters.Add("@Company_Section", contactSummaryDetails.Company_Section);
                    Parameters.Add("@Location", contactSummaryDetails.Location);
                    Parameters.Add("@Title", contactSummaryDetails.Title);
                    Parameters.Add("@Initials", contactSummaryDetails.Initials);
                    Parameters.Add("@Middle_Name", contactSummaryDetails.Middle_Name);
                    Parameters.Add("@Room_Name", contactSummaryDetails.Room_Name);
                    Parameters.Add("@Cost_Center", contactSummaryDetails.Cost_Center);
                    Parameters.Add("@Extension_RID", contactSummaryDetails.Extension_RID);
                    Parameters.Add("@RRG_Pkid", contactSummaryDetails.RRG_Pkid);
                    Parameters.Add("@Technical_Number", contactSummaryDetails.Technical_Number);
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    var rows = con.Execute(query, Parameters);
                    //Log.Information($"InsertToDatabase(): Inserted successfully by client: {contactSummaryDetails.First_Name.Substring(0, contactSummaryDetails.First_Name.Length - 2)}");
                    Log.Information($"routine:{routine} InsertToDatabase(): Inserted successfully by client: {contactSummaryDetails.First_Name}");
                    stopwatch.Stop();
                    time = stopwatch.ElapsedMilliseconds;
                    return true;
                }
                
            }
            catch (Exception ex)
            {
                Log.Error("routine:{routine} InsertToDatabase(): " + ex.Message);
                time = 0;
                return false;
            }
        }

        private bool IDChecker(Guid? id)
        {
            if (connection.State != System.Data.ConnectionState.Open)
            {
                throw new Exception("Connection is not Opened");
            }
            var query = "select * from Customers where id = @id";
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@id", id);
            bool idChecker = false;
            using (SqlConnection con = new SqlConnection(connectionStrings))
            {
                con.Open();
                var reader = con.QueryFirst(query, parameters);

                idChecker = reader.HasRows;
                reader.Close();
            }
            if (idChecker)
            {
                return true;
            }

            return false;
        }

        public void DeleteInDatabase(Guid? id)
        {
            if (connection.State != System.Data.ConnectionState.Open)
            {
                throw new Exception("Connection is not Opened");
            }
            if (id == null || id == Guid.Empty )
            {
                throw new ArgumentException("Id is null of empty", nameof(id));
            }

            if (!IDChecker(id))
            {
                throw new ArgumentException("contact with that id does not exist", nameof(id));
            }


            var query = "Delete from Contact_Summary_Details where Contact_Unique_Ref = @id";
            DynamicParameters parameters = new DynamicParameters();

            parameters.Add("@id", id);

            using (SqlConnection con = new SqlConnection(connectionStrings))
            {
                con.Open();
                con.Execute(query, parameters);
            }
        }

        public bool UpdateInDatabase(ContactSummaryDetails contactSummaryDetails, out long time)
        {
            if (connection.State != System.Data.ConnectionState.Open)
            {
                Log.Error("routine:{routine} UpdateInDatabase():" + "Connection is not Opened");
                //throw new Exception("Connection is not Opened");
                time = 0;
                return false;
            }
            if (String.IsNullOrEmpty(contactSummaryDetails.First_Name))
            {
                Log.Error("routine:{routine} UpdateInDatabase():" + "First_Name should not be null or Empty");
                //throw new ArgumentException("First_Name should not be null or Empty", nameof(contactSummaryDetails.First_Name));
                time = 0;
                return false;
            }

            if (String.IsNullOrEmpty(contactSummaryDetails.Last_Name))
            {
                Log.Error("routine:{routine} UpdateInDatabase():" + "Last_Name should not be null or Empty");
                //throw new ArgumentException("Last_Name should not be null or Empty", nameof(contactSummaryDetails.Last_Name));
                time = 0;
                return false;
            }

            //if (!IDChecker(contactSummaryDetails.Contact_Unique_Ref))
            //{
            //    throw new ArgumentException("customer with that id does not exist", nameof(contactSummaryDetails.Contact_Unique_Ref));
            //}
            try
            {
                    var query = @"Update Contact_Summary_Details set 
                    Contact_Unique_Ref = @Contact_Unique_Ref,
                    Contact_Type = @Contact_Type,
                    Source_Unique_Ref = @Source_Unique_Ref,
                    First_Name= @First_Name ,
                    Last_Name = @Last_Name ,
                    Company_Name = @Company_Name,
                    Department = @Department,
                    Full_Job_Title = @Full_Job_Title,
                    Email = @Email,
                    Email_2 = @Email_2,
                    Email_3 = @Email_3,
                    Extension = @Extension,
                    Extension_Unique_Ref = @Extension_Unique_Ref,
                    Business_1 = @Business_1,
                    Business_2 = @Business_2,
                    Home = @Home,
                    Mobile = @Mobile,
                    FAX = @FAX,
                    Pager = @Pager,
                    Region_Ref = @Region_Ref,
                    Assistant_Contact_Ref = @Assistant_Contact_Ref,
                    User_Profile = @User_Profile,
                    PIN = @PIN,
                    User_Field_1 = @User_Field_1,
                    User_Field_2 = @User_Field_2,
                    User_Field_3 = @User_Field_3,
                    Company_Section = @Company_Section,
                    Location = @Location,
                    Title = @Title,
                    Initials = @Initials,
                    Middle_Name = @Middle_Name,
                    Room_Name = @Room_Name,
                    Cost_Center = @Cost_Center,
                    Extension_RID = @Extension_RID,
                    RRG_Pkid = @RRG_Pkid,
                    Technical_Number = @Technical_Number";

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Contact_Unique_Ref", contactSummaryDetails.Contact_Unique_Ref);
                parameters.Add("@Contact_Type", contactSummaryDetails.Contact_Type);
                parameters.Add("@Source_Unique_Ref", contactSummaryDetails.Source_Unique_Ref);
                parameters.Add("@First_Name", contactSummaryDetails.First_Name);
                parameters.Add("@Last_Name", contactSummaryDetails.Last_Name);
                parameters.Add("@Company_Name", contactSummaryDetails.Company_Name);
                parameters.Add("@Department", contactSummaryDetails.Department);
                parameters.Add("@Full_Job_Title", contactSummaryDetails.Full_Job_Title);
                parameters.Add("@Email", contactSummaryDetails.Email);
                parameters.Add("@Email_2", contactSummaryDetails.Email_2);
                parameters.Add("@Email_3", contactSummaryDetails.Email_3);
                parameters.Add("@Extension", contactSummaryDetails.Extension);
                parameters.Add("@Extension_Unique_Ref", contactSummaryDetails.Extension_Unique_Ref);
                parameters.Add("@Business_1", contactSummaryDetails.Business_1);
                parameters.Add("@Business_2", contactSummaryDetails.Business_2);
                parameters.Add("@Home", contactSummaryDetails.Home);
                parameters.Add("@Mobile", contactSummaryDetails.Mobile);
                parameters.Add("@FAX", contactSummaryDetails.FAX);
                parameters.Add("@Pager", contactSummaryDetails.Pager);
                parameters.Add("@Region_Ref", contactSummaryDetails.Region_Ref);
                parameters.Add("@Assistant_Contact_Ref", contactSummaryDetails.Assistant_Contact_Ref);
                parameters.Add("@User_Profile", contactSummaryDetails.User_Profile);
                parameters.Add("@PIN", contactSummaryDetails.PIN);
                parameters.Add("@User_Field_1", contactSummaryDetails.User_Field_1);
                parameters.Add("@User_Field_2", contactSummaryDetails.User_Field_2);
                parameters.Add("@User_Field_3", contactSummaryDetails.User_Field_3);
                parameters.Add("@Company_Section", contactSummaryDetails.Company_Section);
                parameters.Add("@Location", contactSummaryDetails.Location);
                parameters.Add("@Title", contactSummaryDetails.Title);
                parameters.Add("@Initials", contactSummaryDetails.Initials);
                parameters.Add("@Middle_Name", contactSummaryDetails.Middle_Name);
                parameters.Add("@Room_Name", contactSummaryDetails.Room_Name);
                parameters.Add("@Cost_Center", contactSummaryDetails.Cost_Center);
                parameters.Add("@Extension_RID", contactSummaryDetails.Extension_RID);
                parameters.Add("@RRG_Pkid", contactSummaryDetails.RRG_Pkid);
                parameters.Add("@Technical_Number", contactSummaryDetails.Technical_Number);

                Stopwatch stopwatch = Stopwatch.StartNew();
                using (SqlConnection con = new SqlConnection(connectionStrings))
                {
                    con.Open();
                    con.Execute(query, parameters);
                }
                stopwatch.Stop();
                time = stopwatch.ElapsedMilliseconds;
                //Log.Information($"UpdateInDatabase(): Updated Sucessfully by client: {contactSummaryDetails.First_Name.Substring(0, contactSummaryDetails.First_Name.Length - 2)}");
                Log.Information($"routine:{routine} UpdateInDatabase(): Updated Sucessfully by client: {contactSummaryDetails.First_Name}");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error("routine:{routine} UpdateInDatabase(): " + ex.Message);
                time = 0;
                return false;
            }
        }

        public void Search(Guid? id)
        {
            if (connection.State != System.Data.ConnectionState.Open)
            {
                throw new Exception("Connection is not Opened");
            }
            if (id == null || id == Guid.Empty)
            {
                throw new ArgumentException("Id is null of empty", nameof(id));
            }

            if (!IDChecker(id))
            {
                throw new ArgumentException("customer with that id does not exist", nameof(id));
            }

            connection.Open();

            var query = "select * from Contact_Summary_Details where Contact_Unique_Ref = @id";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@id", id);

            var reader = connection.ExecuteReader(query, parameters);
            reader.Read();

            Console.WriteLine($"{reader.GetInt32(0)}.{reader.GetString(1)} - {reader.GetString(2)}");
            reader.Close();

            connection.Close();
        }

        public bool Read(string client, out long time)
        {
            int count = 0;
            try
            {
                //if (connection.State != System.Data.ConnectionState.Open)
                //{
                //    throw new Exception("Connection is not Opened");
                //}

                using (SqlConnection con = new SqlConnection(connectionStrings))
                {
                    con.Open();
                    var query = "select TOP(1000) * from Contact_Summary_Details";

                    //var reader = cmd.ExecuteReader();

                    //while (reader.Read())
                    //{
                    //    //Console.WriteLine($"{reader.GetGuid(0)} - {reader.GetString(1)} - {reader.GetString(2)}");
                    //}

                    //reader.Close();


                    IDataReader reader = con.ExecuteReader(query);
                    var dbSet = new DataSet();
                    Stopwatch sw = Stopwatch.StartNew();
                    //Console.WriteLine($"Stopwatch.StartNew()");
                    dbSet.Load(reader, LoadOption.OverwriteChanges, "table");
                    int rows = dbSet.Tables["table"].Rows.Count;
                    sw.Stop();
                    Log.Information($"routine:{routine} Read(): {rows} Rows Data read successfully by Client : {client}");
                    time = sw.ElapsedMilliseconds;
                    //dBManager.readTime = sw.ElapsedMilliseconds;
                    //Console.WriteLine($"Read time of client: {client} is: {dBManager.readTime} milliseconds ");
                    QueriesExecuted(client);
                    return true;
                }
            }
            catch (InvalidOperationException ex)
            {
                ////retrying for first time
                //if (count == 0)
                //{
                //    Log.Information($"routine:{routine} retrying...!")
                //}
                //    Read(client, out time);

                //Interlocked.Increment(ref count);
                Log.Error($"routine:{routine} Open(InvalidOperationException): clientName: {client} _ ErrorMessage: { ex.Message}");
                time = 0;
                return false;
            }
            catch (SqlException ex)
            {
                Log.Error($"routine:{routine} Open(SqlException): clientName: {client} _ ErrorNumber:  {ex.Number} _ ErrorCode: {ex.ErrorCode} _ ErrorMessage: {ex.Message}");
                time = 0;
                return false;
            }
            catch (Exception ex)
            {
                Log.Error($"routine:{routine} Read(GeneralException): client : {client} _ ErrorMessage: {ex.Message}");
                time = 0;
                return false;
            }
            
        }

        public bool Read_1(string client, DBManager dBManager)
        {
            int count = 0;
            try
            {
                //if (connection.State != System.Data.ConnectionState.Open)
                //{
                //    throw new Exception("Connection is not Opened");
                //}

                using (SqlConnection con = new SqlConnection(connectionStrings))
                {
                    con.Open();
                    var query = "select * from Contact_Summary_Details";

                    //var reader = cmd.ExecuteReader();

                    //while (reader.Read())
                    //{
                    //    //Console.WriteLine($"{reader.GetGuid(0)} - {reader.GetString(1)} - {reader.GetString(2)}");
                    //}

                    //reader.Close();


                    IDataReader reader = con.ExecuteReader(query);
                    var dbSet = new DataSet();
                    Stopwatch sw = Stopwatch.StartNew();
                    dbSet.Load(reader, LoadOption.OverwriteChanges, "table");
                    int rows = dbSet.Tables["table"].Rows.Count;
                    sw.Stop();
                    Log.Information($"routine:{routine} Read(): {rows} Rows Data read successfully by Client : {client}");
                    dBManager.readTime_1 = sw.ElapsedMilliseconds;
                    Console.WriteLine($"\t\t\t\t\t\tRead_1 time of client: {client} is: {dBManager.readTime_1} milliseconds");
                    return true;
                }
            }
            catch (InvalidOperationException ex)
            {
                ////retrying for first time
                //if (count == 0)
                //{
                //    Log.Information($"routine:{routine} retrying...!")
                //}
                //    Read(client, out time);

                //Interlocked.Increment(ref count);
                Log.Error($"routine:{routine} Open(InvalidOperationException): clientName: {client} _ ErrorMessage: { ex.Message}");
                //time = 0;
                return false;
            }
            catch (SqlException ex)
            {
                Log.Error($"routine:{routine} Open(SqlException): clientName: {client} _ ErrorNumber:  {ex.Number} _ ErrorCode: {ex.ErrorCode} _ ErrorMessage: {ex.Message}");
                //time = 0;
                return false;
            }
            catch (Exception ex)
            {
                Log.Error($"routine:{routine} Read(GeneralException): client : {client} _ ErrorMessage: {ex.Message}");
                //time = 0;
                return false;
            }

        }
        public void DeleteAll()
        {
            if (connection.State != System.Data.ConnectionState.Open)
            {
                throw new Exception("Connection is not Opened");
            }

            var query = "Delete from Contact_Summary_Details";

            connection.Execute(query);
        }

    }
}
