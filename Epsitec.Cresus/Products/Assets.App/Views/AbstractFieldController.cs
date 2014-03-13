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
		public AbstractFieldController(DataAccessor accessor)
		{
			this.accessor = accessor;
			this.ignoreChanges = new SafeCounter ();
		}


		public int								TabIndex;
		public int								LabelWidth = AbstractFieldController.labelWidth;
		public bool								HideAdditionalButtons;
		public EventType						EventType;
		public ObjectField						Field;

		public int								EditWidth
		{
			get
			{
				return this.editWidth;
			}
			set
			{
				this.editWidth = System.Math.Min (value, AbstractFieldController.maxWidth);
			}
		}

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
			this.ScrollToField ();
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

		protected IconButton CreateGotoButton()
		{
			return new IconButton
			{
				Parent        = this.frameBox,
				IconUri       = Misc.GetResourceIconUri ("Field.Goto"),
				AutoFocus     = false,
				Anchor        = AnchorStyles.TopRight,
				PreferredSize = new Size (AbstractFieldController.lineHeight, AbstractFieldController.lineHeight),
				Margins       = new Margins (0, AbstractFieldController.lineHeight*2, 0, 0),
			};
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

				if (textField is TextFieldCombo)
				{
					if (textField.Enable != !isReadOnly)
					{
						textField.Enable = !isReadOnly;
						textField.Invalidate ();  // TODO: pour corriger un bug de Widget !
					}
				}
				else
				{
					if (textField.IsReadOnly != isReadOnly)
					{
						textField.IsReadOnly = isReadOnly;
						textField.Invalidate ();  // TODO: pour corriger un bug de Widget !
					}
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
				if (color.IsVisible)
				{
					textField.BackColor = color;
					textField.TextDisplayMode = TextFieldDisplayMode.UseBackColor;
				}
				else
				{
					textField.BackColor = Color.Empty;
					textField.TextDisplayMode = TextFieldDisplayMode.Default;
				}
			}
		}


		private void ScrollToField()
		{
			//	Déplace l'ascenseur vertical de la zone Scrollable, afin que le FieldController
			//	soit entièrement visible.
			var scrollable = this.ParentScrollable;
			if (scrollable == null)
			{
				return;
			}

			double yTop    = scrollable.Viewport.ActualHeight - scrollable.ViewportOffsetY;
			double yBottom = scrollable.Viewport.ActualHeight - scrollable.ViewportOffsetY - scrollable.ActualHeight;
			var rect = this.frameBox.ActualBounds;

			if (rect.Top > yTop)
			{
				scrollable.ViewportOffsetY = scrollable.Viewport.ActualHeight - rect.Top;
			}

			if (rect.Bottom < yBottom)
			{
				scrollable.ViewportOffsetY = scrollable.Viewport.ActualHeight - rect.Bottom - scrollable.ActualHeight;
			}
		}

		private Scrollable ParentScrollable
		{
			//	Retourne le Scrollable parent.
			get
			{
				if (this.frameBox != null)
				{
					Widget parent = this.frameBox.Parent;

					while (parent != null)
					{
						if (parent is Scrollable)
						{
							return parent as Scrollable;
						}

						parent = parent.Parent;
					}
				}

				return null;
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


		protected void OnGoto(AbstractViewState viewState)
		{
			this.Goto.Raise (this, viewState);
		}

		public event EventHandler<AbstractViewState> Goto;
		#endregion


		public const int lineHeight = 17;
		public const int labelWidth = 100;
		public const int maxWidth   = 380;

		protected DataAccessor					accessor;
		protected readonly SafeCounter			ignoreChanges;

		protected FrameBox						frameBox;
		private string							label;
		private IconButton						historyButton;
		private IconButton						clearButton;
		private int								editWidth;
		protected PropertyState					propertyState;
	}
}
