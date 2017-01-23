using System;
namespace Shadow
{
	public interface INativeFunctions
	{
		void SendSMS(string Number, string Message);
	}
}
