//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.NaiveEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class DateFieldController : AbstractFieldController
	{
		public System.DateTime?					Value
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
						this.textField.Text = DateFieldController.ConvDateToString (this.value);
					}
				}
			}
		}


		public void CreateUI(Widget parent)
		{
			base.CreateUI (parent);

			this.textField = new TextField
			{
				Parent          = this.frameBox,
				PreferredWidth  = 70,
				PreferredHeight = AbstractFieldController.lineHeight,
				Dock            = DockStyle.Left,
				TabIndex        = this.TabIndex,
				Text            = DateFieldController.ConvDateToString (this.value),
				IsReadOnly      = this.PropertyState == PropertyState.Readonly,
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
				this.value = DateFieldController.ConvStringToDate (this.textField.Text);
				this.OnValueChanged ();
			};

			minus.Clicked += delegate
			{
				this.AddDays (-1);
			};

			plus.Clicked += delegate
			{
				this.AddDays (1);
			};
		}

		public override void SetFocus()
		{
			this.textField.SelectAll ();
			this.textField.Focus ();
		}


		private void AddDays(int days)
		{
			if (this.value.HasValue)
			{
				this.Value = this.value.Value.AddDays (days);
			}
		}


		private static string ConvDateToString(System.DateTime? value)
		{
			return Helpers.Converters.DateToString (value);
		}

		private static System.DateTime? ConvStringToDate(string text)
		{
			return Helpers.Converters.ParseDate (text);
		}


		private TextField						textField;
		private System.DateTime?				value;
	}
}
