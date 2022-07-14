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
            string clientUrl = Globals.clientUrl;
            string serverUrl = Globals.serverUrl;
            string clientUsername = Globals.clientUsername;
            string clientPassword = Globals.clientPassword;
            string serverUsername = Globals.severUsername;
            string serverpassword = Globals.serverpassword;
            string platformType = Globals.platformType;
            string serverToken = Globals.serverToken;
            object clientToken = Globals.clientToken;
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
                serverToken = await getServerToken(serverUsername, serverpassword, serverUrl);
                if (serverToken != null)//get serveToken
                {
                    if (platformType.ToLower() != "client")
                    {
                        return;
                    }
                    try
                    {
                        //fetch indicators
                        //var json = JsonConvert.SerializeObject(new { });
                        //var data = new StringContent(json, Encoding.UTF8, "application/json");
                        //var gettUrl1 = clientUrl + "api/metadata?indicators=true&indicatorGroups=true&paging=false";//&indicatorGroups=true
                        var gettUrl1 = clientUrl + "api/indicators?fields=:all&paging=false";
                        //var gettUrl1 = clientUrl + "api/indicators?fields=id,name,lastUpdated,created,shortName,displayName,displayShortName" +
                        //    ",displayNumeratorDescription,denominatorDescription,displayDenominatorDescription,numeratorDescription," +
                        //    "dimensionItem,displayFormName,numerator,denominator,dimensionItemType,indicatorType[id,name],indicatorGroups[id,name,lastUpdated,created]&paging=false";
                        using var client1 = new HttpClient();
                        client1.DefaultRequestHeaders.Add("Authorization", "Basic " + clientcredentials);
                        var response1 = await client1.GetAsync(gettUrl1);
                        
                        var result1 = response1.Content.ReadAsStringAsync().Result;
                        var jsonobjectresult = JObject.Parse(result1);//get childern var items = result["data"].Children().ToList();
                        //var responseinfo = jsonobjectresult.Children().ToList();//get all json object children
                        var indicatoritems = jsonobjectresult["indicators"].ToList();//get a list of indicator objects
                       
                        Console.WriteLine(indicatoritems);
                        //convert each list item to object list item
                        List<Indicators> indicatorslist = new List<Indicators>();
                        List<string> listindicatorGroupid = new List<string>();
                        List<IndicatorGroups> listindicatorgroups = new List<IndicatorGroups>();
                        Console.WriteLine(indicatorslist);

                        foreach (var item in indicatoritems)//loop thru each indicator item
                        {
                           Indicators indicator = item.ToObject<Indicators>();//format each indiccator item to object before accesing it's field values
                           indicatorslist.Add(indicator);
                            Console.WriteLine(indicator.indicatorType.name);
                            bool existingrecordid = false;
                            if (indicator.id != null)
                            {    //check if record already exisiting
                                try
                                {
                                    var postUrl4 = serverUrl + "api/listindicators/"+indicator.id;
                                    using var client4 = new HttpClient();
                                    client4.DefaultRequestHeaders.Add("Authorization", "Token " + serverToken);
                                    var response4 = await client4.GetAsync(postUrl4);
                                    var result4 = response4.Content.ReadAsStringAsync().Result;
                                    Console.WriteLine(result4);
                                    if (JObject.Parse(result4).ToString().ToLower().Contains("not found"))
                                    {
                                        existingrecordid = false;
                                    }
                                    else
                                    {
                                        existingrecordid = true;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ex.Message.ToString();
                                }
                                //skip to next record if record exists
                                if (existingrecordid)
                                {
                                    continue;
                                }
                                else
                                {
                                    //add indicator groups
                                    var indicatorGroups = item.Last().ToList();
                                    foreach (var group in indicatorGroups)
                                    {
                                        if(group.Count()>0) 
                                        {
                                            IndicatorGroups indgroup = group[0].ToObject<IndicatorGroups>();
                                            listindicatorGroupid.Add(indgroup.id);
                                            listindicatorgroups.Add(indgroup);
                                            try
                                            {
                                                var json5 = JsonConvert.SerializeObject(new
                                                {
                                                    id = indgroup.id,
                                                    name = indgroup.name,
                                                    lastUpdated = indgroup.lastUpdated,
                                                    created = indgroup.created,
                                                });
                                                var data5 = new StringContent(json5, Encoding.UTF8, "application/json");
                                                var postUrl5 = serverUrl + "api/create_indicator_group/";//programIndicators//indicators
                                                using var client5 = new HttpClient();
                                                client5.DefaultRequestHeaders.Add("Authorization", "Token " + serverToken);
                                                var response5 = await client5.PostAsync(postUrl5, data5);
                                                var result5 = response5.Content.ReadAsStringAsync().Result;
                                                Console.WriteLine(result5);
                                            }
                                            catch (Exception ex)
                                            {
                                                ex.Message.ToString();
                                            }
                                        }
                                        else
                                        {
                                            //add indicator types
                                            try
                                            {
                                                var json3 = JsonConvert.SerializeObject(new
                                                {
                                                    id = indicator.indicatorType.id,
                                                    name = indicator.indicatorType.name,
                                                });
                                                var data3 = new StringContent(json3, Encoding.UTF8, "application/json");
                                                var postUrl3 = serverUrl + "api/create_indicator_type/";//programIndicators//indicators
                                                using var client3 = new HttpClient();
                                                client3.DefaultRequestHeaders.Add("Authorization", "Token " + serverToken);
                                                var response3 = await client3.PostAsync(postUrl3, data3);
                                                var result3 = response3.Content.ReadAsStringAsync().Result;
                                                Console.WriteLine(result3);
                                            }
                                            catch (Exception ex)
                                            {
                                                ex.Message.ToString();
                                            }
                                        }
                                    }
                                    //add indicators
                                    try
                                    {
                                        var json2 = JsonConvert.SerializeObject(new
                                        {
                                            id = indicator.id,
                                            name = indicator.name,
                                            lastUpdated = indicator.lastUpdated,
                                            created = indicator.created,
                                            shortName = indicator.shortName,
                                            displayName = indicator.displayName,
                                            displayShortName = indicator.displayShortName,
                                            displayNumeratorDescription = indicator.displayNumeratorDescription,
                                            denominatorDescription = indicator.denominatorDescription,
                                            displayDenominatorDescription = indicator.displayDenominatorDescription,
                                            numeratorDescription = indicator.numeratorDescription,
                                            dimensionItem = indicator.dimensionItem,
                                            displayFormName = indicator.displayFormName,
                                            numerator = indicator.numerator,
                                            denominator = indicator.denominator,
                                            dimensionItemType = indicator.dimensionItemType,
                                            indicatorType = indicator.indicatorType.id,
                                            indicatorGroups = listindicatorGroupid,
                                        });
                                        var data2 = new StringContent(json2, Encoding.UTF8, "application/json");
                                        var postUrl2 = serverUrl + "api/create_indicator/";//programIndicators//indicators
                                        using var client2 = new HttpClient();
                                        client2.DefaultRequestHeaders.Add("Authorization", "Token " + serverToken);
                                        var response2 = await client2.PostAsync(postUrl2, data2);
                                        var result2 = response2.Content.ReadAsStringAsync().Result;
                                        Console.WriteLine(result2);
                                    }
                                    catch (Exception ex)
                                    {
                                        ex.Message.ToString();

                                    }
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

        //dhis2 basic authentication
        public static async Task<string> getToken(string username, string password, string url)
        {
            var geturl = url + "api/33/me";
            using var client = new HttpClient();
            var plaincredentials = System.Text.Encoding.UTF8.GetBytes(username + ":" + password);
            var credentials = System.Convert.ToBase64String(plaincredentials);
            client.DefaultRequestHeaders.Add("authorization", "basic " + credentials);
            var response = await client.GetAsync(geturl);
            var result = response.Content.ReadAsStringAsync().Result;
            return result;
        }
        //getserverToken
        public static async Task<string> getServerToken(string username, string password, string url)
        {
            var jsondata = JsonConvert.SerializeObject(new { username, password } );
            var data = new StringContent(jsondata, Encoding.UTF8, "application/json");
            var posturl = url + "api/login/";
            using var client = new HttpClient();
            var response = await client.PostAsync(posturl, data);
            var result = response.Content.ReadAsStringAsync().Result;
            tokenDetails tokendetails = JsonConvert.DeserializeObject<tokenDetails>(result);
            return tokendetails.token;
        }

        class tokenDetails
        {
            public string token;
        }
        public class indicatorType
        {
           public string id { get; set; }
           public string name { get; set; }
        }
        public class IndicatorGroups
        {
            public string id { get; set; }
            public string name { set; get; }
            public DateTime lastUpdated { get; set; }
            public DateTime created { get; set; }
        }
        public class Indicators
        {
          public string id { get; set; }
          public string name { get; set; }
          public DateTime lastUpdated { get; set; }
          public DateTime created { get; set; }
          public string shortName { get; set; }
          public  string displayName { get; set; }
          public string displayShortName { get; set; } 
          public string displayNumeratorDescription { get; set; }
          public string denominatorDescription { get; set; }
          public string displayDenominatorDescription { get; set; }
          public string numeratorDescription { get; set; }
          public string dimensionItem { get; set; }
          public string displayFormName { get; set; }
          public string numerator { get; set; }
          public string denominator { get; set; }
          public string dimensionItemType { get; set; }
          public indicatorType indicatorType { get; set; }
        }
    }
}
