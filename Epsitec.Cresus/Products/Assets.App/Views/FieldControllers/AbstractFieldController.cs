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
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.App.Views.ViewStates;

namespace Epsitec.Cresus.Assets.App.Views.FieldControllers
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
		public bool								Required;
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

		public bool								IsReadOnly
		{
			get
			{
				return this.isReadOnly;
			}
			set
			{
				this.isReadOnly = value;
				this.UpdatePropertyState ();
			}
		}

		public bool								HasError
		{
			get
			{
				return this.hasError;
			}
			set
			{
				this.hasError = value;
				this.UpdatePropertyState ();
			}
		}

		public FrameBox							FrameBox
		{
			get
			{
				return this.frameBox;
			}
		}

		public virtual IEnumerable<FieldColorType> FieldColorTypes
		{
			get
			{
				yield return AbstractFieldController.GetFieldColorType (this.propertyState, isLocked: this.isReadOnly, isError: this.hasError);
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
				Padding         = new Margins (0, 0, 2, 2),
			};

			this.CreateLabel ();

			if (!this.HideAdditionalButtons)
			{
				this.CreateClearButton ();

				if (this.Field != ObjectField.Unknown)
				{
					this.CreateHistoryButton ();
				}
			}

			this.UpdatePropertyState ();
		}

		public void ValueChanged()
		{
			this.OnValueEdited (this.Field);
		}

		public virtual void SetFocus()
		{
			this.OnSetFieldFocus (this.Field);
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

			ToolTip.Default.SetToolTip (this.historyButton, Res.Strings.FieldControllers.ShowHistory.ToString ());

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

			ToolTip.Default.SetToolTip (this.clearButton, Res.Strings.FieldControllers.ClearModification.ToString ());

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
				this.historyButton.Enable     = !this.isReadOnly;
				this.historyButton.Visibility = (this.PropertyState != PropertyState.OneShot &&
												 this.PropertyState != PropertyState.Timeless &&
												 this.PropertyState != PropertyState.Deletable);
			}

			if (this.clearButton != null)
			{
				this.clearButton.Enable     = !this.isReadOnly;
				this.clearButton.Visibility = (this.PropertyState == PropertyState.Single ||
											   this.PropertyState == PropertyState.Deletable);
			}
		}

		public static void UpdateTextField(AbstractTextField textField, FieldColorType type, bool isReadOnly)
		{
			if (textField != null)
			{
				System.Diagnostics.Debug.Assert (!(textField is TextFieldCombo));

				textField.BackColor = AbstractFieldController.GetBackgroundColor (type);

				if (textField.TextDisplayMode == TextFieldDisplayMode.ActiveHint ||
					textField.TextDisplayMode == TextFieldDisplayMode.ActiveHintAndUseBackColor)
				{
					textField.TextDisplayMode = TextFieldDisplayMode.ActiveHintAndUseBackColor;
				}
				else
				{
					textField.TextDisplayMode = TextFieldDisplayMode.UseBackColor;
				}

				textField.IsReadOnly = isReadOnly;
			}
		}

		public static void UpdateCombo(TextFieldCombo combo, FieldColorType type, bool isReadOnly)
		{
			if (combo != null)
			{
				combo.BackColor       = AbstractFieldController.GetBackgroundColor (type);
				combo.TextDisplayMode = TextFieldDisplayMode.UseBackColor;
				combo.IsReadOnly      = true;
				combo.Enable          = !isReadOnly;
			}
		}

		public static void UpdateButton(ColoredButton button, FieldColorType type, bool isReadOnly)
		{
			if (button != null)
			{
				button.NormalColor          = AbstractFieldController.GetBackgroundColor (type);
				button.Enable               = !isReadOnly;
				button.SameColorWhenDisable = true;
			}
		}

		public static Color GetBackgroundColor(FieldColorType type)
		{
			switch (type)
			{
				case FieldColorType.Editable:
					return ColorManager.NormalFieldColor;

				case FieldColorType.Automatic:
					return ColorManager.GetEditSinglePropertyColor (DataAccessor.Simulation).ForceSV (0.12, 1.0);

				case FieldColorType.Defined:
					return ColorManager.GetEditSinglePropertyColor (DataAccessor.Simulation);

				case FieldColorType.Readonly:
					return ColorManager.NormalFieldColor.Delta (-0.1);

				case FieldColorType.Result:
					return ColorManager.GetEditSinglePropertyColor (DataAccessor.Simulation).Delta (-0.1);

				case FieldColorType.Error:
					return ColorManager.ErrorColor;;

				default:
					return Color.Empty;
			}
		}

		public static FieldColorType GetFieldColorType(PropertyState state, bool isLocked = false, bool isError = false)
		{
			if (isLocked)
			{
				return FieldColorType.Readonly;  // gris
			}
			else if (isError)
			{
				return FieldColorType.Error;  // orange
			}
			else
			{
				switch (state)
				{
					case PropertyState.Single:
					case PropertyState.OneShot:
						return FieldColorType.Defined;  // bleu

					case PropertyState.Automatic:
						return FieldColorType.Automatic;  // bleu clair

					default:
						return FieldColorType.Editable;  // blanc
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


		protected void OnDataChanged()
		{
			this.DataChanged.Raise (this);
		}

		public event EventHandler DataChanged;


		protected void OnDeepUpdate()
		{
			this.DeepUpdate.Raise (this);
		}

		public event EventHandler DeepUpdate;


		protected void OnSetFieldFocus(ObjectField field)
		{
			this.SetFieldFocus.Raise (this, field);
		}

		public event EventHandler<ObjectField> SetFieldFocus;


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
		protected bool							isReadOnly;
		protected bool							hasError;
	}
}
