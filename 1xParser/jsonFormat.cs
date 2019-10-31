namespace jsonFormats
{
    //site ex
    public class RootObj
    {
        public int CI { get; set; }
        public string CN { get; set; }
        public int COI { get; set; }
        public E[] E { get; set; }
        public int EC { get; set; }
        public bool HSI { get; set; }
        public int I { get; set; }
        public string L { get; set; }
        public string LE { get; set; }
        public int LI { get; set; }
        public object MIO { get; set; }
        public MI[] MIS { get; set; }
        public int[] MS { get; set; }
        public int N { get; set; }
        public string O1 { get; set; }
        public int O1C { get; set; }
        public string O1E { get; set; }
        public int O1I { get; set; }
        public string[] O1IMG { get; set; }
        public int[] O1IS { get; set; }
        public string O2 { get; set; }
        public int O2C { get; set; }
        public string O2E { get; set; }
        public int O2I { get; set; }
        public string[] O2IMG { get; set; }
        public int[] O2IS { get; set; }
        public int S { get; set; }
        public string SE { get; set; }
        public string SGI { get; set; }
        public int SI { get; set; }
        public string SN { get; set; }
        public int SS { get; set; }
        public int SSI { get; set; }
        public int SST { get; set; }
        public string STI { get; set; }
        public string TN { get; set; }
        public int B { get; set; }
        public int HS { get; set; }
        public string O1CT { get; set; }
        public string O2CT { get; set; }
        public string LS { get; set; }
        public int T { get; set; }
    }

    public class E
    {
        public float C { get; set; }
        public int G { get; set; }
        public int T { get; set; }
        public float P { get; set; }
    }

    public class MI
    {
        public int K { get; set; }
        public string V { get; set; }
    }

    //getUpd result
    public class GetUpdResRoot
    {
        public bool ok { get; set; }
        public Result[] result { get; set; }
    }

    public class Result
    {
        public int update_id { get; set; }
        public Message message { get; set; }
    }

    public class Message
    {
        public int message_id { get; set; }
        public From from { get; set; }
        public Chat chat { get; set; }
        public int date { get; set; }
        public string text { get; set; }
        public Entity[] entities { get; set; }
    }

    public class From
    {
        public int id { get; set; }
        public bool is_bot { get; set; }
        public string first_name { get; set; }
        public string username { get; set; }
        public string language_code { get; set; }
    }

    public class Chat
    {
        public int id { get; set; }
        public string first_name { get; set; }
        public string username { get; set; }
        public string type { get; set; }
    }

    public class Entity
    {
        public int offset { get; set; }
        public int length { get; set; }
        public string type { get; set; }
    }

}