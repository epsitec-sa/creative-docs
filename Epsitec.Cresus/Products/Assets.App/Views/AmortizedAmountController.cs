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
using Epsitec.Cresus.Assets.App.DataFillers;
using Epsitec.Cresus.Assets.App.Popups;

namespace Epsitec.Cresus.Assets.App.Views
{
	/// <summary>
	/// Ce contrôleur est utilisé dans l'onglet "Valeur", pour montrer les valeurs initiales et
	/// finales (InitialAmount et FinalAmount) de la valeur comptable (AmortizedAmount).
	/// Pour certains événements, ces valeurs sont éditables.
	/// Il montre également l'écriture comptable générée, avec EntryController.
	/// </summary>
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
				this.UpdateUI ();
			}
		}

		public AmortizedAmount?					Value
		{
			get
			{
				return this.value;
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
			this.lines = new FrameBox[AmortizedAmountController.LineCount];

			for (int i=0; i<this.lines.Length; i++)
			{
				this.lines[i] = this.CreateFrame (parent);
			}

			{
				int y = AmortizedAmountController.InitialValueLine;
				this.CreateLabel (this.lines[y], 100, Res.Strings.AmortizedAmountController.InitialValue.ToString ());
				this.initialAmountTextField = this.CreateTextField (this.lines[y], AmortizedAmountController.AmountWidth, null, "CHF");
			}

			{
				int y = AmortizedAmountController.AmortizationLine;
				this.CreateLabel (this.lines[y], 100, Res.Strings.AmortizedAmountController.Amortization.ToString ());
				this.amortizationTextField = this.CreateTextField (this.lines[y], AmortizedAmountController.AmountWidth, null, "CHF", this.ChangeAmortization);
			}

			{
				int y = AmortizedAmountController.DecreaseLine;
				this.CreateLabel (this.lines[y], 100, Res.Strings.AmortizedAmountController.Decrease.ToString ());
				this.decreaseTextField = this.CreateTextField (this.lines[y], AmortizedAmountController.AmountWidth, null, "CHF", this.ChangeDecrease);
			}

			{
				int y = AmortizedAmountController.IncreaseLine;
				this.CreateLabel (this.lines[y], 100, Res.Strings.AmortizedAmountController.Increase.ToString ());
				this.increaseTextField = this.CreateTextField (this.lines[y], AmortizedAmountController.AmountWidth, null, "CHF", this.ChangeIncrease);
			}

			{
				int y = AmortizedAmountController.FinalValueLine;
				this.CreateLabel (this.lines[y], 100, Res.Strings.AmortizedAmountController.FinalValue.ToString ());
				this.finalAmountTextField = this.CreateTextField (this.lines[y], AmortizedAmountController.AmountWidth, null, "CHF", this.ChangeFinalAmount);
				this.CreateUnlockButton (this.lines[y]);
			}

			{
				int y = AmortizedAmountController.TraceLine;
				this.CreateLabel (this.lines[y], 100, Res.Strings.AmortizedAmountController.Trace.ToString ());
				this.traceTextField = this.CreateTextField (this.lines[y], AbstractFieldController.maxWidth, null, null);
			}

			{
				int y = AmortizedAmountController.ErrorLine;
				this.CreateLabel (this.lines[y], 100, Res.Strings.AmortizedAmountController.Error.ToString ());
				this.errorTextField = this.CreateTextField (this.lines[y], AbstractFieldController.maxWidth, null, null);
			}

			this.CreateEntryController (parent);
			this.UpdateUI ();
		}

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

		private void CreateUnlockButton(Widget parent)
		{
			this.unlockButton = new Button
			{
				Parent          = parent,
				Text            = Res.Strings.AmortizedAmountController.Button.Unlock.ToString (),
				ButtonStyle     = ButtonStyle.Icon,
				PreferredWidth  = 90,
				PreferredHeight = AbstractFieldController.lineHeight,
				Dock            = DockStyle.Left,
			};

			this.unlockButton.Clicked += delegate
			{
				var question = Res.Strings.AmortizedAmountController.Unlock.Question.ToString ();
				YesNoPopup.Show (this.unlockButton, question, delegate
				{
					this.DeleteFuturAmortizations ();
					this.SetFocus ();
				},
				leftOrRight: false);
			};
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

		private TextField CreateTextField(Widget parent, int width, string tooltip, string unit, System.Action action = null)
		{
			var field = new TextField
			{
				Parent           = parent,
				Dock             = DockStyle.Left,
				PreferredWidth   = width,
				PreferredHeight  = AbstractFieldController.lineHeight,
				Margins          = new Margins (0, 0, 0, 0),
				TabIndex         = ++this.tabIndex,
			};

			if (!string.IsNullOrEmpty (unit))
			{
				new StaticText
				{
					Parent           = parent,
					Text             = unit,
					PreferredWidth   = 40,
					PreferredHeight  = AbstractFieldController.lineHeight - 1,
					Dock             = DockStyle.Left,
					ContentAlignment = ContentAlignment.TopLeft,
					TextBreakMode    = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
					Margins          = new Margins (10, 0, 1, 0),
				};
			}

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
				this.fieldColorTypes.Clear ();

				if (this.value.HasValue)
				{
					this.InitialAmount =  this.value.Value.InitialAmount;
					this.Amortization  =  this.value.Value.Amortization;
					this.Decrease      =  this.value.Value.Amortization;
					this.Increase      = -this.value.Value.Amortization;
					this.FinalAmount   =  this.value.Value.FinalAmount;
					this.traceTextField.Text = ExpressionSimulationTreeTableFiller.ConvertTraceToSingleLine (this.value.Value.Trace);
					this.errorTextField.Text = ExpressionSimulationTreeTableFiller.ConvertTraceToSingleLine (this.value.Value.Error);

					bool hasEntry = Entries.HasEntry (this.accessor, this.asset, this.e, this.value.Value);
					this.deleteEntryButton.Enable = hasEntry && !this.isReadOnly;
					this.showEntryButton  .Enable = hasEntry;
				}
				else
				{
					this.InitialAmount = null;
					this.Amortization  = null;
					this.Decrease      = null;
					this.Increase      = null;
					this.FinalAmount   = null;
					this.traceTextField.Text = null;
					this.errorTextField.Text = null;

					this.deleteEntryButton.Enable = false;
					this.showEntryButton  .Enable = false;
				}

				this.UpdateScenario (this.scenarioFieldCombo);
				AmortizedAmountController.SetScenario (this.scenarioFieldCombo, this.EntryScenario);

				bool isFinalEnable = !this.IsAmortizationAuto;
				bool unlockEnable = false;
				if (isFinalEnable)
				{
					var asset = this.accessor.EditionAccessor.EditedObject;
					var timestamp = this.accessor.EditionAccessor.EditedTimestamp.GetValueOrDefault ();
					if (AssetsLogic.HasFuturAmortizations (asset, timestamp))
					{
						unlockEnable = true;
						isFinalEnable = false;
					}
				}

				bool isDelta = this.IsDelta && isFinalEnable && this.InitialAmount.HasValue;

				this.UpdateField (this.initialAmountTextField, isReadOnly: true);
				this.UpdateField (this.amortizationTextField,  isReadOnly: !isFinalEnable);
				this.UpdateField (this.decreaseTextField,      isReadOnly: !isDelta);
				this.UpdateField (this.increaseTextField,      isReadOnly: !isDelta);
				this.UpdateField (this.finalAmountTextField,   isReadOnly: !isFinalEnable);
				this.unlockButton.Visibility = unlockEnable;
				this.UpdateField (this.traceTextField,         isReadOnly: true);
				this.UpdateField (this.errorTextField,         isReadOnly: true);
				this.UpdateField (this.scenarioFieldCombo,     isReadOnly: false);

				this.lines[AmortizedAmountController.AmortizationLine].Visibility = this.IsAmortization;
				this.lines[AmortizedAmountController.DecreaseLine    ].Visibility = this.IsDelta;
				this.lines[AmortizedAmountController.IncreaseLine    ].Visibility = this.IsDelta;
				this.lines[AmortizedAmountController.TraceLine       ].Visibility = this.IsAmortizationAuto;
				this.lines[AmortizedAmountController.ErrorLine       ].Visibility = this.IsAmortizationAuto;

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


		private void ChangeFinalAmount()
		{
			this.value = AmortizedAmount.SetFinalAmount (this.value.Value, this.FinalAmount);

			using (this.ignoreChanges.Enter ())
			{
				this.Amortization =  this.value.Value.Amortization;
				this.Decrease     =  this.value.Value.Amortization;
				this.Increase     = -this.value.Value.Amortization;
			}

			this.OnValueEdited ();
		}

		private void ChangeAmortization()
		{
			this.value = AmortizedAmount.SetFinalAmount (this.value.Value, this.InitialAmount.GetValueOrDefault () - this.Amortization.GetValueOrDefault ());
			
			using (this.ignoreChanges.Enter ())
			{
				this.FinalAmount = this.value.Value.FinalAmount;
			}
			
			this.OnValueEdited ();
		}

		private void ChangeDecrease()
		{
			this.value = AmortizedAmount.SetFinalAmount (this.value.Value, this.InitialAmount.GetValueOrDefault () - this.Decrease.GetValueOrDefault ());

			using (this.ignoreChanges.Enter ())
			{
				this.FinalAmount =  this.value.Value.FinalAmount;
				this.Increase    = -this.value.Value.Amortization;
			}

			this.OnValueEdited ();
		}

		private void ChangeIncrease()
		{
			this.value = AmortizedAmount.SetFinalAmount (this.value.Value, this.InitialAmount.GetValueOrDefault () + this.Increase.GetValueOrDefault ());

			using (this.ignoreChanges.Enter ())
			{
				this.FinalAmount = this.value.Value.FinalAmount;
				this.Decrease    = this.value.Value.Amortization;
			}

			this.OnValueEdited ();
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

		private decimal? Amortization
		{
			get
			{
				return AmortizedAmountController.GetAmount (this.amortizationTextField);
			}
			set
			{
				AmortizedAmountController.SetAmount (this.amortizationTextField, value);
			}
		}

		private decimal? Decrease
		{
			get
			{
				return AmortizedAmountController.GetAmount (this.decreaseTextField);
			}
			set
			{
				AmortizedAmountController.SetAmount (this.decreaseTextField, value);
			}
		}

		private decimal? Increase
		{
			get
			{
				return AmortizedAmountController.GetAmount (this.increaseTextField);
			}
			set
			{
				AmortizedAmountController.SetAmount (this.increaseTextField, value);
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
				//	Si la valeur n'existe pas, il faut toujours effacer le champ.
				//	Si elle existe, on ne met à jour le champ que si elle a changé.
				if (value == null ||
					AmortizedAmountController.GetAmount (textField) != value)
				{
					textField.Text = TypeConverters.AmountToString (value);
				}
			}
		}


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
					if (this.IsAmortizationAuto)
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


		private void UpdateField(AbstractTextField textField, bool isReadOnly)
		{
			isReadOnly |= this.isReadOnly;
			var type = isReadOnly ? FieldColorType.Result : FieldColorType.Defined;

			if (textField is TextFieldCombo)
			{
				AbstractFieldController.UpdateCombo (textField as TextFieldCombo, type, isReadOnly);
			}
			else
			{
				AbstractFieldController.UpdateTextField (textField, type, isReadOnly);
			}

			this.fieldColorTypes.Add (type);
		}


		private void DeleteFuturAmortizations()
		{
			this.accessor.UndoManager.Start ();
			this.accessor.UndoManager.SetDescription (Res.Strings.AmortizedAmountController.Undo.DeleteAmortizations.ToString ());

			var a = new Amortizations (this.accessor);
			var guid = this.accessor.EditionAccessor.EditedObject.Guid;
			var date = this.accessor.EditionAccessor.EditedTimestamp.GetValueOrDefault ().Date.AddDays (1);
			a.Delete (date, guid);

			this.OnDataChanged ();
			this.OnDeepUpdate ();
			this.UpdateUI ();

			this.accessor.UndoManager.SetAfterViewState ();
		}


		private bool IsAmortization
		{
			get
			{
				return this.eventType == EventType.AmortizationPreview
					|| this.eventType == EventType.AmortizationAuto
					|| this.eventType == EventType.AmortizationExtra
					|| this.eventType == EventType.AmortizationSuppl;
			}
		}

		private bool IsAmortizationAuto
		{
			get
			{
				return this.eventType == EventType.AmortizationPreview
					|| this.eventType == EventType.AmortizationAuto;
			}
		}

		private bool IsDelta
		{
			get
			{
				return this.eventType == EventType.Input
					|| this.eventType == EventType.Decrease
					|| this.eventType == EventType.Increase
					|| this.eventType == EventType.Adjust;
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
				return !this.isReadOnly &&
					(this.eventType == EventType.AmortizationExtra || this.eventType == EventType.AmortizationSuppl);
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


		private void OnDataChanged()
		{
			this.DataChanged.Raise (this);
		}

		public event EventHandler DataChanged;


		private void OnDeepUpdate()
		{
			this.DeepUpdate.Raise (this);
		}

		public event EventHandler DeepUpdate;


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


		private const int InitialValueLine = 0;
		private const int AmortizationLine = 1;
		private const int DecreaseLine     = 2;
		private const int IncreaseLine     = 3;
		private const int FinalValueLine   = 4;
		private const int TraceLine        = 5;
		private const int ErrorLine        = 6;
		private const int LineCount        = 8;

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

		private TextField						initialAmountTextField;
		private TextField						amortizationTextField;
		private TextField						decreaseTextField;
		private TextField						increaseTextField;
		private TextField						finalAmountTextField;
		private Button							unlockButton;
		private TextField						traceTextField;
		private TextField						errorTextField;

		private TextFieldCombo					scenarioFieldCombo;
		private EntryController					entryController;
		private Button							deleteEntryButton;
		private Button							showEntryButton;

		private FrameBox[]						lines;

		private int								tabIndex;
	}
}
