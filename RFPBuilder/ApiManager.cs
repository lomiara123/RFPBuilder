using System;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace RFPBuilder
{
    class ApiManager
    {
        private const string ConfigFilename = "app.config";

        private static string apiUrl;
        private static string apiToken;
        private static HttpClient client;

        public static async Task<double> findRequirementsSimilarity(string text1, string text2)
        {
            var url = apiUrl + "/?";
            url += "lang=en";
            url += "&text1=" + text1;
            url += "&text2=" + text2;
            url += "&token=" + apiToken;

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Connection", "keep-alive");
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                try
                {
                    var contents = await response.Content.ReadAsStringAsync();
                    var json = JObject.Parse(contents);
                    if (json != null)
                        return (double)json.GetValue("similarity");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error occurred for comparing requirements similarity. \n Error: " + ex.Message);
                    throw;
                }
            }

            return 0.0;
        }

        public static void init()
        {
            client = new HttpClient();

            var path = Path.Combine(Directory.GetCurrentDirectory(), ConfigFilename);
            if (File.Exists(path))
            {
                var configurations = File.ReadAllLines(path);
                foreach (var config in configurations)
                {
                    if (config.Split('=').Length == 1)
                    {
                        continue;
                    }

                    var propertyName = config.Split('=')[0];
                    var propertyValue = config.Split('=')[1];

                    if (propertyName.ToUpper().Equals("APIURL"))
                    {
                        apiUrl = propertyValue;
                    }
                    if (propertyName.ToUpper().Equals("APITOKEN"))
                    {
                        apiToken = propertyValue;
                    }
                }
            }
        }
    }
}
