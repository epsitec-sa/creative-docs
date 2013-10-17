//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Cresus.Assets.Server.NaiveEngine;
using Epsitec.Cresus.Assets.App.Widgets;

namespace Epsitec.Cresus.Assets.App.Views
{
	public abstract class AbstractFieldController
	{
		public int								TabIndex;
		public int								LabelWidth = 100;
		public int								EditWidth = 380;
		public PropertyState					PropertyState;
		public bool								HideAdditionalButtons;

		public string							Label
		{
			get
			{
				return this.label;
			}
			set
			{
				this.label = value;
			}
		}


		public virtual void CreateUI(Widget parent)
		{
			this.frameBox = new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Top,
				PreferredHeight = AbstractFieldController.lineHeight,
				Margins         = new Margins (0, 0, 0, 0),
				Padding         = new Margins (2),
				BackColor       = this.BackgroundColor,
			};

			this.CreateLabel ();

			if (!this.HideAdditionalButtons)
			{
				this.CreateClearButton ();
				this.CreateHistoryButton ();
			}
		}

		public virtual void SetFocus()
		{
		}

		private void CreateLabel()
		{
			if (this.LabelWidth != 0)
			{
				new StaticText
				{
					Parent           = this.frameBox,
					Text             = this.label,
					ContentAlignment = ContentAlignment.TopRight,
					TextBreakMode    = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
					Dock             = DockStyle.Left,
					PreferredWidth   = this.LabelWidth,
					PreferredHeight  = AbstractFieldController.lineHeight - 1,
					Margins          = new Margins (0, 10, 1, 0),
				};
			}
		}

		private void CreateHistoryButton()
		{
			var button = new IconButton
			{
				Parent        = this.frameBox,
				IconUri       = AbstractCommandToolbar.GetResourceIconUri ("Field.History"),
				AutoFocus     = false,
				Dock          = DockStyle.Right,
				PreferredSize = new Size (AbstractFieldController.lineHeight, AbstractFieldController.lineHeight),
			};

			ToolTip.Default.SetToolTip (button, "Montre l'historique des modifications");

			button.Clicked += delegate
			{
				this.OnShowHistory (button);
			};
		}

		private void CreateClearButton()
		{
			if (this.PropertyState == PropertyState.Single)
			{
				var button = new IconButton
				{
					Parent        = this.frameBox,
					IconUri       = AbstractCommandToolbar.GetResourceIconUri ("Field.Clear"),
					AutoFocus     = false,
					Dock          = DockStyle.Right,
					PreferredSize = new Size (AbstractFieldController.lineHeight, AbstractFieldController.lineHeight),
				};

				ToolTip.Default.SetToolTip (button, "Supprime cette assignation de l'événement");

				button.Clicked += delegate
				{
				};
			}
			else
			{
				new FrameBox
				{
					Parent        = this.frameBox,
					Dock          = DockStyle.Right,
					PreferredSize = new Size (AbstractFieldController.lineHeight, AbstractFieldController.lineHeight),
				};
			}
		}


		protected Color BackgroundColor
		{
			get
			{
				switch (this.PropertyState)
				{
					case PropertyState.Single:
						return ColorManager.EditSinglePropertyColor;

					case PropertyState.Inherited:
						return ColorManager.EditInheritedPropertyColor;

					default:
						return Color.Empty;
				}
			}
		}


		#region Events handler
		protected void OnValueChanged()
		{
			if (this.ValueChanged != null)
			{
				this.ValueChanged (this);
			}
		}

		public delegate void ValueChangedEventHandler(object sender);
		public event ValueChangedEventHandler ValueChanged;


		protected void OnShowHistory(Widget target)
		{
			if (this.ShowHistory != null)
			{
				this.ShowHistory (this, target);
			}
		}

		public delegate void ShowHistoryEventHandler(object sender, Widget target);
		public event ShowHistoryEventHandler ShowHistory;
		#endregion


		protected static readonly int lineHeight = 17;

		protected FrameBox						frameBox;
		private string							label;
	}
}
