//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.NaiveEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class DecimalFieldController : AbstractFieldController
	{
		public bool								IsRate;

		public decimal?							Value
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
						this.textField.Text = this.ConvDecimalToString (this.value);
					}
				}
			}
		}


		public override void CreateUI(Widget parent)
		{
			base.CreateUI (parent);

			this.textField = new TextField
			{
				Parent          = this.frameBox,
				Dock            = DockStyle.Left,
				PreferredWidth  = this.IsRate ? 50 : 100,
				PreferredHeight = AbstractFieldController.lineHeight,
				Margins         = new Margins (0, 10, 0, 0),
				TabIndex        = this.TabIndex,
				Text            = this.ConvDecimalToString (this.value),
				IsReadOnly      = this.PropertyState == PropertyState.Readonly,
			};

			new StaticText
			{
				Parent           = this.frameBox,
				Dock             = DockStyle.Left,
				PreferredWidth   = 50,
				PreferredHeight  = AbstractFieldController.lineHeight - 1,
				Margins          = new Margins (0, 10, 1, 0),
				Text             = this.IsRate ? "" : "CHF",
				ContentAlignment = ContentAlignment.TopLeft,
			};

			this.textField.TextChanged += delegate
			{
				this.value = this.ConvStringToDecimal (this.textField.Text);
				this.OnValueChanged ();
			};
		}

		public override void SetFocus()
		{
			this.textField.SelectAll ();
			this.textField.Focus ();
		}


		private string ConvDecimalToString(decimal? value)
		{
			if (this.IsRate)
			{
				return Helpers.Converters.PercentToString (value);
			}
			else
			{
				return Helpers.Converters.MontantToString (value);
			}
		}

		private decimal? ConvStringToDecimal(string text)
		{
			if (this.IsRate)
			{
				return Helpers.Converters.ParsePercent (text);
			}
			else
			{
				return Helpers.Converters.ParseMontant (text);
			}
		}


		private TextField						textField;
		private decimal?						value;
	}
}
