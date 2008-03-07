//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Epsitec.ModuleRepository
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				ServiceHost host = new ServiceHost (typeof (ModuleRepositoryService), new System.Uri (Properties.Settings.Default.ServiceUrl));

				//	On Vista, enable access to HTTP via the following command, executed from
				//	an elevated prompt :
				//
				//	> netsh http add urlacl url=http://+:8080/ user=JeanDupont
				//
				//	On Windows XP or Server 2003 :
				//
				//	> httpcfg set urlacl /u http://+:8080/ /a "O:AOG:DAD:(A;;RPWPCCDCLCSWRCWDWOGA;;;S-1-0-0)"
				//
				//	See http://msdn2.microsoft.com/en-us/library/ms733768.aspx for detailed
				//	information


				ServiceMetadataBehavior behavior = new ServiceMetadataBehavior ();
				behavior.HttpGetEnabled = true;
				host.Description.Behaviors.Add (behavior);

				host.AddServiceEndpoint (typeof (IModuleRepositoryService), new BasicHttpBinding (), Properties.Settings.Default.ServiceName);
				host.AddServiceEndpoint (typeof (IMetadataExchange), new BasicHttpBinding (), "MEX");
				host.Open ();

				Program.WriteLine ("Listening for requests");
			}
			catch (System.Exception ex)
			{
				Program.WriteLine ("Exception thrown:\n{0}\n\nStack trace:\n{1}\n", ex.Message, ex.StackTrace);
			}

			System.Console.WriteLine ("Press a key to stop");
			System.Console.ReadKey ();
		}

		public static void WriteLine(string format, params object[] args)
		{
			string message = string.Concat (System.DateTime.Now.ToString (), ": ", string.Format (format, args));

			System.Console.WriteLine (message);
			System.Diagnostics.Trace.WriteLine (message);

			System.IO.File.AppendAllText (Properties.Settings.Default.TraceLogPath, string.Concat (message, "\n").Replace ("\n", "\r\n"), System.Text.Encoding.UTF8);
		}
	}
}
