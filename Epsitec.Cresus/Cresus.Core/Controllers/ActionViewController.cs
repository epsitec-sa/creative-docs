//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Layouts;

using Epsitec.Cresus.Core.Orchestrators;
using Epsitec.Cresus.Core.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	public class ActionViewController : CoreViewController
	{
		public ActionViewController(string name, CoreData data)
			: base (name)
		{
			this.data = data;
		}

		
		public override IEnumerable<CoreController> GetSubControllers()
		{
			yield break;
		}

		public override void CreateUI(Widget container)
		{
			this.buttonFrame = new FrameBox
			{
				Parent = container,
				Dock = DockStyle.Fill,
				MinWidth = 200,
			};

			this.stateMachineFrame = new FrameBox
			{
				Parent = container,
				Dock = DockStyle.Right,
				PreferredWidth = 40,
			};
		}


		public void AddButton(string id, string title, string description, System.Action callback)
		{
			var button = new ConfirmationButton
			{
				Parent = this.buttonFrame,
				Name = id,
				Dock = DockStyle.Top,
				Text = ConfirmationButton.FormatContent (title, description),
				PreferredHeight = 52,
			};

			button.Clicked += (sender, e) => callback ();
		}

		public void RemoveButton(string id)
		{
			var widget = this.buttonFrame.FindChild (id);

			if (widget != null)
			{
				widget.Parent = null;
			}
		}


		private readonly CoreData data;

		private FrameBox buttonFrame;
		private FrameBox stateMachineFrame;
	}
}
