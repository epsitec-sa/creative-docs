//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business.UserManagement;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Workflows;

namespace Epsitec.Cresus.Core.Business.Rules
{
	[BusinessRule]
	internal class AffairBusinessRules : GenericBusinessRule<AffairEntity>
	{
		public override void ApplySetupRule(AffairEntity affair)
		{
			var businessContext = Logic.Current.GetComponent<BusinessContext> ();
			var generatorPool   = Logic.Current.GetComponent<RefIdGeneratorPool> ();

			var currentCustomer = businessContext.GetMasterEntity<CustomerEntity> ();

			if (currentCustomer.IsNotNull ())
			{
				affair.Customer = currentCustomer;
			}

			businessContext.AssignIds (affair, generatorPool);

			affair.Workflow = WorkflowFactory.CreateDefaultWorkflow<AffairEntity> (businessContext);
			affair.ActiveAffairOwner = this.SearchActiveAffairOwner (businessContext);
		}

		private PeopleEntity SearchActiveAffairOwner(BusinessContext businessContext)
		{
			//	Cherche le collaborateur actuellement loggé pour initialiser le
			//	propriétaire de l'affaire.
			var coreData = businessContext.Data;
			var userManager = coreData.GetComponent<UserManager> ();

			return businessContext.GetLocalEntity (userManager.AuthenticatedUser.People);
		}


		public override void ApplyUpdateRule(AffairEntity affair)
		{
			var businessContext  = Logic.Current.GetComponent<BusinessContext> ();
			
			AffairBusinessRules.InitializeDefaults (businessContext, affair);
		}

		
		
		public static void InitializeDefaults(BusinessContext businessContext, AffairEntity affair)
		{
			var businessSettings = businessContext.GetCachedBusinessSettings ();
			var customer         = affair.Customer;

			if (customer.MainRelation.IsNotNull ())
			{
				if (affair.CurrencyCode == Finance.CurrencyCode.None)
				{
					affair.CurrencyCode = customer.MainRelation.DefaultCurrencyCode;
				}

				if (affair.BillingMode == Finance.BillingMode.None)
				{
					affair.BillingMode = customer.CustomerCategory.PriceGroup.BillingMode;
				}
				
				if (string.IsNullOrEmpty (affair.DebtorBookAccount))
				{
					affair.DebtorBookAccount = customer.DefaultDebtorBookAccount;
				}
			}

			if (affair.CurrencyCode == Finance.CurrencyCode.None)
			{
				affair.CurrencyCode = businessSettings.Finance.DefaultCurrencyCode.GetValueOrDefault (Finance.CurrencyCode.Chf);
			}
			
			if (affair.BillingMode == Finance.BillingMode.None)
			{
				affair.BillingMode = businessSettings.Finance.DefaultPriceGroup.BillingMode;
			}
			
			if (string.IsNullOrEmpty (affair.DebtorBookAccount))
			{
				affair.DebtorBookAccount = businessSettings.Finance.DefaultDebtorBookAccount;
			}
		}
	}
}
