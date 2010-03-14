using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SeoEmpire.Utils
{
    public class TextReplacementsContainer : ICloneable
    {
        public string Text;
        public Dictionary<string, string> Replacements = new Dictionary<string, string>();



        #region ICloneable Members

        public object Clone()
        {
            return new TextReplacementsContainer()
                       {
                           Text = Text,
                           Replacements = Replacements
                       };
        }

        #endregion
    }

    public static class TextUtils
    {
        static public string StripAllTags(string input)
        {
            return new Regex("<[^>]+?>",
                            RegexOptions.IgnoreCase)
                    .Replace(input, String.Empty);
        }

        /*static public string ShortText(string input, int symbCount, string endWith)
        {
            
        }*/

        static public string StripTags(string input, string[] tags)
        {
            if (tags == null)
                tags = new[] {String.Empty};

            foreach (string tag in tags)
            {
                input = new Regex(string.Format("<{0}([>]| [^>]*?>)", tag), 
                            RegexOptions.IgnoreCase)
                    .Replace(input, String.Empty);

                input = new Regex(string.Format("<(/[^<>]*?[^A-Za-z]|/){0}>", tag), 
                            RegexOptions.IgnoreCase)
                    .Replace(input, String.Empty);

                input = new Regex(string.Format("<{0}([^A-Za-z][^>]*?/|/)>", tag),
                            RegexOptions.IgnoreCase)
                    .Replace(input, String.Empty);
            }

            return input;
        }

        static public TextReplacementsContainer ReplaceTags(string input, string[] tags)
        {
            int counter = 0;
            const string foundPattern = "<{0}[\\s\\S]+?{0}>";
            const string replaceTo = "wwwdev{0}wwwdev";

            Dictionary<string,string> replacements = new Dictionary<string, string>();

            foreach (string tag in tags)
            {
                foreach (Match match in new Regex(String.Format(foundPattern, tag), 
                        RegexOptions.IgnoreCase)
                    .Matches(input))
                {
                    input = input.Replace(match.Value, String.Format(replaceTo, counter));

                    replacements.Add(String.Format(replaceTo, counter++),match.Value);
                }
            }

            return  new TextReplacementsContainer()
                        {
                            Replacements = replacements,
                            Text = input
                        };
        }

        static public string ClearScriptAndCss(string input)
        {
            TextReplacementsContainer container = ReplaceTags(input, new[] {"style", "script"});

            foreach (KeyValuePair<string, string> pair in container.Replacements)
            {
                container.Text = container.Text.Replace(pair.Key, String.Empty);
            }

            return container.Text;
        }

        static public string UndoReplaceTags(TextReplacementsContainer container)
        {
            foreach (KeyValuePair<string, string> pair in container.Replacements)
                container.Text = container.Text.Replace(pair.Key, pair.Value);
        
            return container.Text;
        }
    }
}
