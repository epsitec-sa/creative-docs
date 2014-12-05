//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Core.Helpers;
using Epsitec.Cresus.Assets.App.Views.FieldControllers;
using Epsitec.Cresus.Assets.App.Views.ViewStates;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class AmortizedAmountController
	{
		public AmortizedAmountController(DataAccessor accessor)
		{
			this.accessor = accessor;

			this.ignoreChanges = new SafeCounter ();
			this.fieldColorTypes = new HashSet<FieldColorType> ();
		}


		public void SetValue(DataObject asset, DataEvent e, AmortizedAmount? value)
		{
			this.asset = asset;
			this.e     = e;

			if (this.value != value)
			{
				this.value = value;
				this.UpdateEventType ();
				this.CreateLines ();
			}
		}

		public AmortizedAmount?					Value
		{
			get
			{
				return this.value;
			}
			set
			{
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

		public bool								HasError
		{
			get
			{
				return this.entryController.HasError;
			}
		}

		public IEnumerable<FieldColorType>		FieldColorTypes
		{
			get
			{
				return this.fieldColorTypes;
			}
		}


		public void UpdateValue()
		{
			this.UpdateUI ();
		}


		public void CreateUI(Widget parent)
		{
			this.lines = new FrameBox[15];

			for (int i=0; i<this.lines.Length; i++)
			{
				this.lines[i] = this.CreateFrame (parent);
			}

			this.CreateEntryController (parent);
			this.CreateLines ();
		}

		private void CreateLines()
		{
			for (int i=0; i<this.lines.Length; i++)
			{
				this.lines[i].Children.Clear ();
			}

			//??if (this.IsAmortization)
			//??{
			//??	this.CreateMethodCombo (this.lines[0]);
			//??	this.CreateInitLines ();
			//??}
			//??else
			//??{
			//??	this.CreateMethodCombo (this.lines[0]);
			//??	this.CreateInitLine ();
			//??}
			this.CreateInitLines ();

			this.UpdateUI ();
		}


		private void CreateScenarioCombo(Widget parent)
		{
			this.CreateLabel (parent, 100, Res.Strings.AmortizedAmountController.Scenario.ToString ());

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
		}

		//??private void CreateMethodCombo(Widget parent)
		//??{
		//??	this.CreateLabel (parent, 100, "");
		//??
		//??	this.methodTextFieldCombo = new TextFieldCombo
		//??	{
		//??		Parent           = parent,
		//??		IsReadOnly       = true,
		//??		Enable           = false,
		//??		Dock             = DockStyle.Left,
		//??		PreferredWidth   = AbstractFieldController.maxWidth,
		//??		PreferredHeight  = AbstractFieldController.lineHeight,
		//??		Margins          = new Margins (0, 10, 0, 0),
		//??		TabIndex         = ++this.tabIndex,
		//??	};
		//??
		//??	this.methodTextFieldCombo.ComboClosed += delegate
		//??	{
		//??		if (this.value.HasValue)
		//??		{
		//??			var method = AmortizedAmountController.GetMethod (this.methodTextFieldCombo);
		//??			if (this.value.Value.AmortizationMethod != method)
		//??			{
		//??				this.value = AmortizedAmount.SetAmortizationMethod (this.value.Value, method);
		//??				this.CreateLines ();
		//??				this.UpdateUI ();
		//??				this.OnValueEdited ();
		//??			}
		//??		}
		//??	};
		//??}

		//??private void CreateInitLine()
		//??{
		//??	this.CreateLabel (this.lines[1], 100, "Valeur comptable");
		//??	this.outputAmountTextField = this.CreateTextField (this.lines[1], AmortizedAmountController.AmountWidth, "TODO");
		//??}

		private void CreateInitLines()
		{
			int i = 0;

			//??this.CreateLabel (this.lines[++i], 100, "Valeur comptable");
			//??this.outputAmountTextField = this.CreateTextField (this.lines[i], AmortizedAmountController.AmountWidth, "TODO");
			//??
			//??this.CreateLabel (this.lines[++i], 100, "Valeur imposée");
			//??this.forcedAmountTextField = this.CreateTextField (this.lines[i], AmortizedAmountController.AmountWidth, "TODO", this.UpdateForcedAmount);
			//??this.CreateArgument (this.lines[i], "ForcedAmount");
			//??
			//??this.CreateLabel (this.lines[++i], 100, "Valeur de base");
			//??this.baseAmountTextField = this.CreateTextField (this.lines[i], AmortizedAmountController.AmountWidth, "TODO");
			//??this.CreateArgument (this.lines[i], "BaseAmount");
			//??
			//??this.CreateLabel (this.lines[++i], 100, "Valeur initiale");
			//??this.initialAmountTextField = this.CreateTextField (this.lines[i], AmortizedAmountController.AmountWidth, "TODO");
			//??this.CreateArgument (this.lines[i], "InitialAmount");
			//??
			//??this.CreateLabel (this.lines[++i], 100, "Valeur résiduelle");
			//??this.residualAmountTextField = this.CreateTextField (this.lines[i], AmortizedAmountController.AmountWidth, "TODO", this.UpdateResidualAmount);
			//??this.CreateArgument (this.lines[i], "ResidualAmount");
			//??
			//??this.CreateLabel (this.lines[++i], 100, "Arrondi");
			//??this.roundAmountTextField = this.CreateTextField (this.lines[i], AmortizedAmountController.AmountWidth, "TODO", this.UpdateRoundAmount);
			//??this.CreateArgument (this.lines[i], "RoundAmount");
			//??
			//??this.CreateLabel (this.lines[++i], 100, "Taux");
			//??this.rateTextField = this.CreateTextField (this.lines[i], AmortizedAmountController.AmountWidth, "TODO", this.UpdateRate);
			//??this.CreateArgument (this.lines[i], "Rate");
			//??
			//??this.CreateLabel (this.lines[++i], 100, "Périodicité");
			//??this.periodicityFactorTextField = this.CreateTextField (this.lines[i], AmortizedAmountController.AmountWidth, "TODO");
			//??this.CreateArgument (this.lines[i], "PeriodicityFactor");
			//??
			//??this.CreateLabel (this.lines[++i], 100, "Prorata num.");
			//??this.prorataNumeratorTextField = this.CreateTextField (this.lines[i], AmortizedAmountController.AmountWidth, "TODO", this.UpdateProrataNumerator);
			//??this.CreateArgument (this.lines[i], "ProrataNumerator");
			//??
			//??this.CreateLabel (this.lines[++i], 100, "Prorata dénom.");
			//??this.prorataDenominatorTextField = this.CreateTextField (this.lines[i], AmortizedAmountController.AmountWidth, "TODO", this.UpdateProrataDenominator);
			//??this.CreateArgument (this.lines[i], "ProrataDenominator");
			//??
			//??this.CreateLabel (this.lines[++i], 100, "Total années");
			//??this.yearCountTextField = this.CreateTextField (this.lines[i], AmortizedAmountController.AmountWidth, "TODO");
			//??this.CreateArgument (this.lines[i], "YearCount");
			//??
			//??this.CreateLabel (this.lines[++i], 100, "Rang année");
			//??this.yearRankTextField = this.CreateTextField (this.lines[i], AmortizedAmountController.AmountWidth, "TODO");
			//??this.CreateArgument (this.lines[i], "YearRank");

			this.CreateLabel (this.lines[++i], 100, "Valeur initiale");
			this.initialAmountTextField = this.CreateTextField (this.lines[i], AmortizedAmountController.AmountWidth, "TODO");
			this.CreateArgument (this.lines[i], "InitialAmount");

			this.CreateLabel (this.lines[++i], 100, "Valeur finale");
			this.finalAmountTextField = this.CreateTextField (this.lines[i], AmortizedAmountController.AmountWidth, "TODO");
			this.CreateArgument (this.lines[i], "FinalAmount");
		}

		private void CreateButtons(Widget parent)
		{
			const int h = 22;

			var line = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = h,
				Dock            = DockStyle.Top,
				Margins         = new Margins (0, 0, 20, 0),
			};


			this.deleteEntryButton = new Button
			{
				Parent        = line,
				Text          = Res.Strings.AmortizedAmountController.Entry.Delete.Text.ToString (),
				ButtonStyle   = ButtonStyle.Icon,
				AutoFocus     = false,
				PreferredSize = new Size (120, h),
				Dock          = DockStyle.Left,
				Margins       = new Margins (100+10, 0, 0, 0),
			};

			this.showEntryButton = new Button
			{
				Parent        = line,
				Text          = Res.Strings.AmortizedAmountController.Entry.Show.Text.ToString (),
				ButtonStyle   = ButtonStyle.Icon,
				AutoFocus     = false,
				PreferredSize = new Size (120, h),
				Dock          = DockStyle.Left,
				Margins       = new Margins (10, 0, 0, 0),
			};

			ToolTip.Default.SetToolTip (this.deleteEntryButton, Res.Strings.AmortizedAmountController.Entry.Delete.Tooltip.ToString ());
			ToolTip.Default.SetToolTip (this.showEntryButton,   Res.Strings.AmortizedAmountController.Entry.Show.Tooltip.ToString ());

			this.deleteEntryButton.Clicked += delegate
			{
				if (this.value.HasValue)
				{
					this.entryController.Clear ();
				}
			};

			this.showEntryButton.Clicked += delegate
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
				Margins         = new Margins (0, 36, 0, 1),
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

		private void CreateArgument(Widget parent, string text)
		{
			new StaticText
			{
				Parent           = parent,
				Dock             = DockStyle.Fill,
				PreferredHeight  = AbstractFieldController.lineHeight - 1,
				Text             = "<i>(data." + text + ")</i>",
				ContentAlignment = ContentAlignment.TopLeft,
				TextBreakMode    = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
				Margins          = new Margins (10, 0, 1, 0),
			};
		}

		//??private StaticText CreateOper(Widget parent, string text)
		//??{
		//??	int width = text.GetTextWidth () + 10;
		//??
		//??	return new StaticText
		//??	{
		//??		Parent           = parent,
		//??		Dock             = DockStyle.Left,
		//??		PreferredWidth   = width,
		//??		PreferredHeight  = AbstractFieldController.lineHeight,
		//??		Text             = text,
		//??		ContentAlignment = ContentAlignment.TopCenter,
		//??		TextBreakMode    = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
		//??		Margins          = new Margins (0, 0, 0, 0),
		//??	};
		//??}

		//??private TextField CreateArg(Widget parent, string text)
		//??{
		//??	int width = text.GetTextWidth () + 10;
		//??
		//??	return new TextField
		//??	{
		//??		Parent           = parent,
		//??		Dock             = DockStyle.Left,
		//??		PreferredWidth   = width,
		//??		PreferredHeight  = AbstractFieldController.lineHeight,
		//??		Text             = text,
		//??		IsReadOnly       = true,
		//??		Margins          = new Margins (0, 0, 0, 0),
		//??		TabIndex         = ++this.tabIndex,
		//??	};
		//??}

		//??private void CreateLink(Widget parent, Widget link)
		//??{
		//??	parent.Parent.Window.ForceLayout ();
		//??
		//??	new LinkLine
		//??	{
		//??		Parent  = parent,
		//??		TopX    = (int) link.ActualBounds.Center.X - (100+10),
		//??		Dock    = DockStyle.Fill,
		//??		Margins = new Margins (100+10, 0, 0, 0),
		//??	};
		//??}

		private void CreateEntryController(Widget parent)
		{
			{
				var line = this.CreateFrame (parent);
				line.Margins = new Margins (0, 0, 0, 10);

				new StaticText
				{
					Parent  = line,
					Text    = Res.Strings.AmortizedAmountController.EntrySample.ToString (),
					Dock    = DockStyle.Fill,
					Margins = new Margins (100+10, 0, 0, 0),
				};
			}

			{
				var line = this.CreateFrame (parent);
				line.Margins = new Margins (0, 0, 0, 10);

				this.CreateScenarioCombo (line);
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
				this.CreateButtons (line);
			}
		}


		public void SetFocus()
		{
			//??this.SetFocus (this.outputAmountTextField);
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
				this.fieldColorTypes.Clear ();

				if (this.value.HasValue)
				{
					//??this.methodTextFieldCombo.Visibility = this.IsAmortization;
					//??
					//??this.OutputAmount       = this.accessor.GetAmortizedAmount (this.Value);
					//??this.ForcedAmount       = this.value.Value.ForcedAmount;
					//??this.BaseAmount         = this.value.Value.BaseAmount;
					//??this.InitialAmount      = this.value.Value.InitialAmount;
					//??this.ResidualAmount     = this.value.Value.ResidualAmount;
					//??this.RoundAmount        = this.value.Value.RoundAmount;
					//??this.Rate               = this.value.Value.Rate;
					//??this.PeriodicityFactor  = this.value.Value.PeriodicityFactor;
					//??this.ProrataNumerator   = this.value.Value.ProrataNumerator;
					//??this.ProrataDenominator = this.value.Value.ProrataDenominator;
					//??this.YearCount          = this.value.Value.YearCount;
					//??this.YearRank           = this.value.Value.YearRank;
					this.InitialAmount      = this.value.Value.InitialAmount;
					this.FinalAmount        = this.value.Value.FinalAmount;

					bool hasEntry = Entries.HasEntry (this.accessor, this.asset, this.e, this.value.Value);
					this.deleteEntryButton.Enable = hasEntry && !this.isReadOnly;
					this.showEntryButton  .Enable = hasEntry;
				}
				else
				{
					//??this.OutputAmount       = null;
					//??this.ForcedAmount       = null;
					//??this.BaseAmount         = null;
					//??this.InitialAmount      = null;
					//??this.ResidualAmount     = null;
					//??this.RoundAmount        = null;
					//??this.Rate               = null;
					//??this.PeriodicityFactor  = null;
					//??this.ProrataNumerator   = null;
					//??this.ProrataDenominator = null;
					//??this.YearCount          = null;
					//??this.YearRank           = null;
					this.InitialAmount      = null;
					this.FinalAmount        = null;

					this.deleteEntryButton.Enable = false;
					this.showEntryButton  .Enable = false;
				}

				//??AmortizedAmountController.UpdateMethod (this.methodTextFieldCombo);
				//??AmortizedAmountController.SetMethod (this.methodTextFieldCombo, this.AmortizationMethod);

				this.UpdateScenario (this.scenarioFieldCombo);
				AmortizedAmountController.SetScenario (this.scenarioFieldCombo, this.EntryScenario);

				if (this.IsAmortization)
				{
					//	Le montant final n'est pas éditable pour un événement d'amortissement.
					//	Il ne faut donc pas toucher à this.finalAmountTextField !
				}
				else
				{
					//??if (string.IsNullOrEmpty (this.outputAmountTextField.Text))
					//??{
					//??	this.outputAmountTextField.Name = "Required";
					//??	this.fieldColorTypes.Add (FieldColorType.Error);
					//??}
					//??else
					//??{
					//??	this.outputAmountTextField.Name = null;
					//??}
				}

				//??this.UpdateBackColor (this.methodTextFieldCombo);
				//??this.UpdateBackColor (this.outputAmountTextField);
				//??this.UpdateBackColor (this.forcedAmountTextField);
				//??this.UpdateBackColor (this.baseAmountTextField);
				//??this.UpdateBackColor (this.initialAmountTextField);
				//??this.UpdateBackColor (this.residualAmountTextField);
				//??this.UpdateBackColor (this.roundAmountTextField);
				//??this.UpdateBackColor (this.rateTextField);
				//??this.UpdateBackColor (this.periodicityFactorTextField);
				//??this.UpdateBackColor (this.prorataNumeratorTextField);
				//??this.UpdateBackColor (this.prorataDenominatorTextField);
				//??this.UpdateBackColor (this.yearCountTextField);
				//??this.UpdateBackColor (this.yearRankTextField);
				//??this.UpdateBackColor (this.scenarioFieldCombo);
				this.UpdateBackColor (this.initialAmountTextField);
				this.UpdateBackColor (this.finalAmountTextField);

				{
					var type = AbstractFieldController.GetFieldColorType (this.propertyState, isLocked: this.isReadOnly);
					this.fieldColorTypes.Add (type);
				}

				if (this.IsAmortization)
				{
					this.fieldColorTypes.Add (FieldColorType.Result);
				}

				this.UpdateEntry ();
			}
		}

		private void UpdateEntry()
		{
			if (this.value.HasValue)
			{
				this.value = Entries.CreateEntry (this.accessor, this.asset, this.e, this.value.Value);
			}

			this.entryController.Asset         = this.asset;
			this.entryController.Event         = this.e;
			this.entryController.Value         = this.value;
			this.entryController.PropertyState = this.propertyState;
			this.entryController.IsReadOnly    = this.isReadOnly;

			foreach (var type in this.entryController.FieldColorTypes)
			{
				this.fieldColorTypes.Add (type);
			}
		}


		//??private void UpdateForcedAmount()
		//??{
		//??	if (this.ignoreChanges.IsZero)
		//??	{
		//??		this.value = AmortizedAmount.SetForcedAmount (this.value.Value, this.ForcedAmount);
		//??		this.UpdateUI ();
		//??		this.OnValueEdited ();
		//??	}
		//??}
		//??
		//??private void UpdateRate()
		//??{
		//??	if (this.ignoreChanges.IsZero)
		//??	{
		//??		this.value = AmortizedAmount.SetRate (this.value.Value, this.Rate);
		//??		this.UpdateUI ();
		//??		this.OnValueEdited ();
		//??	}
		//??}
		//??
		//??private void UpdateResidualAmount()
		//??{
		//??	if (this.ignoreChanges.IsZero)
		//??	{
		//??		this.value = AmortizedAmount.SetResidualAmount (this.value.Value, this.ResidualAmount);
		//??		this.UpdateUI ();
		//??		this.OnValueEdited ();
		//??	}
		//??}
		//??
		//??private void UpdateRoundAmount()
		//??{
		//??	if (this.ignoreChanges.IsZero)
		//??	{
		//??		this.value = AmortizedAmount.SetRoundAmount (this.value.Value, this.RoundAmount);
		//??		this.UpdateUI ();
		//??		this.OnValueEdited ();
		//??	}
		//??}
		//??
		//??private void UpdateProrataNumerator()
		//??{
		//??	if (this.ignoreChanges.IsZero)
		//??	{
		//??		this.value = AmortizedAmount.SetProrataNumerator (this.value.Value, this.ProrataNumerator);
		//??		this.UpdateUI ();
		//??		this.OnValueEdited ();
		//??	}
		//??}
		//??
		//??private void UpdateProrataDenominator()
		//??{
		//??	if (this.ignoreChanges.IsZero)
		//??	{
		//??		this.value = AmortizedAmount.SetProrataDenominator (this.value.Value, this.ProrataDenominator);
		//??		this.UpdateUI ();
		//??		this.OnValueEdited ();
		//??	}
		//??}


		//??private decimal? OutputAmount
		//??{
		//??	get
		//??	{
		//??		return AmortizedAmountController.GetAmount (this.outputAmountTextField);
		//??	}
		//??	set
		//??	{
		//??		AmortizedAmountController.SetAmount (this.outputAmountTextField, value);
		//??	}
		//??}
		//??
		//??private decimal? ForcedAmount
		//??{
		//??	get
		//??	{
		//??		return AmortizedAmountController.GetAmount (this.forcedAmountTextField);
		//??	}
		//??	set
		//??	{
		//??		AmortizedAmountController.SetAmount (this.forcedAmountTextField, value);
		//??	}
		//??}
		//??
		//??private decimal? BaseAmount
		//??{
		//??	get
		//??	{
		//??		return AmortizedAmountController.GetAmount (this.baseAmountTextField);
		//??	}
		//??	set
		//??	{
		//??		AmortizedAmountController.SetAmount (this.baseAmountTextField, value);
		//??	}
		//??}

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

		//??private decimal? ResidualAmount
		//??{
		//??	get
		//??	{
		//??		return AmortizedAmountController.GetAmount (this.residualAmountTextField);
		//??	}
		//??	set
		//??	{
		//??		AmortizedAmountController.SetAmount (this.residualAmountTextField, value);
		//??	}
		//??}
		//??
		//??private decimal? RoundAmount
		//??{
		//??	get
		//??	{
		//??		return AmortizedAmountController.GetAmount (this.roundAmountTextField);
		//??	}
		//??	set
		//??	{
		//??		AmortizedAmountController.SetAmount (this.roundAmountTextField, value);
		//??	}
		//??}
		//??
		//??private decimal? Rate
		//??{
		//??	get
		//??	{
		//??		return AmortizedAmountController.GetRate (this.rateTextField);
		//??	}
		//??	set
		//??	{
		//??		AmortizedAmountController.SetRate (this.rateTextField, value);
		//??	}
		//??}
		//??
		//??private decimal? PeriodicityFactor
		//??{
		//??	get
		//??	{
		//??		return AmortizedAmountController.GetDecimal (this.periodicityFactorTextField);
		//??	}
		//??	set
		//??	{
		//??		AmortizedAmountController.SetDecimal (this.periodicityFactorTextField, value);
		//??	}
		//??}
		//??
		//??private decimal? ProrataNumerator
		//??{
		//??	get
		//??	{
		//??		return AmortizedAmountController.GetDecimal (this.prorataNumeratorTextField);
		//??	}
		//??	set
		//??	{
		//??		AmortizedAmountController.SetDecimal (this.prorataNumeratorTextField, value);
		//??	}
		//??}
		//??
		//??private decimal? ProrataDenominator
		//??{
		//??	get
		//??	{
		//??		return AmortizedAmountController.GetDecimal (this.prorataDenominatorTextField);
		//??	}
		//??	set
		//??	{
		//??		AmortizedAmountController.SetDecimal (this.prorataDenominatorTextField, value);
		//??	}
		//??}
		//??
		//??private decimal? YearCount
		//??{
		//??	get
		//??	{
		//??		return AmortizedAmountController.GetDecimal (this.yearCountTextField);
		//??	}
		//??	set
		//??	{
		//??		AmortizedAmountController.SetDecimal (this.yearCountTextField, value);
		//??	}
		//??}
		//??
		//??private decimal? YearRank
		//??{
		//??	get
		//??	{
		//??		return AmortizedAmountController.GetDecimal (this.yearRankTextField);
		//??	}
		//??	set
		//??	{
		//??		AmortizedAmountController.SetDecimal (this.yearRankTextField, value);
		//??	}
		//??}


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

		private static int? GetInt(TextField textField)
		{
			if (textField == null)
			{
				return null;
			}
			else
			{
				return TypeConverters.ParseInt (textField.Text);
			}
		}

		private static void SetInt(TextField textField, int? value)
		{
			if (textField != null)
			{
				if (AmortizedAmountController.GetDecimal (textField) != value)
				{
					textField.Text = TypeConverters.IntToString (value);
				}
			}
		}


		//??private string GetPeriodicity()
		//??{
		//??	if (this.value.HasValue)
		//??	{
		//??		int n = 12 / AmortizedAmount.GetPeriodMonthCount (this.value.Value.Periodicity);
		//??		var name = EnumDictionaries.GetPeriodicityName (this.value.Value.Periodicity);
		//??		return string.Format ("{0} ({1})", TypeConverters.IntToString (n), name);
		//??	}
		//??	else
		//??	{
		//??		return "1";
		//??	}
		//??}


		private static EntryScenario GetScenario(TextFieldCombo combo)
		{
			if (combo != null)
			{
				foreach (var scenario in EnumDictionaries.EnumEntryScenarios)
				{
					if (combo.Text == EnumDictionaries.GetEntryScenarioName (scenario))
					{
						return (EntryScenario) scenario;
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

				foreach (var scenario in EnumDictionaries.EnumEntryScenarios)
				{
					if (this.IsAmortization)
					{
						if (scenario != EntryScenario.AmortizationAuto &&
							scenario != EntryScenario.AmortizationExtra)
						{
							continue;
						}
					}
					else
					{
						if (scenario == EntryScenario.AmortizationAuto ||
							scenario == EntryScenario.AmortizationExtra)
						{
							continue;
						}
					}

					combo.Items.Add (EnumDictionaries.GetEntryScenarioName (scenario));
				}
			}
		}


		private void UpdateBackColor(AbstractTextField textField)
		{
			if (textField != null)
			{
				FieldColorType type;

				if (this.isReadOnly)
				{
					type = FieldColorType.Readonly;  // gris
				}
				else if (textField.Name == "IsReadOnly")
				{
					type = FieldColorType.Result;  // gris-bleu
				}
				else if (textField.Name == "Required")
				{
					type = FieldColorType.Error;  // orange
				}
				else
				{
					type = FieldColorType.Defined;  // bleu
				}

				var isReadOnly = textField.Name == "IsReadOnly" || this.isReadOnly;

				if (textField is TextFieldCombo)
				{
					AbstractFieldController.UpdateCombo (textField as TextFieldCombo, type, isReadOnly);
				}
				else
				{
					AbstractFieldController.UpdateTextField (textField, type, isReadOnly);
				}
			}
		}


		//??private bool UseBaseValue
		//??{
		//??	//	false -> il faut utiliser la dernière valeur
		//??	//	true  -> il faut utiliser la valeur d'achat
		//??	get
		//??	{
		//??		return this.AmortizationMethod == AmortizationMethod.RateLinear
		//??			&& this.AmortizationType == AmortizationType.Linear;
		//??	}
		//??}

		private bool IsAmortization
		{
			get
			{
				return true;
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
				if (this.e != null)
				{
					this.eventType = this.e.Type;
					return;
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


		private readonly DataAccessor			accessor;
		private readonly SafeCounter			ignoreChanges;
		private readonly HashSet<FieldColorType> fieldColorTypes;

		private DataObject						asset;
		private DataEvent						e;
		private AmortizedAmount?				value;
		private EventType						eventType;
		private PropertyState					propertyState;
		private bool							isReadOnly;

		//??private TextField						outputAmountTextField;
		//??private TextField						forcedAmountTextField;
		//??private TextField						baseAmountTextField;
		private TextField						initialAmountTextField;
		private TextField						finalAmountTextField;
		//??private TextField						residualAmountTextField;
		//??private TextField						roundAmountTextField;
		//??private TextField						rateTextField;
		//??private TextField						periodicityFactorTextField;
		//??private TextField						prorataNumeratorTextField;
		//??private TextField						prorataDenominatorTextField;
		//??private TextField						yearCountTextField;
		//??private TextField						yearRankTextField;

		//??private TextFieldCombo					methodTextFieldCombo;
		private TextFieldCombo					scenarioFieldCombo;

		private EntryController					entryController;
		private Button							deleteEntryButton;
		private Button							showEntryButton;

		private FrameBox[]						lines;

		private int								tabIndex;
	}
}
