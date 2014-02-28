﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Widgets;
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
					this.CreateLines ();
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
			this.line1  = this.CreateFrame (parent);
			this.line12 = this.CreateInter (parent);
			this.line2  = this.CreateFrame (parent);
			this.line23 = this.CreateInter (parent);
			this.line3  = this.CreateFrame (parent);
			this.line34 = this.CreateInter (parent);
			this.line4  = this.CreateFrame (parent);
			this.line45 = this.CreateInter (parent);
			this.line5  = this.CreateFrame (parent);
			this.line56 = this.CreateInter (parent);
			this.line6  = this.CreateFrame (parent);
			this.line67 = this.CreateInter (parent);
			this.line7  = this.CreateFrame (parent);

			this.CreateLines ();
		}

		private void CreateLines()
		{
			this.line1.Children.Clear ();
			this.line12.Children.Clear ();
			this.line2.Children.Clear ();
			this.line23.Children.Clear ();
			this.line3.Children.Clear ();
			this.line34.Children.Clear ();
			this.line4.Children.Clear ();
			this.line45.Children.Clear ();
			this.line5.Children.Clear ();
			this.line56.Children.Clear ();
			this.line6.Children.Clear ();
			this.line67.Children.Clear ();
			this.line7.Children.Clear ();

			this.CreateTypeLine (this.line1);

			if (this.AmortizationType == Server.BusinessLogic.AmortizationType.Linear)
			{
				this.CreateMaxLine     (this.line2, this.line23);
				this.CreateRoundLine   (this.line3, this.line34);
				this.CreateLinearLine1 (this.line4, this.line45);
				this.CreateLinearLine2 (this.line5, this.line56);
				this.CreateProrataLine (this.line6, this.line67);
			}
			else if (this.AmortizationType == Server.BusinessLogic.AmortizationType.Degressive)
			{
				this.CreateMaxLine        (this.line2, this.line23);
				this.CreateRoundLine      (this.line3, this.line34);
				this.CreateDegressiveLine (this.line4, this.line45);
				this.CreateProrataLine    (this.line5, this.line56);
			}
			else
			{
				this.CreateInitLine (this.line2, this.line23);
			}

			this.UpdateUI ();
		}


		private void CreateInitLine(Widget parent, Widget bottomParent)
		{
			this.CreateLabel (parent, 100, "Valeur comptable");
			this.finalAmountTextField = this.CreateTextField (parent, false, 80, "Valeur initiale");
		}

		private void CreateMaxLine(Widget parent, Widget bottomParent)
		{
			this.CreateLabel (parent, 100, "Valeur comptable");
			this.finalAmountTextField = this.CreateTextField (parent, true, 80, "Valeur finale amortie");
			this.CreateOper (parent, "= Max (");
			var x = this.CreateArg (parent, "Valeur arrondie");
			this.CreateOper (parent, ",");
			this.residualAmountTextField = this.CreateTextField (parent, false, 60, "Valeur résiduelle");
			this.CreateOper (parent, ")");

			this.CreateLink (bottomParent, x);
		}

		private void CreateRoundLine(Widget parent, Widget bottomParent)
		{
			this.CreateLabel (parent, 100, "Valeur arrondie");
			this.roundedAmountTextField = this.CreateTextField (parent, true, 80, "Valeur amortie arrondie");
			this.CreateOper (parent, "= Arrondi (");
			var x = this.CreateArg (parent, "Valeur brute");
			this.CreateOper (parent, ",");
			this.roundAmountTextField = this.CreateTextField (parent, false, 60, "Arrondi");
			this.CreateOper (parent, ")");

			this.CreateLink (bottomParent, x);
		}

		private void CreateDegressiveLine(Widget parent, Widget bottomParent)
		{
			this.CreateLabel (parent, 100, "Valeur brute");
			this.brutAmountTextField = this.CreateTextField (parent, true, 80, "Valeur amortie non arrondie");
			this.CreateOper (parent, "=");
			this.initialAmountTextField = this.CreateTextField (parent, true, 80, "Valeur précédente");
			this.CreateOper (parent, "× ( 100% − (");
			this.effectiveRateTextField = this.CreateTextField (parent, false, 45, "Taux adapté selon la périodicité");
			this.CreateOper (parent, "×");
			var x = this.CreateArg (parent, "Prorata");
			this.CreateOper (parent, ") )");

			this.CreateLink (bottomParent, x);
		}

		private void CreateLinearLine1(Widget parent, Widget bottomParent)
		{
			this.CreateLabel (parent, 100, "Valeur brute");
			this.brutAmountTextField = this.CreateTextField (parent, true, 80, "Valeur amortie non arrondie");
			this.CreateOper (parent, "=");
			this.initialAmountTextField = this.CreateTextField (parent, true, 80, "Valeur précédente");
			this.CreateOper (parent, "−");
			var x = this.CreateArg (parent, "Amortissement");

			this.CreateLink (bottomParent, x);
		}

		private void CreateLinearLine2(Widget parent, Widget bottomParent)
		{
			this.CreateLabel (parent, 100, "Amortissement");
			this.amortizationAmountTextField = this.CreateTextField (parent, true, 80, "Amortissement");
			this.CreateOper (parent, "=");
			this.baseAmountTextField = this.CreateTextField (parent, true, 80, "Valeur de base");
			this.CreateOper (parent, "×");
			this.effectiveRateTextField = this.CreateTextField (parent, false, 45, "Taux adapté selon la périodicité");
			this.CreateOper (parent, "×");
			var x = this.CreateArg (parent, "Prorata");

			this.CreateLink (bottomParent, x);
		}

		private void CreateProrataLine(Widget parent, Widget bottomParent)
		{
			this.CreateLabel (parent, 100, "Prorata");
			this.prorataRateTextField = this.CreateTextField (parent, true, 45, "Facteur correctif si \"au prorata\"");
			this.CreateOper (parent, "=");
			this.prorataNumeratorTextField = this.CreateTextField (parent, false, 45, "Prorata, nombre effectif");
			this.CreateOper (parent, "/");
			this.prorataDenominatorTextField = this.CreateTextField (parent, false, 45, "Prorata, nombre total");
		}

		private void CreateTypeLine(Widget parent)
		{
			this.CreateLabel (parent, 100, "Type");

			this.typeTextFieldCombo = new TextFieldCombo
			{
				Parent           = parent,
				IsReadOnly       = true,
				Dock             = DockStyle.Left,
				PreferredWidth   = 100,
				PreferredHeight  = AbstractFieldController.lineHeight,
				Margins          = new Margins (0, 10, 0, 0),
				TabIndex         = ++this.tabIndex,
			};
		}


		private FrameBox CreateFrame(Widget parent)
		{
			return new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Top,
				PreferredHeight = AbstractFieldController.lineHeight,
				Margins         = new Margins (0, 36, 0, 0),
			};
		}

		private FrameBox CreateInter(Widget parent)
		{
			return new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Top,
				PreferredHeight = 13,
				Margins         = new Margins (0, 36, 0, 0),
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

		private StaticText CreateOper(Widget parent, string text)
		{
			int width = Helpers.Text.GetTextWidth (text) + 10;

			return new StaticText
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

		private TextField CreateArg(Widget parent, string text)
		{
			int width = Helpers.Text.GetTextWidth (text) + 10;

			return new TextField
			{
				Parent           = parent,
				Dock             = DockStyle.Left,
				PreferredWidth   = width,
				PreferredHeight  = AbstractFieldController.lineHeight,
				Text             = text,
				IsReadOnly       = true,
				Margins          = new Margins (0, 0, 0, 0),
				TabIndex         = ++this.tabIndex,
			};
		}

		private void CreateLink(Widget parent, Widget link)
		{
			parent.Parent.Window.ForceLayout ();

			new LinkLine
			{
				Parent  = parent,
				TopX    = (int) link.ActualBounds.Center.X - (100+10),
				Dock    = DockStyle.Fill,
				Margins = new Margins (100+10, 0, 0, 0),
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
					this.FinalAmount        = this.amortizedAmount.Value.FinalAmortizedAmount;
					this.ResidualAmount     = this.amortizedAmount.Value.ResidualAmount.GetValueOrDefault (0.0m);

					this.RoundedAmount      = this.amortizedAmount.Value.RoundedAmortizedAmount;
					this.RoundAmount        = this.amortizedAmount.Value.RoundAmount.GetValueOrDefault (0.0m);

					this.BrutAmount         = this.amortizedAmount.Value.BrutAmortizedAmount;
					this.InitialAmount      = this.amortizedAmount.Value.InitialAmount;

					this.AmortizationAmount = this.amortizedAmount.Value.BrutAmortization;
					this.BaseAmount         = this.amortizedAmount.Value.BaseAmount;
					this.EffectiveRate      = this.amortizedAmount.Value.EffectiveRate;

					this.ProrataRate        = this.amortizedAmount.Value.Prorata;
					this.ProrataNumerator   = this.amortizedAmount.Value.ProrataNumerator;
					this.ProrataDenominator = this.amortizedAmount.Value.ProrataDenominator;
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

				AmortizedAmountController.UpdateType (this.typeTextFieldCombo);
				AmortizedAmountController.SetType (this.typeTextFieldCombo, this.AmortizationType);

				this.UpdateTextField (this.finalAmountTextField);
				this.UpdateTextField (this.residualAmountTextField);
				this.UpdateTextField (this.roundedAmountTextField);
				this.UpdateTextField (this.roundAmountTextField);
				this.UpdateTextField (this.brutAmountTextField);
				this.UpdateTextField (this.initialAmountTextField);
				this.UpdateTextField (this.amortizationAmountTextField);
				this.UpdateTextField (this.baseAmountTextField);
				this.UpdateTextField (this.effectiveRateTextField);
				this.UpdateTextField (this.prorataRateTextField);
				this.UpdateTextField (this.prorataNumeratorTextField);
				this.UpdateTextField (this.prorataDenominatorTextField);
				this.UpdateTextField (this.typeTextFieldCombo);
			}
		}

		private decimal? FinalAmount
		{
			get
			{
				return AmortizedAmountController.GetAmount (this.finalAmountTextField);
			}
			set
			{
				AmortizedAmountController.SetAmount (this.finalAmountTextField, value);
			}
		}

		private decimal? ResidualAmount
		{
			get
			{
				return AmortizedAmountController.GetAmount (this.residualAmountTextField);
			}
			set
			{
				AmortizedAmountController.SetAmount (this.residualAmountTextField, value);
			}
		}

		private decimal? RoundedAmount
		{
			get
			{
				return AmortizedAmountController.GetAmount (this.roundedAmountTextField);
			}
			set
			{
				AmortizedAmountController.SetAmount (this.roundedAmountTextField, value);
			}
		}

		private decimal? RoundAmount
		{
			get
			{
				return AmortizedAmountController.GetAmount (this.roundAmountTextField);
			}
			set
			{
				AmortizedAmountController.SetAmount (this.roundAmountTextField, value);
			}
		}

		private decimal? BrutAmount
		{
			get
			{
				return AmortizedAmountController.GetAmount (this.brutAmountTextField);
			}
			set
			{
				AmortizedAmountController.SetAmount (this.brutAmountTextField, value);
			}
		}

		private decimal? InitialAmount
		{
			get
			{
				return AmortizedAmountController.GetAmount (this.initialAmountTextField);
			}
			set
			{
				AmortizedAmountController.SetAmount (this.initialAmountTextField, value);
			}
		}

		private decimal? AmortizationAmount
		{
			get
			{
				return AmortizedAmountController.GetAmount (this.amortizationAmountTextField);
			}
			set
			{
				AmortizedAmountController.SetAmount (this.amortizationAmountTextField, value);
			}
		}

		private decimal? BaseAmount
		{
			get
			{
				return AmortizedAmountController.GetAmount (this.baseAmountTextField);
			}
			set
			{
				AmortizedAmountController.SetAmount (this.baseAmountTextField, value);
			}
		}

		private decimal? EffectiveRate
		{
			get
			{
				return AmortizedAmountController.GetRate (this.effectiveRateTextField);
			}
			set
			{
				AmortizedAmountController.SetRate (this.effectiveRateTextField, value);
			}
		}

		private decimal? ProrataRate
		{
			get
			{
				return AmortizedAmountController.GetRate (this.prorataRateTextField);
			}
			set
			{
				AmortizedAmountController.SetRate (this.prorataRateTextField, value);
			}
		}

		private decimal? ProrataNumerator
		{
			get
			{
				return AmortizedAmountController.GetDecimal (this.prorataNumeratorTextField);
			}
			set
			{
				AmortizedAmountController.SetDecimal (this.prorataNumeratorTextField, value);
			}
		}

		private decimal? ProrataDenominator
		{
			get
			{
				return AmortizedAmountController.GetDecimal (this.prorataDenominatorTextField);
			}
			set
			{
				AmortizedAmountController.SetDecimal (this.prorataDenominatorTextField, value);
			}
		}


		private static decimal? GetAmount(TextField textField)
		{
			if (textField == null)
			{
				return null;
			}
			else
			{
				return TypeConverters.ParseAmount (textField.Text);
			}
		}

		private static void SetAmount(TextField textField, decimal? value)
		{
			if (textField != null)
			{
				textField.Text = TypeConverters.AmountToString (value);
			}
		}

		private static decimal? GetRate(TextField textField)
		{
			if (textField == null)
			{
				return null;
			}
			else
			{
				return TypeConverters.ParseRate (textField.Text);
			}
		}

		private static void SetRate(TextField textField, decimal? value)
		{
			if (textField != null)
			{
				textField.Text = TypeConverters.RateToString (value);
			}
		}

		private static decimal? GetDecimal(TextField textField)
		{
			if (textField == null)
			{
				return null;
			}
			else
			{
				return TypeConverters.ParseDecimal (textField.Text);
			}
		}

		private static void SetDecimal(TextField textField, decimal? value)
		{
			if (textField != null)
			{
				textField.Text = TypeConverters.DecimalToString (value);
			}
		}


		private AmortizationType GetType(TextFieldCombo combo)
		{
			if (combo != null)
			{
				foreach (var e in EnumDictionaries.DictAmortizationTypes)
				{
					if (combo.Text == e.Value)
					{
						return (AmortizationType) e.Key;
					}
				}
			}

			return AmortizationType.Unknown;
		}

		private static void SetType(TextFieldCombo combo, AmortizationType value)
		{
			if (combo != null)
			{
				if (value == AmortizationType.Unknown)
				{
					combo.Text = "Initialisation";
				}
				else
				{
					combo.Text = EnumDictionaries.GetAmortizationTypeName (value);
				}
			}
		}

		private static void UpdateType(TextFieldCombo combo)
		{
			if (combo != null)
			{
				combo.Items.Clear ();
				combo.Items.Add ("Initialisation");

				foreach (var e in EnumDictionaries.DictAmortizationTypes)
				{
					combo.Items.Add (e.Value);
				}
			}
		}


		private void UpdateTextField(AbstractTextField textField)
		{
			if (textField != null)
			{
				if (textField.IsReadOnly && !(textField is TextFieldCombo))
				{
					AbstractFieldController.UpdateBackColor (textField, Color.Empty);
				}
				else
				{
					AbstractFieldController.UpdateBackColor (textField, this.BackgroundColor);
				}
			}
		}


		private AmortizationType AmortizationType
		{
			get
			{
				if (this.amortizedAmount.HasValue)
				{
					return this.amortizedAmount.Value.AmortizationType;
				}
				else
				{
					return AmortizationType.Unknown;
				}
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

		private TextFieldCombo					typeTextFieldCombo;

		private FrameBox						line1;
		private FrameBox						line12;
		private FrameBox						line2;
		private FrameBox						line23;
		private FrameBox						line3;
		private FrameBox						line34;
		private FrameBox						line4;
		private FrameBox						line45;
		private FrameBox						line5;
		private FrameBox						line56;
		private FrameBox						line6;
		private FrameBox						line67;
		private FrameBox						line7;

		private int								tabIndex;
	}
}