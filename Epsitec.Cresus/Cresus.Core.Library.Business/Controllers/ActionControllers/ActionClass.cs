//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Widgets;

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

			this.name        = name;
			this.actionClass = actionClass;
			this.color       = color;
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


		/// <summary>
		/// Gets the <see cref="ActionClass"/> instance for the specified action class.
		/// </summary>
		/// <param name="actionClass">The action class.</param>
		/// <returns>The <see cref="ActionClass"/> instance.</returns>
		public static ActionClass GetActionClass(ActionClasses actionClass)
		{
			return ActionClass.GetActionClass (actionClass.ToString (), actionClass, ActionClass.GetColor (actionClass));
		}

		/// <summary>
		/// Gets the <see cref="ActionClass"/> instance for the specified name, action class
		/// and color specification. If no color is specified, a default color will be provided.
		/// For a given name, this will always return the same instance, regardless of possible
		/// mismatches in the action class or color specifications.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="actionClass">The action class.</param>
		/// <param name="color">The color.</param>
		/// <returns>The <see cref="ActionClass"/> instance.</returns>
		private static ActionClass GetActionClass(string name, ActionClasses actionClass, Color color)
		{
			if (color.IsEmpty || color.IsTransparent)
			{
				color = ActionClass.GetColor (actionClass);
			}

			return ActionClass.GetUnique (name, actionClass, color);
		}

		public static Caption GetDefaultCaption(ActionClasses actionClass)
		{
			switch (actionClass)
			{
				case ActionClasses.Create:
					return Library.Res.Captions.ActionButton.Create;

				case ActionClasses.Delete:
					return Library.Res.Captions.ActionButton.Delete;

				default:
					return Library.Res.Captions.ActionButton.Undefined;
			}
		}

		
		private static Color GetColor(ActionClasses actionClass)
		{
			if (ActionButton.HasPastelColor)
			{
				switch (actionClass)
				{
					case ActionClasses.Create:
					case ActionClasses.NextStep:
						return Color.FromHexa ("e6ffdc");  // vert pâle

					case ActionClasses.Delete:
						return Color.FromHexa ("ffdcdc");  // rouge pâle

					case ActionClasses.Output:
					case ActionClasses.Input:
						return Color.FromHexa ("dcf7ff");  // bleu pâle

					case ActionClasses.Validate:
						return Color.FromHexa ("ffffdc");  // jaune pâle

					default:
						return Color.FromHexa ("ffffff");  // blanc
				}
			}
			else
			{
				switch (actionClass)
				{
					case ActionClasses.Create:
					case ActionClasses.NextStep:
						return Color.FromHexa ("53c15e");  // vert

					case ActionClasses.Delete:
						return Color.FromHexa ("e73333");  // rouge

					case ActionClasses.Output:
					case ActionClasses.Input:
						return Color.FromHexa ("388ebf");  // bleu

					case ActionClasses.Validate:
						return Color.FromHexa ("fff600");  // jaune

					default:
						return Color.FromHexa ("ffffff");  // blanc
				}
			}
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