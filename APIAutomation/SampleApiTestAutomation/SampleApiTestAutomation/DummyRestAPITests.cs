using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Net.Http;
using System.Configuration;
using Newtonsoft.Json;
using FluentAssertions;
using System.Xml.Linq;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleApiTestAutomation
{
    [TestClass]
    public class DummyRestAPITests
    {
        private HttpClient httpClient = new HttpClient()
        {
            BaseAddress = new Uri(ConfigurationManager.AppSettings["DummyAPIUri"])
        };

        [TestMethod]
        [Description("Verify HTTP GET API, response for Get API")]
        public void GetEmployeeDetails()
        {
            // Call Get API - /employee
            var employees = httpClient.GetAsync("api/v1/employees").Result;

            // Verify response is 200
            employees.StatusCode.Should().Be(HttpStatusCode.OK);

            // Verify employee list
            var employeelist = JsonConvert.DeserializeObject<List<Employee>>(employees.Content.ReadAsStringAsync().Result);
            employeelist.Count.Should().BeGreaterThan(0);
            var employee = employeelist.FirstOrDefault();

            employee.id.Should().NotBeNullOrEmpty();
            employee.employee_age.Should().NotBeNullOrEmpty();
            employee.employee_name.Should().NotBeNullOrEmpty();
            employee.employee_salary.Should().NotBeNullOrEmpty();

            // Call Get API - /employee/{id}
            var employeeDetails = httpClient.GetAsync("api/v1/employee/" + employee.id).Result;
            var employeeDetailsFromGetAPI = JsonConvert.DeserializeObject<Employee>(employeeDetails.Content.ReadAsStringAsync().Result);

            // Verify employee response with specific employee id
            employeeDetailsFromGetAPI.id.Should().Be(employee.id);
            employeeDetailsFromGetAPI.profile_image.Should().Be(employee.profile_image);
            employeeDetailsFromGetAPI.employee_age.Should().Be(employee.employee_age);
            employeeDetailsFromGetAPI.employee_name.Should().Be(employee.employee_name);
            employeeDetailsFromGetAPI.employee_salary.Should().Be(employee.employee_salary);
        }


        [TestMethod]
        [Description("Verify HTTP POST API, response for Create API")]
        public void CreateEmployee()
        {
            var content = new CreateUpdateEmployee() { name = Guid.NewGuid().ToString(), age = "33", salary = "90" };

            // Call POST API - /employee
            var httpResponse = httpClient.PostAsync("api/v1/create", new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json"));

            // Verify response is 200
            httpResponse.Result.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseContent = httpResponse.Result.Content.ReadAsStringAsync();

            // Verify employee details returned from server
            var employeeDetails = JsonConvert.DeserializeObject<CreateUpdateEmployee>(responseContent.Result);
            employeeDetails.age.Should().Be(content.age);
            employeeDetails.name.Should().Be(content.name);
            employeeDetails.salary.Should().Be(content.salary);
        }

        [TestMethod]
        [Description("Verify HTTP PUT API, response for Update API")]

        public void UpdateEmployeeDetail()
        {
            var content = new CreateUpdateEmployee() { name = Guid.NewGuid().ToString(), age = "33", salary = "90" };

            // Call update API and update details of employee with id = 1
            var httpResponse = httpClient.PutAsync("api/v1/update/1", new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json"));


            // Verify response is 200
            httpResponse.Result.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseContent = httpResponse.Result.Content.ReadAsStringAsync();

            // Verify employee details (after update) returned from server
            var employeeDetails = JsonConvert.DeserializeObject<CreateUpdateEmployee>(responseContent.Result);
            employeeDetails.age.Should().Be(content.age);
            employeeDetails.name.Should().Be(content.name);
            employeeDetails.salary.Should().Be(content.salary);
        }

        [TestMethod]
        [Description("Verify HTTP DELETE API, response for Update API")]
        public void DeleteEmployee()
        {
            var content = new CreateUpdateEmployee() { name = Guid.NewGuid().ToString(), age = "33", salary = "90" };

            // Call POST API - /employee
            var httpResponse = httpClient.PostAsync("api/v1/create", new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json"));

            // Verify response is 200
            httpResponse.Result.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseContent = httpResponse.Result.Content.ReadAsStringAsync();
            var employeeDetails = JsonConvert.DeserializeObject<CreateUpdateEmployee>(responseContent.Result);

            // Call update API and update details of employee with id = 1
            httpResponse = httpClient.DeleteAsync("api/v1/delete/" + employeeDetails.id);


            // Verify response is 200
            httpResponse.Result.StatusCode.Should().Be(HttpStatusCode.OK);
            responseContent = httpResponse.Result.Content.ReadAsStringAsync();

            // Call Get API for the deleted record, it should return false
            var getDeletedEmployeeDetails = httpClient.GetAsync("api/v1/employee/" + employeeDetails.id).Result;
            getDeletedEmployeeDetails.Content.ReadAsStringAsync().Result.Should().Be("false");
        }
    }

    class Employee
    {
        public string id { get; set; }
        public string employee_name { get; set; }
        public string employee_salary { get; set; }
        public string employee_age { get; set; }
        public string profile_image { get; set; }
    }

    class CreateUpdateEmployee
    {
        public string id { get; set; }
        public string name { get; set; }
        public string salary { get; set; }
        public string age { get; set; }
    }
}
