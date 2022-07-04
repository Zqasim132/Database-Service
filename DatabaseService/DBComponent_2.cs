using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace DatabaseService
{
    internal class DBComponent_2
    {
        public readonly SqlConnection connection;
        private readonly string connectionStrings;
        private string routine = "DBC_2";
        public delegate void QueriesExecutedHandler(string name);
        public event QueriesExecutedHandler QueriesExecuted;
        public DBComponent_2(IConfigurationRoot _configuration)
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
                    var conn = new SqlCommand(
                                @"insert into Contact_Summary_Details values(
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
                    @Technical_Number)",
                                con);

                conn.Parameters.AddWithValue("@Contact_Unique_Ref", contactSummaryDetails.Contact_Unique_Ref);
                conn.Parameters.AddWithValue("@Contact_Type", contactSummaryDetails.Contact_Type);
                conn.Parameters.AddWithValue("@Source_Unique_Ref", contactSummaryDetails.Source_Unique_Ref);
                conn.Parameters.AddWithValue("@First_Name", contactSummaryDetails.First_Name);
                conn.Parameters.AddWithValue("@Last_Name", contactSummaryDetails.Last_Name);
                conn.Parameters.AddWithValue("@Company_Name", contactSummaryDetails.Company_Name);
                conn.Parameters.AddWithValue("@Department", contactSummaryDetails.Department);
                conn.Parameters.AddWithValue("@Full_Job_Title", contactSummaryDetails.Full_Job_Title);
                conn.Parameters.AddWithValue("@Email", contactSummaryDetails.Email);
                conn.Parameters.AddWithValue("@Email_2", contactSummaryDetails.Email_2);
                conn.Parameters.AddWithValue("@Email_3", contactSummaryDetails.Email_3);
                conn.Parameters.AddWithValue("@Extension", contactSummaryDetails.Extension);
                conn.Parameters.AddWithValue("@Extension_Unique_Ref", contactSummaryDetails.Extension_Unique_Ref);
                conn.Parameters.AddWithValue("@Business_1", contactSummaryDetails.Business_1);
                conn.Parameters.AddWithValue("@Business_2", contactSummaryDetails.Business_2);
                conn.Parameters.AddWithValue("@Home", contactSummaryDetails.Home);
                conn.Parameters.AddWithValue("@Mobile", contactSummaryDetails.Mobile);
                conn.Parameters.AddWithValue("@FAX", contactSummaryDetails.FAX);
                conn.Parameters.AddWithValue("@Pager", contactSummaryDetails.Pager);
                conn.Parameters.AddWithValue("@Region_Ref", contactSummaryDetails.Region_Ref);
                conn.Parameters.AddWithValue("@Assistant_Contact_Ref", contactSummaryDetails.Assistant_Contact_Ref);
                conn.Parameters.AddWithValue("@User_Profile", contactSummaryDetails.User_Profile);
                conn.Parameters.AddWithValue("@PIN", contactSummaryDetails.PIN);
                conn.Parameters.AddWithValue("@User_Field_1", contactSummaryDetails.User_Field_1);
                conn.Parameters.AddWithValue("@User_Field_2", contactSummaryDetails.User_Field_2);
                conn.Parameters.AddWithValue("@User_Field_3", contactSummaryDetails.User_Field_3);
                conn.Parameters.AddWithValue("@Company_Section", contactSummaryDetails.Company_Section);
                conn.Parameters.AddWithValue("@Location", contactSummaryDetails.Location);
                conn.Parameters.AddWithValue("@Title", contactSummaryDetails.Title);
                conn.Parameters.AddWithValue("@Initials", contactSummaryDetails.Initials);
                conn.Parameters.AddWithValue("@Middle_Name", contactSummaryDetails.Middle_Name);
                conn.Parameters.AddWithValue("@Room_Name", contactSummaryDetails.Room_Name);
                conn.Parameters.AddWithValue("@Cost_Center", contactSummaryDetails.Cost_Center);
                conn.Parameters.AddWithValue("@Extension_RID", contactSummaryDetails.Extension_RID);
                conn.Parameters.AddWithValue("@RRG_Pkid", contactSummaryDetails.RRG_Pkid);
                conn.Parameters.AddWithValue("@Technical_Number", contactSummaryDetails.Technical_Number);
                Stopwatch stopwatch = Stopwatch.StartNew();
                
                    var rows = conn.ExecuteNonQuery();
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
            var conn = new SqlCommand("select * from Customers where id = @id", connection);

            conn.Parameters.AddWithValue("@id", id);
            var reader = conn.ExecuteReader();

            bool idChecker = reader.HasRows;
            reader.Close();

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


            var conn = new SqlCommand("Delete from Contact_Summary_Details where Contact_Unique_Ref = @id", connection);

            conn.Parameters.AddWithValue("@id", id);

            conn.ExecuteNonQuery();
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
                var conn = new SqlCommand(
                @"Update Contact_Summary_Details set 
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
                    Technical_Number = @Technical_Number", connection);

                conn.Parameters.AddWithValue("@Contact_Unique_Ref", contactSummaryDetails.Contact_Unique_Ref);
                conn.Parameters.AddWithValue("@Contact_Type", contactSummaryDetails.Contact_Type);
                conn.Parameters.AddWithValue("@Source_Unique_Ref", contactSummaryDetails.Source_Unique_Ref);
                conn.Parameters.AddWithValue("@First_Name", contactSummaryDetails.First_Name);
                conn.Parameters.AddWithValue("@Last_Name", contactSummaryDetails.Last_Name);
                conn.Parameters.AddWithValue("@Company_Name", contactSummaryDetails.Company_Name);
                conn.Parameters.AddWithValue("@Department", contactSummaryDetails.Department);
                conn.Parameters.AddWithValue("@Full_Job_Title", contactSummaryDetails.Full_Job_Title);
                conn.Parameters.AddWithValue("@Email", contactSummaryDetails.Email);
                conn.Parameters.AddWithValue("@Email_2", contactSummaryDetails.Email_2);
                conn.Parameters.AddWithValue("@Email_3", contactSummaryDetails.Email_3);
                conn.Parameters.AddWithValue("@Extension", contactSummaryDetails.Extension);
                conn.Parameters.AddWithValue("@Extension_Unique_Ref", contactSummaryDetails.Extension_Unique_Ref);
                conn.Parameters.AddWithValue("@Business_1", contactSummaryDetails.Business_1);
                conn.Parameters.AddWithValue("@Business_2", contactSummaryDetails.Business_2);
                conn.Parameters.AddWithValue("@Home", contactSummaryDetails.Home);
                conn.Parameters.AddWithValue("@Mobile", contactSummaryDetails.Mobile);
                conn.Parameters.AddWithValue("@FAX", contactSummaryDetails.FAX);
                conn.Parameters.AddWithValue("@Pager", contactSummaryDetails.Pager);
                conn.Parameters.AddWithValue("@Region_Ref", contactSummaryDetails.Region_Ref);
                conn.Parameters.AddWithValue("@Assistant_Contact_Ref", contactSummaryDetails.Assistant_Contact_Ref);
                conn.Parameters.AddWithValue("@User_Profile", contactSummaryDetails.User_Profile);
                conn.Parameters.AddWithValue("@PIN", contactSummaryDetails.PIN);
                conn.Parameters.AddWithValue("@User_Field_1", contactSummaryDetails.User_Field_1);
                conn.Parameters.AddWithValue("@User_Field_2", contactSummaryDetails.User_Field_2);
                conn.Parameters.AddWithValue("@User_Field_3", contactSummaryDetails.User_Field_3);
                conn.Parameters.AddWithValue("@Company_Section", contactSummaryDetails.Company_Section);
                conn.Parameters.AddWithValue("@Location", contactSummaryDetails.Location);
                conn.Parameters.AddWithValue("@Title", contactSummaryDetails.Title);
                conn.Parameters.AddWithValue("@Initials", contactSummaryDetails.Initials);
                conn.Parameters.AddWithValue("@Middle_Name", contactSummaryDetails.Middle_Name);
                conn.Parameters.AddWithValue("@Room_Name", contactSummaryDetails.Room_Name);
                conn.Parameters.AddWithValue("@Cost_Center", contactSummaryDetails.Cost_Center);
                conn.Parameters.AddWithValue("@Extension_RID", contactSummaryDetails.Extension_RID);
                conn.Parameters.AddWithValue("@RRG_Pkid", contactSummaryDetails.RRG_Pkid);
                conn.Parameters.AddWithValue("@Technical_Number", contactSummaryDetails.Technical_Number);

                Stopwatch stopwatch = Stopwatch.StartNew();
                conn.ExecuteNonQuery();
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

            var conn = new SqlCommand("select * from Contact_Summary_Details where Contact_Unique_Ref = @id", connection);

            conn.Parameters.AddWithValue("@id", id);

            var reader = conn.ExecuteReader();
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
                    var cmd = new SqlCommand("select TOP(1000) * from Contact_Summary_Details", con);

                    //var reader = cmd.ExecuteReader();

                    //while (reader.Read())
                    //{
                    //    //Console.WriteLine($"{reader.GetGuid(0)} - {reader.GetString(1)} - {reader.GetString(2)}");
                    //}

                    //reader.Close();


                    var adapter = new SqlDataAdapter(cmd);
                    var dbSet = new DataSet();
                    Stopwatch sw = Stopwatch.StartNew();
                    var rows = adapter.Fill(dbSet, "contactSummaryDetails");
                    sw.Stop();
                    Log.Information($"routine:{routine} Read(): {rows} Rows Data read successfully by Client : {client}");
                    time = sw.ElapsedMilliseconds;
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

        public void DeleteAll()
        {
            if (connection.State != System.Data.ConnectionState.Open)
            {
                throw new Exception("Connection is not Opened");
            }

            var conn = new SqlCommand("Delete from Contact_Summary_Details", connection);

            conn.ExecuteNonQuery();
        }

    }
}
