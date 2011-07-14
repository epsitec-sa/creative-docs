//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Bricks;
using System.Linq.Expressions;
using Epsitec.Common.Types;
using System.Collections.Generic;
using System.Linq;

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
			//BuildControllers (new Epsitec.Cresus.Core.Entities.CustomerEntity (), Controllers.ViewControllerMode.Edition);
			//BuildControllers (new Epsitec.Cresus.Core.Entities.MailContactEntity (), Controllers.ViewControllerMode.Summary);
			//BuildControllers (new Epsitec.Cresus.Core.Entities.AffairEntity (), Controllers.ViewControllerMode.Summary);
		}

		private static void BuildControllers(AbstractEntity entity, ViewControllerMode mode)
		{
			var customerSummaryWall = CoreSession.GetBrickWall (entity, mode);
			var name = GetControllerName (entity, mode);

			var filename = "web/" + name.Replace ('.', '/') + ".js";

			var jscontent = "Ext.define('";
			jscontent += name;
			jscontent += "', {";
			jscontent += "extend: 'Ext.Panel',";
			jscontent += "title: '" + name + "',";
			jscontent += "flex: 1,";
			jscontent += "layout: {";
			jscontent += "type: 'vbox',";
			jscontent += "align: 'stretch'";
			jscontent += "},";
			jscontent += "margin: 3,";
			jscontent += "defaults: {";
			jscontent += "border: false";
			jscontent += "},";
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

				jscontent += "{";
				jscontent += "xtype: 'panel',";
				jscontent += "title: '";
				jscontent += Brick.GetProperty (brick, BrickPropertyKey.Title).StringValue;
				jscontent += "',";
				jscontent += "html: [";

				jscontent += "'";
				jscontent += Brick.GetProperty (brick, BrickPropertyKey.Text).StringValue;
				jscontent += "<br/><b>plop</b>',";

				jscontent += "],";
				jscontent += "flex: 1,";
				jscontent += "},";
			}


			jscontent += "]";
			jscontent += "});";
			System.IO.File.WriteAllText (filename, jscontent);

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
	}
}
