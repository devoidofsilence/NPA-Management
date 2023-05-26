using NPAManagement.Helpers;
using NPAManagement.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NPAManagement.Services
{
    public class CommonService
    {
        readonly string connectionString = string.Empty;

        public CommonService()
        {
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        public bool CheckCIFDuplication(string forOperationMode, string CIFNo, int npaMainId)
        {
            bool cifIsDuplicate = false;

            string commandText = $"SELECT COUNT(CIFNo) CIFCOUNT FROM NPAManagementMain where CIFNo={CIFNo}";
            if (npaMainId != 0 && forOperationMode == "U")
            {
                commandText += $" and NPAManagementId<>{npaMainId}";
            }


            using (SqlDataReader reader = SqlHelper.ExecuteReader(connectionString, commandText,
                CommandType.Text))
            {
                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        if ((int)reader["CIFCOUNT"] > 0)
                        {
                            cifIsDuplicate = true;
                        }
                    }
                }
            }
            return cifIsDuplicate;
        }

        public List<SelectListItem> GetEmployeesFromAD()
        {
            List<SelectListItem> slItemEmps = null;
            if (HttpContext.Current.Cache["Employees"] != null)
            {
                slItemEmps = (List<SelectListItem>)HttpContext.Current.Cache["Employees"];
                return slItemEmps.Select(x => { x.Selected = false; return x; }).ToList();
            }

            // Creating a directory entry object by passing LDAP address
            List<SelectListItem> lstEmployees = new List<SelectListItem>();
            PrincipalContext context = new PrincipalContext(ContextType.Domain, "sbl.com", "tp_app", "$bl@!23$");
            PrincipalSearcher search = new PrincipalSearcher(new UserPrincipal(context));
            SelectListItem em = null;
            foreach (UserPrincipal user in search.FindAll())
            {
                if (null != user && null != user.DisplayName)
                {
                    em = new SelectListItem()
                    {
                        Text = $"{user.DisplayName}({user.SamAccountName})",
                        Value = user.SamAccountName
                    };
                    lstEmployees.Add(em);
                }
            }
            HttpContext.Current.Cache["Employees"] = lstEmployees;
            return lstEmployees;
        }

        public List<SelectListItem> GetDdlKeyValuePairs(string tableName, string keyTitle, string valueTitle, string selectedValue = "")
        {
            List<SelectListItem> lstDdlItems = new List<SelectListItem>();

            string commandText = $"SELECT {keyTitle}, {valueTitle} FROM {tableName} order by {keyTitle} asc";


            using (SqlDataReader reader = SqlHelper.ExecuteReader(connectionString, commandText,
                CommandType.Text))
            {
                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        SelectListItem acm = new SelectListItem()
                        {
                            Value = reader[$"{keyTitle}"].ToString(),
                            Text = reader[$"{valueTitle}"].ToString(),
                            Selected = (selectedValue == reader[$"{keyTitle}"].ToString()) ? true : false
                        };
                        lstDdlItems.Add(acm);
                    }
                }
            }
            return lstDdlItems;
        }

        public List<SelectListItem> GetDdlKeyValuePairsCascade(string tableName, string keyTitle, string valueTitle, string parentCodeTitle, string parentCodeValue, string selectedValue = "")
        {
            List<SelectListItem> lstDdlItems = new List<SelectListItem>();

            string commandText = $"SELECT {keyTitle}, {valueTitle} FROM {tableName} WHERE {parentCodeTitle}='{parentCodeValue}' order by {keyTitle} asc";


            using (SqlDataReader reader = SqlHelper.ExecuteReader(connectionString, commandText,
                CommandType.Text))
            {
                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        SelectListItem acm = new SelectListItem()
                        {
                            Value = reader[$"{keyTitle}"].ToString(),
                            Text = reader[$"{valueTitle}"].ToString(),
                            Selected = (selectedValue == reader[$"{keyTitle}"].ToString()) ? true : false
                        };
                        lstDdlItems.Add(acm);
                    }
                }
            }
            return lstDdlItems;
        }
    }
}