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
	public class EntryController
	{
		public EntryController(DataAccessor accessor)
		{
			this.accessor = accessor;
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
					this.UpdateUI ();
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

			using (this.ignoreChanges.Enter ())
			{
			}
		}

		public void CreateUI(Widget parent)
		{
			this.tabIndex = 0;
			this.CreateDateController (parent);
			this.CreateDebitController (parent);
			this.CreateCreditController (parent);
			this.CreateStampController (parent);
			this.CreateTitleController (parent);
			this.CreateAmountController (parent);

			this.UpdateUI ();
		}

		private void CreateDateController(Widget parent)
		{
			this.dateController = new DateFieldController (this.accessor)
			{
				Field                 = ObjectField.Unknown,
				Label                 = "Date",
				HideAdditionalButtons = true,
				TabIndex              = ++this.tabIndex,
			};

			this.dateController.CreateUI (parent);

			this.dateController.ValueEdited += delegate (object sender, ObjectField of)
			{
			};
		}

		private void CreateDebitController(Widget parent)
		{
			this.debitController = new AccountGuidFieldController (this.accessor)
			{
				Accessor              = this.accessor,
				Field                 = ObjectField.Unknown,
				Label                 = "Débit",
				EditWidth             = AbstractFieldController.maxWidth,
				HideAdditionalButtons = true,
				TabIndex              = ++this.tabIndex,
			};

			this.debitController.CreateUI (parent);

			this.debitController.ValueEdited += delegate (object sender, ObjectField of)
			{
			};
		}

		private void CreateCreditController(Widget parent)
		{
			this.creditController = new AccountGuidFieldController (this.accessor)
			{
				Accessor              = this.accessor,
				Field                 = ObjectField.Unknown,
				Label                 = "Crédit",
				EditWidth             = AbstractFieldController.maxWidth,
				HideAdditionalButtons = true,
				TabIndex              = ++this.tabIndex,
			};

			this.creditController.CreateUI (parent);

			this.creditController.ValueEdited += delegate (object sender, ObjectField of)
			{
			};
		}

		private void CreateStampController(Widget parent)
		{
			this.stampController = new StringFieldController (this.accessor)
			{
				Field                 = ObjectField.Unknown,
				Label                 = "Pièce",
				EditWidth             = 100,
				LineCount             = 1,
				HideAdditionalButtons = true,
				TabIndex              = ++this.tabIndex,
			};

			this.stampController.CreateUI (parent);

			this.stampController.ValueEdited += delegate (object sender, ObjectField of)
			{
			};
		}

		private void CreateTitleController(Widget parent)
		{
			this.titleController = new StringFieldController (this.accessor)
			{
				Field                 = ObjectField.Unknown,
				Label                 = "Libellé",
				EditWidth             = AbstractFieldController.maxWidth,
				LineCount             = 1,
				HideAdditionalButtons = true,
				TabIndex              = ++this.tabIndex,
			};

			this.titleController.CreateUI (parent);

			this.titleController.ValueEdited += delegate (object sender, ObjectField of)
			{
			};
		}

		private void CreateAmountController(Widget parent)
		{
			this.amountController = new DecimalFieldController (this.accessor)
			{
				Field                 = ObjectField.Unknown,
				Label                 = "Montant",
				DecimalFormat         = DecimalFormat.Amount,
				HideAdditionalButtons = true,
				TabIndex              = ++this.tabIndex,
			};

			this.amountController.CreateUI (parent);

			this.amountController.ValueEdited += delegate (object sender, ObjectField of)
			{
			};
		}


		public void SetFocus()
		{
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
			this.dateController  .PropertyState = this.propertyState;
			this.debitController .PropertyState = this.propertyState;
			this.creditController.PropertyState = this.propertyState;
			this.stampController .PropertyState = this.propertyState;
			this.titleController .PropertyState = this.propertyState;
			this.amountController.PropertyState = this.propertyState;

			if (this.value.HasValue)
			{
				var entry = this.accessor.GetObject (BaseType.Entries, this.value.Value.EntryGuid);

				if (entry != null)
				{
					var date   = ObjectProperties.GetObjectPropertyDate    (entry, null, ObjectField.EntryDate);
					var debit  = ObjectProperties.GetObjectPropertyGuid    (entry, null, ObjectField.EntryDebitAccount);
					var credit = ObjectProperties.GetObjectPropertyGuid    (entry, null, ObjectField.EntryCreditAccount);
					var stamp  = ObjectProperties.GetObjectPropertyString  (entry, null, ObjectField.EntryStamp);
					var title  = ObjectProperties.GetObjectPropertyString  (entry, null, ObjectField.EntryTitle);
					var amount = ObjectProperties.GetObjectPropertyDecimal (entry, null, ObjectField.EntryAmount);

					this.dateController  .Value = date;
					this.debitController .Value = debit;
					this.creditController.Value = credit;
					this.stampController .Value = stamp;
					this.titleController .Value = title;
					this.amountController.Value = amount;

					return;
				}
			}

			this.dateController  .Value = null;
			this.debitController .Value = Guid.Empty;
			this.creditController.Value = Guid.Empty;
			this.stampController .Value = null;
			this.titleController .Value = null;
			this.amountController.Value = null;
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

		private AmortizedAmount?				value;
		private bool							isReadOnly;
		private Color							backgroundColor;
		private PropertyState					propertyState;

		private int								tabIndex;
		private DateFieldController				dateController;
		private AccountGuidFieldController		debitController;
		private AccountGuidFieldController		creditController;
		private StringFieldController			stampController;
		private StringFieldController			titleController;
		private DecimalFieldController			amountController;
	}
}
