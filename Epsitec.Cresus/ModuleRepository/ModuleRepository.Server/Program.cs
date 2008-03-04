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
			ServiceHost host = new ServiceHost (typeof (ModuleRepositoryService), new System.Uri ("http://localhost:8080/"));

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

			host.AddServiceEndpoint (typeof (IModuleRepositoryService), new BasicHttpBinding (), "ModuleRepository");
			host.AddServiceEndpoint (typeof (IMetadataExchange), new BasicHttpBinding (), "MEX");
			host.Open ();

			System.Console.WriteLine ("Listening for requests");
			System.Console.ReadKey ();
		}
	}
}
