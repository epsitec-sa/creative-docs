//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.Core.Helpers;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionBillingDetailViewController : EditionViewController<Entities.BillingDetailEntity>
	{
#if true
		protected override void CreateBricks(Bricks.BrickWall<BillingDetailEntity> wall)
		{
			wall.AddBrick ()
				.Input ()
				  .Field (x => x.Text)
				.End ()
				.Separator ()
				.Input ()
				  .Field (x => x.AmountDue.PaymentType)
				  .Field (x => x.AmountDue.PaymentMode)
				  .Field (x => x.AmountDue.Amount).Width (100)
				  .Field (x => x.AmountDue.Currency)
				  .Field (x => x.AmountDue.Date).Width (150)
				  .Field (x => x.AmountDue.PaymentData)
				.End ()
				.Separator ()
				.Input ()
				  .Field (x => x.InstalmentRank).Width (100)
				  .Field (x => x.InstalmentName)
				.End ()
				.Separator ()
				.Input ()
				  .Field (x => x.TransactionId)
				  .Field (x => x.IsrReferenceNumber)
				  .Field (x => x.IsrDefinition)
				.End ()
				;
		}
#else
		protected override void CreateUI()
		{
			using (var builder = new UIBuilder (this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.BillingDetails", "Facturation");

				this.CreateUIMain (builder);
				this.CreateUIPaymentMode (builder);

				builder.CreateFooterEditorTile ();
			}
		}


		private void CreateUIMain(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateTextField      (tile, 100, "Date d'échéance",                     Marshaler.Create (() => this.Entity.AmountDue.Date, x => this.Entity.AmountDue.Date = x));
			builder.CreateTextFieldMulti (tile,  36, "Description",                         Marshaler.Create (() => this.Entity.Text, x => this.Entity.Text = x));
			builder.CreateMargin         (tile, horizontalSeparator: true);
			builder.CreateTextField      (tile,  50, "Rang de la mensualité",               Marshaler.Create (() => this.InstalmentRank, x => this.InstalmentRank = x));
			builder.CreateTextField      (tile,   0, "Description de la mensualité",        Marshaler.Create (() => this.Entity.InstalmentName, x => this.Entity.InstalmentName = x));
			builder.CreateMargin         (tile, horizontalSeparator: true);
			builder.CreateTextField      (tile,   0, "N° de référence BVR à 27 chiffres",	Marshaler.Create (() => this.Entity.IsrReferenceNumber, x => this.Entity.IsrReferenceNumber = x));
			builder.CreateMargin         (tile, horizontalSeparator: true);

			FrameBox group = builder.CreateGroup (tile, "Montant à payer");
			             builder.CreateTextField (group, DockStyle.Left, 80, Marshaler.Create (() => this.Entity.AmountDue.Amount, x => this.Entity.AmountDue.Amount = x));
			var button = builder.CreateButton    (group, DockStyle.Fill, 0, "&lt; Mettre le solde");

			button.Clicked += delegate
			{
				this.ComputeAmontDue ();
			};
		}

		private void CreateUIPaymentMode(UIBuilder builder)
		{
			var controller = new SelectionController<PaymentModeEntity> (this.BusinessContext)
			{
				ValueGetter         = () => this.Entity.AmountDue.PaymentMode,
				ValueSetter         = x => this.Entity.AmountDue.PaymentMode = x,
				ReferenceController = new ReferenceController (() => this.Entity.AmountDue.PaymentMode, creator: this.CreateNewPaymentMode),
			};

			builder.CreateAutoCompleteTextField ("Mode de paiement", controller);
		}

		private NewEntityReference CreateNewPaymentMode(DataContext context)
		{
			var paymentMode = context.CreateEntityAndRegisterAsEmpty<PaymentModeEntity> ();
			return paymentMode;
		}


		private string InstalmentRank
		{
			get
			{
				if (this.Entity.InstalmentRank == null)
				{
					return null;
				}
				else
				{
					return (this.Entity.InstalmentRank+1).ToString ();
				}
			}
			set
			{
				if (!string.IsNullOrEmpty (value))
				{
					int n;
					if (int.TryParse (value, out n))
					{
						if (n > 0)
						{
							this.Entity.InstalmentRank = n-1;
							return;
						}
					}
				}

				this.Entity.InstalmentRank = null;
			}
		}


		private void ComputeAmontDue()
		{
			var invoiceDocument = Common.GetParentEntity (this.TileContainer) as BusinessDocumentEntity;

			if (invoiceDocument != null)
			{
				this.Entity.AmountDue.Amount = 0;
				decimal amountDue = 0;

				foreach (var billing in invoiceDocument.BillingDetails)
				{
					amountDue += billing.AmountDue.Amount;
				}

				decimal total = InvoiceDocumentHelper.GetTotalPriceTTC (invoiceDocument).GetValueOrDefault (0);
				this.Entity.AmountDue.Amount = total-amountDue;

				this.TileContainer.UpdateAllWidgets ();
			}
		}
#endif
	}
}
