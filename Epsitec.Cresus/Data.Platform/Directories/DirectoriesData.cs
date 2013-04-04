using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Epsitec.Data.Platform.ServiceReference.Directories.Security;
using Epsitec.Data.Platform.ServiceReference.Directories.SearchAddress;

namespace Epsitec.Data.Platform.Directories
{
	class DirectoriesData
	{

		public DirectoriesData()
		{
			this.ServiceId = "ff1fd734-e3e0-4cd4-bfb7-2cb464038d38";
			this.UserName = "1007651980";
			this.Password = "nW07?+E";

			this.CheckVersion ();
			this.SessionId = this.Login ();
		}

		public DirectoriesData(string ServiceId,string UserName, string Password)
		{
			this.ServiceId = ServiceId;
			this.UserName = UserName;
			this.Password = Password;

			this.CheckVersion ();
			this.SessionId = this.Login ();
		}

		private string ServiceId;
		private string UserName;
		private string Password;
		private string SessionId;
		public const string SecurityVersion="4.0.0";
		public const string SearchVersion="4.0.1";

		private void CheckVersion()
		{
			SecuritySoapClient SecurityClient = new SecuritySoapClient ();
			XElement Response = XElement.Parse (SecurityClient.Version ());

			if (Response.Element ("VersionInfo").Attribute ("Version").Value!=DirectoriesData.SecurityVersion)
			{
				throw new Exception ("Security WS Version don't match, this assembly need an update!");
			}

			SearchAddressSoapClient SearchClient = new SearchAddressSoapClient ();
			Response = XElement.Parse (SearchClient.Version ());

			if (Response.Element ("VersionInfo").Attribute ("Version").Value!=DirectoriesData.SearchVersion)
			{
				throw new Exception ("Search WS Version don't match, this assembly need an update!");
			}

		}

		private string Login()
		{
			///@"<LoginParam ServiceId='ff1fd734-e3e0-4cd4-bfb7-2cb464038d38' UserName='1007651980' Password='nW07?+E'/>"
			XElement LoginParam = new XElement ("LoginParam");
			LoginParam.SetAttributeValue ("ServiceId", this.ServiceId);
			LoginParam.SetAttributeValue ("UserName", this.UserName);
			LoginParam.SetAttributeValue ("Password", this.Password);

			SecuritySoapClient SecurityClient = new SecuritySoapClient ();
			XElement Response = XElement.Parse (SecurityClient.Login (LoginParam.ToString ()));
			if (RequestHasError (Response))
			{
				throw new Exception ("Login failed");
				//TODO PARSE ERROR CODE
			}

			return Response.Element ("SessionInfo").Attribute ("SessionId").Value;

		}

		private bool RequestHasError(XElement Response)
		{
			if (Response.Element ("ErrorInfo").Attribute ("ErrorCode").Value=="0")
			{
				return false;
			}
			else
			{
				return true;
			}
		}


	}
}
