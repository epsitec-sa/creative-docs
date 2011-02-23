//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business.Finance;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Rules
{
	[BusinessRule]
	internal class BusinessBillingDetailBusinessRules : GenericBusinessRule<BillingDetailEntity>
	{
		public override void ApplySetupRule(BillingDetailEntity billingDetails)
		{
			throw new System.NotImplementedException ();
#if false
			var context      = Logic.Current.BusinessContext;
			var dueDate      = Date.Today;
			throw new System.NotImplementedException ();
			var settings     = Logic.Current.BusinessSettings;
			var invoice      = Logic.Current.BusinessContext.GetMasterEntity<BusinessDocumentEntity> ();
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
			billingDetails.Text = string.Format ("Échéance au {0}", Misc.GetDateTimeDescription (dueDate));

			var isrDef = settings.Finance.IsrDefs.FirstOrDefault (x => x.Currency == currencyCode);

			if (isrDef.IsNotNull ())
			{
				billingDetails.IsrDefinition = context.GetLocalEntity (isrDef);
				billingDetails.IsrReferenceNumber = Isr.GetNewReferenceNumber (Logic.Current.Data, isrDef);
			}
#endif
		}
	}
}
