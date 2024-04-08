namespace HealthITMiddleware
{
    static class Globals
    {
        //variables
        public static string org= "vlEQNapvRjh";
        public static string khis_financial_year = "2022Oct";
        public static string khis_tx_curr_quaters = "202212";//;202303;202306;202309
        public static string org_id = "fVra3Pwta0Q";//Migori County
        public static int pageSize=2;
        public static int currentPage=1;
        //KHIS Endpoints
        //TX_CURR API
        public static string khis_indicatorsUrl = $"https://hiskenya.org/api/analytics.json?paging=true&currentPage={currentPage}&pageSize={pageSize}&dimension=ou:HfVjCurKxh2;LEVEL-t9kwHRyMyOC&dimension=dx:OePJt8CcZ0d;lj9QYJqS7bN;gMICOUtzqRb;XiRbc0DSMOH;YXJf27jfkvS;JiuqbydCIcy;pkShOkgNQt2;atSQz5O7e2A;gTkVw97FnQK;du5RMT3aecB;wu0ITFRjUzF;D9YwtS6RhQ1;xMNhnyu7vm1;kLXGWRLzCAw;oCFXmpol7D8;F9OR49Lc1aR;cBTa1jVzT8f;J4vNm7YEkdj;mACm1JUzeLT;Hbc2qRi0U5x;jthpt5cVV9c;LC2OqnUC5Sn;DaNcGZnkclz;yWkSi8L3qGm;zNCSlBKbS6d;K59f8nZ5vhy;Nv4OkbdDvmm;SCMKsiNj6c5;lJpaBye9B0H;WNFWVHMqPv9;ETX9cUWF43c;qSgLzXh46n9&dimension=pe:{khis_financial_year}&tableLayout=true&rows=ou;dx;pe&skipRounding=false&completedOnly=false&hideEmptyRows=true&showHierarchy=true";
        public static string khis_tx_currUrl = $"https://hiskenya.org/api/analytics.json?dimension=ou:HfVjCurKxh2;LEVEL-t9kwHRyMyOC&dimension=dx:wbJOu4h2SSz;GSEmLUnrvzj;Jbu4if6gtDp;AQsTt7jtKbt;oOOnacUi9Jm;EhZQp3PTA3C;e93GKJTHKAX;yNCUlEYkmyA;pR7VzBydoj3;SJL4k6Gl53C&dimension=pe:{khis_tx_curr_quaters}&tableLayout=true&rows=ou;dx;pe&skipRounding=false&completedOnly=false&hideEmptyRows=true&showHierarchy=true&page={currentPage}&pageSize={pageSize}";
        //PMTCT PRI ENDPOINTS
        public static string pmtct_priUrl = $"https://khis.pmtct.uonbi.ac.ke/api/29/analytics.json?dimension=pe:LAST_4_QUARTERS&dimension=ou:LEVEL-5;{org_id}&dimension=dx:Ig7aiZbF6sj;Tt9i9rV8ICT;sw9rRvhK5uE;iojjQ7yoxSB;w88SD4XPRHv;aV0JHG4QXpV;U3ohB7AehIN;eehnmEKnXTC;bZu1tvWcKhU&displayProperty=NAME&hierarchyMeta=true&outputIdScheme=NAME";
        //DATIM Endpoints

        //Server Endpoints
        public static string localServerUrl = "http://127.0.0.1:8000/";
        //Remote server Credentials
        public static string pmtctcServerUsername = "healthit";
        public static string pmtctServerPassword = "rr23H3@1th1Tmtct";
        //------------------------------------------------------
        public static string remoteServertUsername = "TitusO";
        public static string remoteServerPassword = "Password@123";
        //Local Server Credentials
        public static string localSeverUsername = "admin";
        public static string localServerpassword = "@Super123";
        public static string platformType = "client";
        public static string localSserverToken = "";
        public static object remoteServerToken = "";
    }
}
