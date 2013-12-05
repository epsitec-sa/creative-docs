//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Widgets;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class SimulationPopup : AbstractPopup
	{
		public SimulationPopup()
		{
		}


		public int								Simulation;

		protected override Size					DialogSize
		{
			get
			{
				int dx = SimulationPopup.dialogWidth;
				int dy = SimulationPopup.dialogHeight;

				return new Size (dx, dy);
			}
		}

		public override void CreateUI()
		{
			this.CreateTitle ("Simulation");

			this.CreateLine (this.mainFrameBox, 2);
			this.CreateLine (this.mainFrameBox, 1);
			this.CreateLine (this.mainFrameBox, 0);
		}

		private void CreateLine(FrameBox parent, int rank)
		{
			var line = new FrameBox
			{
				Parent           = parent,
				Dock             = DockStyle.Bottom,
				PreferredHeight  = SimulationPopup.lineHeight,
				Padding          = new Margins (SimulationPopup.margins, 0, 5, 5),
				BackColor        = SimulationPopup.GetSimulationColor (rank),
			};

			int size = SimulationPopup.lineHeight - 5 - 5;

			//	Ne fonctionne pas avec "Radio.No" !
			var useButton = new IconButton
			{
				Parent        = line,
				Dock          = DockStyle.Left,
				PreferredSize = new Size (size, size),
				IconUri       = Misc.GetResourceIconUri (rank == this.Simulation ? "Radio.Yes" : "Radio.No2"),
			};

			useButton.Clicked += delegate
			{
				this.Simulation = rank;
				this.OnButtonClicked ("use-"+rank.ToString (System.Globalization.CultureInfo.InvariantCulture));
				this.ClosePopup ();
			};

			if (rank > 0)
			{
				var clearButton = new Button
				{
					Parent        = line,
					Text          = "Effacer",
					ButtonStyle   = ButtonStyle.Icon,
					Dock          = DockStyle.Left,
					PreferredSize = new Size (80, size),
					Margins       = new Margins (10, 0, 0, 0),
				};

				clearButton.Clicked += delegate
				{
					this.OnButtonClicked ("clear-"+rank.ToString (System.Globalization.CultureInfo.InvariantCulture));
					this.ClosePopup ();
				};
			}
		}

		private static Color GetSimulationColor(int rank)
		{
			return ColorManager.GetEditSinglePropertyColor (rank);
		}


		private const int margins      = 10;
		private const int dialogWidth  = 135;
		private const int dialogHeight = 114;
		private const int lineHeight   = 30;
	}
}