using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Epsitec.Data.Platform.ServiceReference.Directories.SearchAddress;

namespace Epsitec.Data.Platform.Directories
{
	public class DirectoriesSearchAddressExecutor
	{
		public DirectoriesSearchAddressExecutor(DirectoriesSearchAddressQuery query)
		{
			XElement request = query.ForgeRequest ();
			SearchAddressSoapClient SearchClient = new SearchAddressSoapClient ();
			this.Response = XElement.Parse (SearchClient.SearchAddress (request.ToString ()));
			if (DirectoriesResponseChecker.RequestHasError (this.Response))
			{
				throw new Exception ("Query Error");
			}
		}

		private XElement Response;

		public List<DirectoriesEntry> GetEntries()
		{
			List<DirectoriesEntry> ResultSet = new List<DirectoriesEntry> ();
			
			//TODO Parse XElement
			var entries = from e in this.Response.Element ("Entries").Elements ()
						  select e;

			foreach (XElement elem in entries)
			{
				DirectoriesEntry entry = new DirectoriesEntry (elem);
				ResultSet.Add (entry);
			}

			return ResultSet;
		}
	}
}
