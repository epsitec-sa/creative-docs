//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Orchestrators;
using Epsitec.Cresus.Core.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	public class ActionViewController : CoreViewController
	{
		public ActionViewController(DataViewOrchestrator orchestrator)
			: base ("Action", orchestrator)
		{
			this.data = this.Orchestrator.Data;
		}

		
		public override IEnumerable<CoreController> GetSubControllers()
		{
			yield break;
		}

		public override void CreateUI(Widget container)
		{
			base.CreateUI (container);
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

		public void AddButton(string id, FormattedText title, FormattedText description, System.Action callback)
		{
			var button = new ConfirmationButton
			{
				Parent = this.buttonFrame,
				Name = id,
				Dock = DockStyle.Top,
				FormattedText = ConfirmationButton.FormatContent (title, description),
				PreferredHeight = 52,
			};

			button.Clicked += (sender, e) => callback ();
		}

		public void RemoveButton(string id)
		{
			var button = this.buttonFrame.FindChild (id);

			if (button != null)
			{
				button.Dispose ();
			}
		}

		public void ClearButtons()
		{
			var buttons = this.buttonFrame.Children.OfType<ConfirmationButton> ().ToArray ();

			buttons.ForEach (button => button.Dispose ());
		}


		private readonly CoreData data;

		private FrameBox buttonFrame;
		private FrameBox stateMachineFrame;
	}
}