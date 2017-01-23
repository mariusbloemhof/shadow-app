//using Android.Telephony;
//using Foundation;
using System;
using System.Globalization;
using System.IO;
using System.Net;
using Xamarin.Forms;

namespace Shadow
{
    public static class RouteSMS
    {
        private static WebRequest webrequest;
        private static WebResponse webresponse;

        private static String server = "smsplus.routesms.com";
        private static int port = 8080;
        private static String username = "shadowS";
        private static string password = "zBc9Oih9";
        //0: means plain text
        //1: means flash
        //2: means Unicode (Message content should be in Hex)
        //6: means Unicode Flash(Message content should be in Hex)
        private static int messageType = 0;
        private static int DLR = 0;
        private static string from = "Shadow";


        static RouteSMS()
        {

        }

        public static Boolean SendSMS(string phoneno, string countryCode, string smsMessage)
        {
            string countryCD = CultureInfo.CurrentUICulture.Name;
            int index = countryCD.IndexOf('-');
            countryCD = countryCD.Remove(0, index + 1);

            //phoneno = PhoneNumberUtils.FormatNumberToE164(phoneno, countryCD);

            System.Uri.EscapeUriString(smsMessage);
            string response = "";
            string URL = "http://" + server + ":" + port + "/bulksms/bulksms?username=" + username + "&password="
                   + password + "&type=" + messageType + "&dlr=" + DLR + "&destination=" + phoneno
                   + "&source=" + from + "&message= " + smsMessage + "";
            System.Uri.EscapeUriString(URL);

            webrequest = HttpWebRequest.Create(URL);
            webrequest.Timeout = 25000;
            try
            {
                webresponse = webrequest.GetResponse();
                StreamReader reader = new StreamReader(webresponse.GetResponseStream());
                response = reader.ReadToEnd();
                webresponse.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
                //
            }
        }
    }
}

