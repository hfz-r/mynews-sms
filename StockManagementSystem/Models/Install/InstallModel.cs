using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using StockManagementSystem.Core.Data;
using StockManagementSystem.Validators.Install;
using StockManagementSystem.Web.Models;
using StockManagementSystem.Web.Mvc.ModelBinding;

namespace StockManagementSystem.Models.Install
{
    [Validator(typeof(InstallValidator))]
    public class InstallModel : BaseModel
    {
        public string AdminEmail { get; set; }
        public string AdminUsername { get; set; }
        [NoTrim]
        [DataType(DataType.Password)]
        public string AdminPassword { get; set; }
        [NoTrim]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        public string DatabaseConnectionString { get; set; }
        public DataProviderType DataProvider { get; set; }
        //SQL Server properties
        public string SqlConnectionInfo { get; set; }

        public string SqlServerName { get; set; }
        public string SqlDatabaseName { get; set; }
        public string SqlServerUsername { get; set; }
        [DataType(DataType.Password)]
        public string SqlServerPassword { get; set; }
        public string SqlAuthenticationType { get; set; }
        public bool SqlServerCreateDatabase { get; set; }

        public bool UseCustomCollation { get; set; }
        public string Collation { get; set; }

        public bool DisableSampleDataOption { get; set; }
        public bool InstallSampleData { get; set; }
    }
}