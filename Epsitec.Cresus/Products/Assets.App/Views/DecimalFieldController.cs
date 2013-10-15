//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class DecimalFieldController : AbstractFieldController
	{
		public decimal? Value
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
						this.textField.Text = DecimalFieldController.ConvDecimalToString (this.value);
					}
				}
			}
		}

		public bool IsRate;


		public override void CreateUI(Widget parent)
		{
			base.CreateUI (parent);

			this.textField = new TextField
			{
				Parent         = this.frameBox,
				Dock           = DockStyle.Left,
				PreferredWidth = this.IsRate ? 50 : 100,
				Margins        = new Margins (0, 10, 0, 0),
				Text           = DecimalFieldController.ConvDecimalToString (this.value),
			};

			new StaticText
			{
				Parent         = this.frameBox,
				Dock           = DockStyle.Left,
				PreferredWidth = 50,
				Margins        = new Margins (0, 10, 3, 0),
				Text           = this.IsRate ? "%" : "CHF",
			};

			this.textField.TextChanged += delegate
			{
				this.value = DecimalFieldController.ConvStringToDecimal (this.textField.Text);
				this.OnValueChanged ();
			};
		}


		private static string ConvDecimalToString(decimal? value)
		{
			if (value.HasValue)
			{
				return value.Value.ToString ();
			}
			else
			{
				return null;
			}
		}

		private static decimal? ConvStringToDecimal(string text)
		{
			if (!string.IsNullOrEmpty (text))
			{
				decimal value;
				if (decimal.TryParse (text, out value))
				{
					return value;
				}
			}

			return null;
		}


		private TextField						textField;
		private decimal?						value;
	}
}
