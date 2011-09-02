//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Dialogs
{
	/// <summary>
	/// Dialogue pour choisir le mode de paiement lors de la création d'une facture.
	/// </summary>
	public class CreateInvoiceDialog : CoreDialog
	{
		public CreateInvoiceDialog(IBusinessContext businessContext)
			: base (businessContext.Data.Host)
		{
			this.businessContext = businessContext;
			this.paymentCategoryEntities = this.businessContext.GetAllEntities<PaymentCategoryEntity> ();
		}


		public PaymentTransactionEntity PaymentTransaction
		{
			get;
			set;
		}


		protected override void SetupWindow(Window window)
		{
			window.Text = "Création d'une facture";
			window.MakeFixedSizeWindow ();
			window.ClientSize = new Size (250, 200);
		}

		protected override void SetupWidgets(Window window)
		{
			var frame = new FrameBox
			{
				Parent = window.Root,
				Dock = DockStyle.Fill,
				Margins = new Margins (10, 10, 10, 0),
			};

			var footer = new FrameBox
			{
				Parent = window.Root,
				PreferredHeight = 20,
				Dock = DockStyle.Bottom,
				Margins = new Margins (10, 10, 10, 10),
			};

			//	Crée la partie principale.
			new StaticText
			{
				Parent = frame,
				Text = "Choix du mode de paiement :",
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, 10),
			};

			this.CreateUIRadioButtons (frame);

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
				Text = "Créer la facture",
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

		private void CreateUIRadioButtons(Widget parent)
		{
			int index = 0;

			foreach (var entity in this.paymentCategoryEntities)
			{
				var radio = new RadioButton
				{
					Parent = parent,
					FormattedText = entity.Name,
					Index = index++,
					Dock = DockStyle.Top,
				};

				radio.Clicked += delegate
				{
					this.paymentCategoryEntity = this.paymentCategoryEntities.ElementAt (radio.Index);
					this.UpdateButtons ();
				};
			}
		}

		private void UpdateButtons()
		{
			this.acceptButton.Enable = (this.paymentCategoryEntity != null);
		}


		private void ExecuteOkCommand()
		{
			this.PaymentTransaction = this.MakePaymentTransaction ();

			this.Result = DialogResult.Accept;
			this.CloseDialog ();
		}

		private void ExecuteCancelCommand()
		{
			this.PaymentTransaction = null;

			this.Result = DialogResult.Cancel;
			this.CloseDialog ();
		}


		private PaymentTransactionEntity MakePaymentTransaction()
		{
			//	Crée l'entité PaymentDetailEntity.
			var paymentDetail = this.businessContext.CreateEntity<PaymentDetailEntity> ();

			paymentDetail.PaymentType = Business.Finance.PaymentDetailType.Due;
			paymentDetail.PaymentCategory = this.paymentCategoryEntity;
			//?paymentDetail.Currency = ???

			int term = this.paymentCategoryEntity.StandardPaymentTerm.HasValue ? this.paymentCategoryEntity.StandardPaymentTerm.Value : 30;
			paymentDetail.Date    = Common.Types.Date.Today;
			paymentDetail.DueDate = Common.Types.Date.Today.AddDays (term);

			//	Crée l'entité PaymentTransactionEntity.
			var paymentTransaction = this.businessContext.CreateEntity<PaymentTransactionEntity> ();

			paymentTransaction.Text = this.paymentCategoryEntity.Name;  // TODO: juste ?
			paymentTransaction.PaymentDetail = paymentDetail;

			return paymentTransaction;
		}



		private readonly IBusinessContext						businessContext;
		private readonly IEnumerable<PaymentCategoryEntity>		paymentCategoryEntities;

		private PaymentCategoryEntity							paymentCategoryEntity;
		private Button											acceptButton;
		private Button											cancelButton;
	}
}
