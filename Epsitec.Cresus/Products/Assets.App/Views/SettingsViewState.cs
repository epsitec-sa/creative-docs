//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class SettingsViewState : AbstractViewState, System.IEquatable<AbstractViewState>
	{
		public BaseType							BaseType;
		public Guid								SelectedGuid;


		#region IEquatable<AbstractViewState> Members
		public override bool Equals(AbstractViewState other)
		{
			if (!base.Equals (other))
			{
				return false;
			}

			var o = other as SettingsViewState;

			if (o == null)
			{
				return false;
			}

			return this.ViewType     == o.ViewType
				&& this.BaseType     == o.BaseType
				&& this.SelectedGuid == o.SelectedGuid;
		}
		#endregion


		protected override string GetDescription(DataAccessor accessor)
		{
			string name, sel;

			switch (this.BaseType)
			{
				case BaseType.Assets:
					name = StaticDescriptions.GetViewTypeDescription (ViewType.AssetsSettings);
					sel = UserFieldsLogic.GetSummary (accessor, this.SelectedGuid);
					break;

				case BaseType.Persons:
					name = StaticDescriptions.GetViewTypeDescription (ViewType.PersonsSettings);
					sel = UserFieldsLogic.GetSummary (accessor, this.SelectedGuid);
					break;

				case BaseType.Accounts:
					name = StaticDescriptions.GetViewTypeDescription (ViewType.AccountsSettings);
					sel = AccountsLogic.GetSummary (accessor, this.SelectedGuid);
					break;

				default:
					throw new System.InvalidOperationException (string.Format ("Unsupported BaseType {0}", this.BaseType.ToString ()));
			}

			if (string.IsNullOrEmpty (sel))
			{
				return name;
			}
			else
			{
				return string.Concat (name, ", ", sel);
			}
		}
	}
}
