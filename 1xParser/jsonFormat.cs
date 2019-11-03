namespace jsonFormats
{
    //live res

    public class LiveRootObj
    {
        public string CN { get; set; }
        public int CO { get; set; }
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
        public int N { get; set; }
        public string O1 { get; set; }
        public int O1C { get; set; }
        public string O1CT { get; set; }
        public string O1E { get; set; }
        public int O1I { get; set; }
        public string[] O1IMG { get; set; }
        public int[] O1IS { get; set; }
        public string O2 { get; set; }
        public int O2C { get; set; }
        public string O2CT { get; set; }
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
        public int SST { get; set; }
        public string STI { get; set; }
        public int T { get; set; }
        public string TN { get; set; }
        public int HMH { get; set; }
        public int R { get; set; }
        public SC SC { get; set; }
        public int ZP { get; set; }
        public int HS { get; set; }
    }

    public class SC
    {
        public int CP { get; set; }
        public string CPS { get; set; }
        public FS FS { get; set; }
        public int HC { get; set; }
        public P[] PS { get; set; }
        public ST[] ST { get; set; }
        public int TS { get; set; }
    }

    public class FS
    {
        public int S1 { get; set; }
        public int S2 { get; set; }
    }

    public class P
    {
        public int Key { get; set; }
        public Value Value { get; set; }
    }

    public class Value
    {
        public int S1 { get; set; }
        public int S2 { get; set; }
    }

    public class ST
    {
        public int Key { get; set; }
        public Value1[] Value { get; set; }
    }

    public class Value1
    {
        public int ID { get; set; }
        public object N { get; set; }
        public string S1 { get; set; }
        public string S2 { get; set; }
    }


    //line res
    public class LineRootObj
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
        public double P { get; set; }
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