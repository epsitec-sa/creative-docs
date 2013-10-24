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
		public DecimalFormat					DecimalFormat;

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
						using (this.ignoreChanges.Enter ())
						{
							this.textField.Text = this.ConvDecimalToString (this.value);
						}
					}

					this.OnValueChanged ();
				}
			}
		}

		protected override void ClearValue()
		{
			this.Value = null;
		}


		public override void CreateUI(Widget parent)
		{
			base.CreateUI (parent);

			this.textField = new TextField
			{
				Parent          = this.frameBox,
				Dock            = DockStyle.Left,
				PreferredWidth  = this.Width,
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
				Text             = this.Unit,
				ContentAlignment = ContentAlignment.TopLeft,
			};

			this.textField.TextChanged += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					this.Value = this.ConvStringToDecimal (this.textField.Text);
				}
			};
		}

		public override void SetFocus()
		{
			this.textField.SelectAll ();
			this.textField.Focus ();
		}



		private int Width
		{
			get
			{
				switch (this.DecimalFormat)
				{
					case DecimalFormat.Rate:
						return 50;

					default:
						return 90;
				}
			}
		}

		private string Unit
		{
			get
			{
				switch (this.DecimalFormat)
				{
					case DecimalFormat.Amount:
						return "CHF";

					default:
						return "";
				}
			}
		}


		private string ConvDecimalToString(decimal? value)
		{
			switch (this.DecimalFormat)
			{
				case DecimalFormat.Rate:
					return Helpers.Converters.RateToString (value);

				case DecimalFormat.Amount:
					return Helpers.Converters.AmountToString (value);

				case DecimalFormat.Real:
					return Helpers.Converters.DecimalToString (value);

				default:
					return null;
			}
		}

		private decimal? ConvStringToDecimal(string text)
		{
			switch (this.DecimalFormat)
			{
				case DecimalFormat.Rate:
					return Helpers.Converters.ParseRate (text);

				case DecimalFormat.Amount:
					return Helpers.Converters.ParseAmount (text);

				case DecimalFormat.Real:
					return Helpers.Converters.ParseDecimal (text);

				default:
					return null;
			}
		}


		private TextField						textField;
		private decimal?						value;
	}
}
