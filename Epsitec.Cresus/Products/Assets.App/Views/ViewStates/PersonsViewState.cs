//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.ViewStates
{
	public class PersonsViewState : AbstractViewState
	{
		public Guid								SelectedGuid;


		public override bool IsUsed(BaseType baseType, Guid guid)
		{
			return baseType == BaseType.Persons
				&& guid == this.SelectedGuid;
		}


		public override bool StrictlyEquals(AbstractViewState other)
		{
			var o = other as PersonsViewState;
			if (o == null)
			{
				return false;
			}

			return this.ViewType     == o.ViewType
				&& this.PageType     == o.PageType
				&& this.SelectedGuid == o.SelectedGuid;
		}


		protected override string GetDescription(DataAccessor accessor)
		{
			if (!this.SelectedGuid.IsEmpty)
			{
				var list = new List<string> ();

				list.Add (PersonsLogic.GetSummary (accessor, this.SelectedGuid));

				if (this.PageType != Views.PageType.Unknown)
				{
					list.Add (StaticDescriptions.GetObjectPageDescription (this.PageType));
				}

				return UniversalLogic.NiceJoin (list.ToArray ());
			}

			return null;
		}
	}
}
