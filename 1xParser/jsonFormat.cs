namespace jsonFormats
{
    //game res
    public class GameResRootObj
    {
        public string Error { get; set; }
        public int ErrorCode { get; set; }
        public string Guid { get; set; }
        public int Id { get; set; }
        public bool Success { get; set; }
        public ValueGR Value { get; set; }
    }
    public class ValueGR
    {
        public string CN { get; set; }
        public int CO { get; set; }
        public int COI { get; set; }
        public object E { get; set; }
        public int EC { get; set; }
        public int I { get; set; }
        public string L { get; set; }
        public string LE { get; set; }
        public int LI { get; set; }
        public object MIO { get; set; }
        public MI[] MIS { get; set; }
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
        public int SI { get; set; }
        public string SN { get; set; }
        public int T { get; set; }
        public string TN { get; set; }
        public bool F { get; set; } = false;
        public int R { get; set; } = 0;
        public SCgr SC { get; set; }
    }
    public class SCgr
    {
        public int CP { get; set; }
        public string CPS { get; set; }
        public FS FS { get; set; }
        public int GS { get; set; }
        public P[] PS { get; set; }
        public object S { get; set; }
        public int TR { get; set; }
        public int TS { get; set; }
    }

    //live res
    public class LiveRootObj
    {
        public string Error { get; set; }
        public int ErrorCode { get; set; }
        public string Guid { get; set; }
        public int Id { get; set; }
        public bool Success { get; set; }
        public ValueLV[] Value { get; set; }
    }
    public class ValueLV
    {
        public string CN { get; set; }
        public int CO { get; set; }
        public int COI { get; set; }
        public E[] E { get; set; }
        public int EC { get; set; }
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
        public string O2E { get; set; }
        public int O2I { get; set; }
        public string[] O2IMG { get; set; }
        public int[] O2IS { get; set; }
        public int S { get; set; }
        public string SE { get; set; }
        public int SI { get; set; }
        public string SN { get; set; }
        public string TN { get; set; }
        public int HMH { get; set; }
        public int R { get; set; }
        public SC SC { get; set; }
    }
    public class SC
    {
        public int CP { get; set; }
        public string CPS { get; set; }
        public FS FS { get; set; }
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
        public Value1 Value { get; set; }
    }
    public class Value1
    {
        public int S1 { get; set; }
        public int S2 { get; set; }
    }
    public class ST
    {
        public int Key { get; set; }
        public Value2[] Value { get; set; }
    }
    public class Value2
    {
        public int ID { get; set; }
        public object N { get; set; }
        public string S1 { get; set; }
        public string S2 { get; set; }
    }


    //line res
    public class LineRootObj
    {
        public string Error { get; set; }
        public int ErrorCode { get; set; }
        public string Guid { get; set; }
        public int Id { get; set; }
        public bool Success { get; set; }
        public ValueLN[] Value { get; set; }
    }
    public class ValueLN
    {
        public int CI { get; set; }
        public string CN { get; set; }
        public int COI { get; set; }
        public E[] E { get; set; }
        public int EC { get; set; }
        public int HS { get; set; }
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
        public int SSI { get; set; }
        public int SST { get; set; }
        public string STI { get; set; }
        public int T { get; set; }
        public string TN { get; set; }
        public int B { get; set; }
        public int GVE { get; set; }
        public string LS { get; set; }
    }
    public class MIO
    {
        public string Loc { get; set; }
        public string TSt { get; set; }
    }
    public class E
    {
        public double C { get; set; } = 0;
        public int G { get; set; } = 0;
        public int T { get; set; } = 0;
        public double P { get; set; } = 0;
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
        public Entity[] entities { get; set; } = null;
    }
    public class From
    {
        public int id { get; set; }
        public bool is_bot { get; set; }
        public string first_name { get; set; }
        public string username { get; set; }
        public string language_code { get; set; } = "";
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


    //sendMessage result

    public class SendMsgResRoot
    {
        public bool ok { get; set; }
        public Message result { get; set; }
    }
}