
using System.Windows.Forms;


namespace SpamAssassinInterface
{


    static class Program
    {


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

            System.Collections.Generic.List<SimpleSpamAssassin.RuleResult> Report = 
                SimpleSpamAssassin.GetReport("192.168.1.11", "hello world !");
            System.Console.WriteLine(Report);
        } // End Sub Main


    } // End Class Program


} // End Namespace SpamAssassinInterface 
