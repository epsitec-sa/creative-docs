//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using Epsitec.Common.Debugging;
using Epsitec.Common.Support.Extensions;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Server.NancyComponents;

namespace Epsitec.Cresus.Core.Server
{
	public sealed class CoreServerProgram
	{
		public CoreServerProgram()
		{
			Epsitec.Common.Document.Engine.Initialize ();

			RunNancy ();

			//var server = CoreServer.Instance;
			//var session = server.CreateSession ();

			//this.ExperimentalProfiling ();

			//PanelBuilder.CoreSession = session;
			//PanelBuilder.ExperimentalCode ();

			//this.ExperimentalEntityManipulations (session);

			//session.DisposeBusinessContext ();
			//server.DeleteSession (session.Id);


			//	TODO: wait until the server shuts down...
		}

		private void ExperimentalEntityManipulations(CoreSession session)
		{
			var context = session.GetBusinessContext ();

			var customer = (from x in context.GetAllEntities<CustomerEntity> ()
							where x.Relation.Person is NaturalPersonEntity
							let person = x.Relation.Person as NaturalPersonEntity
							where person.Lastname == "Schmid"
							select x).FirstOrDefault ();

			//	Jonas: utiliser IsNull() pour savoir si une entité est 'null' ou pas; en effet, un mécanisme
			//	appelé le NullEntityReferenceVirtualizer crée parfois des entités vides à la volée, pour ne
			//	pas avoir besoin de tester si chaque champ est nul, dans un accès du type 'customer.Relation.Person.Contacts'
			//	En plus, le système est assez futé pour créer des entités manquantes si on fait un accès en écriture...
			//	mais faudrait éviter, parce que ça ne peut pas initialiser proprement les entités et dans le cas de
			//	customer.Relation.Person la création d'une entité personne aboutirait (à tort) à la fabrication d'une
			//	entité 'AbstractPerson'.

			if (customer.IsNull ())
			{
				var titleRepo = context.GetRepository<PersonTitleEntity> ();

				//	Le repository permet de retrouver des données dans la base à partir d'exemples :

				var title  = titleRepo.GetByExample (new PersonTitleEntity ()
				{
					Name = "Monsieur"
				}).FirstOrDefault ();

				//	Crée une personne dans le contexte

				var person = context.CreateEntity<NaturalPersonEntity> ();

				person.Firstname = "Jonas";
				person.Lastname = "Schmid";
				person.Title = title;

				customer = context.CreateEntity<CustomerEntity> ();

				//	NB : ici, le client a déjà été initialisé (customer.IdA contient un n° de client)

				customer.Relation = context.CreateEntity<RelationEntity> ();
				customer.Relation.Person = person;
			}

			context.SetActiveEntity (customer);

			//	A partir d'ici, tu peux travailler avec le client...

			customer.IdB = System.DateTime.Now.ToShortTimeString ();

			//context.Discard ();
			context.SaveChanges ();
		}

		private void ExperimentalProfiling()
		{
			var server = CoreServer.Instance;

			for (int i = 0; i < 3; i++)
			{
				long time;
				var session = Profiler.ElapsedMilliseconds (server.CreateSession, out time);

				System.Diagnostics.Debug.WriteLine (string.Format ("Attempt {0}, creating session took {1} ms", i+1, time));

				server.DeleteSession (session.Id);
			}


			Profiler.ElapsedMicroseconds (() => CoreSession.GetBrickWall (new CustomerEntity (), Controllers.ViewControllerMode.Summary));

			for (int i = 0; i < 10; i++)
			{
				long time = Profiler.ElapsedMicroseconds (() => CoreSession.GetBrickWall (new CustomerEntity (), Controllers.ViewControllerMode.Edition));

				System.Diagnostics.Debug.WriteLine (string.Format ("Attempt {0}: fetching EditionController took {1} μs", i+1, time));
			}
		}

		private static void RunNancy()
		{
			CoreServerProgram.RunSelf ();
			//CoreServerProgram.RunWcf ();
			//System.Threading.Thread.Sleep (60*1000);
		}

		private static void RunSelf()
		{
			var coreHost = new CoreHost (BaseUri);
			coreHost.Start ();

			System.Console.WriteLine ("Nancy now listening - navigate to {0}", BaseUri);

			//coreHost.Stop ();
			coreHost.Join ();
		}

		// Ne fonctionne pas
		//private static void RunWcf()
		//{
		//    using (CreateAndOpenWebServiceHost ())
		//    {
		//        System.Console.WriteLine ("Service is now running on: {0}", BaseUri);
		//    }

		////    CreateAndOpenWebServiceHost ();
		////    System.Console.WriteLine ("Service is now running on: {0}", BaseUri);
		//}

		//private static WebServiceHost CreateAndOpenWebServiceHost()
		//{
		//    var host = new WebServiceHost (new NancyWcfGenericService (), BaseUri);

		//    host.AddServiceEndpoint (typeof (NancyWcfGenericService), new WebHttpBinding (), "");
		//    host.Open ();

		//    return host;
		//}

		private static readonly System.Uri BaseUri = new System.Uri ("http://localhost:12345/");
	}
}
