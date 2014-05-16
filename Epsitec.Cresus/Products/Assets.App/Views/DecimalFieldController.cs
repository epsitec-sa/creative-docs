//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Core;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class DecimalFieldController : AbstractFieldController
	{
		public DecimalFieldController(DataAccessor accessor)
			: base (accessor)
		{
		}


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
						if (this.ignoreChanges.IsZero)
						{
							using (this.ignoreChanges.Enter ())
							{
								this.textField.Text = this.ConvDecimalToString (this.value);
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
				this.textField.Text = this.ConvDecimalToString (this.value);
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

			AbstractFieldController.UpdateTextField (this.textField, this.propertyState, this.isReadOnly, this.hasError);
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

			this.UpdatePropertyState ();

			this.textField.TextChanged += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					using (this.ignoreChanges.Enter ())
					{
						this.Value = this.ConvStringToDecimal (this.textField.Text);
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
		}

		public override void SetFocus()
		{
			this.textField.SelectAll ();
			this.textField.Focus ();

			base.SetFocus ();
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
					return TypeConverters.RateToString (value);

				case DecimalFormat.Amount:
					return TypeConverters.AmountToString (value);

				case DecimalFormat.Real:
					return TypeConverters.DecimalToString (value);

				default:
					return null;
			}
		}

		private decimal? ConvStringToDecimal(string text)
		{
			switch (this.DecimalFormat)
			{
				case DecimalFormat.Rate:
					return TypeConverters.ParseRate (text);

				case DecimalFormat.Amount:
					return TypeConverters.ParseAmount (text);

				case DecimalFormat.Real:
					return TypeConverters.ParseDecimal (text);

				default:
					return null;
			}
		}


		private TextField						textField;
		private decimal?						value;
	}
}
