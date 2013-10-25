//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Views;
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

		protected override void CreateUI()
		{
			var frame = this.CreateFullFrame ();

			this.CreateTitle (frame, "Simulation");

			this.CreateLine (frame, 2);
			this.CreateLine (frame, 1);
			this.CreateLine (frame, 0);
		}

		private void CreateTitle(FrameBox parent, string text)
		{
			new StaticText
			{
				Parent           = parent,
				Text             = text,
				ContentAlignment = Common.Drawing.ContentAlignment.MiddleCenter,
				Dock             = DockStyle.Top,
				PreferredHeight  = SimulationPopup.titleHeight,
				BackColor        = ColorManager.SelectionColor,
			};

			new FrameBox
			{
				Parent           = parent,
				Dock             = DockStyle.Top,
				PreferredHeight  = 4,
				BackColor        = ColorManager.SelectionColor,
			};
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
				IconUri       = AbstractCommandToolbar.GetResourceIconUri (rank == this.Simulation ? "Radio.Yes" : "Radio.No2"),
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
			switch (rank)
			{
				case 1:
					return Color.FromHexa ("fa9696");  // rouge

				case 2:
					return Color.FromHexa ("a1ed97");  // vert

				default:
					return ColorManager.EditSinglePropertyColor;
			}
		}


		private static readonly int margins      = 10;
		private static readonly int titleHeight  = 20;
		private static readonly int dialogWidth  = 135;
		private static readonly int dialogHeight = 114;
		private static readonly int lineHeight   = 30;
	}
}