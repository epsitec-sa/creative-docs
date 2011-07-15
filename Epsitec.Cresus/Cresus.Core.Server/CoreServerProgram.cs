//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Debugging;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Epsitec.Common.Support;

namespace Epsitec.Cresus.Core.Server
{
	public sealed class CoreServerProgram
	{
		public CoreServerProgram()
		{
			Epsitec.Common.Document.Engine.Initialize ();

			var server = new CoreServer ();
			var session = server.CreateSession ();

			PanelBuilder.Session = session;

			//this.ExperimentalProfiling ();
			PanelBuilder.ExperimentalCode ();

			this.ExperimentalJSON (session);

			//this.ExperimentalEntityManipulations (session);

			session.DisposeBusinessContext ();
			server.DeleteSession (session.Id);


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
			var server = new CoreServer ();

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

		private void ExperimentalJSON(CoreSession session)
		{
			var context = session.GetBusinessContext ();

			var customer = (from x in context.GetAllEntities<CustomerEntity> ()
							where x.Relation.Person is NaturalPersonEntity
							let person = x.Relation.Person as NaturalPersonEntity
							where person.Lastname == "Arnaud"
							select x).FirstOrDefault ();

			var writer = new JsonFx.Json.JsonWriter ();

			// Stackoverflow
			//var json = writer.Write (customer);

			var p = customer.Relation.Person as NaturalPersonEntity;

			var obj = new
			{
				name = p.Firstname
			};

			var json = writer.Write (obj);

			System.IO.File.WriteAllText ("web/data/person.json", json);

		}
	}
}
