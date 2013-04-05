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
            SearchClient.ChannelFactory.
			this.Response = XElement.Parse (SearchClient.SearchAddress (request.ToString ()));
			if (DirectoriesResponseChecker.RequestHasError (this.Response))
			{
                if(DirectoriesResponseChecker.NoResult(this.Response))
                {
                    this.HasResult = false;
                }
                else
                {
                    throw new Exception (this.Response.ToString());
                }
				
			}
		}

		private XElement Response;
        private bool HasResult = true;

        public DirectoriesResultInfo GetResultInfo()
        {
            if (this.HasResult)
            {
                return new DirectoriesResultInfo(this.Response.Element("ResultInfo"));
            }
            else
            {
                return new DirectoriesResultInfo();
            }
        }

		public IList<DirectoriesEntry> GetEntries()
		{
			IList<DirectoriesEntry> ResultSet = new List<DirectoriesEntry> ();

            if (this.HasResult)
            {
                var entries = from e in this.Response.Element("Entries").Elements()
                              select e;

                foreach (XElement elem in entries)
                {
                    DirectoriesEntry entry = new DirectoriesEntry(elem);
                    ResultSet.Add(entry);
                }
            }
			

			return ResultSet;
		}
	}
}
