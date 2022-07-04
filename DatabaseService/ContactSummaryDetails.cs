using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DatabaseService
{
    internal class ContactSummaryDetails
    {
        public Guid Contact_Unique_Ref { get; set; }
        [MaxLength(1, ErrorMessage = "maximum {1} characters allowed")]
        public string Contact_Type { get; set; }
        [MaxLength(9, ErrorMessage = "maximum {9} characters allowed")]
        public string Source_Unique_Ref { get; set; }
        [MaxLength(40, ErrorMessage = "maximum {40} characters allowed")]
        public string First_Name { get; set; }
        [MaxLength(50, ErrorMessage = "maximum {50} characters allowed")]
        public string Last_Name { get; set; }
        [MaxLength(100, ErrorMessage = "maximum {100} characters allowed")]
        public string Company_Name { get; set; }
        [MaxLength(100, ErrorMessage = "maximum {100} characters allowed")]
        public string Department { get; set; }
        [MaxLength(100, ErrorMessage = "maximum {100} characters allowed")]
        public string Full_Job_Title { get; set; }
        [MaxLength(100, ErrorMessage = "maximum {100} characters allowed")]
        public string Email { get; set; }
        [MaxLength(100, ErrorMessage = "maximum {100} characters allowed")]
        public string Email_2 { get; set; }
        [MaxLength(100, ErrorMessage = "maximum {100} characters allowed")]
        public string Email_3 { get; set; }
        [MaxLength(40, ErrorMessage = "maximum {40} characters allowed")]
        public string Extension { get; set; }
        public Guid Extension_Unique_Ref { get; set; }
        [MaxLength(40, ErrorMessage = "maximum {40} characters allowed")]
        public string Business_1 { get; set; }
        [MaxLength(40, ErrorMessage = "maximum {40} characters allowed")]
        public string Business_2 { get; set; }
        [MaxLength(40, ErrorMessage = "maximum {40} characters allowed")]
        public string Home { get; set; }
        [MaxLength(40, ErrorMessage = "maximum {40} characters allowed")]
        public string Mobile { get; set; }
        [MaxLength(40, ErrorMessage = "maximum {40} characters allowed")]
        public string FAX { get; set; }
        [MaxLength(40, ErrorMessage = "maximum {40} characters allowed")]
        public string Pager { get; set; }
        [MaxLength(9, ErrorMessage = "maximum {9} characters allowed")]
        public string Region_Ref { get; set; }
        public Guid Assistant_Contact_Ref { get; set; }
        [MaxLength(255, ErrorMessage = "maximum {255} characters allowed")]
        public string User_Profile { get; set; }
        [MaxLength(20, ErrorMessage = "maximum {20} characters allowed")]
        public string PIN { get; set; }
        [MaxLength(100, ErrorMessage = "maximum {100} characters allowed")]
        public string User_Field_1 { get; set; }
        [MaxLength(100, ErrorMessage = "maximum {100} characters allowed")]
        public string User_Field_2 { get; set; }
        [MaxLength(100, ErrorMessage = "maximum {100} characters allowed")]
        public string User_Field_3 { get; set; }
        [MaxLength(50, ErrorMessage = "maximum {50} characters allowed")]
        public string Company_Section { get; set; }
        [MaxLength(100, ErrorMessage = "maximum {100} characters allowed")]
        public string Location { get; set; }
        [MaxLength(4, ErrorMessage = "maximum {4} characters allowed")]
        public string Title { get; set; }
        [MaxLength(40, ErrorMessage = "maximum {40} characters allowed")]
        public string Initials { get; set; }
        [MaxLength(40, ErrorMessage = "maximum {40} characters allowed")]
        public string Middle_Name { get; set; }
        [MaxLength(50, ErrorMessage = "maximum {50} characters allowed")]
        public string Room_Name { get; set; }
        [MaxLength(100, ErrorMessage = "maximum {100} characters allowed")]
        public string Cost_Center { get; set; }
        [MaxLength(255, ErrorMessage = "maximum {255} characters allowed")]
        public string Extension_RID { get; set; }
        public Guid RRG_Pkid { get; set; }
        [MaxLength(40, ErrorMessage = "maximum {40} characters allowed")]
        public string Technical_Number { get; set; }

        public ContactSummaryDetails(string fName, string lName)
        {
           string rand =  new Random().Next(10000, 999999).ToString();
            Contact_Unique_Ref = Guid.NewGuid();
            Contact_Type = "I";
            Source_Unique_Ref = "EXS100006";
            First_Name = fName;
            Last_Name = lName;
            Company_Name = "";
            Department = "R&D";
            Full_Job_Title = "";
            Email = "";
            Email_2 = "";
            Email_3 = "";
            Extension = rand;
            Extension_Unique_Ref = Guid.NewGuid();
            Business_1 = "";
            Business_2 = "";
            Home = "";
            Mobile = "";
            FAX = "";
            Pager = "";
            Region_Ref = "";
            Assistant_Contact_Ref = Guid.NewGuid();
            User_Profile = "";
            PIN = "";
            User_Field_1 = "";
            User_Field_2 = "";
            User_Field_3 = "";
            Company_Section = "";
            Location = "";
            Title = "";
            Initials = "";
            Middle_Name = "";
            Room_Name = "";
            Cost_Center = "";
            Extension_RID = "";
            RRG_Pkid = Guid.NewGuid();
            Technical_Number = rand;
        }
    }
}
