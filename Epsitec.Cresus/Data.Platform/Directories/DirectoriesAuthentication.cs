using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Epsitec.Data.Platform.Directories.Helpers;
using Epsitec.Data.Platform.ServiceReference.Directories.Security;

namespace Epsitec.Data.Platform.Directories
{
	public class DirectoriesAuthentication
	{
		public string ServiceId
		{
			get
			{
				return this.serviceid;
			}

			set
			{
				this.serviceid = value;
			}
		}

		public string UserName
		{
			get
			{
				return this.username;
			}

			set
			{
				this.username = value;
			}
		}

		public string Password
		{
			get
			{
				return this.password;
			}

			set
			{
				this.password = value;
			}
		}

		public string SessionId
		{
			get
			{
				if (this.sessionid==null)
				{
					this.Login ();
				}
				return this.sessionid;
			}
		}

		private void Login()
		{
			///@"<LoginParam ServiceId='ff1fd734-e3e0-4cd4-bfb7-2cb464038d38' UserName='1007651980' Password='nW07?+E'/>"
			XElement LoginParam = new XElement ("LoginParam");
			LoginParam.SetAttributeValue ("ServiceId", this.serviceid);
			LoginParam.SetAttributeValue ("UserName", this.username);
			LoginParam.SetAttributeValue ("Password", this.password);

			SecuritySoapClient SecurityClient = new SecuritySoapClient ();
			XElement Response = XElement.Parse (SecurityClient.Login (LoginParam.ToString ()));
			if (DirectoriesResponseChecker.RequestHasError (Response))
			{
				throw new Exception ("DirectoriesAuthenticationError: Login failed");
				//TODO PARSE ERROR CODE
			}

			this.sessionid = Response.Element ("SessionInfo").Attribute ("SessionId").Value;

		}

		private string serviceid;
		private string username;
		private string password;
		private string sessionid;


		public XElement GetAuthenticationElement()
		{
			XElement Authentication = new XElement ("Authentication");
			Authentication.SetAttributeValue ("ServiceId", this.ServiceId);
			Authentication.SetAttributeValue ("SessionId", this.SessionId);
			return Authentication;
		}
	}
}
