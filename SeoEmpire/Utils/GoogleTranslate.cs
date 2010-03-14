using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace SeoEmpire.Utils
{
    public enum Language { en, ru, uk }

    public static class GoogleTranslate
    {
        private static readonly Regex TranslatedText = 
            new Regex(@"overflow:auto"">([\s\S]+?)</textarea>", RegexOptions.Compiled);

        private static readonly Regex HackRegex =
            new Regex(@"<[\s]*/[\s]*[A-Za-z]+[\s]*>", RegexOptions.Compiled);

        public static string Send(string text,Language from,Language to)
        {
            if (String.IsNullOrEmpty(text)) return text;

            string result = CreateRequest(
                text,
                from,
                to);

            Match match = TranslatedText.Match(result);

            if (match == null)
                return null;

            return match.Groups[1].Value;
        }

        private static string ResolveLang(Language lang)
        {
            return Enum.GetName(typeof(Language), lang);
        }

        private static String CreateRequest(string text, Language lFrom, Language lTo)
        {
            string from = ResolveLang(lFrom);
            string to = ResolveLang(lTo);

	        HttpWebRequest gRequest =
              (HttpWebRequest)WebRequest.Create("http://translate.google.ru/");

            gRequest.Proxy.GetProxy(new Uri("http://localhost:8888"));

            String content = String.Format(
                "js=y&prev=_t&hl={0}&ie=UTF-8&text={2}&file=&sl={0}&tl={1}",
                from, to, Uri.EscapeDataString(text));

            gRequest.Referer = "http://translate.google.ru/";
            gRequest.Method = "POST";
            gRequest.ContentType = "application/x-www-form-urlencoded";

            byte[] ByteArr = System.Text.Encoding.GetEncoding(1251).GetBytes(content);
            gRequest.ContentLength = ByteArr.Length;
            gRequest.GetRequestStream().Write(ByteArr, 0, ByteArr.Length);

            try
            {
                WebResponse response = gRequest.GetResponse();

                Stream stream = response.GetResponseStream();

                byte[] buff = new byte[1024];
                int readed = 0;
                string output = string.Empty;

                while ((readed = stream.Read(buff, 0, buff.Length)) > 0)
                {
                    switch (lTo)
                    {
                        case Language.ru:
                            output += Encoding.GetEncoding("KOI8-R").GetString(buff, 0, readed);
                            break;
                        case Language.uk:
                            output += Encoding.GetEncoding("Windows-1251").GetString(buff, 0, readed);
                            break;
                        default:
                            throw new Exception("Unsupported Language");
                    }
                    
                    readed = 0;

                }

                return output;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }

        public static string GoogleTranslateClosedTagsFastHack(string text)
        {
            return HackRegex.Replace(text, m => m.Value.Replace(" ", String.Empty).ToLower());
        }
    }
}
