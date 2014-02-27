//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.Helpers;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class AmortizedAmountController
	{
		public AmortizedAmountController()
		{
			this.ignoreChanges = new SafeCounter ();
		}


		public AmortizedAmount?					AmortizedAmount
		{
			get
			{
				return this.amortizedAmount;
			}
			set
			{
				if (this.amortizedAmount != value)
				{
					this.amortizedAmount = value;
					this.UpdateUI ();
				}
			}
		}

		public AmortizedAmount?					AmortizedAmountNoEditing
		{
			set
			{
				if (this.amortizedAmount != value)
				{
					this.amortizedAmount = value;
					this.UpdateNoEditingUI ();
				}
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
				if (this.isReadOnly != value)
				{
					this.isReadOnly = value;
					this.UpdateUI ();
				}
			}
		}

		public Color							BackgroundColor
		{
			get
			{
				return this.backgroundColor;
			}
			set
			{
				if (this.backgroundColor != value)
				{
					this.backgroundColor = value;
					this.UpdateUI ();
				}
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
					this.UpdateUI ();
				}
			}
		}


		public void UpdateValue()
		{
			this.UpdateUI ();
		}


		public void CreateUI(Widget parent)
		{
			this.CreateAmountLine (parent);
			this.CreateAmortizationLine (parent);
			this.CreateProrataLine (parent);

			this.UpdateUI ();
		}

		private void CreateAmountLine(Widget parent)
		{
			var frame = this.CreateFrame (parent);

			this.CreateOper (frame, 30, "V =");
			this.finalAmountTextField = this.CreateAmount (frame, true, "Valeur finale amortie");
			this.CreateOper (frame, 20, "=");
			this.initialAmountTextField = this.CreateAmount (frame, true, "Valeur précédente");
			this.CreateOper (frame, 50, "−  A  +");
			this.roundAmountTextField = this.CreateAmount (frame, false, "Arrondi");
		}

		private void CreateAmortizationLine(Widget parent)
		{
			var frame = this.CreateFrame (parent);

			this.CreateOper (frame, 30, "A =");
			this.amortizationAmountTextField = this.CreateAmount (frame, true, "Amortissement");
			this.CreateOper (frame, 20, "=");
			this.baseAmountTextField = this.CreateAmount (frame, true, "Valeur de base");
			this.CreateOper (frame, 20, "×");
			this.effectiveRateTextField = this.CreateRate (frame, false, "Taux adapté selon la périodicité");
			this.CreateOper (frame, 30, "×  P");
		}

		private void CreateProrataLine(Widget parent)
		{
			var frame = this.CreateFrame (parent);

			this.CreateOper (frame, 30, "P =");
			this.prorataRateTextField = this.CreateRate (frame, true, "Facteur correctif si \"au prorata\"");
			this.CreateOper (frame, 40, "= 1−(");
			this.prorataNumeratorTextField = this.CreateDecimal (frame, false, "Prorata, nombre effectif");
			this.CreateOper (frame, 20, "/");
			this.prorataDenominatorTextField = this.CreateDecimal (frame, false, "Prorata, nombre total");
			this.CreateOper (frame, 20, ")");
		}

		private FrameBox CreateFrame(Widget parent)
		{
			return new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Top,
				PreferredHeight = AbstractFieldController.lineHeight,
				Margins         = new Margins (0, 0, 0, 5),
			};
		}

		private TextField CreateAmount(Widget parent, bool isReadonly, string tooltip = null)
		{
			return this.CreateTextField (parent, isReadonly, 80, tooltip);
		}

		private TextField CreateRate(Widget parent, bool isReadonly, string tooltip = null)
		{
			return this.CreateTextField (parent, isReadonly, 45, tooltip);
		}

		private TextField CreateDecimal(Widget parent, bool isReadonly, string tooltip = null)
		{
			return this.CreateTextField (parent, isReadonly, 55, tooltip);
		}

		private TextField CreateTextField(Widget parent, bool isReadonly, int width, string tooltip = null)
		{
			var field = new TextField
			{
				Parent           = parent,
				Dock             = DockStyle.Left,
				PreferredWidth   = width,
				PreferredHeight  = AbstractFieldController.lineHeight,
				IsReadOnly       = isReadonly,
				Margins          = new Margins (0, 0, 0, 0),
				TabIndex         = ++this.tabIndex,
			};

			if (!string.IsNullOrEmpty (tooltip))
			{
				ToolTip.Default.SetToolTip (field, tooltip);
			}

			return field;
		}

		private void CreateOper(Widget parent, int width, string text)
		{
			new StaticText
			{
				Parent           = parent,
				Dock             = DockStyle.Left,
				PreferredWidth   = width,
				PreferredHeight  = AbstractFieldController.lineHeight,
				Text             = text,
				ContentAlignment = ContentAlignment.TopCenter,
				Margins          = new Margins (0, 0, 0, 0),
			};
		}


		public void SetFocus()
		{
			this.SetFocus (this.roundAmountTextField);
		}

		private void SetFocus(TextField textField)
		{
			textField.SelectAll ();
			textField.Focus ();
		}



		public void UpdateNoEditingUI()
		{
			this.UpdateUI ();
		}

		private void UpdateUI()
		{
			using (this.ignoreChanges.Enter ())
			{
				if (this.amortizedAmount.HasValue)
				{
					this.FinalAmount        = this.amortizedAmount.Value.FinalAmount;
					this.InitialAmount      = this.amortizedAmount.Value.InitialAmount;
					this.RoundAmount        = this.amortizedAmount.Value.RoundAmount;

					this.BaseAmount         = this.amortizedAmount.Value.BaseAmount;
					this.EffectiveRate      = this.amortizedAmount.Value.EffectiveRate;
					this.AmortizationAmount = this.BaseAmount * this.EffectiveRate.GetValueOrDefault (0.0m) * this.ProrataRate.GetValueOrDefault (1.0m);
					
					this.ProrataNumerator   = this.amortizedAmount.Value.ProrataNumerator;
					this.ProrataDenominator = this.amortizedAmount.Value.ProrataDenominator;
					this.ProrataRate        = this.ProrataNumerator.GetValueOrDefault (1.0m) / this.ProrataDenominator.GetValueOrDefault (1.0m);
				}
				else
				{
					this.FinalAmount        = null;
					this.InitialAmount      = null;
					this.RoundAmount        = null;

					this.BaseAmount         = null;
					this.EffectiveRate      = null;
					this.AmortizationAmount = null;

					this.ProrataNumerator   = null;
					this.ProrataDenominator = null;
					this.ProrataRate        = null;
				}
			}
		}

		private decimal? InitialAmount
		{
			get
			{
				return TypeConverters.ParseAmount (this.initialAmountTextField.Text);
			}
			set
			{
				this.initialAmountTextField.Text = TypeConverters.AmountToString (value);
			}
		}

		private decimal? BaseAmount
		{
			get
			{
				return TypeConverters.ParseAmount (this.baseAmountTextField.Text);
			}
			set
			{
				this.baseAmountTextField.Text = TypeConverters.AmountToString (value);
			}
		}

		private decimal? EffectiveRate
		{
			get
			{
				return TypeConverters.ParseRate (this.effectiveRateTextField.Text);
			}
			set
			{
				this.effectiveRateTextField.Text = TypeConverters.RateToString (value);
			}
		}

		private decimal? ProrataNumerator
		{
			get
			{
				return TypeConverters.ParseDecimal (this.prorataNumeratorTextField.Text);
			}
			set
			{
				this.prorataNumeratorTextField.Text = TypeConverters.DecimalToString (value);
			}
		}

		private decimal? ProrataDenominator
		{
			get
			{
				return TypeConverters.ParseDecimal (this.prorataDenominatorTextField.Text);
			}
			set
			{
				this.prorataDenominatorTextField.Text = TypeConverters.DecimalToString (value);
			}
		}

		private decimal? ProrataRate
		{
			get
			{
				return TypeConverters.ParseRate (this.prorataRateTextField.Text);
			}
			set
			{
				this.prorataRateTextField.Text = TypeConverters.RateToString (value);
			}
		}

		private decimal? AmortizationAmount
		{
			get
			{
				return TypeConverters.ParseAmount (this.amortizationAmountTextField.Text);
			}
			set
			{
				this.amortizationAmountTextField.Text = TypeConverters.AmountToString (value);
			}
		}

		private decimal? RoundAmount
		{
			get
			{
				return TypeConverters.ParseAmount (this.roundAmountTextField.Text);
			}
			set
			{
				this.roundAmountTextField.Text = TypeConverters.AmountToString (value);
			}
		}

		private decimal? FinalAmount
		{
			get
			{
				return TypeConverters.ParseAmount (this.finalAmountTextField.Text);
			}
			set
			{
				this.finalAmountTextField.Text = TypeConverters.AmountToString (value);
			}
		}


		#region Events handler
		private void OnValueEdited()
		{
			this.ValueEdited.Raise (this);
		}

		public event EventHandler ValueEdited;


		protected void OnFocusLost()
		{
			this.FocusLost.Raise (this);
		}

		public event EventHandler FocusLost;
		#endregion


		protected const int LineHeight = AbstractFieldController.lineHeight;
		protected const int EditWidth  = 90;


		private readonly SafeCounter			ignoreChanges;

		private AmortizedAmount?				amortizedAmount;
		private bool							isReadOnly;
		private Color							backgroundColor;
		private PropertyState					propertyState;

		private TextField						initialAmountTextField;
		private TextField						baseAmountTextField;
		private TextField						amortizationAmountTextField;
		private TextField						effectiveRateTextField;
		private TextField						prorataNumeratorTextField;
		private TextField						prorataDenominatorTextField;
		private TextField						prorataRateTextField;
		private TextField						roundAmountTextField;
		private TextField						finalAmountTextField;
		private int								tabIndex;
	}
}
