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

//-			this.ExperimentalProfiling ();
			this.ExperimentalCode ();

			var server = new CoreServer ();
			var session = server.CreateSession ();

			this.ExperimentalEntityManipulations (session);

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

//			context.Discard ();
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

		private void ExperimentalCode()
		{
			BuildControllers (new CustomerEntity (), Controllers.ViewControllerMode.Summary);
			BuildControllers (new CustomerEntity (), Controllers.ViewControllerMode.Edition);
			BuildControllers (new MailContactEntity (), Controllers.ViewControllerMode.Summary);
			BuildControllers (new AffairEntity (), Controllers.ViewControllerMode.Summary);
		}

		private static void BuildControllers(AbstractEntity entity, ViewControllerMode mode)
		{
			var customerSummaryWall = CoreSession.GetBrickWall (entity, mode);
			
			var name = CoreServerProgram.GetControllerName (entity, mode);
			var path = CoreServerProgram.GetJsFilePath (name);

			var jscontent = "Ext.define('";
			jscontent += name;
			jscontent += "', {";
			jscontent += "extend: 'Epsitec.Cresus.Core.Static.SummaryPanel',";
			jscontent += string.Format ("title: '{0}',", name);
			jscontent += "items: [";

			// Do something with the bricks

			foreach (var brick in customerSummaryWall.Bricks)
			{
				//// Contient AsType, on inclu le panel pour ce propre type
				//if (Brick.ContainsProperty (brick, BrickPropertyKey.AsType))
				//{
				//}

				//if (Brick.ContainsProperty (brick, BrickPropertyKey.AsType))
				//{
				//    var t = Brick.GetProperty (brick, BrickPropertyKey.AsType);
				//    var b = t.Brick;
				//    var a = b.GetType ().GetGenericArguments ();
				//    CreateDefaultProperties (brick, a.First ());
				//}
				//else
				//{
				//    CreateDefaultProperties (brick, entity.GetType ());
				//}

				CreateDefaultTextProperties (brick);

				jscontent += CreatePanelContent (brick);
			}


			jscontent += "]";
			jscontent += "});";
			System.IO.File.WriteAllText (path, jscontent);

		}

		private static string GetJsFilePath(string name)
		{
			var path = string.Format ("web/js/{0}.js", name.Replace ('.', '/'));
			CoreServerProgram.EnsureDirectoryStructureExists (path);
			return path;
		}

		private static string GetImageFilePath(string name)
		{
			var path = string.Format ("web/images/{0}.png", name.Replace ('.', '/'));
			CoreServerProgram.EnsureDirectoryStructureExists (path);
			return path;
		}

		private static void EnsureDirectoryStructureExists(string path)
		{
			var dir  = System.IO.Path.GetDirectoryName (path);

			if (System.IO.Directory.Exists (dir) == false)
			{
				System.IO.Directory.CreateDirectory (dir);
			}
		}
		private static string CreateIcon(Brick brick)
		{
			if (!Brick.ContainsProperty (brick, BrickPropertyKey.Icon))
			{
				return null;
			}

			var iconRes = Brick.GetProperty (brick, BrickPropertyKey.Icon).StringValue;
			var iconName = iconRes;

			if (iconName.StartsWith ("manifest:"))
			{
				iconName = iconName.Substring (9);
			}

			var icon = ImageProvider.Default.GetImage (iconRes, Resources.DefaultManager);

			if (icon == null)
			{
				return null;
			}

			var bitmap = icon.BitmapImage;

			var bytes = bitmap.Save (ImageFormat.Png);
			string path = CoreServerProgram.GetImageFilePath (iconName);
			System.IO.File.WriteAllBytes (path, bytes);

			return path;
		}


		private static object CreatePanelContent(Brick brick)
		{
			var jscontent = "Ext.create('Epsitec.Cresus.Core.Static.TilePanel', {";

			jscontent += "title: '";
			jscontent += Brick.GetProperty (brick, BrickPropertyKey.Title).StringValue;
			jscontent += "',";

			jscontent += "data: { name: 'Jonas' },";


			//jscontent += "html: [";

			//jscontent += "'";
			//jscontent += Brick.GetProperty (brick, BrickPropertyKey.Text).StringValue;
			//jscontent += "<br/><b>plop</b>',";

			//jscontent += "],";

			var icon = CreateIcon (brick);
			if (icon != null)
			{
				jscontent += string.Format ("icon: '{0}',", icon);
			}


			jscontent += "}),";

			return jscontent;
		}

		private static string GetControllerName(AbstractEntity entity, ViewControllerMode mode)
		{
			return string.Format ("Epsitec.Cresus.Core.Controllers.{0}Controllers.{1}", mode, entity.GetType ().Name);
		}

		private static void CreateDefaultTextProperties(Brick brick)
		{
			if (!Brick.ContainsProperty (brick, BrickPropertyKey.Text))
			{
				Expression<System.Func<AbstractEntity, FormattedText>> expression = x => x.GetSummary ();
				Brick.AddProperty (brick, new BrickProperty (BrickPropertyKey.Text, expression));
			}

			if (!Brick.ContainsProperty (brick, BrickPropertyKey.TextCompact))
			{
				Expression<System.Func<AbstractEntity, FormattedText>> expression = x => x.GetCompactSummary ();
				Brick.AddProperty (brick, new BrickProperty (BrickPropertyKey.TextCompact, expression));
			}
		}

		/*
		public enum BrickPropertyKey
		{
			Name,
			Icon,
			Title,
			TitleCompact,
			Text,
			TextCompact,

			Attribute,

			Template,
			AsType,

			Input,
			Field,
			Width,
			Height,
			Separator,
			HorizontalGroup,
			FromCollection,
			SpecialController,
			GlobalWarning,

			CollectionAnnotation,
			Include,
		}
		 */
	}
}
