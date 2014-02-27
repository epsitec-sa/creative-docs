//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
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
			this.CreateMaxLine (parent);
			this.CreateRoundLine (parent);
			this.CreateAmountLine (parent);
			this.CreateAmortizationLine (parent);
			this.CreateProrataLine (parent);

			this.UpdateUI ();
		}

		private void CreateMaxLine(Widget parent)
		{
			var frame = this.CreateFrame (parent);

			this.CreateLabel (frame, 100, "Valeur compable");
			this.finalAmountTextField = this.CreateTextField (frame, true, 80, "Valeur finale amortie");
			this.CreateOper (frame, 135, "= Max ( Valeur arrondie ,");
			this.residualAmountTextField = this.CreateTextField (frame, false, 60, "Valeur résiduelle");
			this.CreateOper (frame, 20, ")");
		}

		private void CreateRoundLine(Widget parent)
		{
			var frame = this.CreateFrame (parent);

			this.CreateLabel (frame, 100, "Valeur arrondie");
			this.roundedAmountTextField = this.CreateTextField (frame, true, 80, "Valeur amortie arrondie");
			this.CreateOper (frame, 135, "= Arrondi ( Valeur brute ,");
			this.roundAmountTextField = this.CreateTextField (frame, false, 60, "Arrondi");
			this.CreateOper (frame, 20, ")");
		}

		private void CreateAmountLine(Widget parent)
		{
			var frame = this.CreateFrame (parent);

			this.CreateLabel (frame, 100, "Valeur brute");
			this.brutAmountTextField = this.CreateTextField (frame, true, 80, "Valeur amortie non arrondie");
			this.CreateOper (frame, 20, "=");
			this.initialAmountTextField = this.CreateTextField (frame, true, 80, "Valeur précédente");
			this.CreateOper (frame, 100, "−  Amortissement");
		}

		private void CreateAmortizationLine(Widget parent)
		{
			var frame = this.CreateFrame (parent);

			this.CreateLabel (frame, 100, "Amortissement");
			this.amortizationAmountTextField = this.CreateTextField (frame, true, 80, "Amortissement");
			this.CreateOper (frame, 20, "=");
			this.baseAmountTextField = this.CreateTextField (frame, true, 80, "Valeur de base");
			this.CreateOper (frame, 20, "×");
			this.effectiveRateTextField = this.CreateTextField (frame, false, 45, "Taux adapté selon la périodicité");
			this.CreateOper (frame, 65, "×  Prorata");
		}

		private void CreateProrataLine(Widget parent)
		{
			var frame = this.CreateFrame (parent);

			this.CreateLabel (frame, 100, "Prorata");
			this.prorataRateTextField = this.CreateTextField (frame, true, 45, "Facteur correctif si \"au prorata\"");
			this.CreateOper (frame, 40, "= 1−(");
			this.prorataNumeratorTextField = this.CreateTextField (frame, false, 45, "Prorata, nombre effectif");
			this.CreateOper (frame, 20, "/");
			this.prorataDenominatorTextField = this.CreateTextField (frame, false, 45, "Prorata, nombre total");
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

		private void CreateLabel(Widget parent, int width, string text)
		{
			new StaticText
			{
				Parent           = parent,
				Dock             = DockStyle.Left,
				PreferredWidth   = width,
				PreferredHeight  = AbstractFieldController.lineHeight - 1,
				Text             = text,
				ContentAlignment = ContentAlignment.TopRight,
				TextBreakMode    = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
				Margins          = new Margins (0, 10, 1, 0),
			};
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
				TextBreakMode    = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
				Margins          = new Margins (0, 0, 0, 0),
			};
		}


		public void SetFocus()
		{
			this.SetFocus (this.finalAmountTextField);
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
					var fi = this.amortizedAmount.Value.FinalAmount;
					var re = this.amortizedAmount.Value.ResidualAmount.GetValueOrDefault (0.0m);
					var ii = this.amortizedAmount.Value.InitialAmount;
					var rn = this.amortizedAmount.Value.RoundAmount.GetValueOrDefault (0.0m);
					var ba = this.amortizedAmount.Value.BaseAmount;
					var er = this.amortizedAmount.Value.EffectiveRate;
					var nu = this.amortizedAmount.Value.ProrataNumerator;
					var de = this.amortizedAmount.Value.ProrataDenominator;
					var pr = 1.0m - (nu.GetValueOrDefault (0.0m) / de.GetValueOrDefault (1.0m));
					var br = ii - (ba * er * pr);
					var rd = AmortizationDetails.Round (br.GetValueOrDefault (), rn);
					var mx = System.Math.Max (rd, re);

					this.FinalAmount        = fi;
					this.ResidualAmount     = re;

					this.RoundedAmount      = rd;
					this.RoundAmount        = rn;

					this.BrutAmount         = br;
					this.InitialAmount      = ii;

					this.AmortizationAmount = ba * er.GetValueOrDefault (0.0m) * pr;
					this.BaseAmount         = ba;
					this.EffectiveRate      = er;

					this.ProrataRate        = pr;
					this.ProrataNumerator   = nu;
					this.ProrataDenominator = de;
				}
				else
				{
					this.FinalAmount        = null;
					this.ResidualAmount     = null;

					this.RoundedAmount      = null;
					this.RoundAmount        = null;

					this.BrutAmount         = null;
					this.InitialAmount      = null;

					this.AmortizationAmount = null;
					this.BaseAmount         = null;
					this.EffectiveRate      = null;

					this.ProrataRate        = null;
					this.ProrataNumerator   = null;
					this.ProrataDenominator = null;
				}
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

		private decimal? ResidualAmount
		{
			get
			{
				return TypeConverters.ParseAmount (this.residualAmountTextField.Text);
			}
			set
			{
				this.residualAmountTextField.Text = TypeConverters.AmountToString (value);
			}
		}

		private decimal? RoundedAmount
		{
			get
			{
				return TypeConverters.ParseAmount (this.roundedAmountTextField.Text);
			}
			set
			{
				this.roundedAmountTextField.Text = TypeConverters.AmountToString (value);
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

		private decimal? BrutAmount
		{
			get
			{
				return TypeConverters.ParseAmount (this.brutAmountTextField.Text);
			}
			set
			{
				this.brutAmountTextField.Text = TypeConverters.AmountToString (value);
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

		private TextField						finalAmountTextField;
		private TextField						residualAmountTextField;

		private TextField						roundedAmountTextField;
		private TextField						roundAmountTextField;

		private TextField						brutAmountTextField;
		private TextField						initialAmountTextField;

		private TextField						amortizationAmountTextField;
		private TextField						baseAmountTextField;
		private TextField						effectiveRateTextField;

		private TextField						prorataRateTextField;
		private TextField						prorataNumeratorTextField;
		private TextField						prorataDenominatorTextField;

		private int								tabIndex;
	}
}
