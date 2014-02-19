//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class SettingsViewState : AbstractViewState, System.IEquatable<AbstractViewState>
	{
		public ToolbarCommand					SelectedCommand;


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

			return this.ViewType        == o.ViewType
				&& this.SelectedCommand == o.SelectedCommand;
		}
		#endregion


		protected override string GetDescription(DataAccessor accessor)
		{
			return SettingsToolbar.GetCommandDescription (this.SelectedCommand);
		}
	}
}
