//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.NodeGetters;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.ViewStates
{
	public class AccountsViewState : AbstractViewState
	{
		public bool								ShowGraphic;


		public override LastViewNode GetNavigationNode(DataAccessor accessor)
		{
			var timestamp = new Timestamp (this.ViewType.AccountsDateRange.IncludeFrom, 0);
			return new LastViewNode (this.guid, this.ViewType, this.PageType, timestamp, this.GetDescription (accessor), this.Pin);
		}

		protected override string GetDescription(DataAccessor accessor)
		{
			if (!this.ViewType.AccountsDateRange.IsEmpty)
			{
				return string.Format ("Période {0}", this.ViewType.AccountsDateRange.ToNiceString ());
			}

			return null;
		}
	}
}
