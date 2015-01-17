//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Epsitec.Aider.Entities;
using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.NoSQL;
using Epsitec.Cresus.Database;
using Nest;
using Newtonsoft.Json;

namespace Epsitec.Aider.Data.Job
{
	internal class ElasticSearchLoader
	{
		public ElasticSearchLoader(CoreData coreData)
		{
			this.coreData = coreData;
			this.jobDateTime    = System.DateTime.Now;
			this.jobName        = "ElasticSearch DB Job";
			this.jobDescription = string.Format ("Importation des données Aider dans ElasticSearch");
			this.startDate      = Date.Today;
		}

		public void ProcessJob()
		{
			var time = this.LogToConsole ("starting main job");

			this.IndexContacts ();

			this.LogToConsole ("done in {0}", time);
		}

		private void IndexContacts()
		{
			this.ExecuteWithBusinessContext (
			businessContext =>
			{
				this.LogToConsole ("Fetching contacts to index...");
				
				var contactExample = new AiderContactEntity()
				{
					AddressType = Enumerations.AddressType.Default
				};

				var contactsToIndex = businessContext.DataContext.GetByExample<AiderContactEntity> (contactExample);	

				this.LogToConsole ("Done.");
				var total = contactsToIndex.Count ();
				var client = ElasticClient;

				if (!client.IndexExists ("aider").Exists)
				{
					client.CreateIndex ("aider", new IndexSettings ());
				}
				var count = 0;
				foreach (var contact in contactsToIndex)
				{

					var id = businessContext.DataContext.GetNormalizedEntityKey (contact).Value.ToString ();
					var druid = (Druid) Res.CommandIds.Base.ShowAiderContact;
					var name = contact.GetDisplayName ();
					var text = contact.Address.GetDisplayAddress ().ToSimpleText ();
					var document = new ElasticSearchDocument ()
					{
						DocumentId  = id,
						DatasetId = druid.ToCompactString (),
						EntityId  = id.Replace ('/', '-'),
						Name = name,
						Text = text
					};
					this.LogToConsole ("Indexing {0}\n{1}/{2} indexed", document.Name, count++, total);
					client.Index (document, "aider", "contacts", document.DocumentId);
					
				}

			});
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

		private void ExecuteWithBusinessContext(System.Action<BusinessContext> action)
		{
			var stackTrace    = new System.Diagnostics.StackTrace ();
			var stackFrames   = stackTrace.GetFrames ();
			var callingMethod = stackFrames[0].GetMethod ();

			var callerName = callingMethod.Name;

			var time = this.LogToConsole ("{0}, start job", callerName);

			using (var businessContext = new BusinessContext (this.coreData, false))
			{
				action (businessContext);
			}

			this.LogToConsole ("done in {0}",time);
		}

		private System.Diagnostics.Stopwatch LogToConsole(string format, params object[] args)
		{
			var message = string.Format (format, args);

			if (message.StartsWith ("Error"))
			{
				System.Console.ForegroundColor = System.ConsoleColor.Red;
			}

			System.Console.WriteLine ("ElasticSearch: {0}", message);
			System.Console.ResetColor ();

			var time = new System.Diagnostics.Stopwatch ();

			time.Start ();

			return time;
		}

		private CoreData						coreData;

		private System.DateTime					jobDateTime;
		private string							jobName;
		private string							jobDescription;
		private Date							startDate;
	}
}
