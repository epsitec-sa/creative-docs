﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
		public EditionBillingDetailViewController(string name, Entities.BillingDetailEntity entity)
			: base (name, entity)
		{
		}

		protected override void CreateUI(TileContainer container)
		{
			this.tileContainer = container;

			using (var builder = new UIBuilder (container, this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.BillingDetails", "Facturation");

				this.CreateUIMain (builder);
				this.CreateUIPaymentMode (builder);

				builder.CreateFooterEditorTile ();
			}
		}


		protected override EditionStatus GetEditionStatus()
		{
			if (string.IsNullOrEmpty (this.Entity.Title))
			{
				return EditionStatus.Empty;
			}

			return EditionStatus.Valid;
		}

		protected override void UpdateEmptyEntityStatus(DataLayer.Context.DataContext context, bool isEmpty)
		{
			var entity = this.Entity;

			context.UpdateEmptyEntityStatus (entity, isEmpty);
		}


		private void CreateUIMain(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateTextField      (tile, 100, "Date d'échéance",                       Marshaler.Create (() => this.Entity.AmountDue.Date, x => this.Entity.AmountDue.Date = x));
			builder.CreateTextFieldMulti (tile,  36, "Description",                           Marshaler.Create (() => this.Entity.Title, x => this.Entity.Title = x));
			builder.CreateMargin         (tile, horizontalSeparator: true);
			builder.CreateTextField      (tile,  50, "Rang de la mensualité",                 Marshaler.Create (() => this.InstalmentRank, x => this.InstalmentRank = x));
			builder.CreateTextField      (tile,   0, "Description de la mensualité",          Marshaler.Create (() => this.Entity.InstalmentName, x => this.Entity.InstalmentName = x));
			builder.CreateMargin         (tile, horizontalSeparator: true);
			builder.CreateTextField      (tile, 100, "CCP du destinataire",                   Marshaler.Create (() => this.Entity.EsrCustomerNumber, x => this.Entity.EsrCustomerNumber = x));
			builder.CreateTextField      (tile,   0, "Numéro de référence BVR à 27 chiffres", Marshaler.Create (() => this.Entity.EsrReferenceNumber, x => this.Entity.EsrReferenceNumber = x));
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
			builder.CreateAutoCompleteTextField ("Mode de paiement",
				new SelectionController<PaymentModeEntity>
				{
					ValueGetter = () => this.Entity.AmountDue.PaymentMode,
					ValueSetter = x => this.Entity.AmountDue.PaymentMode = x.WrapNullEntity (),
					ReferenceController = new ReferenceController (() => this.Entity.AmountDue.PaymentMode, creator: this.CreateNewPaymentMode),
					PossibleItemsGetter = () => CoreProgram.Application.Data.GetPaymentModes (),

					ToTextArrayConverter     = x => new string[] { x.Name },
					ToFormattedTextConverter = x => TextFormater.FormatText (x.Name)
				});
		}

		private NewEntityReference CreateNewPaymentMode(DataContext context)
		{
			var paymentMode = context.CreateEmptyEntity<PaymentModeEntity> ();
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
			var invoiceDocument = Common.GetParentEntity (this.tileContainer) as InvoiceDocumentEntity;

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

				this.tileContainer.UpdateAllWidgets ();
			}
		}



		private TileContainer tileContainer;
	}
}
