//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.ViewStates
{
	public class WarningsViewState : AbstractViewState
	{
		public string							PersistantUniqueId;


		public override bool ApproximatelyEquals(AbstractViewState other)
		{
			var o = other as WarningsViewState;
			if (o == null)
			{
				return false;
			}

			return this.ViewType == o.ViewType;
		}

		public override bool StrictlyEquals(AbstractViewState other)
		{
			var o = other as WarningsViewState;
			if (o == null)
			{
				return false;
			}

			return this.ViewType           == o.ViewType
				&& this.PersistantUniqueId == o.PersistantUniqueId;
		}


		protected override string GetDescription(DataAccessor accessor)
		{
			return null;
		}
	}
}
