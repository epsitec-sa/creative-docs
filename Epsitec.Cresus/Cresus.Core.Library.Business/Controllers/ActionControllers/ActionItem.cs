//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.ActionControllers
{
	public sealed class ActionItem
	{
		public ActionItem(ActionClasses actionClass, System.Action action, double weight = 0.0)
			: this (actionClass, action, ActionClass.GetDefaultCaption (actionClass), weight)
		{
		}

		public ActionItem(ActionClasses actionClass, System.Action action, Druid captionId, double weight = 0.0)
			: this (actionClass, action, TextFormatter.GetCurrentCultureCaption (captionId), weight)
		{
		}

		public ActionItem(ActionClasses actionClass, System.Action action, Caption caption, double weight = 0.0)
			: this (null, ActionClass.GetActionClass (actionClass), action, ActionItem.GetLabel (caption), ActionItem.GetDescription (caption), weight)
		{
		}

		public ActionItem(ActionClasses actionClass, System.Action action, FormattedText label, FormattedText description = default (FormattedText), double weight = 0.0)
			: this (null, ActionClass.GetActionClass (actionClass), action, label, description, weight)
		{
		}

		public ActionItem(ActionClass actionClass, System.Action action, FormattedText label, FormattedText description = default (FormattedText), double weight = 0.0)
			: this (null, actionClass, action, label, description, weight)
		{
		}

		public ActionItem(string name, ActionClass actionClass, System.Action action, FormattedText label, FormattedText description = default (FormattedText), double weight = 0.0)
		{
			if (ActionItem.IsIcon (label) && description.IsNullOrEmpty)
			{
				var caption = ActionClass.GetDefaultCaption (actionClass.Class);

				if (string.IsNullOrEmpty (caption.Description))
				{
					description = caption.DefaultLabel;
				}
				else
				{
					description = caption.Description;
				}
			}

			this.name        = name;
			this.actionClass = actionClass;
			this.action      = action;
			this.label       = label;
			this.description = description;
			this.weight      = weight;
		}


		public string							Name
		{
			get
			{
				return this.name;
			}
		}

		public ActionClass						ActionClass
		{
			get
			{
				return this.actionClass;
			}
		}

		public System.Action					Action
		{
			get
			{
				return this.action;
			}
		}

		public FormattedText					Label
		{
			get
			{
				return this.label;
			}
		}

		public FormattedText					Description
		{
			get
			{
				return this.description;
			}
		}

		public double							Weight
		{
			get
			{
				return this.weight;
			}
		}

		public bool								ContainsIcon
		{
			get
			{
				return ActionItem.IsIcon (this.label);
			}
		}


		public void ExecuteAction()
		{
			if (this.action != null)
			{
				this.action ();
			}
		}


		public static FormattedText GetIcon(string icon)
		{
			return string.Format ("<img src=\"manifest:Epsitec.Cresus.Core.Images.{0}.icon\"/>", icon);
		}

		public static bool IsIcon(FormattedText text)
		{
			if (text.IsNullOrEmpty)
			{
				return false;
			}
			else
			{
				return text.ToString ().StartsWith ("<img src=\"manifest:");
			}
		}


		private static FormattedText GetLabel(Caption caption)
		{
			if (string.IsNullOrEmpty (caption.Icon))
			{
				return caption.DefaultLabel;
			}
			else
			{
				return string.Format ("<img src=\"{0}\"/>", caption.Icon);
			}
		}

		private static FormattedText GetDescription(Caption caption)
		{
			if (string.IsNullOrEmpty (caption.Description))
			{
				return caption.DefaultLabel;
			}
			else
			{
				return caption.Description;
			}
		}

		
		private readonly string					name;
		private readonly ActionClass			actionClass;
		private readonly System.Action			action;
		private readonly FormattedText			label;
		private readonly FormattedText			description;
		private readonly double					weight;
	}
}
