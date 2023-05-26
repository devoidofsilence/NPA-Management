using NPAManagement.Helpers;
using NPAManagement.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace NPAManagement.Services
{
    public static class LoginCheck
    {
        static string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public static bool ValidateLogin(string username, string password)
        {
            try
            {
                bool isValidUser = false;
                //api call to validate user login creds
                //string checkUrl = "http://192.168.100.197:9003/adloginvalidation/validatecredentials?username=" + username + "&password=" + password;
                //var client = new RestClient("http://localhost:26887/adloginvalidation/validatecredentials");
                var client = new RestClient("http://192.168.100.197:9003/adloginvalidation/validatecredentials");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                LoginModel lm = new LoginModel()
                {
                    UserName = username,
                    Password = password
                };
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", JsonConvert.SerializeObject(lm), ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                isValidUser = response.Content.Trim().ToLower() == "true" ? true : false;
                return isValidUser;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //public static bool CheckIfUserIsRegisteredInSystem(string username)
        //{
        //    bool isValidUser = false;

        //    string commandText = @"SELECT * FROM Users where IsActive=1 and Upper(rtrim(ltrim(Username)))=Upper(rtrim(ltrim('" + username + "')))";


        //    using (SqlDataReader reader = SqlHelper.ExecuteReader(connectionString, commandText,
        //        CommandType.Text))
        //    {
        //        while (reader.Read())
        //        {
        //            if (reader.HasRows)
        //            {
        //                isValidUser = true;
        //            }
        //        }
        //    }
        //    return isValidUser;
        //}

        public static LoggedInUser CheckIfUserIsRegisteredInSystem(string username)
        {
            LoggedInUser user = null;

            string commandText = $"SELECT * FROM Users where upper(ltrim(rtrim(Username)))=upper(ltrim(rtrim('{username}')))";


            using (SqlDataReader reader = SqlHelper.ExecuteReader(connectionString, commandText,
                CommandType.Text))
            {
                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        user = new LoggedInUser()
                        {
                            Username = username,
                            UserId = reader["UserId"].ToString(),
                            UserRole = reader["Role"].ToString(),
                            UserActiveFlag = Convert.ToBoolean(reader["IsActive"])
                        };
                    }
                }
            }
            return user;
        }
    }
}