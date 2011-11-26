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
			: this (null, ActionClass.GetActionClass (actionClass), action, caption.DefaultLabel, caption.Description, weight)
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


		public void ExecuteAction()
		{
			if (this.action != null)
			{
				this.action ();
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
