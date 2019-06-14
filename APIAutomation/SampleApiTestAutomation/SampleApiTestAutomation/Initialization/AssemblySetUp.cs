using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SampleApiTestAutomation.Initialization
{
    [TestClass]
    public abstract class AssemblySetUp
    {
        public static HttpClient httpClient = new HttpClient();

        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext testContext)
        {
            httpClient.BaseAddress = new Uri(ConfigurationManager.AppSettings["DummyAPIUri"]);
        }
    }
}
