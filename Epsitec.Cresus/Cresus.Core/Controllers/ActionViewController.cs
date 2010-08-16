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

			var button1 = new ConfirmationButton
			{
				Parent = this.buttonFrame,
				Dock = DockStyle.Top,
				Text = ConfirmationButton.FormatContent ("Nouvelle offre", "Crée une offre vide"),
				PreferredHeight = 52,
			};

			var button2 = new ConfirmationButton
			{
				Parent = this.buttonFrame,
				Dock = DockStyle.Top,
				Text = ConfirmationButton.FormatContent ("Variante de l'offre", "Copie l'offre sélectionnée pour en créer une variante"),
				PreferredHeight = 52,
			};

			var button3 = new ConfirmationButton
			{
				Parent = this.buttonFrame,
				Dock = DockStyle.Top,
				Text = ConfirmationButton.FormatContent ("Imprime l'offre", "Imprime l'offre sélectionnée"),
				PreferredHeight = 52,
			};
		}


		private readonly CoreData data;

		private FrameBox buttonFrame;
		private FrameBox stateMachineFrame;
	}
}
