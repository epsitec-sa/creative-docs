﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.NodeGetters;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.ViewStates
{
	public class AmortizationsViewState : AbstractViewState
	{
		public Timestamp?						SelectedTimestamp;
		public Guid								SelectedGuid;


		public override bool StrictlyEquals(AbstractViewState other)
		{
			var o = other as AmortizationsViewState;
			if (o == null)
			{
				return false;
			}

			return this.ViewType          == o.ViewType
				&& this.PageType          == o.PageType
				&& this.SelectedTimestamp == o.SelectedTimestamp
				&& this.SelectedGuid      == o.SelectedGuid;
		}


		public override LastViewNode GetNavigationNode(DataAccessor accessor)
		{
			return new LastViewNode (this.guid, this.ViewType, this.PageType, this.SelectedTimestamp, this.GetDescription (accessor), this.Pin);
		}

		protected override string GetDescription(DataAccessor accessor)
		{
			if (!this.SelectedGuid.IsEmpty)
			{
				return AssetsLogic.GetSummary (accessor, this.SelectedGuid);
			}

			return null;
		}
	}
}