using System;

using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;


    public class Program
    {
        private static MarketDataDataContext dtbase = new MarketDataDataContext();
        public static void Main(string[] args)
        {
            updateLWA();
            updateMaxSMP();
            updateMinSMP();
        }
        public static void updateLWA()
        {
            StreamWriter sr = new StreamWriter("lwafails.txt");
            DateTime startDate;
            var lastDate = from u in dtbase.LWAs
                           select u.Date;
            try
            {
                startDate = lastDate.Max().AddDays(1);
            }
            catch
            {
                startDate = DateTime.Parse("1/1/2010");
            }
            DateTime endDate = DateTime.Now.AddDays(-1);
            int dataPoints = (endDate - startDate).Days + 1;
            if (endDate < startDate)
            {
                dataPoints = 0;
            }

            LWA info;
            for (int i = 0; i < dataPoints; i++)
            {
                Console.WriteLine(startDate.AddDays(i).ToString("dd-MMM-yyyy"));
                info = getLWA(startDate.AddDays(i).ToString("dd-MMM-yyyy"));

                Console.WriteLine("Getting: {0} out of {1}", i + 1, dataPoints);
                dtbase.LWAs.InsertOnSubmit(info);

                try
                {
                    dtbase.SubmitChanges();
                }
                catch
                {
                    Console.WriteLine("Fail");
                    sr.WriteLine(startDate.AddDays(i).ToString("dd-MMM-yyyy"));
                    continue;
                }
            }
            sr.Close();
        }
        //returns an array of LWA data on specified date from semo
        public static LWA getLWA(string date)
        {
            string htmlCode = "", data = "";
            using (WebClient client = new WebClient())
            {
                //htmlCode = client.DownloadString("http://semorep.sem-o.com/SemoWebSite/?qpReportServer=&qpReportURL=/SEMO%20Dynamic%20Reports/Dynamic%20Reporting%20-%20Predefined/All%20Predefined%20Reports/Load%20Weighted%20Average%20SMP&prm_GetFromDate=01-Jan-2014&prm_GetToDate=01-Jan-2014&prm_GetRunType=EA&prm_GetCurrency=EUR&prm_Chart_Table_Toggle=Table&qpWindowType=Popout&usr_Login=fbasemomember%3aniall_mcd%40hotmail.com&rpt_Toolbar=1&rpt_Print=1&rpt_Export=1&rpt_Zoom=1&rpt_ZoomPerc=100&rpt_Find=1&rpt_Navigate=1");
                htmlCode = client.DownloadString("http://semorep.sem-o.com/SemoWebSite/?qpReportServer=&qpReportURL=/SEMO%20Dynamic%20Reports/Dynamic%20Reporting%20-%20Predefined/All%20Predefined%20Reports/Load%20Weighted%20Average%20SMP&prm_GetFromDate=" + date + "&prm_GetToDate=" + date + "&prm_GetRunType=EA&prm_GetCurrency=EUR&prm_Chart_Table_Toggle=Table&qpWindowType=Popout&usr_Login=fbasemomember%3aniall_mcd%40hotmail.com&rpt_Toolbar=1&rpt_Print=1&rpt_Export=1&rpt_Zoom=1&rpt_ZoomPerc=100&rpt_Find=1&rpt_Navigate=1");
            }
            bool begin = false;
            foreach (string s in htmlCode.Split('>'))
            {
                if (s.StartsWith(@"Trade Date"))
                {
                    begin = true;
                }
                if (s.StartsWith(@"Run Date"))
                {
                    begin = false;
                }
                if ((!s.StartsWith("<")) && begin)
                {
                    //sr.WriteLine(s.Substring(0,s.Length-5));
                    data += s.Substring(0, s.Length - 5) + ",";
                }
            }
            string[] temp = new string[5];
            LWA info = new LWA(data.Split(',')[5], data.Split(',')[6], data.Split(',')[7], data.Split(',')[8], data.Split(',')[9]);

            return info;
        }
        public static void updateMaxSMP()
        {
            StreamWriter sr = new StreamWriter("maxsmpfails.txt");
            DateTime startDate;

            var lastDate = from u in dtbase.MaxSMPs
                           select u.Date;
            try
            {
                startDate = lastDate.Max().AddDays(1);
            }
            catch
            {
                startDate = DateTime.Parse("1/1/2010");
            }
            DateTime endDate = DateTime.Now.AddDays(-1);
            int dataPoints = (endDate - startDate).Days + 1;
            if (endDate < startDate)
            {
                dataPoints = 0;
            }
            MaxSMP info;
            for (int i = 0; i < dataPoints; i++)
            {
                Console.WriteLine(startDate.AddDays(i).ToString("dd-MMM-yyyy"));
                info = getMaxSMP(startDate.AddDays(i).ToString("dd-MMM-yyyy"));

                Console.WriteLine("Getting: {0} out of {1}", i + 1, dataPoints);

                dtbase.MaxSMPs.InsertOnSubmit(info);

                try
                {
                    dtbase.SubmitChanges();
                }
                catch
                {
                    Console.WriteLine("Fail");
                    sr.WriteLine(startDate.AddDays(i).ToString("dd-MMM-yyyy"));
                    continue;
                }
            }
            sr.Close();
        }
        public static MaxSMP getMaxSMP(string date)
        {
            string htmlCode = "", data = "";
            using (WebClient client = new WebClient())
            {
                //htmlCode = client.DownloadString("http://semorep.sem-o.com/SemoWebSite/?qpReportServer=&qpReportURL=/SEMO%20Dynamic%20Reports/Dynamic%20Reporting%20-%20Predefined/All%20Predefined%20Reports/Maximum%20SMP&prm_GetFromDate=01-Jan-2015&prm_GetToDate=31-Jan-2015&prm_GetRunType=EA&prm_GetCurrency=EUR&prm_Chart_Table_Toggle=Table&qpWindowType=Popout&usr_Login=fbasemomember%3aniall_mcd%40hotmail.com&rpt_Toolbar=1&rpt_Print=1&rpt_Export=1&rpt_Zoom=1&rpt_ZoomPerc=100&rpt_Find=1&rpt_Navigate=1");
                htmlCode = client.DownloadString("http://semorep.sem-o.com/SemoWebSite/?qpReportServer=&qpReportURL=/SEMO%20Dynamic%20Reports/Dynamic%20Reporting%20-%20Predefined/All%20Predefined%20Reports/Maximum%20SMP&prm_GetFromDate=" + date + @"&prm_GetToDate=" + date + @"&prm_GetRunType=EA&prm_GetCurrency=EUR&prm_Chart_Table_Toggle=Table&qpWindowType=Popout&usr_Login=fbasemomember%3aniall_mcd%40hotmail.com&rpt_Toolbar=1&rpt_Print=1&rpt_Export=1&rpt_Zoom=1&rpt_ZoomPerc=100&rpt_Find=1&rpt_Navigate=1");
            }
            //FileStream fs = new FileStream("test.txt",FileMode.Create);
            //StreamWriter sr = new StreamWriter(fs);
            //sr.AutoFlush = true;
            bool begin = false;
            foreach (string s in htmlCode.Split('>'))
            {
                if (s.StartsWith(@"Trade Date"))
                {
                    begin = true;
                }
                if (s.StartsWith(@"Run Date"))
                {
                    begin = false;
                }
                if ((!s.StartsWith("<")) && begin)
                {
                    //sr.WriteLine(s.Substring(0,s.Length-5));
                    data += s.Substring(0, s.Length - 5) + ",";
                }
            }
            string[] temp = new string[5];
            MaxSMP info = new MaxSMP(DateTime.ParseExact(data.Split(',')[5], "dd/MM/yyyy", CultureInfo.InvariantCulture), data.Split(',')[6], data.Split(',')[7], decimal.Parse(data.Split(',')[8]), decimal.Parse(data.Split(',')[9]));

            return info;
        }
        public static void updateMinSMP()
        {
            StreamWriter sr = new StreamWriter("minsmpfails.txt");
            DateTime startDate;
            var lastDate = from u in dtbase.MinSMPs
                           select u.Date;
            try
            {
                startDate = lastDate.Max().AddDays(1);
            }
            catch
            {
                startDate = DateTime.Parse("1/1/2010");
            }
            DateTime endDate = DateTime.Now.AddDays(-1);
            int dataPoints = (endDate - startDate).Days + 1;
            if (endDate < startDate)
            {
                dataPoints = 0;
            }


            MinSMP info;
            for (int i = 0; i < dataPoints; i++)
            {
                Console.WriteLine(startDate.AddDays(i).ToString("dd-MMM-yyyy"));
                info = getMinSMP(startDate.AddDays(i).ToString("dd-MMM-yyyy"));

                Console.WriteLine("Getting: {0} out of {1}", i + 1, dataPoints);
                //change to min
                dtbase.MinSMPs.InsertOnSubmit(info);

                try
                {
                    dtbase.SubmitChanges();
                }
                catch
                {
                    Console.WriteLine("Fail");
                    sr.WriteLine(startDate.AddDays(i).ToString("dd-MMM-yyyy"));
                    continue;
                }
            }
            sr.Close();
        }
        public static MinSMP getMinSMP(string date)
        {
            string htmlCode = "", data = "";
            using (WebClient client = new WebClient())
            {
                //htmlCode = client.DownloadString("http://semorep.sem-o.com/SemoWebSite/?qpReportServer=&qpReportURL=/SEMO%20Dynamic%20Reports/Dynamic%20Reporting%20-%20Predefined/All%20Predefined%20Reports/Minimum%20SMP&prm_GetFromDate=01-Jan-2015&prm_GetToDate=31-Jan-2015&prm_GetRunType=EA&prm_GetCurrency=EUR&prm_Chart_Table_Toggle=Chart&qpWindowType=Popout&usr_Login=fbasemomember%3aniall_mcd%40hotmail.com&rpt_Toolbar=1&rpt_Print=1&rpt_Export=1&rpt_Zoom=1&rpt_ZoomPerc=100&rpt_Find=1&rpt_Navigate=1");
                htmlCode = client.DownloadString("http://semorep.sem-o.com/SemoWebSite/?qpReportServer=&qpReportURL=/SEMO%20Dynamic%20Reports/Dynamic%20Reporting%20-%20Predefined/All%20Predefined%20Reports/Minimum%20SMP&prm_GetFromDate=" + date + @"&prm_GetToDate=" + date + @"&prm_GetRunType=EA&prm_GetCurrency=EUR&prm_Chart_Table_Toggle=Table&qpWindowType=Popout&usr_Login=fbasemomember%3aniall_mcd%40hotmail.com&rpt_Toolbar=1&rpt_Print=1&rpt_Export=1&rpt_Zoom=1&rpt_ZoomPerc=100&rpt_Find=1&rpt_Navigate=1");
            }
            //FileStream fs = new FileStream("test.txt",FileMode.Create);
            //StreamWriter sr = new StreamWriter(fs);
            //sr.AutoFlush = true;
            bool begin = false;
            foreach (string s in htmlCode.Split('>'))
            {
                if (s.StartsWith(@"Trade Date"))
                {
                    begin = true;
                }
                if (s.StartsWith(@"Run Date"))
                {
                    begin = false;
                }
                if ((!s.StartsWith("<")) && begin)
                {
                    //sr.WriteLine(s.Substring(0,s.Length-5));
                    data += s.Substring(0, s.Length - 5) + ",";
                }
            }
            string[] temp = new string[5];
            MinSMP info = new MinSMP(DateTime.ParseExact(data.Split(',')[5], "dd/MM/yyyy", CultureInfo.InvariantCulture), data.Split(',')[6], data.Split(',')[7], decimal.Parse(data.Split(',')[8]), decimal.Parse(data.Split(',')[9]));

            return info;
        }
    }
    public partial class LWA
    {
        public LWA(string date, string RunType, string Currency, string LWA, string SevenDayLWA)
        {
            this.Date = DateTime.Parse(date);
            this.Run_Type = RunType;
            this.Currency = Currency;
            this.Lwa1 = decimal.Parse(LWA);
            this.SevenDayLWA = decimal.Parse(SevenDayLWA);
        }
    }
    public partial class MaxSMP
    {
        public MaxSMP(DateTime date, string RunType, string Currency, decimal SMP, decimal SevenDaySMP)
        {
            this.Date = date;
            this.Run_Type = RunType;
            this.Currency = Currency;
            this.MaxSMP1 = SMP;
            this.SevenDayMaxSMP = SevenDaySMP;
        }
    }
    public partial class MinSMP
    {
        public MinSMP(DateTime date, string RunType, string Currency, decimal SMP, decimal SevenDaySMP)
        {
            this.Date = date;
            this.Run_Type = RunType;
            this.Currency = Currency;
            this.MinSMP1 = SMP;
            this.SevenDayMinSMP = SevenDaySMP;
        }
    }
