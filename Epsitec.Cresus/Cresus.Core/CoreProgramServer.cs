//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Bricks;
using System.Linq.Expressions;
using Epsitec.Common.Types;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection;
using Epsitec.Common.Drawing;

namespace Epsitec.Cresus.Core
{
	internal static class CoreProgramServer
	{
		public static void ExecuteServer()
		{
			//	Pour Jonas : obtenir un "brick wall" à partir d'une entité, même vide :

			//var customerSummaryWall = CoreSession.GetBrickWall (new Epsitec.Cresus.Core.Entities.CustomerEntity (), Controllers.ViewControllerMode.Summary);

			//	...et pour mesurer le temps pris, une fois que tout est "chaud" :

			//var watch = new System.Diagnostics.Stopwatch ();

			//for (int i = 0; i < 10; i++)
			//{
			//    watch.Restart ();
			//    CoreSession.GetBrickWall (new Epsitec.Cresus.Core.Entities.CustomerEntity (), Controllers.ViewControllerMode.Edition);
			//    watch.Stop ();

			//    System.Diagnostics.Debug.WriteLine (string.Format ("Attempt {0}: fetching EditionController took {1} μs", i+1, 1000L*1000L * watch.ElapsedTicks / System.Diagnostics.Stopwatch.Frequency));
			//}


			//			CoreSession session = new CoreSession ();

			BuildControllers (new Epsitec.Cresus.Core.Entities.CustomerEntity (), Controllers.ViewControllerMode.Summary);
			BuildControllers (new Epsitec.Cresus.Core.Entities.CustomerEntity (), Controllers.ViewControllerMode.Edition);
			BuildControllers (new Epsitec.Cresus.Core.Entities.MailContactEntity (), Controllers.ViewControllerMode.Summary);
			BuildControllers (new Epsitec.Cresus.Core.Entities.AffairEntity (), Controllers.ViewControllerMode.Summary);
		}

		private static void BuildControllers(AbstractEntity entity, ViewControllerMode mode)
		{
			var customerSummaryWall = CoreSession.GetBrickWall (entity, mode);
			var name = GetControllerName (entity, mode);

			var filename = string.Format ("web/js/{0}.js", name.Replace ('.', '/'));

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
			System.IO.File.WriteAllText (filename, jscontent);

		}

		private static string CreateIcon(Brick brick)
		{
			if (!Brick.ContainsProperty (brick, BrickPropertyKey.Icon))
			{
				return null;
			}

			var iconRes = Brick.GetProperty (brick, BrickPropertyKey.Icon).StringValue;

			if (iconRes.IndexOf ("manifest:") == 0)
			{
				iconRes = iconRes.Substring (9);
			}

			Epsitec.Common.Document.Engine.Initialize ();
			var icon = Bitmap.FromManifestResource (iconRes, Assembly.GetExecutingAssembly ());

			if (icon == null)
			{
				return null;
			}

			var bitmap = icon.BitmapImage;

			var bytes = bitmap.Save (ImageFormat.Png);
			string filename = string.Format ("web/images/{0}.png", iconRes.Replace ('.', '/'));
			System.IO.File.WriteAllBytes (filename, bytes);

			return filename;
		}


		private static object CreatePanelContent(Brick brick)
		{
			var jscontent = "Ext.create('Epsitec.Cresus.Core.Static.TilePanel', {";

			jscontent += "title: '";
			jscontent += Brick.GetProperty (brick, BrickPropertyKey.Title).StringValue;
			jscontent += "',";
			jscontent += "html: [";

			jscontent += "'";
			jscontent += Brick.GetProperty (brick, BrickPropertyKey.Text).StringValue;
			jscontent += "<br/><b>plop</b>',";

			jscontent += "],";

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
