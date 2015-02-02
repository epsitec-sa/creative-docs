//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.App.Views.FieldControllers;

namespace Epsitec.Cresus.Assets.App.Views
{
	/// <summary>
	/// Contrôleur permettant de saisir une écriture.
	/// </summary>
	public class EntryController
	{
		public EntryController(DataAccessor accessor)
		{
			this.accessor = accessor;

			this.ignoreChanges = new SafeCounter ();
			this.fieldColorTypes = new HashSet<FieldColorType> ();

			this.lastEntrySeed = -1;
		}


		public DataObject						Asset;
		public DataEvent						Event;

		public AmortizedAmount?					Value
		{
			get
			{
				return this.value;
			}
			set
			{
				if (this.value != value || this.lastEntrySeed != this.CurrentEntrySeed)
				{
					this.value = value;
					this.lastEntrySeed = this.CurrentEntrySeed;

					this.UpdateUI ();
				}
			}
		}

		public AmortizedAmount?					ValueNoEditing
		{
			set
			{
				if (this.value != value || this.lastEntrySeed != this.CurrentEntrySeed)
				{
					this.value = value;
					this.lastEntrySeed = this.CurrentEntrySeed;

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

		public bool								HasError
		{
			get
			{
				return this.dateController   .HasError
					|| this.debitController  .HasError
					|| this.creditController .HasError
					|| this.stampController  .HasError
					|| this.titleController  .HasError
					|| this.amountController .HasError
					|| this.vatCodeController.HasError;
			}
		}

		public IEnumerable<FieldColorType>		FieldColorTypes
		{
			get
			{
				return this.fieldColorTypes;
			}
		}


		public void Clear()
		{
			this.accessor.EditionAccessor.SetField (ObjectField.AssetEntryForcedAmount, 0.0m);
			this.OnValueEdited ();
		}

		public void CreateUI(Widget parent)
		{
			this.tabIndex = 0;

			this.CreateDateController    (parent);
			this.CreateDebitController   (parent);
			this.CreateCreditController  (parent);
			this.CreateStampController   (parent);
			this.CreateTitleController   (parent);
			this.CreateAmountController  (parent);
			this.CreateVatCodeController (parent);

			this.UpdateUI ();
		}

		private void CreateDateController(Widget parent)
		{
			this.dateController = new DateFieldController (this.accessor)
			{
				Field                 = ObjectField.Unknown,
				Label                 = Res.Strings.EntryController.Date.ToString (),
				HideAdditionalButtons = false,
				DateRangeCategory     = DateRangeCategory.Mandat,
				TabIndex              = this.tabIndex,
			};

			this.dateController.CreateUI (parent);
			this.tabIndex = this.dateController.TabIndex;

			this.dateController.ValueEdited += delegate (object sender, ObjectField of)
			{
				this.SetDate (this.dateController.Value);
			};
		}

		private void CreateDebitController(Widget parent)
		{
			this.debitController = new AccountFieldController (this.accessor)
			{
				Field                 = ObjectField.Unknown,
				Label                 = Res.Strings.EntryController.Debit.ToString (),
				EditWidth             = AbstractFieldController.maxWidth,
				HideAdditionalButtons = false,
				TabIndex              = this.tabIndex,
			};

			this.debitController.CreateUI (parent);
			this.tabIndex = this.debitController.TabIndex;

			this.debitController.ValueEdited += delegate (object sender, ObjectField of)
			{
				this.SetDebit (this.debitController.Value);
			};
		}

		private void CreateCreditController(Widget parent)
		{
			this.creditController = new AccountFieldController (this.accessor)
			{
				Field                 = ObjectField.Unknown,
				Label                 = Res.Strings.EntryController.Credit.ToString (),
				EditWidth             = AbstractFieldController.maxWidth,
				HideAdditionalButtons = false,
				TabIndex              = this.tabIndex,
			};

			this.creditController.CreateUI (parent);
			this.tabIndex = this.creditController.TabIndex;

			this.creditController.ValueEdited += delegate (object sender, ObjectField of)
			{
				this.SetCredit (this.creditController.Value);
			};
		}

		private void CreateStampController(Widget parent)
		{
			this.stampController = new StringFieldController (this.accessor)
			{
				Field                 = ObjectField.Unknown,
				Label                 = Res.Strings.EntryController.Stamp.ToString (),
				EditWidth             = 100,
				LineCount             = 1,
				HideAdditionalButtons = false,
				TabIndex              = this.tabIndex,
			};

			this.stampController.CreateUI (parent);
			this.tabIndex = this.stampController.TabIndex;

			this.stampController.ValueEdited += delegate (object sender, ObjectField of)
			{
				this.SetStamp (this.stampController.Value);
			};
		}

		private void CreateTitleController(Widget parent)
		{
			this.titleController = new StringFieldController (this.accessor)
			{
				Field                 = ObjectField.Unknown,
				Label                 = Res.Strings.EntryController.Title.ToString (),
				EditWidth             = AbstractFieldController.maxWidth,
				LineCount             = 1,
				HideAdditionalButtons = false,
				TabIndex              = this.tabIndex,
			};

			this.titleController.CreateUI (parent);
			this.tabIndex = this.titleController.TabIndex;

			this.titleController.ValueEdited += delegate (object sender, ObjectField of)
			{
				this.SetTitle (this.titleController.Value);
			};
		}

		private void CreateAmountController(Widget parent)
		{
			this.amountController = new DecimalFieldController (this.accessor)
			{
				Field                 = ObjectField.Unknown,
				Label                 = Res.Strings.EntryController.Amount.ToString (),
				DecimalFormat         = DecimalFormat.Amount,
				HideAdditionalButtons = false,
				TabIndex              = this.tabIndex,
			};

			this.amountController.CreateUI (parent);
			this.tabIndex = this.amountController.TabIndex;

			this.amountController.ValueEdited += delegate (object sender, ObjectField of)
			{
				this.SetAmount (this.amountController.Value);
			};
		}

		private void CreateVatCodeController(Widget parent)
		{
			this.vatCodeController = new VatCodeFieldController (this.accessor)
			{
				Field                 = ObjectField.Unknown,
				Label                 = Res.Strings.EntryController.VatCode.ToString (),
				EditWidth             = AbstractFieldController.maxWidth,
				HideAdditionalButtons = false,
				TabIndex              = this.tabIndex,
			};

			this.vatCodeController.CreateUI (parent);
			this.tabIndex = this.vatCodeController.TabIndex;

			this.vatCodeController.ValueEdited += delegate (object sender, ObjectField of)
			{
				this.SetVatCode (this.vatCodeController.Value);
			};
		}


		private void SetDate(System.DateTime? value)
		{
			this.accessor.EditionAccessor.SetField (ObjectField.AssetEntryForcedDate, value);
			this.OnValueEdited ();
		}

		private void SetDebit(string value)
		{
			this.accessor.EditionAccessor.SetField (ObjectField.AssetEntryForcedDebit, value);
			this.OnValueEdited ();
		}

		private void SetCredit(string value)
		{
			this.accessor.EditionAccessor.SetField (ObjectField.AssetEntryForcedCredit, value);
			this.OnValueEdited ();
		}

		private void SetStamp(string value)
		{
			this.accessor.EditionAccessor.SetField (ObjectField.AssetEntryForcedStamp, value);
			this.OnValueEdited ();
		}

		private void SetTitle(string value)
		{
			this.accessor.EditionAccessor.SetField (ObjectField.AssetEntryForcedTitle, value);
			this.OnValueEdited ();
		}

		private void SetAmount(decimal? value)
		{
			this.accessor.EditionAccessor.SetField (ObjectField.AssetEntryForcedAmount, value);
			this.OnValueEdited ();
		}

		private void SetVatCode(string value)
		{
			this.accessor.EditionAccessor.SetField (ObjectField.AssetEntryForcedVatCode, value);
			this.OnValueEdited ();
		}



		public void UpdateNoEditingUI()
		{
			this.UpdateUI ();
		}

		private void UpdateUI()
		{
			this.fieldColorTypes.Clear ();

			if (this.value.HasValue)
			{
				this.debitController  .Date = this.accessor.EditionAccessor.EventDate;
				this.creditController .Date = this.accessor.EditionAccessor.EventDate;
				this.vatCodeController.Date = this.accessor.EditionAccessor.EventDate;

				EntryProperties baseProperties = null;
				EntryProperties editProperties = null;

				using (var entries = new Entries (this.accessor))
				{
					baseProperties = entries.GetEntryProperties (this.Asset, this.Event, this.value.Value, Entries.GetEntryPropertiesType.Base);
					editProperties = entries.GetEntryProperties (this.Asset, this.Event, this.value.Value, Entries.GetEntryPropertiesType.EditedOrBase);
				}

				this.UpdatePropertyState (this.dateController,    baseProperties.Date    == editProperties.Date);
				this.UpdatePropertyState (this.debitController,   baseProperties.Debit   == editProperties.Debit);
				this.UpdatePropertyState (this.creditController,  baseProperties.Credit  == editProperties.Credit);
				this.UpdatePropertyState (this.stampController,   baseProperties.Stamp   == editProperties.Stamp);
				this.UpdatePropertyState (this.titleController,   baseProperties.Title   == editProperties.Title);
				this.UpdatePropertyState (this.amountController,  baseProperties.Amount  == editProperties.Amount);
				this.UpdatePropertyState (this.vatCodeController, baseProperties.VatCode == editProperties.VatCode);

				this.dateController   .Value = editProperties.Date;
				this.debitController  .Value = editProperties.Debit;
				this.creditController .Value = editProperties.Credit;
				this.stampController  .Value = editProperties.Stamp;
				this.titleController  .Value = editProperties.Title;
				this.amountController .Value = editProperties.Amount;
				this.vatCodeController.Value = editProperties.VatCode;
			}
			else
			{
				this.dateController   .PropertyState = this.propertyState;
				this.debitController  .PropertyState = this.propertyState;
				this.creditController .PropertyState = this.propertyState;
				this.stampController  .PropertyState = this.propertyState;
				this.titleController  .PropertyState = this.propertyState;
				this.amountController .PropertyState = this.propertyState;
				this.vatCodeController.PropertyState = this.propertyState;

				this.dateController   .IsReadOnly = this.isReadOnly;
				this.debitController  .IsReadOnly = this.isReadOnly;
				this.creditController .IsReadOnly = this.isReadOnly;
				this.stampController  .IsReadOnly = this.isReadOnly;
				this.titleController  .IsReadOnly = this.isReadOnly;
				this.amountController .IsReadOnly = this.isReadOnly;
				this.vatCodeController.IsReadOnly = this.isReadOnly;

				this.dateController   .Value = null;
				this.debitController  .Value = null;
				this.creditController .Value = null;
				this.stampController  .Value = null;
				this.titleController  .Value = null;
				this.amountController .Value = null;
				this.vatCodeController.Value = null;

				var type = AbstractFieldController.GetFieldColorType (this.propertyState, isLocked: this.isReadOnly);
				this.fieldColorTypes.Add (type);
			}
		}

		private void UpdatePropertyState(AbstractFieldController fieldController, bool equal)
		{
			//	Si le champ est défini automatiquement (equal = true), le contrôleur a un fond
			//	bleu clair. S'il est défini manuellement, il a un fond bleu.

			fieldController.PropertyState = equal ? PropertyState.Automatic : PropertyState.Single;
			fieldController.IsReadOnly = this.isReadOnly;

			if (fieldController.IsReadOnly)
			{
				this.fieldColorTypes.Add (FieldColorType.Readonly);
			}
			else if (fieldController.HasError)
			{
				this.fieldColorTypes.Add (FieldColorType.Error);
			}
			else
			{
				this.fieldColorTypes.Add (equal ? FieldColorType.Automatic : FieldColorType.Defined);
			}
		}


		private int CurrentEntrySeed
		{
			get
			{
				if (this.value.HasValue)
				{
					return this.value.Value.EntrySeed;
				}
				else
				{
					return -1;
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
		protected const int EditWidth  = 90;


		private readonly DataAccessor			accessor;
		private readonly SafeCounter			ignoreChanges;
		private readonly HashSet<FieldColorType> fieldColorTypes;

		private AmortizedAmount?				value;
		private int								lastEntrySeed;
		private bool							isReadOnly;
		private Color							backgroundColor;
		private PropertyState					propertyState;

		private int								tabIndex;
		private DateFieldController				dateController;
		private AccountFieldController			debitController;
		private AccountFieldController			creditController;
		private StringFieldController			stampController;
		private StringFieldController			titleController;
		private DecimalFieldController			amountController;
		private VatCodeFieldController			vatCodeController;
	}
}
