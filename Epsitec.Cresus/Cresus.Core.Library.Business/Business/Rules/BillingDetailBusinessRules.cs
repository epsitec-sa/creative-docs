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
	internal class BillingDetailBusinessRules : GenericBusinessRule<BillingDetailEntity>
	{
		public override void ApplySetupRule(BillingDetailEntity billingDetails)
		{
			var context      = Logic.Current.GetComponent<BusinessContext> ();
			var dueDate      = Date.Today;
			var settings     = context.GetCachedBusinessSettings ();
			var invoice      = context.GetMasterEntity<BusinessDocumentEntity> ();
			var currencyCode = CurrencyCode.Chf;
			int paymentTerm  = 0;

			if (invoice.IsNotNull ())
			{
				if (invoice.BillingDate.HasValue)
				{
					dueDate = invoice.BillingDate.Value;
				}
				if (invoice.CurrencyCode != CurrencyCode.None)
				{
					currencyCode = invoice.CurrencyCode;
				}
			}

			var currencyEntity = context.GetAllEntities<CurrencyEntity> ().FirstOrDefault (x => x.CurrencyCode == currencyCode);
			var paymentMode    = settings.Finance.PaymentModes.FirstOrDefault ();

			billingDetails.AmountDue = context.CreateEntity<PaymentDetailEntity> ();
			billingDetails.AmountDue.PaymentType = Business.Finance.PaymentDetailType.AmountDue;
			billingDetails.AmountDue.PaymentMode = context.GetLocalEntity (paymentMode);
			billingDetails.AmountDue.Currency    = context.GetLocalEntity (currencyEntity);

			paymentTerm = billingDetails.AmountDue.PaymentMode.StandardPaymentTerm.GetValueOrDefault (30);
			dueDate     = dueDate.AddDays (paymentTerm);

			billingDetails.AmountDue.Date = dueDate;
			billingDetails.Text = string.Format ("Échéance au {0}", Misc.GetDateTimeDescription (dueDate));

			var isrDef = settings.Finance.IsrDefs.FirstOrDefault (x => x.Currency == currencyCode);

			if (isrDef.IsNotNull ())
			{
				billingDetails.IsrDefinition      = context.GetLocalEntity (isrDef);
				billingDetails.IsrReferenceNumber = Isr.GetNewReferenceNumber (context, isrDef);
			}
		}
	}
}
