
using System.Windows.Forms;


namespace SpamAssassinInterface
{


    static class Program
    {


        // http://stackoverflow.com/questions/1323283/how-to-match-url-in-c
        public static string UrlMatch(string text)
        {
            System.Text.RegularExpressions.Regex r =
                new System.Text.RegularExpressions.Regex(@"(?<Protocol>\w+):\/\/(?<Domain>[\w@][\w.:@]+)\/?[\w\.?=%&=\-@/$,]*");
            // Match the regular expression pattern against a text string.
            System.Text.RegularExpressions.Match m = r.Match(text);
            while (m.Success)
            {
                System.Console.WriteLine(m);

                text = text.Replace(m.Value, "<a href='" + m.Value + "'>" + m.Value + "</a>");

                //do things with your matching text 
                m = m.NextMatch();
            } // Whend 

            return text;
        } // End Function UrlMatch 


        // http://rickyrosario.com/blog/converting-a-url-into-a-link-in-csharp-using-regular-expressions/
        public static string ConvertUrlsToLinks(string msg)
        {
            string regex = @"((www\.|(http|https|ftp|news|file)+\:\/\/)[&#95;.a-z0-9-]+\.[a-z0-9\/&#95;:@=.+?,##%&~-]*[^.|\'|\# |!|\(|?|,| |>|<|;|\)])";
            System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(regex, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            return r.Replace(msg, "<a href=\"$1\" title=\"Click to open in a new window or tab\" target=\"&#95;blank\">$1</a>").Replace("href=\"www", "href=\"http://www");
        } // End Function ConvertUrlsToLinks  


        // http://weblogs.asp.net/farazshahkhan/regex-to-find-url-within-text-and-make-them-as-link
        public static string MakeLink(string txt)
        {
            System.Text.RegularExpressions.Regex regx =
                new System.Text.RegularExpressions.Regex("https?://([\\w+?\\.\\w+])+([a-zA-Z0-9\\~\\!\\@\\#\\$\\%\\^\\&amp;\\*\\(\\)_\\-\\=\\+\\\\\\/\\?\\.\\:\\;\\'\\,]*)?"
                    , System.Text.RegularExpressions.RegexOptions.IgnoreCase
            );

            System.Text.RegularExpressions.MatchCollection matches = regx.Matches(txt);

            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                txt = txt.Replace(match.Value, "<a href='" + match.Value + "'>" + match.Value + "</a>");
            } // Next match 

            return txt;
        } // End Function MakeLink 


        public static void LoadHtml()
        {
            /*
            var html = @"<a href=""http://msdn.microsoft.com/en-us/library/Aa538627.aspx"" onclick=""trackClick(this, '117', 'http\x3a\x2f\x2fmsdn.microsoft.com\x2fen-us\x2flibrary\x2fAa538627.aspx', '15');"">ToolStripItemOwnerCollectionUIAdapter.GetInsertingIndex Method ...</a>";
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(html);

            var link = document.DocumentNode.SelectSingleNode("//a");
            if (link != null)
            {
                var href = link.Attributes["href"].Value;
                var innerText = link.InnerText;
            }
            */
        } // End Function LoadHtml 


        // Very good regex
        // http://stackoverflow.com/questions/10576686/c-sharp-regex-pattern-to-extract-urls-from-given-string-not-full-html-urls-but
        public static string Linkify(string text)
        {
            // http://en.wikipedia.org/wiki/URI_scheme

            System.Text.RegularExpressions.Regex linkParser =
                // new System.Text.RegularExpressions.Regex(@"\b(?:https?://|www\.)\S+\b"
                new System.Text.RegularExpressions.Regex(@"\b(?:(https?|ftp|ftps|sftp|chrome|cvs|data|dav|dns|facetime|fax|feed|file|finger|git|gtalk|gopher|imap|irc|irc6|ldaps?|magnet|mailto|mms|news|nfs|rsync|ssh|tftp):(//)?|www\.)\S+\b"
                    , System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.IgnoreCase
            );

            // foreach (System.Text.RegularExpressions.Match m in linkParser.Matches(text)) System.Console.WriteLine(m.Value);

            text = linkParser.Replace(text, new System.Text.RegularExpressions.MatchEvaluator(
                     delegate(System.Text.RegularExpressions.Match m)
                     {
                         return "<a href=\"" + m.Value + "\">" + m.Value + "</a>";
                     }
                )
            );

            System.Console.WriteLine(text);
            return text;
        } // End Function Linkify 


        // Really foobar
        // http://stackoverflow.com/questions/758135/c-sharp-code-to-linkify-urls-in-a-string
        public static void Foobar(string text)
        {
            string html = System.Text.RegularExpressions.Regex.Replace(text, @"^(http|https|ftp)\://[a-zA-Z0-9\-\.]+" +
                             @"\.[a-zA-Z]{2,3}(:[a-zA-Z0-9]*)?/?" +
                             @"([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*$",
                             "<a href=\"$1\">$1</a>");

            System.Console.WriteLine(html);
        } // End Sub Foobar 


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [System.STAThread]
        static void Main()
        {
#if false
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            return;
#endif

            /*
            System.Collections.Generic.List<SimpleSpamAssassin.RuleResult> Report = 
                SimpleSpamAssassin.GetReport("192.168.1.11", "hello world !");
            
            System.Console.WriteLine(Report);
            */


            string strText = @"
Hello I'm a https://www.google.com/ncr?foo=bar person

lolipop://foobar.com.lol?load=magic
http://88.84.21.77
hello 88.84.21.77
www.cvp-altstätten.ch


and I don't like noobs
";


            string str = UrlMatch(strText);
            System.Console.WriteLine(str);
            Foobar(strText);
            Linkify(strText);


            str = ConvertUrlsToLinks(strText);
            System.Console.WriteLine(str);

            str = MakeLink(strText);
            System.Console.WriteLine(str);


            System.Console.WriteLine(System.Environment.NewLine);
            System.Console.WriteLine(" --- Press any key to continue --- ");
            System.Console.ReadKey();
        } // End Sub Main


    } // End Class Program


} // End Namespace SpamAssassinInterface 
