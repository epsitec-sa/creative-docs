//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;

namespace Epsitec.Cresus.Core
{
	public class CoreCommands
	{
		public CoreCommands(CoreApplication application)
		{
			this.application = application;
			this.application.CommandDispatcher.RegisterController (this);
		}


		[Command (Mai2008.Res.CommandIds.Edition.Accept)]
		public void EditionAccept()
		{
			if (this.application.EndEdit (true))
			{
				return;
			}
		}

		[Command (Mai2008.Res.CommandIds.Edition.Cancel)]
		public void EditionCancel()
		{
			if (this.application.EndEdit (false))
			{
				return;
			}
		}

		[Command (Mai2008.Res.CommandIds.Edition.Edit)]
		public void EditionEdit()
		{
			States.CoreState          state     = this.application.StateManager.ActiveState;
			States.FormWorkspaceState formState = state as States.FormWorkspaceState;

			if (formState != null)
			{
				AbstractEntity entity = formState.CurrentEntity;

				if (entity != null)
				{
					System.Diagnostics.Debug.Assert (EntityContext.IsSearchEntity (entity) == false);

					Druid formId = formState.Workspace.FormId;

					this.application.StartEdit (entity, formId);
				}
			}
		}

		[Command (Mai2008.Res.CommandIds.SwitchToBase.BillOut)]
		public void SwitchToBaseBillOut()
		{
			this.application.StartNewSearch (Mai2008.Entities.FactureEntity.EntityStructuredTypeId, Mai2008.FormIds.Facture);
		}

		[Command (Mai2008.Res.CommandIds.SwitchToBase.Customers)]
		public void SwitchToBaseCustomers()
		{
			this.application.StartNewSearch (AddressBook.Entities.AdressePersonneEntity.EntityStructuredTypeId, AddressBook.FormIds.AdressePersonne);
		}



		private readonly CoreApplication application;
	}
}
