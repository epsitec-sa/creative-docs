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

		public void Query(string query)
		{
			var result = ElasticClient.Search<Document>(s => s
							.Index("aider")
							.AllTypes()
							.QueryString(query));

			Clients.Caller.processResult (result);
		}


		public void IndexDocument(string id,string name,string text,string type)
		{
			var client = ElasticClient;
			var document = new Document ()
			{
				DocumentId = id,
				Name = name,
				Text = text
			};

			if (!client.IndexExists ("aider").Exists)
			{
				client.CreateIndex ("aider", new IndexSettings ());
			}

			client.Index (document, "aider", type, document.DocumentId);

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
