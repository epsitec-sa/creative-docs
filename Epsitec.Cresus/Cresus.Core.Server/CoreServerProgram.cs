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

			//this.ExperimentalProfiling ();
			CoreServerProgram.ExperimentalCode ();

			var server = new CoreServer ();
			var session = server.CreateSession ();

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

		private static void ExperimentalCode()
		{
			// Recrée un fichier CSS vide
			CoreServerProgram.EnsureDirectoryStructureExists (CoreServerProgram.cssFilename);
			System.IO.File.Create (CoreServerProgram.cssFilename).Close ();

			BuildController (new CustomerEntity (), Controllers.ViewControllerMode.Summary);
			BuildController (new CustomerEntity (), Controllers.ViewControllerMode.Edition);
			BuildController (new MailContactEntity (), Controllers.ViewControllerMode.Summary);
			BuildController (new AffairEntity (), Controllers.ViewControllerMode.Summary);
		}

		/// <summary>
		/// Crée un controlleur en fonction d'une entité et d'une mode de la vue
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="mode"></param>
		private static void BuildController(AbstractEntity entity, ViewControllerMode mode)
		{
			// Récupération du brickwall
			var customerSummaryWall = CoreSession.GetBrickWall (entity, mode);
			
			var name = CoreServerProgram.GetControllerName (entity, mode);
			var path = CoreServerProgram.GetJsFilePath (name);

			// Ouverture du panel principal
			var jscontent = "Ext.define('";
			jscontent += name;
			jscontent += "', {";
			jscontent += "extend: 'Epsitec.Cresus.Core.Static.SummaryPanel',";
			jscontent += string.Format ("title: '{0}',", name);
			jscontent += "items: [";

			foreach (var brick in customerSummaryWall.Bricks)
			{
				//// Contient AsType, on inclu le panel pour ce propre type
				//if (Brick.ContainsProperty (brick, BrickPropertyKey.AsType))
				//{
				//}

				// Propriétés par défaut
				CoreServerProgram.CreateDefaultTextProperties (brick);

				// Création du panel pour cette tuile
				jscontent += CoreServerProgram.CreatePanelContent (brick);
			}

			// Fermeture du panel principal et écriture dans le fichier
			jscontent += "]";
			jscontent += "});";
			System.IO.File.WriteAllText (path, jscontent);

		}



		/// <summary>
		/// Retourne le nom d'un controlleur javascript en fonction d'une entité et du mode de la vue
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="mode"></param>
		/// <returns></returns>
		private static string GetControllerName(AbstractEntity entity, ViewControllerMode mode)
		{
			return string.Format ("Epsitec.Cresus.Core.Controllers.{0}Controllers.{1}", mode, entity.GetType ().Name);
		}

		/// <summary>
		/// Obtient le nom de fichier javascript à partir d'un namespace/classname
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		private static string GetJsFilePath(string name)
		{
			var path = string.Format (CoreServerProgram.jsFilename, name.Replace ('.', '/'));
			CoreServerProgram.EnsureDirectoryStructureExists (path);
			return path;
		}

		/// <summary>
		/// Obtient le nom de fichier image à partir d'un namespace/classname
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		private static string GetImageFilePath(string name)
		{
			var path = string.Format (CoreServerProgram.imagesFilename, name.Replace ('.', '/'));
			CoreServerProgram.EnsureDirectoryStructureExists (path);
			return path;
		}

		/// <summary>
		/// Vérifie qu'un dossier existe, sinon le crée
		/// </summary>
		/// <param name="path"></param>
		private static void EnsureDirectoryStructureExists(string path)
		{
			var dir  = System.IO.Path.GetDirectoryName (path);

			if (System.IO.Directory.Exists (dir) == false)
			{
				System.IO.Directory.CreateDirectory (dir);
			}
		}

		/// <summary>
		/// Crée une image à partir d'une brique.
		/// Cette image est l'icône représentant la brique.
		/// </summary>
		/// <param name="brick">Brick à utiliser pour créer une icône</param>
		/// <returns>Pair clé/valeur contenant respectivement le nom de l'icône et le nom de fichier créé</returns>
		private static KeyValuePair<string, string> CreateIcon(Brick brick)
		{
			// Pas d'icone pour cette brique
			if (!Brick.ContainsProperty (brick, BrickPropertyKey.Icon))
			{
				return default (KeyValuePair<string, string>);
			}

			// Récupération du nom de ressource de l'icone
			var iconRes = Brick.GetProperty (brick, BrickPropertyKey.Icon).StringValue;
			var iconName = iconRes;
			if (iconName.StartsWith ("manifest:"))
			{
				iconName = iconName.Substring (9);
			}

			// Récupération de l'image depuis les ressources
			var icon = ImageProvider.Default.GetImage (iconRes, Resources.DefaultManager);
			if (icon == null)
			{
				return default (KeyValuePair<string, string>);
			}
			var bitmap = icon.BitmapImage;

			// Sauvegarde de l'image
			var bytes = bitmap.Save (ImageFormat.Png);
			string path = CoreServerProgram.GetImageFilePath (iconName);
			System.IO.File.WriteAllBytes (path, bytes);

			return new KeyValuePair<string, string> (iconName, path);
		}

		/// <summary>
		/// Génère du contenu CSS pour être capable d'appeler une icone depuis le HTML
		/// </summary>
		/// <param name="iconName">Nom de l'icone (ressource)</param>
		/// <param name="path">Nom du fichier dans lequel est enregistrée l'icone</param>
		/// <returns>Nom de la classe CSS permettant d'utiliser l'icone</returns>
		private static string CreateCssFromIcon(string iconName, string path)
		{
			// CSS n'aime pas les "."
			var cssClass = iconName.Replace ('.', '-').ToLower ();
			var css = string.Format (".{0} {{ ", cssClass);
			css += string.Format ("background-image: url({0}) !important;", path.Replace ("web", "../.."));
			css += "background-size: 16px 16px;";
			css += "} ";

			System.IO.File.AppendAllText (CoreServerProgram.cssFilename, css);

			return cssClass;
		}

		/// <summary>
		/// Crée un panel en fonction d'une brique
		/// </summary>
		/// <param name="brick">Brique à utiliser</param>
		/// <returns>Contenu javascript permettant de créer le panel</returns>
		private static string CreatePanelContent(Brick brick)
		{
			var jscontent = "Ext.create('Epsitec.Cresus.Core.Static.TilePanel', {";

			jscontent += "title: '";
			jscontent += Brick.GetProperty (brick, BrickPropertyKey.Title).StringValue;
			jscontent += "',";

			// Données de test
			jscontent += "data: { name: 'Jonas' },";

			// Création d'une icone si disponible
			var icon = CoreServerProgram.CreateIcon (brick);
			if (!icon.Equals (default (KeyValuePair<string, string>)))
			{
				var iconCls = CoreServerProgram.CreateCssFromIcon (icon.Key, icon.Value);
				jscontent += string.Format ("iconCls: '{0}',", iconCls);
			}

			// Fermeture du panel
			jscontent += "}),";

			return jscontent;
		}

		/// <summary>
		/// Met des propriétés par défaut dans la brique
		/// </summary>
		/// <param name="brick"></param>
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

		// Nom des fichiers générés
		private static readonly string cssFilename = "web/css/generated/style.css";
		private static readonly string jsFilename = "web/js/{0}.js";
		private static readonly string imagesFilename = "web/images/{0}.png";

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
