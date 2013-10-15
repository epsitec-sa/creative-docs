//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Microsoft.AspNet.SignalR;

using System.Threading.Tasks;
using Nest;
using Epsitec.Cresus.WebCore.Server.ElasticSearch;

namespace Epsitec.Cresus.WebCore.Server.Owin.Hubs
{
	public class ElasticSearchHub : Hub
	{

		private void Query(string q)
		{
			var result = ElasticClient.Search<Document> (body =>
				// return first 5 results, default is 10
				body.Size (5).Query (query =>
					query.QueryString (qs => qs.Query (q))));

			Clients.Caller.processResult (result);
		}


		private void AddDocument()
		{

		}

		public override Task OnDisconnected()
		{
			return base.OnDisconnected ();
		}

		public override Task OnReconnected()
		{
			return base.OnReconnected ();
		}

		private static ElasticClient ElasticClient
		{
			get
			{
				try
				{
					var uriString = "http://localhost:9200";
					var searchBoxUri = new System.Uri (uriString);
					var settings = new ConnectionSettings (searchBoxUri);
					settings.SetDefaultIndex ("sample");
					return new ElasticClient (settings);
				}
				catch (System.Exception)
				{
					throw;
				}
			}
		}
	}
}
