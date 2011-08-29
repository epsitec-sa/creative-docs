//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Business.Finance;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Rules
{
	[BusinessRule]
	internal class PaymentTransactionBusinessRules : GenericBusinessRule<PaymentTransactionEntity>
	{
		public override void ApplySetupRule(PaymentTransactionEntity payment)
		{
			var businessContext = Logic.Current.GetComponent<BusinessContext> ();
			var generatorPool   = Logic.Current.GetComponent<RefIdGeneratorPool> ();

			payment.Code = (string) ItemCodeGenerator.NewCode ();
			
			payment.PaymentDetail = businessContext.CreateEntity<PaymentDetailEntity> ();
			payment.PaymentDetail.PaymentType = Business.Finance.PaymentDetailType.None;
		}

		public static void CreateInvoicePaymentTransaction(BusinessContext businessContext)
		{
			var payment = businessContext.CreateEntity<PaymentTransactionEntity> ();

			System.Diagnostics.Debug.Assert (string.IsNullOrEmpty (payment.Code) == false);
			System.Diagnostics.Debug.Assert (payment.PaymentDetail.IsNotNull ());

			var dueDate      = Date.Today;
			var settings     = businessContext.GetCachedBusinessSettings ();
			var invoice      = businessContext.GetMasterEntity<BusinessDocumentEntity> ();
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

			var currencyEntity  = businessContext.GetAllEntities<CurrencyEntity> ().FirstOrDefault (x => x.CurrencyCode == currencyCode);
			var paymentCategory = settings.Finance.PaymentCategories.FirstOrDefault ();

			payment.PaymentDetail.PaymentType     = Business.Finance.PaymentDetailType.Due;
			payment.PaymentDetail.PaymentCategory = businessContext.GetLocalEntity (paymentCategory);
			payment.PaymentDetail.Currency        = businessContext.GetLocalEntity (currencyEntity);

			paymentTerm = payment.PaymentDetail.PaymentCategory.StandardPaymentTerm.GetValueOrDefault (30);
			dueDate     = dueDate.AddDays (paymentTerm);

			payment.PaymentDetail.Date = dueDate;
			payment.Text = string.Format ("Payable net au {0}", Misc.GetDateTimeDescription (dueDate));

			var isrDef = settings.Finance.IsrDefs.FirstOrDefault (x => x.Currency == currencyCode);

			if (isrDef.IsNotNull ())
			{
				payment.IsrDefinition      = businessContext.GetLocalEntity (isrDef);
				payment.IsrReferenceNumber = Isr.GetNewReferenceNumber (businessContext, isrDef);
			}
		}
	}
}
