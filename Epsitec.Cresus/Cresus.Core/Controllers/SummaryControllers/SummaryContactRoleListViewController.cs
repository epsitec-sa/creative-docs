//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	[ControllerSubType (3)]
	public class SummaryContactRoleListViewController : SummaryViewController<AbstractContactEntity>
	{
		public SummaryContactRoleListViewController(string name, AbstractContactEntity entity)
			: base (name, entity)
		{
		}


		protected override void CreateUI()
		{
			using (var data = TileContainerController.Setup (this))
			{
				this.CreateUIContactRoles (data);
			}
		}


		private void CreateUIContactRoles(SummaryDataItems data)
		{
			data.Add (
				new SummaryData
				{
					AutoGroup    = true,
					Name		 = "ContactRoles",
					IconUri		 = "Data.ContactRole",
					Title		 = TextFormatter.FormatText ("Tous les rôles connus"),
					CompactTitle = TextFormatter.FormatText ("Tous les rôles connus"),
					Text		 = CollectionTemplate.DefaultEmptyText,
				});

			var template = new CollectionTemplate<ContactRoleEntity> ("ContactRoles", this.BusinessContext);

			template.DefineText        (x => TextFormatter.FormatText (GetContactRoleSummary (x)));
			template.DefineCompactText (x => TextFormatter.FormatText (GetContactRoleSummary (x)));
//-			template.DefineUpdateMethod (() => this.Orchestrator.UpdateUI ());

			data.Add (this.CreateCollectionAccessor (template));
		}

		private static FormattedText GetContactRoleSummary(ContactRoleEntity group)
		{
			return TextFormatter.FormatText (group.Name);
		}
	}
}
