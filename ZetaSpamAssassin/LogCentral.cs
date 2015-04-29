
using System;
using System.Collections.Generic;
using System.Text;


namespace ZetaSpamAssassin
{
    public class LogCentral
    {

        public static cCurrentLog Current = new cCurrentLog();
    }


    public class cCurrentLog
    {

        public void LogDebug(string str)
        {
            System.Console.WriteLine(str);
        }


        public void LogWarn(string str)
        {
            System.Console.WriteLine(str);
        }

    }


}
