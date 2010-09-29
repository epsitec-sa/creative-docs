//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public class SummaryBusinessSettingsViewController : SummaryViewController<Entities.BusinessSettingsEntity>
	{
		public SummaryBusinessSettingsViewController(string name, Entities.BusinessSettingsEntity entity)
			: base (name, entity)
		{
		}


		protected override void CreateUI()
		{
			using (var data = TileContainerController.Setup (this))
			{
				this.CreateUIMain (data);
				this.CreateUIIsrDefinitions (data);
				this.CreateUIPaymentReminderDefinitions (data);
			}
		}

		private void CreateUIMain(SummaryDataItems data)
		{
			data.Add (
				new SummaryData
				{
					Name				= "BusinessSettings",
					IconUri				= "Data.BusinessSettings",
					Title				= TextFormatter.FormatText ("Réglages de l'entreprise"),
					CompactTitle		= TextFormatter.FormatText ("Réglages de l'entreprise"),
					TextAccessor		= this.CreateAccessor (x => TextFormatter.FormatText (this.Entity.Company.Person.GetSummary ())),
					CompactTextAccessor = this.CreateAccessor (x => TextFormatter.FormatText (this.Entity.Company.Person.GetSummary ())),
					EntityMarshaler		= this.CreateEntityMarshaler (),
				});
		}

		private void CreateUIIsrDefinitions(SummaryDataItems data)
		{
			data.Add (
				new SummaryData
				{
					AutoGroup    = true,
					Name		 = "IsrDefinition",
					IconUri		 = "Data.IsrDefinition",
					Title		 = TextFormatter.FormatText ("Contrats BVR"),
					CompactTitle = TextFormatter.FormatText ("Contrats BVR"),
					Text		 = CollectionTemplate.DefaultEmptyText,
				});

			var template = new CollectionTemplate<IsrDefinitionEntity> ("IsrDefinition", this.BusinessContext);

			template.DefineText (x => x.GetSummary ());
			template.DefineCompactText (x => x.GetSummary ());

			data.Add (this.CreateCollectionAccessor (template, x => x.FinanceSettings.IsrDefs));
		}

		private void CreateUIPaymentReminderDefinitions(SummaryDataItems data)
		{
			data.Add (
				new SummaryData
				{
					AutoGroup    = true,
					Name		 = "PaymentReminderDefinition",
					IconUri		 = "Data.PaymentReminderDefinition",
					Title		 = TextFormatter.FormatText ("Rappels"),
					CompactTitle = TextFormatter.FormatText ("Rappels"),
					Text		 = CollectionTemplate.DefaultEmptyText,
				});

			var template = new CollectionTemplate<PaymentReminderDefinitionEntity> ("PaymentReminderDefinition", this.BusinessContext);

			template.DefineText (x => x.GetSummary ());
			template.DefineCompactText (x => x.GetSummary ());

			data.Add (this.CreateCollectionAccessor (template, x => x.FinanceSettings.PaymentReminderDefs));
		}

	}
}
