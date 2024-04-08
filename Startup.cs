using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace HealthITMiddleware
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            //handleCsv.readCsv();
           _ = syncIndicatorsDetails();
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

        public static async Task syncIndicatorsDetails()
        {
            bool syncdata = true;
            var remoteServerplainCredentials = System.Text.Encoding.UTF8.GetBytes(Globals.remoteServertUsername + ":" + Globals.remoteServerPassword);
            var remoteservercredentials = System.Convert.ToBase64String(remoteServerplainCredentials);
            var localServerUrl = Globals.localServerUrl;
            var localserverplainCredentials = System.Text.Encoding.UTF8.GetBytes(Globals.localSeverUsername + ":" + Globals.localServerpassword);
            var localservercredentials = System.Convert.ToBase64String(localserverplainCredentials);
            var remoteServerToken = "";
            var localserverToken = "";
            while (true)
            {
                await dataSynch(syncdata);
            //    Thread.Sleep(2000);
            //    //clientToken = await getToken(clientUsername, clientPassword, clientUrl);
            //    Console.WriteLine(remoteServerToken);
            //    localserverToken = await getlocalServerToken(Globals.localSeverUsername, Globals.localServerpassword, localServerUrl);
            //    if (localserverToken != null)//get serveToken
            //    {
            //        //sync schedule data
            //        try
            //        {
            //            var requestUrl1 = localServerUrl + "api/listschedules/";
            //            using var client1 = new HttpClient();
            //            client1.DefaultRequestHeaders.Add("Authorization", "Bearer " + localserverToken);
            //            var response1 = await client1.GetAsync(requestUrl1);
            //            var result1 = response1.Content.ReadAsStringAsync().Result;
            //            var objectlist = JArray.Parse(result1).ToList();
            //            foreach (var sc in objectlist)
            //            {
            //                Schedule s = sc.ToObject<Schedule>();
            //                string myday = "";
            //                if (s.sync_m == 1 && DateTime.Now.DayOfWeek.ToString().ToLower() == "monday")
            //                {
            //                    myday = "monday";
            //                }
            //                else if (s.sync_t == 1 && DateTime.Now.DayOfWeek.ToString().ToLower() == "tuesday")
            //                {
            //                    myday = "tuesday";
            //                }
            //                else if (s.sync_w == 1 && DateTime.Now.DayOfWeek.ToString().ToLower() == "wednesday")
            //                {
            //                    myday = "wednesday";
            //                }
            //                else if (s.sync_th == 1 && DateTime.Now.DayOfWeek.ToString().ToLower() == "thursday")
            //                {
            //                    myday = "thursday";
            //                }
            //                else if (s.sync_f == 1 && DateTime.Now.DayOfWeek.ToString().ToLower() == "friday")
            //                {
            //                    myday = "friday";
            //                }
            //                else if (s.sync_s == 1 && DateTime.Now.DayOfWeek.ToString().ToLower() == "saturday")
            //                {
            //                    myday = "saturday";
            //                }
            //                else if (s.sync_su == 1 && DateTime.Now.DayOfWeek.ToString().ToLower() == "sunday")
            //                {
            //                    myday = "sunday";
            //                }

            //                DateTime scheduleTime = Convert.ToDateTime((DateTime.Now.Date.ToString("yyyy-MM-dd") + " " + s.sync_time));
            //                Console.WriteLine(scheduleTime);
            //                TimeSpan scheduleTimeSpan = DateTime.Now - scheduleTime;
            //                Console.WriteLine(scheduleTimeSpan);
            //                Console.WriteLine(scheduleTimeSpan.TotalSeconds);
            //                if (DateTime.Now.DayOfWeek.ToString().ToLower() == myday && (scheduleTimeSpan.TotalSeconds >= 0 || scheduleTimeSpan.TotalSeconds <= 10))
            //                {
            //                    await dataSynch(true);
            //                }
            //            }

            //        }
            //        catch (Exception ex)
            //        {
            //            ex.Message.ToString();
            //        }
            //        syncdata = await CheckSyncStatus(serverUrl, serverToken);
            //        if (syncdata)
            //        {  //sync data
            //            await dataSynch(false);
            //        }
            //    }
            }
        }
        public static async Task dataSynch(bool scheduled_status)
        {
            var recordsCount = 0;
            //POST https://play.dhis2.org/dev/api/apiToken
            //Content - Type: application / json
            //Authorization: Basic admin district

            //{ }
            var remoteServerplainCredentials = System.Text.Encoding.UTF8.GetBytes(Globals.remoteServertUsername + ":" + Globals.remoteServerPassword);
            var remoteServercredentials = System.Convert.ToBase64String(remoteServerplainCredentials);
            var localServerToken = await getlocalServerToken(Globals.localSeverUsername, Globals.localServerpassword, Globals.localServerUrl);
            if (Globals.platformType.ToLower() != "client")
            {
                return;
            }
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    // Initialize variables for pagination
                    bool hasMoreData = true;
                    while (hasMoreData)
                    {
                        // Send the GET request to the API
                        httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + remoteServercredentials);
                        var response1 = await httpClient.GetAsync(Globals.pmtct_priUrl);
                        if (response1.IsSuccessStatusCode)
                        {
                            // Read the response content as a string
                            var result1 = response1.Content.ReadAsStringAsync().Result;
                            var jsonobjectresult = JObject.Parse(result1).ToString();//get childern var items = result["data"].Children().ToList();
                                                                          // Deserialize the JSON into a Root object
                            Root root = JsonConvert.DeserializeObject<Root>(jsonobjectresult);

                            // Extract rows
                            List<List<string>> rows = root?.rows;
                            var indicatoritems = jsonobjectresult;//get a list of indicator objects
                            //post collected data
                            foreach (var item in rows)//loop thru each indicator item
                            {
                                if (scheduled_status == false)
                                {
                                    if (await CheckSyncStatus(Globals.localServerUrl, localServerToken) != true)
                                    {
                                        break;
                                    }
                                }
                                if (item != null && item.Contains("MOH"))
                                {    //check if record already exisiting
                                    try
                                    {
                                        var postUrl4 = Globals.localServerUrl + "api/listindicators/" + item[9];
                                        httpClient.DefaultRequestHeaders.Add("Authorization", "Token " + localServerToken);
                                        var response4 = await httpClient.GetAsync(postUrl4);
                                        var result4 = response4.Content.ReadAsStringAsync().Result;
                                        Console.WriteLine(result4);
                                        if (!JObject.Parse(result4).ToString().ToLower().Contains("not found"))
                                        {
                                            continue;
                                        }
                                        else
                                        {
                                            //add indicators
                                            try
                                            {
                                                var json2 = JsonConvert.SerializeObject(new
                                                {
                                                    MOH_Indicator_ID = rows[0],
                                                    MOH_Indicator_Name = rows[1]
                                                });
                                                var data2 = new StringContent(json2, Encoding.UTF8, "application/json");
                                                var postUrl2 = Globals.localServerUrl + "api/create_indicator/";//programIndicators//indicators
                                                //httpClient.DefaultRequestHeaders.Add("Authorization", "Token " + serverToken);
                                                var response2 = await httpClient.PostAsync(postUrl2, data2);
                                                var result2 = response2.Content.ReadAsStringAsync().Result;
                                                Console.WriteLine(result2);
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
                            recordsCount += indicatoritems.ToList().Count;
                            //total records
                            // Check if there are more pages of data
                            if (recordsCount < Globals.pageSize)
                            {
                                hasMoreData = false; // No more data to retrieve
                            }
                            else
                            {
                                Globals.currentPage++; // Move to the next page
                            }
                            //update record count

                        }
                        else
                        {
                            // Handle API request error
                            Console.WriteLine($"API request failed with status code: {response1.StatusCode}");
                            hasMoreData = false; // Stop the loop on error
                        }
                        //post final count
                        try
                        {
                            var jsondata0 = JsonConvert.SerializeObject(new { recordsCount });
                            var data = new StringContent(jsondata0, Encoding.UTF8, "application/json");
                            var posturl = Globals.localServerUrl + "api/total_count/";
                            using var client = new HttpClient();
                            client.DefaultRequestHeaders.Add("Authorization", "Token " + localServerToken);
                            var response = await client.PostAsync(posturl, data);
                            //var result = response.Content.ReadAsStringAsync().Result;
                        }
                        catch (Exception e)
                        {
                            e.Message.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

        }
        
        //getserverToken
        public static async Task<bool> CheckSyncStatus(string url, string token)
        {
            bool syncdata = false;
            var postUrl0 = url + "api/sync_data/";//sync status
            using var client0 = new HttpClient();
            client0.DefaultRequestHeaders.Add("Authorization", "Token " + token);
            var response0 = await client0.GetAsync(postUrl0);
            var result0 = response0.Content.ReadAsStringAsync().Result;
            var obectset = JObject.Parse(result0);
            middleware_settings set = obectset.ToObject<middleware_settings>();
            System.Boolean.TryParse(set.synctdata, out syncdata);
            return syncdata;
        }
        public static async Task<string> getlocalServerToken(string username, string password, string url)
        {
            var jsondata = JsonConvert.SerializeObject(new { username, password });
            var data = new StringContent(jsondata, Encoding.UTF8, "application/json");
            var posturl = url + "api/login/";
            using var client = new HttpClient();
            var response = await client.PostAsync(posturl, data);
            var result = response.Content.ReadAsStringAsync().Result;
            tokenDetails tokendetails = JsonConvert.DeserializeObject<tokenDetails>(result);
            return tokendetails.token;
        }

        //Client data
        public class Header
        {
            public string name { get; set; }
            public string column { get; set; }
            public string valueType { get; set; }
            public string type { get; set; }
            public bool hidden { get; set; }
            public bool meta { get; set; }
        }

        public class MetaDataItem
        {
            public string name { get; set; }
        }

        public class OuNameHierarchy
        {
            public string e6A37uCtmiH { get; set; }
            public string owUuxpPfLnk { get; set; }
            // Add more properties for other items if needed
        }

        public class Dimension
        {
            public List<string> dx { get; set; }
            public List<string> pe { get; set; }
            public List<string> ou { get; set; }
            // Add more properties if needed
        }

        public class Root
        {
            public List<Header> headers { get; set; }
            public Dictionary<string, MetaDataItem> metaData { get; set; }
            public OuNameHierarchy ouNameHierarchy { get; set; }
            public Dimension dimensions { get; set; }
            public List<List<string>> rows { get; set; }
            public int height { get; set; }
            public int width { get; set; }
            public int headerWidth { get; set; }
        }

        public class tokenDetails
        {
            public string token { get; set; }
        }

        public class middleware_settings
        {
            public int id { get; set; }
            public string synctdata { get; set; }
            public string client_url { get; set; }
        }

        public class Schedule
        {
            public string shedule_description { get; set; }
            public string sync_time { get; set; }
            public int sync_m { get; set; }
            public int sync_t { get; set; }
            public int sync_w { get; set; }
            public int sync_th { get; set; }
            public int sync_f { get; set; }
            public int sync_s { get; set; }
            public int sync_su { get; set; }
        }

        public class Indicators
        {
            public string id { get; set; }
            public string name { get; set; }
        }
    }
}
