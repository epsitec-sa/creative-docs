//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.ViewStates
{
	public class EntriesViewState : AbstractViewState
	{
		public Guid								SelectedGuid;
		public bool								ShowGraphic;
		public ObjectField						SortingField;


		public override bool IsReferenced(BaseType baseType, Guid guid)
		{
			return baseType == BaseType.Entries
				&& guid == this.SelectedGuid;
		}


		public override bool StrictlyEquals(AbstractViewState other)
		{
			var o = other as EntriesViewState;
			if (o == null)
			{
				return false;
			}

			return this.ViewType     == o.ViewType
				&& this.SelectedGuid == o.SelectedGuid
				&& this.ShowGraphic  == o.ShowGraphic
				&& this.SortingField == o.SortingField;
		}


		protected override string GetDescription(DataAccessor accessor)
		{
			if (!this.SelectedGuid.IsEmpty)
			{
				return EntriesLogic.GetSummary (accessor, this.SelectedGuid);
			}

			return null;
		}
	}
}
