using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SpamAssassinInterface
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
#if false
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            return;
#endif

            var lol = SimpleSpamAssassin.GetReport("192.168.1.11", "hello world !");
            System.Console.WriteLine(lol);
        }
    }
}
