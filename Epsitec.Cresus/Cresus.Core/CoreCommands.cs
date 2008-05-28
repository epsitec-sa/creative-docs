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

		[Command (Mai2008.Res.CommandIds.Edition.New)]
		public void EditionNew()
		{
			if (this.application.CreateRecord ())
			{
				return;
			}
		}

		[Command (Mai2008.Res.CommandIds.Edition.Delete)]
		public void EditionDelete()
		{
			//	TODO: implement delete
		}

		[Command (Mai2008.Res.CommandIds.Edition.Edit)]
		public void EditionEdit()
		{
			this.application.StartEdit ();
		}

		[Command (Mai2008.Res.CommandIds.SwitchToBase.BillOut)]
		public void SwitchToBaseBillOut()
		{
			this.application.StartNewSearch (Mai2008.Entities.FactureEntity.EntityStructuredTypeId, Mai2008.FormIds.Facture);
		}

		[Command (Mai2008.Res.CommandIds.SwitchToBase.Customers)]
		public void SwitchToBaseCustomers()
		{
			this.application.StartNewSearch (Mai2008.Entities.ClientEntity.EntityStructuredTypeId, Mai2008.FormIds.Client);
		}

		[Command (Mai2008.Res.CommandIds.SwitchToBase.Items)]
		public void SwitchToBaseItems()
		{
			this.application.StartNewSearch (Mai2008.Entities.ArticleEntity.EntityStructuredTypeId, Mai2008.FormIds.Article);
		}



		[Command (Mai2008.Res.CommandIds.Quick.CreateBillForCustomer)]
		public void QuickCreateBillForCustomer()
		{
			States.FormWorkspaceState formState = this.application.GetCurrentFormWorkspaceState ();
			
			if ((formState != null) &&
				(formState.Workspace.CurrentItem != null))
			{
				Mai2008.Entities.ClientEntity address = formState.Workspace.CurrentItem as Mai2008.Entities.ClientEntity;

				if (address != null)
				{
					this.application.CreateRecord (Mai2008.Entities.FactureEntity.EntityStructuredTypeId,
						null,
						(e) => (e as Mai2008.Entities.FactureEntity).AdresseFacturation = address);
				}
			}
		}


		[Command (Core.Res.CommandIds.Test.Crash)]
		public void TestCrash()
		{
			throw new System.Exception ("Crashing the application on purpose");
		}

		private readonly CoreApplication application;
	}
}
