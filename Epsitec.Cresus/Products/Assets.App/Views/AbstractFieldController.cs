//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.Server.SimpleEngine;
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
		public EventType						EventType;
		public ObjectField						Field;

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
			this.historyButton = new IconButton
			{
				Parent        = this.frameBox,
				IconUri       = Misc.GetResourceIconUri ("Field.History"),
				AutoFocus     = false,
				Anchor        = AnchorStyles.TopRight,
				PreferredSize = new Size (AbstractFieldController.lineHeight, AbstractFieldController.lineHeight),
				Margins       = new Margins (0, AbstractFieldController.lineHeight, 0, 0),
			};

			ToolTip.Default.SetToolTip (this.historyButton, "Montre l'historique des modifications");

			this.historyButton.Clicked += delegate
			{
				this.OnShowHistory (this.historyButton, this.Field);
			};
		}

		private void CreateClearButton()
		{
			this.clearButton = new IconButton
			{
				Parent        = this.frameBox,
				IconUri       = Misc.GetResourceIconUri ("Field.Clear"),
				AutoFocus     = false,
				Anchor        = AnchorStyles.TopRight,
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


		protected virtual void UpdatePropertyState()
		{
			if (this.historyButton != null)
			{
				this.historyButton.Visibility = (this.PropertyState != PropertyState.OneShot &&
												 this.PropertyState != PropertyState.Timeless);
			}

			if (this.clearButton != null)
			{
				this.clearButton.Visibility = (this.PropertyState == PropertyState.Single);
			}
		}

		protected void UpdateTextField(AbstractTextField textField)
		{
			if (textField != null)
			{
				bool isReadOnly = (this.propertyState == PropertyState.Readonly);

				if (textField.IsReadOnly != isReadOnly)
				{
					textField.IsReadOnly = isReadOnly;
					textField.Invalidate ();  // TODO: pour corriger un bug de Widget !
				}
			}
		}

		protected Color BackgroundColor
		{
			get
			{
				switch (this.propertyState)
				{
					case PropertyState.Single:
						return ColorManager.GetEditSinglePropertyColor (DataAccessor.Simulation);

					case PropertyState.Inherited:
						return ColorManager.EditInheritedPropertyColor;

					default:
						return Color.Empty;
				}
			}
		}

		public static void UpdateBackColor(AbstractTextField textField, Color color)
		{
			if (textField != null)
			{
				if (!color.IsVisible)
				{
					textField.BackColor = Color.Empty;
					textField.TextDisplayMode = TextFieldDisplayMode.Default;
				}
				else
				{
					textField.BackColor = color;
					textField.TextDisplayMode = TextFieldDisplayMode.UseBackColor;
				}
			}
		}


		#region Events handler
		protected void OnValueEdited(ObjectField field)
		{
			this.ValueEdited.Raise (this, field);
		}

		public event EventHandler<ObjectField> ValueEdited;


		protected void OnShowHistory(Widget target, ObjectField field)
		{
			this.ShowHistory.Raise (this, target, field);
		}

		public event EventHandler<Widget, ObjectField> ShowHistory;


		protected void OnGoto(ViewState viewState)
		{
			this.Goto.Raise (this, viewState);
		}

		public event EventHandler<ViewState> Goto;
		#endregion


		public const int lineHeight = 17;

		protected readonly SafeCounter			ignoreChanges;

		protected FrameBox						frameBox;
		private string							label;
		private IconButton						historyButton;
		private IconButton						clearButton;
		protected PropertyState					propertyState;
	}
}
