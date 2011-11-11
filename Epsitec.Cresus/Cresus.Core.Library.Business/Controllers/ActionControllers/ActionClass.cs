//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.ActionControllers
{
	/// <summary>
	/// The <c>ActionClass</c> class specifies the information which is used to
	/// identify buttons with similar actions; this will allow to specify a common
	/// color to all buttons belonging to a given action class.
	/// </summary>
	public sealed class ActionClass
	{
		private ActionClass(string name, ActionClasses actionClass, Color color)
		{
			name.ThrowIfNullOrEmpty ("name");

			this.name  = name;
			this.actionClass = actionClass;
			this.color = color;
		}

		
		public string							Name
		{
			get
			{
				return this.name;
			}
		}

		public Color							Color
		{
			get
			{
				return this.color;
			}
		}

		public ActionClasses					Class
		{
			get
			{
				return this.actionClass;
			}
		}

		
		public static ActionClass GetActionClass(ActionClasses actionClass)
		{
			return ActionClass.GetActionClass (actionClass.ToString (), actionClass, ActionClass.GetColor (actionClass));
		}

		public static ActionClass GetActionClass(string name, ActionClasses actionClass, Color color)
		{
			if ((color.IsEmpty) ||
				(color.IsTransparent))
			{
				color = ActionClass.GetColor (actionClass);
			}

			return ActionClass.GetUnique (name, actionClass, color);
		}

		private static Color GetColor(ActionClasses actionClass)
		{
			Color color = Color.FromBrightness (0.4);
			
			switch (actionClass)
			{
				case ActionClasses.Create:
					//	TODO: assign colors
					break;

				default:
					break;
			}

			return color;
		}

		private static ActionClass GetUnique(string name, ActionClasses actionClass, Color color)
		{
			if (string.IsNullOrEmpty (name))
			{
				return null;
			}
			
			lock (ActionClass.cache)
			{
				ActionClass item;

				if (ActionClass.cache.TryGetValue (name, out item) == false)
				{
					item = new ActionClass (name, actionClass, color);
					ActionClass.cache[name] = item;
				}

				return item;
			}
		}


		private readonly static Dictionary<string, ActionClass> cache = new Dictionary<string, ActionClass> ();
		
		private readonly string					name;
		private readonly Color					color;
		private readonly ActionClasses			actionClass;
	}
}