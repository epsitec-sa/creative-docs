//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business.Finance;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.BusinessLogic.Rules
{
	[BusinessRule (RuleType.Setup)]
	internal class BusinessBillingDetailSetupRule : GenericBusinessRule<BillingDetailEntity>
	{
		protected override void Apply(BillingDetailEntity billingDetails)
		{
			var dueDate      = Date.Today;
			var settings     = Logic.Current.BusinessSettings;
			var invoice      = Logic.Current.BusinessContext.GetMasterEntity<InvoiceDocumentEntity> ();
			var currencyCode = CurrencyCode.Chf;
			int paymentTerm  = 0;

			if (invoice.IsNotNull ())
			{
				if (invoice.BillingDate.HasValue)
				{
					dueDate = invoice.BillingDate.Value;
				}

				currencyCode = invoice.CurrencyCode;
			}


			billingDetails.AmountDue = Logic.Current.BusinessContext.CreateEntity<PaymentDetailEntity> ();
			billingDetails.AmountDue.PaymentType = Business.Finance.PaymentDetailType.AmountDue;
//			billingDetails.AmountDue.PaymentMode = Logic.Current.BusinessContext.GetLocalEntity (settings.FinanceSettings.DefaultPaymentMode);
			billingDetails.AmountDue.Currency    = Logic.Current.BusinessContext.GetLocalEntity (Logic.Current.Data.GetAllEntities<CurrencyEntity> ().Where (x => x.CurrencyCode == currencyCode).FirstOrDefault ());

			paymentTerm = billingDetails.AmountDue.PaymentMode.StandardPaymentTerm.GetValueOrDefault (30);
			dueDate     = dueDate.AddDays (paymentTerm);
				
			billingDetails.AmountDue.Date = dueDate;
			billingDetails.Title = string.Format ("Échéance au {0}", Misc.GetDateTimeDescription (dueDate));

			var isrDef = settings.FinanceSettings.IsrDefs.FirstOrDefault (x => x.Currency == currencyCode);

			if (isrDef.IsNull () == false)
			{
				billingDetails.IsrDefinition = isrDef;
				billingDetails.IsrReferenceNumber  = Isr.GetNewReferenceNumber (Logic.Current.Data, isrDef);
			}
		}
	}
}
