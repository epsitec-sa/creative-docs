//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Microsoft.AspNet.SignalR;

using System.Threading.Tasks;
using Nest;
using Epsitec.Cresus.Core.NoSQL;

namespace Epsitec.Cresus.WebCore.Server.Owin.Hubs
{
	public class ElasticSearchHub : Hub
	{

		public void Query(string query)
		{
			var result = ElasticClient.Search<ElasticSearchDocument> (s => s
				.Index ("aider")
				.Type ("contacts")
				.Query (q => q
					.FuzzyLikeThis (f => f
						.LikeText (query)
			)));


			Clients.Caller.processResult (result);
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
					var uri = new System.Uri (uriString);
					var settings = new ConnectionSettings (uri);
					settings.SetDefaultIndex ("contacts");
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
