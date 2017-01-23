using System;

namespace Shadow
{
	public static class Logger
	{
		public static void LogError(string Module, string Func, string Detail)
		{
			string Log = "";

			if(Shadow.IO.LocalStorage.FileExists("root","log1.txt").Result)
				Log = Shadow.IO.LocalStorage.ReadTextFile ("root", "log1.txt").Result;

			Log = "Error: " + DateTime.Now.ToString () + " @ " + Module + " " + Func + " " + Detail + "<br /><br />" + Log;
			bool result = Shadow.IO.LocalStorage.WriteTextFile ("root", "log1.txt", Log).Result;
			//Console.WriteLine ("Error: " + DateTime.Now.ToLongTimeString () + " @ " + Module + " " + Func + " " + Detail);
		}

		public static void LogDebug(string Module, string Func, string Detail)
		{
			/*string Log = "";

			if(Shadow.IO.LocalStorage.FileExists("root","log1.txt").Result)
				Log = Shadow.IO.LocalStorage.ReadTextFile ("root", "log1.txt").Result;
			
			Log = "Debug: " + DateTime.Now.ToString() + " @ " + Module + " " + Func + " " + Detail + "<br /><br />" + Log;
			bool result =  Shadow.IO.LocalStorage.WriteTextFile ("root", "log1.txt", Log).Result;
			//Console.WriteLine ("Debug: " + DateTime.Now.ToLongTimeString () + " @ " + Module + " " + Func + " " + Detail);*/
		}
	}
}

