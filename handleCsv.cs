using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace HealthITMiddleware
{
    public static class handleCsv
    {
        static string datimfilepath = "C:\\Users\\Masterspace\\Documents\\projects\\HealITProjects\\HealthITMiddleware\\tx_curr_datim.csv";
        static string khisfilepath = "C:\\Users\\Masterspace\\Documents\\projects\\HealITProjects\\HealthITMiddleware\\tx_curr_khis.csv";

        public static List<MappedData> finaldata = new List<MappedData>();
        public static List<DatimValues> datimvalues = File.ReadAllLines(datimfilepath)
                                          .Skip(1)
                                          .Select(v => DatimValues.FromCsv(v))
                                          .ToList();
        public static List<KhisValues> khisvalues = File.ReadAllLines(khisfilepath)
                                    .Skip(1)
                                    .Select(v => KhisValues.FromCsv(v))
                                    .ToList();
        public static string gender ="";
        public static int mageset;
        public static int dageset;
        public static int count = 0;


        public static string ExtractMOHAgeGroup(string str,string pattern)
        {
            // public methods should validate their values; null string is a case
            if (string.IsNullOrEmpty(str))
                return null;

            var match = Regex.Match(str, pattern);

            return match.Success ? match.Value : null;
        }

        public static void append_data(KhisValues m,DatimValues d, int iterator= 0)
        {
            if (iterator > 0)
            {
                finaldata.Add(new MappedData
                {
                    faclity = m.facility,
                    ward= m.ward,
                    subcounty = m.subcounty,
                    county = m.county,  
                    MOH_Facilty_UID=m.MOH_Indicator_ID,
                    MOH_Indicator_ID=m.MOH_Indicator_ID,
                    MOH_Indicator_Name=m.MOH_Indicator_Name,
                    khis_data=m.khis_data,
                    datim_data=d.datim_data,
                });
            }
            else
            {
                finaldata.Add(new MappedData
                {
                    faclity = m.facility,
                    ward = m.ward,
                    subcounty = m.subcounty,
                    county = m.county,
                    MOH_Facilty_UID = m.MOH_Indicator_ID,
                    MOH_Indicator_ID = m.MOH_Indicator_ID,
                    MOH_Indicator_Name = m.MOH_Indicator_Name,
                    khis_data = m.khis_data,
                    datim_data = d.datim_data,
                });

            }

        }

        public static void readCsv()
        {
            Console.WriteLine(string.Format("total=>{0}", khisvalues.Count));
            foreach (var m in khisvalues){
                Console.WriteLine(string.Format("moh name{0}\tmoh data {1}", m.MOH_Indicator_Name, m.khis_data));
                if (count == khisvalues.Count)
                {
                    break;
                }
                foreach (var d in datimvalues)
                {
                    count++;
                    Console.WriteLine(string.Format("count=>{0}", count));
                    Console.WriteLine(string.Format("datim name{0}\tdatim data {1}",d.DATIM_Disag_Name,d.datim_data));
                    if(count == khisvalues.Count)
                    {
                        break;
                    }
                    if(m.MOH_Indicator_Name.Contains("Completed IPT_12months")){
                       if (d.DATIM_Indicator_Category.Contains("TB_PREV") && d.DATIM_Disag_Name.Contains("<") ||  d.DATIM_Disag_Name.Contains("+") &&
                           d.DATIM_Disag_Name.Contains("Female") || d.DATIM_Disag_Name.Contains("Male") || d.DATIM_Disag_Name.Contains("Unknown Sex") &&
                           d.DATIM_Disag_Name.Contains("Newly Enrolled") || d.DATIM_Disag_Name.Contains("Previously Enrolled")){
                            Console.WriteLine(string.Format("{0}\t<= Completed IPT_12months =>\t{1}",d.DATIM_Disag_Name, m.MOH_Indicator_Name));
                            //append_data(m, d);
                       }
                    }
                    else
                    {
                        //get datim ageset
                        string dko = d.DATIM_Disag_Name.Replace("|","");
                        string mko = m.MOH_Indicator_Name.Replace("_", " ").Replace("(", "\t(");
                        //get khis ageset
                        string pattern = @"(\d+[-]\d+)|(\d+[+])";
                        //mageset = Convert.ToInt16(ExtractMOHAgeGroup(mko,pattern));
                        Console.WriteLine(string.Format("values=>{0}",ExtractMOHAgeGroup(mko, pattern)));
                        //Console.WriteLine(mko);
                        //Console.WriteLine(string.Format("magest=>{0}", mageset));
                        if (dko.Contains("+"))
                        {
                            dageset = Convert.ToInt16(dko.Split()[0].Replace("+", ""));
                            Console.WriteLine(string.Format("ageplus=>{0}",dageset));

                        }else{
                            dageset = Convert.ToInt16(dko.Split()[0].Replace("<",""));
                            Console.WriteLine(string.Format("ageless=>{0}", dageset));
                        }
                        
                    }
                    continue;
                }
            }
        }
    }
    public class MappedData
    {
        public string faclity {  get; set; }
        public string ward { get; set; }
        public string subcounty { get; set; }
        public string county { get; set; }
        public string MOH_Facilty_UID { get; set; }
        public string MOH_Indicator_ID { get; set; }
        public string MOH_Indicator_Name { get; set; }
        public string khis_data { get; set; }
        public string datim_data { get; set; } 

    }
    public class KhisValues
    {
        public string facility { get; set; }
        public string ward { get; set; }
        public string subcounty { get; set; }
        public string county { get; set; }
        public string MOH_Facilty_UID { get; set; }
        public string MOH_Indicator_ID { get; set; }
        public string MOH_Indicator_Name { get; set; }
        public string khis_data { get; set; }

        public static KhisValues FromCsv(string csvLine)
        {
            string[] values = csvLine.Split(',');
            KhisValues khisValues = new KhisValues();
            khisValues.facility =values[0];
            khisValues.ward = values[1];
            khisValues.subcounty = values[2];
            khisValues.county = values[3];
            khisValues.MOH_Facilty_UID = values[4];
            khisValues.MOH_Indicator_ID =values[5];
            khisValues.MOH_Indicator_Name = values[6];
            khisValues.khis_data =values[7];
            //Console.WriteLine(khisValues.facility);
            return khisValues;
        }
    }
}
public class DatimValues
{
    public string DATIM_Indicator_Category { get; set; }
    public string facility { get; set; }
    public string ward { get; set; }
    public string subcounty { get; set; }
    public string county { get; set; }
    public string Datim_Facility_UID { get; set; }
    public string DATIM_Disag_ID { get; set; }
    public string DATIM_Disag_Name { get; set; }
    public string datim_data { get; set; }

    public static DatimValues FromCsv(string csvLine)
    {
        string[] values = csvLine.Split(',');
        DatimValues datimValues = new DatimValues();
        datimValues.DATIM_Indicator_Category = values[0];
        datimValues.facility = values[1];
        datimValues.ward = values[2];
        datimValues.subcounty = values[3];
        datimValues.county = values[4];
        datimValues.Datim_Facility_UID = values[5];
        datimValues.Datim_Facility_UID = values[6];
        datimValues.DATIM_Disag_Name = values[7];
        datimValues.datim_data = values[8];
        //Console.WriteLine(datimValues.facility);
        return datimValues;
    }
}

