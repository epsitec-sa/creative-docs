//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.Helpers;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class IntFieldController : AbstractFieldController
	{
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
			}
		}

		protected override void ClearValue()
		{
			this.Value = null;
			this.OnValueEdited (this.Field);
		}

		protected override void UpdatePropertyState()
		{
			base.UpdatePropertyState ();

			AbstractFieldController.UpdateBackColor (this.textField, this.BackgroundColor);
			this.UpdateTextField (this.textField);
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
				TabIndex        = this.TabIndex,
				Text            = IntFieldController.ConvIntToString (this.value),
			};

			var minus = new GlyphButton
			{
				Parent        = this.frameBox,
				GlyphShape    = GlyphShape.Minus,
				ButtonStyle   = ButtonStyle.ToolItem,
				Dock          = DockStyle.Left,
				PreferredSize = new Size (AbstractFieldController.lineHeight, AbstractFieldController.lineHeight),
			};

			var plus = new GlyphButton
			{
				Parent        = this.frameBox,
				GlyphShape    = GlyphShape.Plus,
				ButtonStyle   = ButtonStyle.ToolItem,
				Dock          = DockStyle.Left,
				PreferredSize = new Size (AbstractFieldController.lineHeight, AbstractFieldController.lineHeight),
			};

			this.textField.TextChanged += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					using (this.ignoreChanges.Enter ())
					{
						this.Value = IntFieldController.ConvStringToInt (this.textField.Text);
						this.OnValueEdited (this.Field);
					}
				}
			};

			this.textField.KeyboardFocusChanged += delegate (object sender, DependencyPropertyChangedEventArgs e)
			{
				bool focused = (bool) e.NewValue;

				if (focused)  // pris le focus ?
				{
					this.SetFocus ();
				}
				else  // perdu le focus ?
				{
					this.UpdateValue ();
				}
			};

			minus.Clicked += delegate
			{
				this.AddDelta (-1);
			};

			plus.Clicked += delegate
			{
				this.AddDelta (1);
			};
		}

		public override void SetFocus()
		{
			this.textField.SelectAll ();
			this.textField.Focus ();
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
		private int?							value;
	}
}
