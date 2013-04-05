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


		public IList<DirectoriesEntry> SearchAddressByPhone(string PhoneNumber)
		{
            DirectoriesPaging Paging = new DirectoriesPaging();
            Paging.StartAtIndex = 1;
            Paging.FinishAtIndex = 1;

			var query= new DirectoriesSearchAddressQuery (this.Authentication,Paging);
			query.AddPhoneParameter (PhoneNumber);
			return new DirectoriesSearchAddressExecutor (query).GetEntries ();
		}

		public IList<DirectoriesEntry> SearchAddressByFirstName(string FirstName,int from,int to)
		{
            DirectoriesPaging Paging = new DirectoriesPaging();
            Paging.StartAtIndex = from;
            Paging.FinishAtIndex = to;
			var query = new DirectoriesSearchAddressQuery (this.Authentication, Paging);
			query.AddFirstNameParameter (FirstName, false, DirectoriesPrecisionCode.FieldLevel);
			return new DirectoriesSearchAddressExecutor (query).GetEntries ();
		}

		public IList<DirectoriesEntry> SearchAddressByLastName(string LastName,int from,int to)
		{
            DirectoriesPaging Paging = new DirectoriesPaging();
            Paging.StartAtIndex = from;
            Paging.FinishAtIndex = to;
            
			var query = new DirectoriesSearchAddressQuery (this.Authentication, Paging);
			query.AddLastNameParameter (LastName, false, DirectoriesPrecisionCode.FieldLevel);
			return new DirectoriesSearchAddressExecutor (query).GetEntries ();
		}

		public IList<DirectoriesEntry> SearchAddressByLastNameAndLocation(string LastName,string Location,int from,int to)
		{
            DirectoriesPaging Paging = new DirectoriesPaging();
            Paging.StartAtIndex = from;
            Paging.FinishAtIndex = to;

			var query = new DirectoriesSearchAddressQuery (this.Authentication, Paging);
            query.AddLocationParameter(Location, false);
			query.AddLastNameParameter (LastName, false, DirectoriesPrecisionCode.FieldLevel);
			return new DirectoriesSearchAddressExecutor (query).GetEntries ();
		}

		


	}
}
