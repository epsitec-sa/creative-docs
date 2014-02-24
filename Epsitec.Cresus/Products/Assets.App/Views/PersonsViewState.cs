//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class PersonsViewState : AbstractViewState, System.IEquatable<AbstractViewState>
	{
		public Guid								SelectedGuid;


		#region IEquatable<AbstractViewState> Members
		public override bool Equals(AbstractViewState other)
		{
			if (!base.Equals (other))
			{
				return false;
			}

			var o = other as PersonsViewState;

			if (o == null)
			{
				return false;
			}

			return this.ViewType     == o.ViewType
				&& this.PageType     == o.PageType
				&& this.SelectedGuid == o.SelectedGuid;
		}
		#endregion


		protected override string GetDescription(DataAccessor accessor)
		{
			if (!this.SelectedGuid.IsEmpty)
			{
				return PersonsLogic.GetSummary (accessor, this.SelectedGuid);
			}

			return null;
		}
	}
}
