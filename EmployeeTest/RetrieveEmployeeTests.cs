using System;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EmployeeTest.Models;

namespace EmployeeTest
{
    [TestClass]
    public class RetrieveEmployeeTests
    {
        [TestMethod]
        public void GetEmployeeDetailsTests()
        {
            try
            {
                var postData = new List<KeyValuePair<string, string>>();
                postData.Add(new KeyValuePair<string, string>("username", "sa"));
                postData.Add(new KeyValuePair<string, string>("password", "Ch3ck1t{}"));
                postData.Add(new KeyValuePair<string, string>("grant_type", "password")); // password is a default value
                postData.Add(new KeyValuePair<string, string>("client_id", "clientId101"));
                postData.Add(new KeyValuePair<string, string>("client_secret", "clientSecret101"));

                var content = new FormUrlEncodedContent(postData);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

                HttpResponseMessage tokenResponse;
                using (var client = new HttpClient())
                {
                    // Call hosted web API
                    tokenResponse = client.PostAsync("https://www.rvluzuriaga.somee.com/oauth2/token", content).Result;
                }

                var tokenResult = tokenResponse.Content.ReadAsStringAsync().Result;

                // Assertion
                Assert.IsTrue(tokenResponse.IsSuccessStatusCode, tokenResult);
                Assert.IsNotNull(tokenResult, "Token is null.");

                // Get Token
                var dictResult = JsonConvert.DeserializeObject<Dictionary<string, string>>(tokenResult);
                var accessToken = dictResult.Where(x => x.Key == "access_token")
                                            .Select(v => v.Value)
                                            .FirstOrDefault();

                HttpResponseMessage employeeResponse;
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    // Call hosted web API
                    employeeResponse = client.GetAsync("https://www.rvluzuriaga.somee.com/api/Employee/GetEmployeeDetails").Result;
                }

                var employeeResult = employeeResponse.Content.ReadAsStringAsync().Result;

                Assert.IsTrue(employeeResponse.IsSuccessStatusCode, employeeResult);

                var employees = JsonConvert.DeserializeObject<List<Employee>>(employeeResult);

                Assert.IsNotNull(employees, "Employees object is null.");

                Assert.AreNotEqual(0, employees.Count);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }
    }
}
