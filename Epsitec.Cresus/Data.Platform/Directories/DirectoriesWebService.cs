using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Epsitec.Data.Platform.ServiceReference.Directories.SearchAddress;
using Epsitec.Data.Platform.ServiceReference.Directories.Security;

namespace Epsitec.Data.Platform.Directories
{
	public class DirectoriesWebService
	{

		public DirectoriesWebService()
		{
			this.Authentication = new DirectoriesAuthentication ();
			this.Paging = new DirectoriesPaging ();
			this.Paging.StartAtIndex = 1;
			this.Paging.FinishAtIndex = 1;

			this.Authentication.ServiceId = "ff1fd734-e3e0-4cd4-bfb7-2cb464038d38";
			this.Authentication.UserName = "1007651980";
			this.Authentication.Password = "nW07?+E";

			this.CheckVersion ();
		}

		public DirectoriesWebService(string ServiceId,string UserName, string Password)
		{
			this.Authentication.ServiceId = ServiceId;
			this.Authentication.UserName = UserName;
			this.Authentication.Password = Password;
	
			this.CheckVersion ();
		}

		private DirectoriesAuthentication Authentication;
		private DirectoriesPaging Paging;
		public const string SecurityVersion="4.0.0";
		public const string SearchVersion="4.1.1";
		

		private void CheckVersion()
		{
			SecuritySoapClient SecurityClient = new SecuritySoapClient ();
			XElement Response = XElement.Parse (SecurityClient.Version ());

			string SecurityRemoteVersion = Response.Element ("VersionInfo").Attribute ("Version").Value;
			
			string [] CurrentVersionPattern = DirectoriesWebService.SecurityVersion.Split ('.');
			string [] RemoteVersionPattern = SecurityRemoteVersion.Split('.');
			if(CurrentVersionPattern[0]!=RemoteVersionPattern[0])
			{
				throw new Exception ("Security Version Check: Major change detected,  Input parameters and/or fields in the results of a web method have been removed.");
			}
			if (CurrentVersionPattern[1]!=RemoteVersionPattern[1])
			{
				throw new Exception ("Security Version Check: Minor change detected,  Additional method and/or fields in the results of a web method.");
			}
			

			SearchAddressSoapClient SearchClient = new SearchAddressSoapClient ();
			Response = XElement.Parse (SearchClient.Version ());

			CurrentVersionPattern = DirectoriesWebService.SearchVersion.Split ('.');
			RemoteVersionPattern = Response.Element ("VersionInfo").Attribute ("Version").Value.Split ('.');

			if (CurrentVersionPattern[0]!=RemoteVersionPattern[0])
			{
				throw new Exception ("Search Version Check: Major change detected,  Input parameters and/or fields in the results of a web method have been removed.");
			}
			if (CurrentVersionPattern[1]!=RemoteVersionPattern[1])
			{
				throw new Exception ("Search Version Check: Minor change detected,  Additional method and/or fields in the results of a web method.");
			}

		}


		public List<DirectoriesEntry> SearchAddressByPhone(string PhoneNumber)
		{
			var query= new DirectoriesSearchAddressQuery (this.Authentication,this.Paging);
			query.AddPhoneParameter (PhoneNumber);
			return new DirectoriesSearchAddressExecutor (query).GetEntries ();
		}

		public List<DirectoriesEntry> SearchAddressByFirstName(string FirstName)
		{
			var query = new DirectoriesSearchAddressQuery (this.Authentication, this.Paging);
			query.AddFirstNameParameter (FirstName, false, DirectoriesPrecisionCode.FieldLevel);
			return new DirectoriesSearchAddressExecutor (query).GetEntries ();
		}

		public List<DirectoriesEntry> SearchAddressByLastName(string LastName)
		{
			var query = new DirectoriesSearchAddressQuery (this.Authentication, this.Paging);
			query.AddLastNameParameter (LastName, false, DirectoriesPrecisionCode.FieldLevel);
			return new DirectoriesSearchAddressExecutor (query).GetEntries ();
		}

		public List<DirectoriesEntry> SearchAddressByFullName(string FirstName, string LastName)
		{
			var query = new DirectoriesSearchAddressQuery (this.Authentication, this.Paging);
			query.AddFirstNameParameter (FirstName, false, DirectoriesPrecisionCode.FieldLevel);
			query.AddLastNameParameter (LastName, false, DirectoriesPrecisionCode.FieldLevel);
			return new DirectoriesSearchAddressExecutor (query).GetEntries ();
		}

		


	}
}
