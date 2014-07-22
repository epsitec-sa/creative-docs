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
using Epsitec.Cresus.Assets.Server.SimpleEngine;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Core.Helpers;
using Epsitec.Cresus.Assets.App.Views.FieldControllers;
using Epsitec.Cresus.Assets.App.Views.ViewStates;
using Epsitec.Cresus.Assets.App.Views.EditorPages;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class AmortizedAmountController
	{
		public AmortizedAmountController(DataAccessor accessor)
		{
			this.accessor = accessor;

			this.ignoreChanges = new SafeCounter ();
			this.commentaryTypes = new HashSet<CommentaryType> ();
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
					this.UpdateEventType ();
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
					this.UpdateEventType ();
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

		public IEnumerable<CommentaryType>		CommentaryTypes
		{
			get
			{
				return this.commentaryTypes;
			}
		}


		public void UpdateValue()
		{
			this.UpdateUI ();
		}


		public void CreateUI(Widget parent)
		{
			this.line1  = this.CreateFrame (parent);
			this.line12 = this.CreateInter (parent, 22);
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

			this.CreateEntryController (parent);
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

			if (this.IsAmortization)
			{
				this.CreateCombos      (this.line1);
				this.CreateMaxLine     (this.line2, this.line23);
				this.CreateRoundLine   (this.line3, this.line34);
				this.CreateLine1       (this.line4, this.line45);
				this.CreateLine2       (this.line5, this.line56);
				this.CreateProrataLine (this.line6, this.line67);
			}
			else
			{
				this.CreateCombos   (this.line1);
				this.CreateInitLine (this.line2, this.line23);
			}

			this.UpdateUI ();
		}


		private void CreateCombos(Widget parent)
		{
			this.CreateLabel (parent, 100, "Opération");

			this.scenarioFieldCombo = new TextFieldCombo
			{
				Parent           = parent,
				IsReadOnly       = true,
				Dock             = DockStyle.Left,
				PreferredWidth   = 180,
				PreferredHeight  = AbstractFieldController.lineHeight,
				Margins          = new Margins (0, 10, 0, 0),
				TabIndex         = ++this.tabIndex,
			};

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

			this.scenarioFieldCombo.ComboClosed += delegate
			{
				if (this.value.HasValue)
				{
					var scenario = AmortizedAmountController.GetScenario (this.scenarioFieldCombo);
					if (this.value.Value.EntryScenario != scenario)
					{
						this.value = AmortizedAmount.SetEntryScenario (this.value.Value, scenario);
						this.UpdateUI ();
						this.OnValueEdited ();
					}
				}
			};

			this.typeTextFieldCombo.ComboClosed += delegate
			{
				if (this.value.HasValue)
				{
					var type = AmortizedAmountController.GetType (this.typeTextFieldCombo);
					if (this.value.Value.AmortizationType != type)
					{
						this.value = AmortizedAmount.SetAmortizationType (this.value.Value, type);
						this.UpdateUI ();
						this.OnValueEdited ();
					}
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

		private void CreateButton(Widget parent)
		{
			const int h = 22;

			var line = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = h,
				Dock            = DockStyle.Top,
				Margins         = new Margins (0, 0, 20, 0),
			};


			var button = new Button
			{
				Parent        = line,
				Text          = "Voir l'écriture",
				ButtonStyle   = ButtonStyle.Icon,
				AutoFocus     = false,
				PreferredSize = new Size (100, h),
				Dock          = DockStyle.Left,
				Margins       = new Margins (100+10, 0, 0, 0),
			};

			button.Clicked += delegate
			{
				if (this.value.HasValue)
				{
					if (!this.value.Value.EntryGuid.IsEmpty)
					{
						var viewState = EntriesView.GetViewState (this.value.Value.EntryGuid);
						this.OnGoto (viewState);
					}
				}
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

		private FrameBox CreateInter(Widget parent, int h = 13)
		{
			return new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Top,
				PreferredHeight = h,
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
				Name             = (action == null) ? "IsReadOnly" : "",
				Margins          = new Margins (0, 0, 0, 0),
				TabIndex         = ++this.tabIndex,
			};

			if (!string.IsNullOrEmpty (tooltip))
			{
				ToolTip.Default.SetToolTip (field, tooltip);
			}

			if (action != null)
			{
				field.TextChanged += delegate
				{
					if (this.ignoreChanges.IsZero)
					{
						action ();
					}
				};
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

		private void CreateEntryController(Widget parent)
		{
			{
				var line = this.CreateFrame (parent);
				line.Margins = new Margins (0, 0, 0, 10);

				new StaticText
				{
					Parent  = line,
					Text    = "Ecriture comptable générée :",
					Dock    = DockStyle.Fill,
					Margins = new Margins (100+10, 0, 0, 0),
				};
			}

			this.entryController = new EntryController (this.accessor);
			this.entryController.CreateUI (parent);

			this.entryController.ValueEdited += delegate
			{
				this.UpdateUI ();
				this.OnValueEdited ();
			};

			{
				var line = this.CreateFrame (parent);
				this.CreateButton (line);
			}
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
				this.commentaryTypes.Clear ();

				if (this.value.HasValue)
				{
					this.typeTextFieldCombo.Visibility = this.IsAmortization;

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

				this.UpdateScenario (this.scenarioFieldCombo);
				AmortizedAmountController.SetScenario (this.scenarioFieldCombo, this.EntryScenario);

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

				{
					var type = AbstractFieldController.GetCommentaryType (this.propertyState, isReadOnly: false);
					this.commentaryTypes.Add (type);
				}

				{
					var type = AbstractFieldController.GetCommentaryType (this.propertyState, isReadOnly: true);
					this.commentaryTypes.Add (type);
				}

				this.UpdateEntry ();
			}
		}

		private void UpdateEntry()
		{
			if (this.value.HasValue)
			{
				this.value = Entries.CreateEntry (this.accessor, this.value.Value);
			}

			this.entryController.Value         = this.value;
			this.entryController.PropertyState = this.propertyState;
			this.entryController.IsReadOnly    = this.isReadOnly;

			foreach (var type in this.entryController.CommentaryTypes)
			{
				this.commentaryTypes.Add (type);
			}
		}


		private void UpdateInitAmount()
		{
			if (this.ignoreChanges.IsZero)
			{
				this.value = AmortizedAmount.SetInitialAmount (this.value.Value, this.FinalAmount);
				this.UpdateUI ();
				this.OnValueEdited ();
			}
		}

		private void UpdateEffectiveRate()
		{
			if (this.ignoreChanges.IsZero)
			{
				this.value = AmortizedAmount.SetEffectiveRate (this.value.Value, this.EffectiveRate);
				this.UpdateUI ();
				this.OnValueEdited ();
			}
		}

		private void UpdateProrataNumerator()
		{
			if (this.ignoreChanges.IsZero)
			{
				this.value = AmortizedAmount.SetProrataNumerator (this.value.Value, this.ProrataNumerator);
				this.UpdateUI ();
				this.OnValueEdited ();
			}
		}

		private void UpdateProrataDenominator()
		{
			if (this.ignoreChanges.IsZero)
			{
				this.value = AmortizedAmount.SetProrataDenominator (this.value.Value, this.ProrataDenominator);
				this.UpdateUI ();
				this.OnValueEdited ();
			}
		}

		private void UpdateRoundAmount()
		{
			if (this.ignoreChanges.IsZero)
			{
				this.value = AmortizedAmount.SetRoundAmount (this.value.Value, this.RoundAmount);
				this.UpdateUI ();
				this.OnValueEdited ();
			}
		}

		private void UpdateResidualAmount()
		{
			if (this.ignoreChanges.IsZero)
			{
				this.value = AmortizedAmount.SetResidualAmount (this.value.Value, this.ResidualAmount);
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

		private void UpdateScenario(TextFieldCombo combo)
		{
			if (combo != null)
			{
				combo.Items.Clear ();

				foreach (var e in EnumDictionaries.DictEntryScenarios)
				{
					var s = (EntryScenario) e.Key;

					if (this.IsAmortization)
					{
						if (s != EntryScenario.AmortizationAuto &&
							s != EntryScenario.AmortizationExtra)
						{
							continue;
						}
					}
					else
					{
						if (s == EntryScenario.AmortizationAuto ||
							s == EntryScenario.AmortizationExtra)
						{
							continue;
						}
					}

					combo.Items.Add (e.Value);
				}
			}
		}


		private void UpdateBackColor(AbstractTextField textField)
		{
			if (textField != null)
			{
				if (textField is TextFieldCombo)
				{
					AbstractFieldController.UpdateCombo (textField as TextFieldCombo, this.propertyState, this.isReadOnly);
				}
				else
				{
					AbstractFieldController.UpdateTextField (textField, this.propertyState, this.isReadOnly);
				}
			}
		}


		private bool IsAmortization
		{
			get
			{
				return this.AmortizationType != AmortizationType.Unknown;
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

		private EntryScenario EntryScenario
		{
			get
			{
				if (this.value.HasValue)
				{
					return this.value.Value.EntryScenario;
				}
				else
				{
					return EntryScenario.None;
				}
			}
		}


		public bool IsEditionAllowed
		{
			get
			{
				return !this.isReadOnly && this.eventType == EventType.AmortizationExtra;
			}
		}

		public void UpdateEventType()
		{
			if (this.value.HasValue)
			{
				var obj = this.accessor.GetObject (BaseType.Assets, this.value.Value.AssetGuid);
				if (obj != null)
				{
					var e = obj.GetEvent (this.value.Value.EventGuid);
					if (e != null)
					{
						this.eventType = e.Type;
						return;
					}
				}
			}

			this.eventType = EventType.Unknown;
		}


		#region Events handler
		private void OnValueEdited()
		{
			this.ValueEdited.Raise (this);
		}

		public event EventHandler ValueEdited;


		protected void OnFocusEngage()
		{
			this.FocusEngage.Raise (this);
		}

		public event EventHandler FocusEngage;


		protected void OnFocusLost()
		{
			this.FocusLost.Raise (this);
		}

		public event EventHandler FocusLost;


		private void OnGoto(AbstractViewState viewState)
		{
			this.Goto.Raise (this, viewState);
		}

		public event EventHandler<AbstractViewState> Goto;
		#endregion


		private const int AmountWidth = 80;
		private const int RoundWidth  = 60;
		private const int RateWidth   = 60;
		private const int IntWidth    = 45;


		private readonly DataAccessor			accessor;
		private readonly SafeCounter			ignoreChanges;
		private readonly HashSet<CommentaryType> commentaryTypes;

		private AmortizedAmount?				value;
		private EventType						eventType;
		private PropertyState					propertyState;
		private bool							isReadOnly;

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

		private EntryController					entryController;

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
