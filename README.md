# SqliteDataTierGenerator
Given a Sqlite Database, this generates a set of c# classes which performs basic CRUD operations. 

**Capabilities:**
* For every table, a corresponding class is created with Attributes defined for every field.
* Foreign Key Properties are defined on parent and child classes.
* Instances can be synchronised with the database using the Insert, Update, Delete and AddUpdate methods.

**How to use:**

1. Import the nuget package into your project using the command: dotnet add package Sqlite.CodeCreate
2. In a code create harness project, create a valid Sqlite connection to your database:

~~~c#
SqliteConnectionStringBuilder sb = new SqliteConnectionStringBuilder();
sb.DataSource = filename;
SqliteConnection conn = new SqliteConnection(sb.ToString());
conn.Open();
~~~

Here is a sample schema:

![Database Schema](/img/chinook-schema.svg?raw=true "Example Sqlite Schema")

And for the Customers Table, the code created looks as follows:

~~~c#
using Sqlite.CodeCreate;
using Microsoft.Data.Sqlite;

namespace Sqlite.CodeCreate
{
    public class Customers: DataEntityBase<Customers>, IEntityCreator<Customers>
    {

        ///<summary>
        ///Definition of Key Fields
        ///</summary>
        static Customers() {
            KeyFields = new(["CustomerId"]);
        }


        ///<summary>
        ///Gets / Sets the value for CustomerId
        ///This column forms part of the Primary Key
        ///This columns is not Nullable
        ///The underlying data type is INTEGER
        ///<summary>
        public long CustomerId { get; set; }

        ///<summary>
        ///Gets / Sets the value for FirstName
        ///This columns is not Nullable
        ///The underlying data type is NVARCHAR(40)
        ///<summary>
        public string FirstName { get; set; } = String.Empty;

        ///<summary>
        ///Gets / Sets the value for LastName
        ///This columns is not Nullable
        ///The underlying data type is NVARCHAR(20)
        ///<summary>
        public string LastName { get; set; } = String.Empty;

        ///<summary>
        ///Gets / Sets the value for Company
        ///This columns is Nullable
        ///The underlying data type is NVARCHAR(80)
        ///<summary>
        public string? Company { get; set; } = String.Empty;

        ///<summary>
        ///Gets / Sets the value for Address
        ///This columns is Nullable
        ///The underlying data type is NVARCHAR(70)
        ///<summary>
        public string? Address { get; set; } = String.Empty;

        ///<summary>
        ///Gets / Sets the value for City
        ///This columns is Nullable
        ///The underlying data type is NVARCHAR(40)
        ///<summary>
        public string? City { get; set; } = String.Empty;

        ///<summary>
        ///Gets / Sets the value for State
        ///This columns is Nullable
        ///The underlying data type is NVARCHAR(40)
        ///<summary>
        public string? State { get; set; } = String.Empty;

        ///<summary>
        ///Gets / Sets the value for Country
        ///This columns is Nullable
        ///The underlying data type is NVARCHAR(40)
        ///<summary>
        public string? Country { get; set; } = String.Empty;

        ///<summary>
        ///Gets / Sets the value for PostalCode
        ///This columns is Nullable
        ///The underlying data type is NVARCHAR(10)
        ///<summary>
        public string? PostalCode { get; set; } = String.Empty;

        ///<summary>
        ///Gets / Sets the value for Phone
        ///This columns is Nullable
        ///The underlying data type is NVARCHAR(24)
        ///<summary>
        public string? Phone { get; set; } = String.Empty;

        ///<summary>
        ///Gets / Sets the value for Fax
        ///This columns is Nullable
        ///The underlying data type is NVARCHAR(24)
        ///<summary>
        public string? Fax { get; set; } = String.Empty;

        ///<summary>
        ///Gets / Sets the value for Email
        ///This columns is not Nullable
        ///The underlying data type is NVARCHAR(60)
        ///<summary>
        public string Email { get; set; } = String.Empty;

        ///<summary>
        ///Gets / Sets the value for SupportRepId
        ///This columns is Nullable
        ///The underlying data type is INTEGER
        ///<summary>
        public long SupportRepId { get; set; }

        ///<summary>
        ///Returns the parent Employees
        ///</summary>
        public Employees FK_Employees
        {
            get
            {
                return Employees.ScalarStrict(("EmployeeId",this.SupportRepId));
            }
        }

        ///<summary>
        ///Returns the Collection of child Invoices records.
        ///</summary>
        public IEnumerable<Invoices> FK_Invoices
        {
            get
            {
                return Invoices.Collection(("CustomerId",this.CustomerId));
            }
        }


        ///<summary>
        ///Creates an instance of the Customers type from a database record.
        ///</summary>
        public static Customers Instance(SqliteDataReader reader)
        {
            Customers result = new();
            result.CustomerId = reader.GetInt64(0);
            result.FirstName = reader.GetString(1);
            result.LastName = reader.GetString(2);
            result.Company = reader.GetString(3);
            result.Address = reader.GetString(4);
            result.City = reader.GetString(5);
            result.State = reader.GetString(6);
            result.Country = reader.GetString(7);
            result.PostalCode = reader.GetString(8);
            result.Phone = reader.GetString(9);
            result.Fax = reader.GetString(10);
            result.Email = reader.GetString(11);
            result.SupportRepId = reader.GetInt64(12);
            return result;
        }

        public void AddUpdate()
        {
            Upsert(("CustomerId", this.CustomerId),("FirstName", this.FirstName),("LastName", this.LastName),("Company", this.Company),("Address", this.Address),("City", this.City),("State", this.State),("Country", this.Country),("PostalCode", this.PostalCode),("Phone", this.Phone),("Fax", this.Fax),("Email", this.Email),("SupportRepId", this.SupportRepId));
        }

        public void Insert()
        {
            Insert(("CustomerId", this.CustomerId),("FirstName", this.FirstName),("LastName", this.LastName),("Company", this.Company),("Address", this.Address),("City", this.City),("State", this.State),("Country", this.Country),("PostalCode", this.PostalCode),("Phone", this.Phone),("Fax", this.Fax),("Email", this.Email),("SupportRepId", this.SupportRepId));
        }

        public void Update()
        {
            Update(("CustomerId", this.CustomerId),("FirstName", this.FirstName),("LastName", this.LastName),("Company", this.Company),("Address", this.Address),("City", this.City),("State", this.State),("Country", this.Country),("PostalCode", this.PostalCode),("Phone", this.Phone),("Fax", this.Fax),("Email", this.Email),("SupportRepId", this.SupportRepId));
        }

        public void Delete()
        {
            Delete(("CustomerId", this.CustomerId),("FirstName", this.FirstName),("LastName", this.LastName),("Company", this.Company),("Address", this.Address),("City", this.City),("State", this.State),("Country", this.Country),("PostalCode", this.PostalCode),("Phone", this.Phone),("Fax", this.Fax),("Email", this.Email),("SupportRepId", this.SupportRepId));
        }
    }
}
~~~
