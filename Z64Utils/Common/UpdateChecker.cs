using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class UpdateChecker
    {
        public const string ReleaseURL = @"https://github.com/Random06457/Z64Utils/releases";
        public const string CurrentTag = "v2.0.1";
        public static string GetLatestTag()
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create($"{ReleaseURL}/latest");
            var resp = req.GetResponse();
            string tag = resp.ResponseUri.AbsoluteUri.Replace($"{ReleaseURL}/tag/", "");
            return tag;
        }
    }
}
