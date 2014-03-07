//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
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


		public AmortizedAmount?					Value
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
					this.CreateLines ();
				}
			}
		}

		public AmortizedAmount?					ValueNoEditing
		{
			set
			{
				if (this.value != value)
				{
					this.value = value;
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
			this.line0  = this.CreateFrame (parent);
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

			this.CreateRadios (this.line0);
			this.CreateLines ();
		}

		private void CreateRadios(Widget parent)
		{
			this.radioFix = new RadioButton
			{
				Parent         = parent,
				Text           = "Montant fixe",
				PreferredWidth = 100,
				Dock           = DockStyle.Left,
				Margins        = new Margins (100+10, 0, 0, 10),
			};

			this.radioAmo = new RadioButton
			{
				Parent         = parent,
				Text           = "Amortissement",
				PreferredWidth = 100,
				Dock           = DockStyle.Left,
				Margins        = new Margins (0, 0, 0, 10),
			};

			this.radioFix.Clicked += delegate
			{
				if (this.value.HasValue)
				{
					this.value = AmortizedAmount.CreateType (this.value.Value, AmortizationType.Unknown);
					this.CreateLines ();
					this.OnValueEdited ();
				}
			};

			this.radioAmo.Clicked += delegate
			{
				if (this.value.HasValue)
				{
					this.value = AmortizedAmount.CreateType (this.value.Value, AmortizationType.Linear);
					this.CreateLines ();
					this.OnValueEdited ();
				}
			};
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

			if (this.AmortizationType == Server.BusinessLogic.AmortizationType.Linear ||
				this.AmortizationType == Server.BusinessLogic.AmortizationType.Degressive)
			{
				this.CreateTypeLine    (this.line1);
				this.CreateMaxLine     (this.line2, this.line23);
				this.CreateRoundLine   (this.line3, this.line34);
				this.CreateLine1       (this.line4, this.line45);
				this.CreateLine2       (this.line5, this.line56);
				this.CreateProrataLine (this.line6, this.line67);
				this.CreateScenario    (this.line7);
			}
			else
			{
				this.CreateInitLine (this.line1, this.line23);
				this.CreateScenario (this.line2);
			}

			this.UpdateUI ();
		}


		private void CreateTypeLine(Widget parent)
		{
			this.CreateLabel (parent, 100, "Type d'amort.");

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

			this.typeTextFieldCombo.ComboClosed += delegate
			{
				if (this.value.HasValue)
				{
					var type = AmortizedAmountController.GetType (this.typeTextFieldCombo);
					if (this.value.Value.AmortizationType != type)
					{
						this.value = AmortizedAmount.CreateType (this.value.Value, type);
						this.CreateLines ();
						this.OnValueEdited ();
					}
				}
			};
		}

		private void CreateScenario(Widget parent)
		{
			this.CreateLabel (parent, 100, "Ecritures générées");

			this.scenarioFieldCombo = new TextFieldCombo
			{
				Parent           = parent,
				IsReadOnly       = true,
				Dock             = DockStyle.Left,
				PreferredWidth   = 100,
				PreferredHeight  = AbstractFieldController.lineHeight,
				Margins          = new Margins (0, 10, 0, 0),
				TabIndex         = ++this.tabIndex,
			};

			this.scenarioFieldCombo.ComboClosed += delegate
			{
				if (this.value.HasValue)
				{
					var type = AmortizedAmountController.GetScenario (this.scenarioFieldCombo);
					//?if (this.value.Value.AmortizationType != type)
					//?{
					//?	this.value = AmortizedAmount.CreateType (this.value.Value, type);
					//?	this.CreateLines ();
					//?	this.OnValueEdited ();
					//?}
				}
			};
		}

		private void CreateInitLine(Widget parent, Widget bottomParent)
		{
			this.CreateLabel (parent, 100, "Valeur comptable");
			this.finalAmountTextField = this.CreateTextField (parent, AmortizedAmountController.AmountWidth, "Valeur initiale", this.UpdateInitAmount);
		}

		private void CreateMaxLine(Widget parent, Widget bottomParent)
		{
			this.CreateLabel (parent, 100, "Valeur comptable");
			this.finalAmountTextField = this.CreateTextField (parent, AmortizedAmountController.AmountWidth, "Valeur finale amortie");
			this.CreateOper (parent, "= Max (");
			var x = this.CreateArg (parent, "Valeur arrondie");
			this.CreateOper (parent, ",");
			this.residualAmountTextField = this.CreateTextField (parent, AmortizedAmountController.RoundWidth, "Valeur résiduelle", this.UpdateResidualAmount);
			this.CreateOper (parent, ")");

			this.CreateLink (bottomParent, x);
		}

		private void CreateRoundLine(Widget parent, Widget bottomParent)
		{
			this.CreateLabel (parent, 100, "Valeur arrondie");
			this.roundedAmountTextField = this.CreateTextField (parent, AmortizedAmountController.AmountWidth, "Valeur amortie arrondie");
			this.CreateOper (parent, "= Arrondi (");
			var x = this.CreateArg (parent, "Valeur brute");
			this.CreateOper (parent, ",");
			this.roundAmountTextField = this.CreateTextField (parent, AmortizedAmountController.RoundWidth, "Arrondi", this.UpdateRoundAmount);
			this.CreateOper (parent, ")");

			this.CreateLink (bottomParent, x);
		}

		private void CreateLine1(Widget parent, Widget bottomParent)
		{
			this.CreateLabel (parent, 100, "Valeur brute");
			this.brutAmountTextField = this.CreateTextField (parent, AmortizedAmountController.AmountWidth, "Valeur amortie non arrondie");
			this.CreateOper (parent, "=");
			this.initialAmountTextField = this.CreateTextField (parent, AmortizedAmountController.AmountWidth, "Valeur précédente");
			this.CreateOper (parent, "−");
			var x = this.CreateArg (parent, "Amortissement");

			this.CreateLink (bottomParent, x);
		}

		private void CreateLine2(Widget parent, Widget bottomParent)
		{
			this.CreateLabel (parent, 100, "Amortissement");
			this.amortizationAmountTextField = this.CreateTextField (parent, AmortizedAmountController.AmountWidth, "Amortissement");
			this.CreateOper (parent, "=");
			this.baseAmountTextField = this.CreateTextField (parent, AmortizedAmountController.AmountWidth, this.AmortizationType == AmortizationType.Linear ? "Valeur de base" : "Valeur précédente");
			this.CreateOper (parent, "×");
			this.effectiveRateTextField = this.CreateTextField (parent, AmortizedAmountController.RateWidth, "Taux adapté selon la périodicité", this.UpdateEffectiveRate);
			this.CreateOper (parent, "×");
			var x = this.CreateArg (parent, "Prorata");

			this.CreateLink (bottomParent, x);
		}

		private void CreateProrataLine(Widget parent, Widget bottomParent)
		{
			this.CreateLabel (parent, 100, "Prorata");
			this.prorataRateTextField = this.CreateTextField (parent, AmortizedAmountController.RateWidth, "Facteur correctif si \"au prorata\"");
			this.CreateOper (parent, "=");
			this.prorataNumeratorTextField = this.CreateTextField (parent, AmortizedAmountController.IntWidth, "Prorata, nombre effectif", this.UpdateProrataNumerator);
			this.CreateOper (parent, "/");
			this.prorataDenominatorTextField = this.CreateTextField (parent, AmortizedAmountController.IntWidth, "Prorata, nombre total", this.UpdateProrataDenominator);
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

		private TextField CreateTextField(Widget parent, int width, string tooltip, System.Action action = null)
		{
			var field = new TextField
			{
				Parent           = parent,
				Dock             = DockStyle.Left,
				PreferredWidth   = width,
				PreferredHeight  = AbstractFieldController.lineHeight,
				IsReadOnly       = action == null,
				Margins          = new Margins (0, 0, 0, 0),
				TabIndex         = ++this.tabIndex,
			};

			if (!string.IsNullOrEmpty (tooltip))
			{
				ToolTip.Default.SetToolTip (field, tooltip);
			}

			field.TextChanged += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					action ();
				}
			};

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
			int width = text.GetTextWidth () + 10;

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
			int width = text.GetTextWidth () + 10;

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
				if (this.value.HasValue)
				{
					this.radioFix.ActiveState = (this.value.Value.AmortizationType == AmortizationType.Unknown) ? ActiveState.Yes : ActiveState.No;
					this.radioAmo.ActiveState = (this.value.Value.AmortizationType != AmortizationType.Unknown) ? ActiveState.Yes : ActiveState.No;

					this.FinalAmount        = this.value.Value.FinalAmortizedAmount;
					this.ResidualAmount     = this.value.Value.ResidualAmount.GetValueOrDefault (0.0m);

					this.RoundedAmount      = this.value.Value.RoundedAmortizedAmount;
					this.RoundAmount        = this.value.Value.RoundAmount.GetValueOrDefault (0.0m);

					this.BrutAmount         = this.value.Value.BrutAmortizedAmount;
					this.InitialAmount      = this.value.Value.InitialAmount;

					this.AmortizationAmount = this.value.Value.BrutAmortization;
					this.BaseAmount         = this.value.Value.BaseAmount;
					this.EffectiveRate      = this.value.Value.EffectiveRate;

					this.ProrataRate        = this.value.Value.Prorata;
					this.ProrataNumerator   = this.value.Value.ProrataNumerator;
					this.ProrataDenominator = this.value.Value.ProrataDenominator;
				}
				else
				{
					this.radioFix.ActiveState = ActiveState.Yes;
					this.radioAmo.ActiveState = ActiveState.No;

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

				AmortizedAmountController.UpdateScenario (this.scenarioFieldCombo);
				AmortizedAmountController.SetScenario (this.scenarioFieldCombo, EntryScenario.None);

				this.UpdateBackColor (this.typeTextFieldCombo);
				this.UpdateBackColor (this.finalAmountTextField);
				this.UpdateBackColor (this.residualAmountTextField);
				this.UpdateBackColor (this.roundedAmountTextField);
				this.UpdateBackColor (this.roundAmountTextField);
				this.UpdateBackColor (this.brutAmountTextField);
				this.UpdateBackColor (this.initialAmountTextField);
				this.UpdateBackColor (this.amortizationAmountTextField);
				this.UpdateBackColor (this.baseAmountTextField);
				this.UpdateBackColor (this.effectiveRateTextField);
				this.UpdateBackColor (this.prorataRateTextField);
				this.UpdateBackColor (this.prorataNumeratorTextField);
				this.UpdateBackColor (this.prorataDenominatorTextField);
				this.UpdateBackColor (this.scenarioFieldCombo);
			}
		}


		private void UpdateInitAmount()
		{
			if (this.ignoreChanges.IsZero)
			{
				this.value = new AmortizedAmount (this.FinalAmount);
				this.UpdateUI ();
				this.OnValueEdited ();
			}
		}

		private void UpdateEffectiveRate()
		{
			if (this.ignoreChanges.IsZero)
			{
				this.value = AmortizedAmount.CreateEffectiveRate (this.value, this.EffectiveRate);
				this.UpdateUI ();
				this.OnValueEdited ();
			}
		}

		private void UpdateProrataNumerator()
		{
			if (this.ignoreChanges.IsZero)
			{
				this.value = AmortizedAmount.CreateProrataNumerator (this.value, this.ProrataNumerator);
				this.UpdateUI ();
				this.OnValueEdited ();
			}
		}

		private void UpdateProrataDenominator()
		{
			if (this.ignoreChanges.IsZero)
			{
				this.value = AmortizedAmount.CreateProrataDenominator (this.value, this.ProrataDenominator);
				this.UpdateUI ();
				this.OnValueEdited ();
			}
		}

		private void UpdateRoundAmount()
		{
			if (this.ignoreChanges.IsZero)
			{
				this.value = AmortizedAmount.CreateRoundAmount (this.value, this.RoundAmount);
				this.UpdateUI ();
				this.OnValueEdited ();
			}
		}

		private void UpdateResidualAmount()
		{
			if (this.ignoreChanges.IsZero)
			{
				this.value = AmortizedAmount.CreateResidualAmount (this.value, this.ResidualAmount);
				this.UpdateUI ();
				this.OnValueEdited ();
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
				if (this.AmortizationType == AmortizationType.Linear)
				{
					AmortizedAmountController.SetAmount (this.initialAmountTextField, value);
				}
				else
				{
					AmortizedAmountController.SetAmount (this.initialAmountTextField, value);
					AmortizedAmountController.SetAmount (this.baseAmountTextField, value);
				}
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
				if (this.AmortizationType == AmortizationType.Linear)
				{
					AmortizedAmountController.SetAmount (this.baseAmountTextField, value);
				}
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
				if (AmortizedAmountController.GetAmount (textField) != value)
				{
					textField.Text = TypeConverters.AmountToString (value);
				}
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
				if (AmortizedAmountController.GetRate (textField) != value)
				{
					textField.Text = TypeConverters.RateToString (value);
				}
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
				if (AmortizedAmountController.GetDecimal (textField) != value)
				{
					textField.Text = TypeConverters.DecimalToString (value);
				}
			}
		}


		private static AmortizationType GetType(TextFieldCombo combo)
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
				combo.Text = EnumDictionaries.GetAmortizationTypeName (value);
			}
		}

		private static void UpdateType(TextFieldCombo combo)
		{
			if (combo != null)
			{
				combo.Items.Clear ();

				foreach (var e in EnumDictionaries.DictAmortizationTypes)
				{
					combo.Items.Add (e.Value);
				}
			}
		}


		private static EntryScenario GetScenario(TextFieldCombo combo)
		{
			if (combo != null)
			{
				foreach (var e in EnumDictionaries.DictEntryScenarios)
				{
					if (combo.Text == e.Value)
					{
						return (EntryScenario) e.Key;
					}
				}
			}

			return EntryScenario.None;
		}

		private static void SetScenario(TextFieldCombo combo, EntryScenario value)
		{
			if (combo != null)
			{
				combo.Text = EnumDictionaries.GetEntryScenarioName (value);
			}
		}

		private static void UpdateScenario(TextFieldCombo combo)
		{
			if (combo != null)
			{
				combo.Items.Clear ();

				foreach (var e in EnumDictionaries.DictEntryScenarios)
				{
					combo.Items.Add (e.Value);
				}
			}
		}


		private void UpdateBackColor(AbstractTextField textField)
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
				if (this.value.HasValue)
				{
					return this.value.Value.AmortizationType;
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


		private const int AmountWidth = 80;
		private const int RoundWidth  = 60;
		private const int RateWidth   = 60;
		private const int IntWidth    = 45;


		private readonly SafeCounter			ignoreChanges;

		private AmortizedAmount?				value;
		private bool							isReadOnly;
		private Color							backgroundColor;
		private PropertyState					propertyState;

		private RadioButton						radioFix;
		private RadioButton						radioAmo;

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
		private TextFieldCombo					scenarioFieldCombo;

		private FrameBox						line0;
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
