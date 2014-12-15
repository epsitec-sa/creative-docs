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
			this.lines = new FrameBox[4];

			for (int i=0; i<this.lines.Length; i++)
			{
				this.lines[i] = this.CreateFrame (parent);
			}

			this.CreateLabel (this.lines[0], 100, "Valeur initiale");
			this.initialAmountTextField = this.CreateTextField (this.lines[0], AmortizedAmountController.AmountWidth, null);

			this.CreateLabel (this.lines[1], 100, "Valeur finale");
			this.finalAmountTextField = this.CreateTextField (this.lines[1], AmortizedAmountController.AmountWidth, null, this.ChangeFinalAmount);

			this.CreateLabel (this.lines[2], 100, "Trace");
			this.traceTextField = this.CreateTextField (this.lines[2], AbstractFieldController.maxWidth, null);

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

			new StaticText
			{
				Parent           = parent,
				Dock             = DockStyle.Fill,
				PreferredHeight  = AbstractFieldController.lineHeight - 1,
				Text             = "CHF",
				ContentAlignment = ContentAlignment.TopLeft,
				TextBreakMode    = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
				Margins          = new Margins (10, 0, 1, 0),
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
					this.InitialAmount = this.value.Value.InitialAmount;
					this.FinalAmount   = this.value.Value.FinalAmount;
					this.traceTextField.Text = ExpressionSimulationTreeTableFiller.ConvertTraceToSingleLine (this.value.Value.Trace);

					bool hasEntry = Entries.HasEntry (this.accessor, this.asset, this.e, this.value.Value);
					this.deleteEntryButton.Enable = hasEntry && !this.isReadOnly;
					this.showEntryButton  .Enable = hasEntry;
				}
				else
				{
					this.InitialAmount = null;
					this.FinalAmount   = null;
					this.traceTextField.Text = null;

					this.deleteEntryButton.Enable = false;
					this.showEntryButton  .Enable = false;
				}

				this.UpdateScenario (this.scenarioFieldCombo);
				AmortizedAmountController.SetScenario (this.scenarioFieldCombo, this.EntryScenario);

				this.UpdateField (this.initialAmountTextField, true);
				this.UpdateField (this.finalAmountTextField, !this.IsFinalEnable);
				this.UpdateField (this.traceTextField, true);
				this.UpdateField (this.scenarioFieldCombo, false);

				this.lines[2].Visibility = !this.IsFinalEnable;

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
				if (AmortizedAmountController.GetAmount (textField) != value)
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
					if (this.IsFinalEnable)
					{
						if (scenario == EntryScenario.AmortizationAuto ||
							scenario == EntryScenario.AmortizationExtra)
						{
							continue;
						}
					}
					else
					{
						if (scenario != EntryScenario.AmortizationAuto &&
							scenario != EntryScenario.AmortizationExtra)
						{
							continue;
						}
					}

					combo.Items.Add (EnumDictionaries.GetEntryScenarioName (scenario));
				}
			}
		}


		private void UpdateField(AbstractTextField textField, bool isReadonly)
		{
			isReadonly |= this.isReadOnly;

			var type = isReadonly ? FieldColorType.Result : FieldColorType.Defined;

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


		private bool IsFinalEnable
		{
			get
			{
				return this.eventType != EventType.AmortizationPreview
					&& this.eventType != EventType.AmortizationAuto;
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

		private TextField						initialAmountTextField;
		private TextField						finalAmountTextField;
		private TextField						traceTextField;

		private TextFieldCombo					scenarioFieldCombo;
		private EntryController					entryController;
		private Button							deleteEntryButton;
		private Button							showEntryButton;

		private FrameBox[]						lines;

		private int								tabIndex;
	}
}
