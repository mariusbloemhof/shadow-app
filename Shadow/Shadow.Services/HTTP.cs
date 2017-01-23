using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using System.Net.Http;
using ModernHttpClient;

namespace Shadow.Services
{
	public class HTTP : IDisposable
	{
		bool disposed = false;

		public string GetRequest (string URL)
		{
			HttpClient httpClient = new HttpClient();

			string responseBodyAsText = httpClient.GetStringAsync (URL).Result;
			return responseBodyAsText;

			try{

			} catch (Exception ex) {
				Shadow.Logger.LogError("HTTP","GetRequest","Exception " + ex.Message);
			return "";
			}
			finally {
				
			}
			return "";
		}


		public void Dispose()
		{ 
			Dispose(true);
			GC.SuppressFinalize(this);           
		}

		// Protected implementation of Dispose pattern.
		protected virtual void Dispose(bool disposing)
		{
			if (disposed)
				return; 

			if (disposing) {
				// Free any other managed objects here.
				//
			}

			// Free any unmanaged objects here.
			//
			disposed = true;
		}
	}
}