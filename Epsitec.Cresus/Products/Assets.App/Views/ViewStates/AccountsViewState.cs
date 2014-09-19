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
		public string							SelectedAccount;  // voir (*)
		public bool								ShowGraphic;

		//	(*)
		//	SelectedAccount n'est volontairement pas "historié". Ainsi, le retour à une
		//	vue "plan comptable" contenant un compte sélectionné ne le sélectionne plus.
		//	De même, la sélection d'un autre compte ne génère pas une nouvelle ligne dans
		//	l'historique.
		//	Cela se justifie car les comptes ne sont pas éditables.
		//	Cette propriété sert uniquement pour le bouton "goto" de AccountFieldController,
		//	afin de sélectionner le bon compte.


		public override bool StrictlyEquals(AbstractViewState other)
		{
			var o = other as AccountsViewState;
			if (o == null)
			{
				return false;
			}

			return this.ViewType    == o.ViewType
				&& this.ShowGraphic == o.ShowGraphic;
		}


		public override LastViewNode GetNavigationNode(DataAccessor accessor)
		{
			var timestamp = new Timestamp (this.ViewType.AccountsDateRange.IncludeFrom, 0);
			return new LastViewNode (this.guid, this.ViewType, ViewMode.Unknown, EventType.Unknown, this.PageType, timestamp, this.GetDescription (accessor), this.Pin);
		}

		protected override string GetDescription(DataAccessor accessor)
		{
			if (!this.ViewType.AccountsDateRange.IsEmpty)
			{
				return string.Format (Res.Strings.ViewStates.Accounts.Description.ToString (), this.ViewType.AccountsDateRange.ToNiceString ());
			}

			return null;
		}
	}
}
