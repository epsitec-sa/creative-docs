//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Core.Helpers;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.FieldControllers
{
	public class IntFieldController : AbstractFieldController
	{
		public IntFieldController(DataAccessor accessor)
			: base (accessor)
		{
		}


		public int?								Value
		{
			get
			{
				return this.value;
			}
			set
			{
				if (this.value != value)
				{
					this.value = value;

					if (this.textField != null)
					{
						if (this.ignoreChanges.IsZero)
						{
							using (this.ignoreChanges.Enter ())
							{
								this.textField.Text = IntFieldController.ConvIntToString (this.value);
								this.textField.SelectAll ();

								this.UpdateError ();
							}
						}
					}
				}
			}
		}

		private void UpdateValue()
		{
			using (this.ignoreChanges.Enter ())
			{
				this.textField.Text = IntFieldController.ConvIntToString (this.value);
				this.textField.SelectAll ();

				this.UpdateError ();
			}
		}

		private void UpdateError()
		{
			if (this.Required)
			{
				bool error = !this.value.HasValue;
				if (this.hasError != error)
				{
					this.hasError = error;
					this.UpdatePropertyState ();
				}
			}
		}

		protected override void ClearValue()
		{
			this.Value = null;
			this.UpdateButtons ();
			this.OnValueEdited (this.Field);
		}

		protected override void UpdatePropertyState()
		{
			base.UpdatePropertyState ();

			var type = AbstractFieldController.GetFieldColorType (this.propertyState, isLocked: this.isReadOnly, isError: this.hasError);
			AbstractFieldController.UpdateTextField (this.textField, type, this.isReadOnly);
			this.UpdateButtons ();
		}


		public override void CreateUI(Widget parent)
		{
			base.CreateUI (parent);

			this.textField = new TextField
			{
				Parent          = this.frameBox,
				PreferredWidth  = 50,
				PreferredHeight = AbstractFieldController.lineHeight,
				Dock            = DockStyle.Left,
				TabIndex        = ++this.TabIndex,
				Text            = IntFieldController.ConvIntToString (this.value),
			};

			this.minusButton = new GlyphButton
			{
				Parent        = this.frameBox,
				GlyphShape    = GlyphShape.Minus,
				ButtonStyle   = ButtonStyle.ToolItem,
				Dock          = DockStyle.Left,
				PreferredSize = new Size (AbstractFieldController.lineHeight, AbstractFieldController.lineHeight),
			};

			this.plusButton = new GlyphButton
			{
				Parent        = this.frameBox,
				GlyphShape    = GlyphShape.Plus,
				ButtonStyle   = ButtonStyle.ToolItem,
				Dock          = DockStyle.Left,
				PreferredSize = new Size (AbstractFieldController.lineHeight, AbstractFieldController.lineHeight),
			};

			this.UpdateError ();
			this.UpdatePropertyState ();

			//	Connexion des événements.
			this.frameBox.Entered += delegate
			{
				this.isMouseInside = true;
				this.UpdateButtons ();
			};

			this.frameBox.Exited += delegate
			{
				this.isMouseInside = false;
				this.UpdateButtons ();
			};

			this.textField.TextChanged += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					using (this.ignoreChanges.Enter ())
					{
						this.Value = IntFieldController.ConvStringToInt (this.textField.Text);
						this.UpdateButtons ();
						this.UpdateError ();
						this.OnValueEdited (this.Field);
					}
				}
			};

			this.textField.CursorChanged += delegate
			{
				this.UpdateButtons ();
			};

			this.textField.SelectionChanged += delegate
			{
				this.UpdateButtons ();
			};

			this.textField.KeyboardFocusChanged += delegate (object sender, DependencyPropertyChangedEventArgs e)
			{
				bool focused = (bool) e.NewValue;

				if (focused)  // pris le focus ?
				{
					this.hasFocus = true;
					this.SetFocus ();
				}
				else  // perdu le focus ?
				{
					this.hasFocus = false;
					this.UpdateValue ();
				}
			};

			this.minusButton.Clicked += delegate
			{
				this.AddDelta (-1);
			};

			this.plusButton.Clicked += delegate
			{
				this.AddDelta (1);
			};
		}

		public override void SetFocus()
		{
			this.textField.SelectAll ();
			this.textField.Focus ();

			base.SetFocus ();
		}


		private void UpdateButtons()
		{
			if (this.minusButton == null)
			{
				return;
			}

			this.minusButton.Visibility = this.AreButtonsVisible;
			this.plusButton .Visibility = this.AreButtonsVisible;
		}

		private bool AreButtonsVisible
		{
			get
			{
				return this.isMouseInside || this.hasFocus;
			}
		}


		private void AddDelta(int delta)
		{
			if (this.value.HasValue)
			{
				this.Value = this.value.Value + delta;
				this.OnValueEdited (this.Field);
			}
		}


		private static string ConvIntToString(int? value)
		{
			return TypeConverters.IntToString (value);
		}

		private static int? ConvStringToInt(string text)
		{
			return TypeConverters.ParseInt (text);
		}


		private TextField						textField;
		private GlyphButton						minusButton;
		private GlyphButton						plusButton;
		private int?							value;
		private bool							hasFocus;
		private bool							isMouseInside;
	}
}
