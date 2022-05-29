using Microsoft.AspNetCore.Http;
using Nivara.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Nivara.Models
{
    public class EmployeeModel
    {
        public EmployeeModel()
        {
            Cities = new List<CitiesModel>();
            CompanyRoles = new List<CompanyRolesModel>();
            States = new List<StatesModel>();
            Countries = new List<CountriesModel>();
            Titles = new List<string>();
        }
        public int Id { get; set; }
        public string AspNetUserId { get; set; }
        public string Email { get; set; }
        public int CompanyId { get; set; }
        public int CompanyRoleId { get; set; }
        public string Prefix { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public int CityId { get; set; }
        public int StateId { get; set; }
        public int CountryId { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string PostalCode { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime? ModifiedDateTime { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
        public string ContactNumber { get; set; }
        public List<CountriesModel> Countries { get; set; }
        public List<StatesModel> States { get; set; }
        public List<CitiesModel> Cities { get; set; }
        public List<CompanyRolesModel> CompanyRoles { get; set; }
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please choose profile image")]
        [Display(Name = "Profile Picture")]
        [JsonIgnore]
        public IFormFile ProfileImage { get; set; }
        [JsonIgnore]
        public List<IFormFile> postedFiles { get; set; }
        public string ProfilePicture { get; set; }
        public List<string> Titles { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public string EncrytedId { get; set; }
        public bool IsResetPwd { get; set; }
        public string Token { get; set; }
        public int EmpId { get; set; }
    }

    public class JqueryDatatableParam
    {
        public string sEcho { get; set; }
        public string sSearch { get; set; }
        public int iDisplayLength { get; set; }
        public int iDisplayStart { get; set; }
        public int iColumns { get; set; }
        public int iSortCol_0 { get; set; }
        public string sSortDir_0 { get; set; }
        public int iSortingCols { get; set; }
        public string sColumns { get; set; }
    }
}
