﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Cresus.Assets.Server.NaiveEngine;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Cresus.Assets.App.Views
{
	public abstract class AbstractFieldController
	{
		public AbstractFieldController()
		{
			this.ignoreChanges = new SafeCounter ();
		}


		public int								TabIndex;
		public int								LabelWidth = 100;
		public int								EditWidth = 380;
		public bool								HideAdditionalButtons;

		public PropertyState					PropertyState
		{
			get
			{
				return this.propertyState;
			}
			set
			{
				if (this.propertyState != value)
				{
					this.propertyState = value;
					this.UpdatePropertyState ();
				}
			}
		}

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
			};

			this.CreateLabel ();

			if (!this.HideAdditionalButtons)
			{
				this.CreateClearButton ();
				this.CreateHistoryButton ();
			}

			this.UpdatePropertyState ();
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
				Anchor        = AnchorStyles.BottomRight,
				PreferredSize = new Size (AbstractFieldController.lineHeight, AbstractFieldController.lineHeight),
				Margins       = new Margins (0, AbstractFieldController.lineHeight, 0, 0),
			};

			ToolTip.Default.SetToolTip (button, "Montre l'historique des modifications");

			button.Clicked += delegate
			{
				this.OnShowHistory (button);
			};
		}

		private void CreateClearButton()
		{
			this.clearButton = new IconButton
			{
				Parent        = this.frameBox,
				IconUri       = AbstractCommandToolbar.GetResourceIconUri ("Field.Clear"),
				AutoFocus     = false,
				Anchor        = AnchorStyles.BottomRight,
				PreferredSize = new Size (AbstractFieldController.lineHeight, AbstractFieldController.lineHeight),
			};

			ToolTip.Default.SetToolTip (this.clearButton, "Supprime cette modification de l'événement");

			this.clearButton.Clicked += delegate
			{
				this.ClearValue ();
			};
		}

		protected virtual void ClearValue()
		{
		}


		private void UpdatePropertyState()
		{
			if (this.frameBox == null)
			{
				return;
			}

			this.frameBox.BackColor = this.BackgroundColor;
			this.clearButton.Visibility = (this.PropertyState == PropertyState.Single);
		}

		protected Color BackgroundColor
		{
			get
			{
				switch (this.propertyState)
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

		protected readonly SafeCounter			ignoreChanges;

		protected FrameBox						frameBox;
		private string							label;
		private IconButton						clearButton;
		private PropertyState					propertyState;
	}
}
