using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Entities;

namespace Epsitec.Cresus.Core.Server
{
	/// <summary>
	/// Allow to create an ExtJS 4 panel by inferring the layout using
	/// AbstractEntities 
	/// </summary>
	class PanelBuilder
	{
		// TODO Maybe we don't want the session to be there
		// Only here for testing purposes
		public static CoreSession CoreSession
		{
			get;
			set;
		}

		private PanelBuilder(AbstractEntity entity, ViewControllerMode mode)
		{
			this.entity = entity;
			this.controllerMode = mode;
		}


		internal static void ExperimentalCode()
		{
			// Recreate an empty CSS file
			PanelBuilder.EnsureDirectoryStructureExists (PanelBuilder.cssFilename);
			System.IO.File.Create (PanelBuilder.cssFilename).Close ();

			BuildController (new MailContactEntity (), Controllers.ViewControllerMode.Summary);
			BuildController (new AffairEntity (), Controllers.ViewControllerMode.Summary);

			var context = PanelBuilder.CoreSession.GetBusinessContext ();

			var customer = (from x in context.GetAllEntities<CustomerEntity> ()
							where x.Relation.Person is NaturalPersonEntity
							let person = x.Relation.Person as NaturalPersonEntity
							where person.Lastname == "Arnaud"
							select x).FirstOrDefault ();

			BuildController (customer, Controllers.ViewControllerMode.Summary);
			BuildController (customer, Controllers.ViewControllerMode.Edition);
		}

		/// <summary>
		/// Create use a builder to create a panel
		/// </summary>
		/// <param name="entity">Entity to use to create the panelBuilder</param>
		/// <param name="mode">Controller mode</param>
		/// <returns></returns>
		public static IDictionary<string, object> BuildController(AbstractEntity entity, ViewControllerMode mode)
		{
			var builder = new PanelBuilder (entity, mode);
			return builder.Run ();
		}

		/// <summary>
		/// Creates a controller according to an entity and a ViewMode
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="mode"></param>
		/// <returns>Name of the generated panel</returns>
		private IDictionary<string, object> Run()
		{
			var name = PanelBuilder.GetControllerName (this.entity, this.controllerMode);

			// The panel already exists
			//if (PanelBuilder.constructedPanels.Contains (name))
			//{
			//    return name;
			//}

			var customerSummaryWall = CoreSession.GetBrickWall (this.entity, this.controllerMode);

			// Open the main panel
			var dic = new Dictionary<string, object> ();

			dic.Add ("title", name);

			var defferedItems = new List<object> ();
			dic.Add ("defferedItems", defferedItems);

			foreach (var brick in customerSummaryWall.Bricks)
			{
				var b = brick;

				//if (Brick.ContainsProperty (brick, BrickPropertyKey.AsType))
				//{
				//    b = Brick.GetProperty (brick, BrickPropertyKey.AsType).Brick;
				//}

				PanelBuilder.CreateDefaultTextProperties (b);

				defferedItems.Add (CreatePanelContent (b));
			}

			PanelBuilder.constructedPanels.Add (name);

			return dic;
		}

		/// <summary>
		/// Create "leaves" for a panel. 
		/// Uses the "Include" property from the Entity
		/// </summary>
		/// <param name="brick"></param>
		/// <returns></returns>
		private List<IDictionary<string, object>> CreateChildren(Brick brick)
		{
			if (!Brick.ContainsProperty (brick, BrickPropertyKey.Include))
			{
				return null;
			}

			var list = new List<IDictionary<string, object>> ();

			var includes = Brick.GetProperties (brick, BrickPropertyKey.Include);

			foreach (var include in includes)
			{
				var lambda = include.ExpressionValue as LambdaExpression;
				var func   = lambda.Compile ();
				var child = func.DynamicInvoke (this.entity) as AbstractEntity;

				if (child.IsNull ())
				{
					continue;
				}

				// Recursively build the panels
				var name = PanelBuilder.BuildController (child, this.controllerMode);
				list.Add (name);
			}

			return list;

		}

		/// <summary>
		/// Create a panel according to a brick
		/// </summary>
		/// <param name="brick">Brick to use</param>
		/// <returns>Javascript code to create the panel</returns>
		private IDictionary<string, object> CreatePanelContent(Brick brick)
		{

			var dic = new Dictionary<string, object> ();

			dic.Add ("name", "Epsitec.Cresus.Core.Static.WallPanel");
			var options = new Dictionary<string, object> ();
			dic.Add ("options", options);

			options.Add ("title", Brick.GetProperty (brick, BrickPropertyKey.Title).StringValue + "  6");

			var data = new Dictionary<string, object> ();
			options.Add("data", data);

			var brickType = brick.GetFieldType ();
			var resolver = brick.GetResolver (brickType);


			if (Brick.ContainsProperty (brick, BrickPropertyKey.CollectionAnnotation))
			{
				var obj = resolver.DynamicInvoke (this.entity);

				var col = (obj as IEnumerable).Cast<AbstractEntity> ();

				if (col != null && col.Count () > 0)
				{
					data.Add ("name", col.First ().ToString () + " * " + col.Count ());
				}
				else
				{
					data.Add ("name", "empty");
				}

				//var entities = obj as EntityCollectionProxy<AbstractContactEntity>;
				//if (entities == null)
				//{
				//    var entities2 = obj as EntityCollectionProxy<AffairEntity>;
				//    if (entities2 != null && entities2.Count > 0)
				//    {
				//        jscontent += entities2.First ().BusinessCodeVector;
				//    }
				//    else
				//    {
				//        jscontent += "empty";
				//    }
				//}
				//else
				//{
				//    if (entities != null && entities.Count > 0)
				//    {
				//        jscontent += entities.First ().NaturalPerson.DateOfBirth;
				//    }
				//    else
				//    {
				//        jscontent += "empty";
				//    }
				//}
			}
			else
			{
				var entity = resolver.DynamicInvoke (this.entity) as AbstractEntity;

				data.Add ("name", entity.IsNotNull () ? entity.GetEntitySerialId ().ToString () : "empty");
			}


			var icon = PanelBuilder.CreateIcon (brick);
			if (!icon.Equals (default (KeyValuePair<string, string>)))
			{
				var iconCls = PanelBuilder.CreateCssFromIcon (icon.Key, icon.Value);
				options.Add ("iconCls", iconCls);
			}


			//var children = CreateChildren (brick);
			//if (children != null && children.Count > 0)
			//{
			//    jscontent += "defferedItems: [";
			//    children.ForEach (c => jscontent += string.Format ("{{name: '{0}'}},", c));
			//    jscontent += "],";
			//}

			// TODO check
			//var children = CreateChildren (brick);
			//if (children != null && children.Count > 0)
			//{
			//    var ch = new Dictionary<string, object> ();
			//    children.ForEach (c => jscontent += string.Format ("{{name: '{0}'}},", c));
			//    options.Add ("defferedItems", ch);
			//}

			return dic;
		}


		/// <summary>
		/// Returns a name for a javascript controller 
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="mode"></param>
		/// <returns></returns>
		private static string GetControllerName(AbstractEntity entity, ViewControllerMode mode)
		{
			return string.Format ("Epsitec.Cresus.Core.Controllers.{0}Controllers.{1}", mode, entity.GetType ().Name);
		}

		/// <summary>
		/// Get the filename of a controller
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		private static string GetJsFilePath(string name)
		{
			var path = string.Format (PanelBuilder.jsFilename, name.Replace ('.', '/'));
			PanelBuilder.EnsureDirectoryStructureExists (path);
			return path;
		}

		/// <summary>
		/// Get the filename of an image
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		private static string GetImageFilePath(string name)
		{
			var path = string.Format (PanelBuilder.imagesFilename, name.Replace ('.', '/'));
			PanelBuilder.EnsureDirectoryStructureExists (path);
			return path;
		}

		/// <summary>
		/// checks if the folder exists, otherwise creates it
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
		/// Create an image using a brick
		/// </summary>
		/// <param name="brick">Brick to use</param>
		/// <returns>Key/value pair with the icon name and the filename</returns>
		private static KeyValuePair<string, string> CreateIcon(Brick brick)
		{
			// No icon for this brick
			if (!Brick.ContainsProperty (brick, BrickPropertyKey.Icon))
			{
				return default (KeyValuePair<string, string>);
			}

			// Get the ressources from the icon name
			var iconRes = Brick.GetProperty (brick, BrickPropertyKey.Icon).StringValue;
			var iconName = iconRes;
			if (iconName.StartsWith ("manifest:"))
			{
				iconName = iconName.Substring (9);
			}
			var icon = ImageProvider.Default.GetImage (iconRes, Resources.DefaultManager) as Canvas;

			if (icon == null)
			{
				return default (KeyValuePair<string, string>);
			}

			Canvas.IconKey key = new Canvas.IconKey ();

			key.Size.Width  = 32;
			key.Size.Height = 32;

			icon = icon.GetImageForIconKey (key) as Canvas;
			icon.DefineZoom (0.5);

			var bitmap = icon.BitmapImage;

			// Save the image
			var bytes = bitmap.Save (ImageFormat.Png);
			string path = PanelBuilder.GetImageFilePath (iconName);
			System.IO.File.WriteAllBytes (path, bytes);

			return new KeyValuePair<string, string> (iconName, path);
		}

		/// <summary>
		/// Create CSS content to be able to call an icon from the HTML code
		/// </summary>
		/// <param name="iconName">Name of the icon (ressource)</param>
		/// <param name="path">Icon filename</param>
		/// <returns>CSS classname</returns>
		private static string CreateCssFromIcon(string iconName, string path)
		{
			// CSS does not like "."
			var cssClass = iconName.Replace ('.', '-').ToLower ();
			var css = string.Format (".{0} {{ ", cssClass);
			css += string.Format ("background-image: url({0}) !important;", path.Replace ("web", "../.."));
			css += "background-size: 16px 16px;";
			css += "} ";

			System.IO.File.AppendAllText (PanelBuilder.cssFilename, css);

			return cssClass;
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

		// Generated files filenames
		private readonly static string cssFilename = "web/css/generated/style.css";
		private readonly static string jsFilename = "web/js/{0}.js";
		private readonly static string imagesFilename = "web/images/{0}.png";

		private readonly static List<string> constructedPanels = new List<string> ();

		private readonly AbstractEntity entity;
		private readonly ViewControllerMode controllerMode;

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
