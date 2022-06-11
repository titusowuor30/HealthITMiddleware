using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HealthITMiddleware
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _ = syncPatientDetails();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }

        public static async Task syncPatientDetails()
        {
            string clientUrl = Globals.clientUrl;
            string serverUrl = Globals.serverUrl;
            string clientUsername = Globals.clientUsername;
            string clientPassword = Globals.clientPassword;
            string serverUsername = Globals.severUsername;
            string serverpassword = Globals.serverpassword;
            string platformType = Globals.platformType;
            Object serverToken = Globals.serverToken;
            Object clientToken = Globals.clientToken;
            //POST https://play.dhis2.org/dev/api/apiToken
            //Content - Type: application / json
            //Authorization: Basic admin district

            //{ }
            var clientplainCredentials = System.Text.Encoding.UTF8.GetBytes(clientUsername + ":" + clientPassword);
            var clientcredentials = System.Convert.ToBase64String(clientplainCredentials);
            var serverplainCredentials = System.Text.Encoding.UTF8.GetBytes(serverUsername + ":" + serverUsername);
            var servercredentials = System.Convert.ToBase64String(serverplainCredentials);
            while (true)
            {
                Console.WriteLine(clientUrl);
                //Thread.Sleep(2000);

                clientToken = await getToken(clientUsername, clientPassword, clientUrl);
                Console.WriteLine(clientToken);
                //serverToken = await getToken(serverUsername, serverpassword, serverUrl);

                if (clientToken != null)//serveToken
                {
                    if (platformType.ToLower() != "client")
                    {
                        return;
                    }

                    try
                    {
                        //fetch indicators
                        var json = JsonConvert.SerializeObject(new { });
                        var data = new StringContent(json, Encoding.UTF8, "application/json");
                        var gettUrl1 = clientUrl + "api/indicators";
                        using var client1 = new HttpClient();
                        client1.DefaultRequestHeaders.Add("Authorization", "Basic " + clientcredentials);
                        var response1 = await client1.GetAsync(gettUrl1);
                        var result1 = response1.Content.ReadAsStringAsync().Result;
                        Console.WriteLine(result1);
                        List<Patient> patients = JsonConvert.DeserializeObject<List<Patient>>(result1);

                        Patient p = new Patient();

                        foreach (var pat in patients)
                        {
                            p = pat;
                            if (p.Id != null)
                            {
                                try
                                {
                                    var json2 = JsonConvert.SerializeObject(new
                                    {
                                        PatientId = p.PatientId,
                                        PatientName = p.PatientName,
                                        PatientDiagnosis = p.PatientDiagnosis,
                                        HomeCounty = p.HomeCounty,
                                        delete_status = p.delete_status,
                                        status = p.status,
                                    });
                                    var data2 = new StringContent(json2, Encoding.UTF8, "application/json");
                                    var postUrl2 = serverUrl + "api/Patients";
                                    using var client2 = new HttpClient();
                                    client2.DefaultRequestHeaders.Add("Authorization", "Bearer " + serverToken);
                                    var response2 = await client2.PostAsync(postUrl2, data2);
                                    var result2 = response2.Content.ReadAsStringAsync().Result;
                                    Console.WriteLine(result2);

                                }
                                catch (Exception ex)
                                {
                                    ex.Message.ToString();
                                }
                                try
                                {
                                    var json3 = JsonConvert.SerializeObject(new
                                    {
                                        Id = p.Id,
                                        PatientId = p.PatientId,
                                        PatientName = p.PatientName,
                                        PatientDiagnosis = p.PatientDiagnosis,
                                        HomeCounty = p.HomeCounty,
                                        delete_status = p.delete_status,
                                        status = "S",
                                        exported = 1,
                                    });
                                    var data3 = new StringContent(json3, Encoding.UTF8, "application/json");
                                    var postUrl3 = clientUrl + "api/Patients/" + p.Id;
                                    using var client3 = new HttpClient();
                                    client3.DefaultRequestHeaders.Add("Authorization", "Bearer " + clientToken);
                                    var response3 = await client3.PutAsync(postUrl3, data3);
                                    var result3 = response3.Content.ReadAsStringAsync().Result;
                                    Console.WriteLine(response3);
                                    if (p.Id != null && response3 == null)//add track record for failed transactions
                                    {
                                        try
                                        {
                                            var errjson = JsonConvert.SerializeObject(new
                                            {
                                                Id = p.Id,
                                                PatientId = p.PatientId,
                                                PatientName = p.PatientName,
                                                PatientDiagnosis = p.PatientDiagnosis,
                                                HomeCounty = p.HomeCounty,
                                                delete_status = p.delete_status,
                                                status = "F",
                                                exported = 0,
                                            });
                                            var data4 = new StringContent(errjson, Encoding.UTF8, "application/json");
                                            var postUrl4 = clientUrl + "api/Patients/" + p.Id;
                                            using var client4 = new HttpClient();
                                            client4.DefaultRequestHeaders.Add("Authorization", "Bearer" + clientToken);
                                            var response4 = await client4.PutAsync(postUrl4, data4);
                                            var result4 = response4.Content.ReadAsStringAsync().Result;
                                            Console.WriteLine(result4);
                                        }
                                        catch (Exception ex)
                                        {
                                            ex.Message.ToString();
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ex.Message.ToString();

                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.Message.ToString();
                    }
                }
            }
        }

        //DHIS2 Basic Authentication
        public static async Task<Object> getToken(string username, string password, string url)
        {
            var geturl = url+"api/32/me";
            using var client = new HttpClient();
            var plainCredentials = System.Text.Encoding.UTF8.GetBytes(username+":"+password);
            var credentials = System.Convert.ToBase64String(plainCredentials);
            client.DefaultRequestHeaders.Add("Authorization", "Basic " + credentials);
            var response = await client.GetAsync(geturl);
            var result = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.SerializeObject(new{ result });
        }

        public class tokenDetails
        {
            public string token { get; set; }
            public string success { get; set; }
            public string errors { get; set; }
        }

        public class Patient
        {
            public int Id { get; set; }
            public int PatientId { get; set; }
            public string PatientName { get; set; }
            public string PatientDiagnosis { get; set; }
            public string HomeCounty { get; set; }
            public int delete_status { get; set; }
            public string status { get; set; }
        }

    }
}
