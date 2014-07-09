//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class SettingsViewState : AbstractViewState
	{
		public BaseType							BaseType;
		public Guid								SelectedGuid;
		public bool								ShowGraphic;


		public override bool StrictlyEquals(AbstractViewState other)
		{
			var o = other as SettingsViewState;
			if (o == null)
			{
				return false;
			}

			return this.ViewType     == o.ViewType
				&& this.BaseType     == o.BaseType
				&& this.SelectedGuid == o.SelectedGuid;
		}


		protected override string GetDescription(DataAccessor accessor)
		{
			switch (this.BaseType.Kind)
			{
				case BaseTypeKind.Assets:
					return UserFieldsLogic.GetSummary (accessor, this.SelectedGuid);

				case BaseTypeKind.Persons:
					return UserFieldsLogic.GetSummary (accessor, this.SelectedGuid);

				default:
					throw new System.InvalidOperationException (string.Format ("Unsupported BaseType {0}", this.BaseType.ToString ()));
			}
		}
	}
}
