//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.Finance;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Dialogs
{
	/// <summary>
	/// Dialogue pour choisir le mode de paiement lors de la création d'une facture.
	/// Attention, la facture n'est pas encore créée !
	/// </summary>
	public class CreateInvoiceDialog : CoreDialog
	{
		public CreateInvoiceDialog(BusinessContext businessContext, DocumentMetadataEntity sourceDocument, bool invoiceValidation)
			: base (businessContext.Data.Host)
		{
			//	'sourceDocument' correspond en principe au bulletin de livraison, car la facture
			//	n'est pas encore créée.
			this.businessContext   = businessContext;
			this.sourceDocument    = sourceDocument;
			this.invoiceValidation = invoiceValidation;

			this.paymentCategoryEntities = this.businessContext.GetAllEntities<PaymentCategoryEntity> ();
			this.dateConverter = new DateConverter ();
		}


		public PaymentTransactionEntity PaymentTransaction
		{
			get;
			set;
		}


		protected override void SetupWindow(Window window)
		{
			window.Text = this.invoiceValidation ? "Validation d'une facture directe" : "Création d'une facture";
			//?window.MakeFixedSizeWindow ();
			window.ClientSize = new Size (400, 350);
		}

		protected override void SetupWidgets(Window window)
		{
			var frame = new FrameBox
			{
				Parent = window.Root,
				Dock = DockStyle.Fill,
				Margins = new Margins (0, 0, 10, 0),
				TabIndex = ++this.tabIndex,
			};

			var footer = new FrameBox
			{
				Parent = window.Root,
				PreferredHeight = 20,
				Dock = DockStyle.Bottom,
				Margins = new Margins (10),
				TabIndex = ++this.tabIndex,
			};

			//	Crée la partie principale.
			this.CreateUIAmountDue (frame);
			this.CreateUISeparator (frame);
			this.CreateUIRadioButtons (frame);
			this.CreateUISeparator (frame);
			this.CreateUIDate (frame);
			this.CreateUIDueDate (frame);
			this.CreateUIText (frame);
			this.CreateUISeparator (frame, DockStyle.Bottom);

			//	Crée le pied de page.
			this.cancelButton = new Button ()
			{
				Parent = footer,
				Text = "Annuler",
				ButtonStyle = Common.Widgets.ButtonStyle.DefaultCancel,
				Dock = DockStyle.Right,
				Margins = new Margins (10, 0, 0, 0),
				TabIndex = 101,
			};

			this.acceptButton = new Button ()
			{
				Parent = footer,
				Text = this.invoiceValidation ? "Valider la facture" : "Créer la facture",
				PreferredWidth = 120,
				ButtonStyle = Common.Widgets.ButtonStyle.DefaultAccept,
				Dock = DockStyle.Right,
				TabIndex = 100,
			};

			this.acceptButton.Clicked += delegate
			{
				this.ExecuteOkCommand ();
			};

			this.cancelButton.Clicked += delegate
			{
				this.ExecuteCancelCommand ();
			};

			this.UpdateButtons ();
		}

		private void CreateUIAmountDue(Widget parent)
		{
			var frame = new FrameBox
			{
				Parent = parent,
				PreferredHeight = 20,
				Dock = DockStyle.Top,
				Margins = new Margins (10, 10, 0, 0),
				TabIndex = ++this.tabIndex,
			};

			new StaticText
			{
				Parent = frame,
				Text = "Montant dû",
				PreferredWidth = CreateInvoiceDialog.labelWidth,
				Dock = DockStyle.Left,
			};

			new TextField
			{
				Parent = frame,
				Text = Misc.PriceToString (this.AmountDue),
				IsReadOnly = true,
				PreferredWidth = CreateInvoiceDialog.fieldWidth,
				Dock = DockStyle.Left,
				TabIndex = ++this.tabIndex,
			};
		}

		private void CreateUIDate(Widget parent)
		{
			var frame = new FrameBox
			{
				Parent = parent,
				PreferredHeight = 20,
				Dock = DockStyle.Top,
				Margins = new Margins (10, 10, 0, 1),
				TabIndex = ++this.tabIndex,
			};

			new StaticText
			{
				Parent = frame,
				Text = "Date d'émission",
				PreferredWidth = CreateInvoiceDialog.labelWidth,
				Dock = DockStyle.Left,
			};

			this.dateField = new TextField
			{
				Parent = frame,
				PreferredWidth = CreateInvoiceDialog.fieldWidth,
				Dock = DockStyle.Left,
				TabIndex = ++this.tabIndex,
			};

			this.Date = Common.Types.Date.Today;

			this.dateField.TextChanged += delegate
			{
				this.UpdateButtons ();
			};
		}

		private void CreateUIDueDate(Widget parent)
		{
			var frame = new FrameBox
			{
				Parent = parent,
				PreferredHeight = 20,
				Dock = DockStyle.Top,
				Margins = new Margins (10, 10, 0, 1),
				TabIndex = ++this.tabIndex,
			};

			new StaticText
			{
				Parent = frame,
				Text = "Date d'échéance",
				PreferredWidth = CreateInvoiceDialog.labelWidth,
				Dock = DockStyle.Left,
			};

			this.dueDateField = new TextField
			{
				Parent = frame,
				PreferredWidth = CreateInvoiceDialog.fieldWidth,
				Dock = DockStyle.Left,
				TabIndex = ++this.tabIndex,
			};
		}

		private void CreateUIText(Widget parent)
		{
			var frame = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Fill,
				Margins = new Margins (10, 10, 0, 0),
				TabIndex = ++this.tabIndex,
			};

			new StaticText  // ce texte occupera plusieurs lignes
			{
				Parent = frame,
				Text = "Texte du pied de page",
				PreferredWidth = CreateInvoiceDialog.labelWidth-5,
				ContentAlignment = ContentAlignment.TopLeft,
				Dock = DockStyle.Left,
				Margins = new Margins (0, 5, 4, 0),
			};

			this.textField = new TextFieldMulti
			{
				Parent = frame,
				Dock = DockStyle.Fill,
				TabIndex = ++this.tabIndex,
			};

			this.textField.TextChanged += delegate
			{
				this.UpdateButtons ();
			};
		}

		private void CreateUIRadioButtons(Widget parent)
		{
			new StaticText
			{
				Parent = parent,
				Text = "Choix du mode de paiement :",
				Dock = DockStyle.Top,
				Margins = new Margins (10, 10, 0, 5),
			};

			int index = 0;

			foreach (var entity in this.paymentCategoryEntities)
			{
				var radio = new RadioButton
				{
					Parent = parent,
					FormattedText = entity.Name,
					Index = index++,
					Dock = DockStyle.Top,
					Margins = new Margins (10, 10, 0, 0),
					TabIndex = ++this.tabIndex,
				};

				radio.Clicked += delegate
				{
					this.paymentCategoryEntity = this.paymentCategoryEntities.ElementAt (radio.Index);
					this.UpdateText ();
					this.UpdateButtons ();
				};
			}
		}

		private void CreateUISeparator(Widget parent, DockStyle dockStyle = DockStyle.Top)
		{
			new Separator
			{
				Parent = parent,
				PreferredHeight = 1,
				IsHorizontalLine = true,
				Dock = dockStyle,
				Margins = new Margins (0, 0, 10, (dockStyle == DockStyle.Bottom) ? 0 : 10),
			};
		}


		private void UpdateText()
		{
			if (this.paymentCategoryEntity.Description.IsNullOrEmpty ())
			{
				this.Text = this.paymentCategoryEntity.Name;
			}
			else
			{
				this.Text = this.paymentCategoryEntity.Description;
			}
		}

		private void UpdateButtons()
		{
			bool dateOK = this.dateConverter.CanConvertFromString (this.dateField.Text);
			bool textOK = !this.textField.FormattedText.IsNullOrEmpty ();

			if (this.paymentCategoryEntity != null && dateOK)
			{
				this.dueDateField.Text = this.dateConverter.ConvertToString (this.DueDate);
			}
			else
			{
				this.dueDateField.Text = null;
			}

			this.acceptButton.Enable = (this.paymentCategoryEntity != null && dateOK && textOK);
		}


		private void ExecuteOkCommand()
		{
			this.PaymentTransaction = this.CreatePaymentTransaction ();

			this.Result = DialogResult.Accept;
			this.CloseDialog ();
		}

		private void ExecuteCancelCommand()
		{
			this.PaymentTransaction = null;

			this.Result = DialogResult.Cancel;
			this.CloseDialog ();
		}


		private Common.Types.Date DueDate
		{
			get
			{
				int term = 30;

				if (this.paymentCategoryEntity != null &&
					this.paymentCategoryEntity.StandardPaymentTerm.HasValue)
				{
					term = this.paymentCategoryEntity.StandardPaymentTerm.Value;
				}

				return this.Date.AddDays (term);
			}
		}

		private Common.Types.Date Date
		{
			get
			{
				var x = this.dateConverter.ConvertFromString (this.dateField.Text);

				if (x.HasValue)
				{
					return x.Value;
				}
				else
				{
					return Common.Types.Date.Today;
				}
			}
			set
			{
				this.dateField.Text = this.dateConverter.ConvertToString (value);
			}
		}

		private FormattedText Text
		{
			get
			{
				return this.textField.FormattedText;
			}
			set
			{
				this.textField.FormattedText = value;
			}
		}

		private decimal AmountDue
		{
			get
			{
				var endTotal = this.BusinessDocumentEntity.Lines.OfType<EndTotalDocumentItemEntity> ().FirstOrDefault ();
				decimal amountDue;

				if (endTotal.FixedPriceAfterTax.HasValue)
				{
					amountDue = endTotal.FixedPriceAfterTax.Value;
				}
				else
				{
					amountDue = endTotal.PriceAfterTax.GetValueOrDefault (0);
				}

				return PriceCalculator.ClipPriceValue (amountDue, this.BusinessDocumentEntity.CurrencyCode);
			}
		}


		private PaymentTransactionEntity CreatePaymentTransaction()
		{
			//	Crée l'entité PaymentTransactionEntity, qui sera attachée plus tard au document.
			var paymentTransaction = this.businessContext.CreateEntity<PaymentTransactionEntity> ();

			paymentTransaction.Text = this.Text;

			if (!this.paymentCategoryEntity.IsrDefinition.IsNull ())
			{
				PaymentTransaction.IsrReferenceNumber = Isr.GetNewReferenceNumber (this.businessContext, this.paymentCategoryEntity.IsrDefinition);
			}

			//	Initialise le PaymentDetailEntity.
			var paymentDetail = paymentTransaction.PaymentDetail;
			System.Diagnostics.Debug.Assert (paymentDetail != null);

			paymentDetail.PaymentType     = Business.Finance.PaymentDetailType.Due;
			paymentDetail.PaymentCategory = this.paymentCategoryEntity;
			paymentDetail.Amount          = this.AmountDue;
			paymentDetail.Currency        = this.GetDocumentCurrencyEntity ();
			paymentDetail.Date            = this.Date;
			paymentDetail.DueDate         = this.DueDate;

			return paymentTransaction;
		}

		private CurrencyEntity GetDocumentCurrencyEntity()
		{
			//	Retourne l'entité correspondant à la monnaie du document en cours.
			var example = new CurrencyEntity ();
			example.CurrencyCode = this.BusinessDocumentEntity.CurrencyCode;

			return this.businessContext.DataContext.GetByExample<CurrencyEntity> (example).FirstOrDefault ();
		}

		private BusinessDocumentEntity BusinessDocumentEntity
		{
			get
			{
				return this.sourceDocument.BusinessDocument as BusinessDocumentEntity;
			}
		}


		private static readonly double labelWidth = 90;
		private static readonly double fieldWidth = 75;

		private readonly BusinessContext						businessContext;
		private readonly DocumentMetadataEntity					sourceDocument;
		private readonly bool									invoiceValidation;
		private readonly IEnumerable<PaymentCategoryEntity>		paymentCategoryEntities;
		private readonly DateConverter							dateConverter;

		private int												tabIndex;
		private PaymentCategoryEntity							paymentCategoryEntity;
		private TextField										dateField;
		private TextField										dueDateField;
		private TextFieldMulti									textField;
		private Button											acceptButton;
		private Button											cancelButton;
	}
}
