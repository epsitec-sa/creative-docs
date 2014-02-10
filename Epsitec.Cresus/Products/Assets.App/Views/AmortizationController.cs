//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class AmortizationController
	{
		public AmortizationController(DataAccessor accessor)
		{
			this.accessor = accessor;
		}


		public System.DateTime?					Date;


		public void CreateUI(Widget parent)
		{
			parent.PreferredHeight = AmortizationController.height;

			this.frameBox = new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Top,
				PreferredHeight = AmortizationController.height,
				Margins         = new Margins (10),
			};

			var x = this.CreateFrame (200);
			this.CreateDate (x);

			this.CreateButton ("Preview", "Aperçu");
			this.CreateButton ("Create", "Générer");
			this.CreateButton ("Delete", "Supprimer");
		}

		private void CreateDate(Widget parent)
		{
			new StaticText
			{
				Parent           = parent,
				Text             = "Jusqu'au",
				ContentAlignment = ContentAlignment.MiddleRight,
				PreferredWidth   = 80,
				PreferredHeight  = AmortizationController.height,
				Margins          = new Margins (0, 10, 0, 1),
				Dock             = DockStyle.Left,
			};

			var frame = new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Fill,
				PreferredHeight = AmortizationController.height,
				BackColor       = ColorManager.WindowBackgroundColor,
			};

			this.dateController = new DateFieldController
			{
				Label      = null,
				LabelWidth = 0,
				Value      = this.Date,
			};

			this.dateController.HideAdditionalButtons = true;
			this.dateController.CreateUI (frame);

			this.dateController.ValueEdited += delegate
			{
				this.Date = this.dateController.Value;
			};
		}

		private Button CreateButton(string name, string text, string tooltip = null)
		{
			var button = new Button
			{
				Parent          = this.frameBox,
				Name            = name,
				Text            = text,
				ButtonStyle     = ButtonStyle.Icon,
				AutoFocus       = false,
				PreferredWidth  = 100,
				PreferredHeight = AmortizationController.height,
				Dock            = DockStyle.Left,
				Margins         = new Margins (0, 10, 0, 0),
			};

			if (!string.IsNullOrEmpty (tooltip))
			{
				ToolTip.Default.SetToolTip (button, tooltip);
			}

			button.Clicked += delegate
			{
			};

			return button;
		}

		private FrameBox CreateFrame(int dx)
		{
			var frame = new FrameBox
			{
				Parent        = this.frameBox,
				Dock          = DockStyle.Left,
				PreferredSize = new Size (dx, AmortizationController.height),
				Margins       = new Margins (0, 10, 0, 0),
			};

			return frame;
		}


		private const int height = 17;

		private readonly DataAccessor			accessor;

		private FrameBox						frameBox;
		private DateFieldController				dateController;
	}
}
