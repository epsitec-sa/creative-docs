//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;
using Epsitec.Cresus.Assets.App.Helpers;

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

			this.CreateButton (this.OnPreviewAmortizations,   "Générer aperçu");
			this.CreateButton (this.OnFixAmortizations,       "Fixer l'aperçu");
			this.CreateButton (this.OnUnpreviewAmortizations, "Supprimer aperçu");
			this.CreateButton (this.OnDeleteAmortizations,    "Supprimer ordinaires");
			this.CreateButton (this.OnInfoAmortizations,      "i", width: 20);
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

		private Button CreateButton(System.Action<Widget> action, string text, string tooltip = null, int width = 120)
		{
			var button = new Button
			{
				Parent          = this.frameBox,
				Text            = text,
				ButtonStyle     = ButtonStyle.Icon,
				AutoFocus       = false,
				PreferredWidth  = width,
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
				action (button);
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


		#region Events handler
		private void OnPreviewAmortizations(Widget button)
		{
			this.PreviewAmortizations.Raise (this, button);
		}

		public event EventHandler<Widget> PreviewAmortizations;


		private void OnFixAmortizations(Widget button)
		{
			this.FixAmortizations.Raise (this, button);
		}

		public event EventHandler<Widget> FixAmortizations;


		private void OnUnpreviewAmortizations(Widget button)
		{
			this.UnpreviewAmortizations.Raise (this, button);
		}

		public event EventHandler<Widget> UnpreviewAmortizations;


		private void OnDeleteAmortizations(Widget button)
		{
			this.DeleteAmortizations.Raise (this, button);
		}

		public event EventHandler<Widget> DeleteAmortizations;


		private void OnInfoAmortizations(Widget button)
		{
			this.InfoAmortizations.Raise (this, button);
		}

		public event EventHandler<Widget> InfoAmortizations;
		#endregion


		private const int height = 17;

		private readonly DataAccessor			accessor;

		private FrameBox						frameBox;
		private DateFieldController				dateController;
	}
}
