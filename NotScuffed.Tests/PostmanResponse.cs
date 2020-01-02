using System.Collections.Generic;

namespace NotScuffed.Tests
{
    public class PostmanResponse
    {
        public Dictionary<string, string> Args { get; set; }
        public string Data { get; set; }
        public Dictionary<string, string> Form { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public string Url { get; set; }
        public dynamic Files { get; set; }
        public dynamic Json { get; set; }
    }
}