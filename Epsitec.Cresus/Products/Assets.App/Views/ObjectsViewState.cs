//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.NodeGetters;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class ObjectsViewState : AbstractViewState, System.IEquatable<AbstractViewState>
	{
		public ViewMode							ViewMode;
		public bool								IsShowEvents;
		public Timestamp?						SelectedTimestamp;
		public Guid								SelectedGuid;


		#region IEquatable<AbstractViewState> Members
		public override bool Equals(AbstractViewState other)
		{
			if (!base.Equals (other))
			{
				return false;
			}

			var o = other as ObjectsViewState;

			if (o == null)
			{
				return false;
			}

			return this.ViewType          == o.ViewType
				&& this.ViewMode          == o.ViewMode
				&& this.PageType          == o.PageType
				&& this.IsShowEvents      == o.IsShowEvents
				&& this.SelectedTimestamp == o.SelectedTimestamp
				&& this.SelectedGuid      == o.SelectedGuid;
		}
		#endregion


		public override LastViewNode GetNavigationNode(DataAccessor accessor)
		{
			return new LastViewNode (this.guid, this.ViewType, this.PageType, this.SelectedTimestamp, this.GetDescription (accessor), this.Pin);
		}

		protected override string GetDescription(DataAccessor accessor)
		{
			if (!this.SelectedGuid.IsEmpty)
			{
				return ObjectsLogic.GetShortName (accessor, this.SelectedGuid);
			}

			return null;
		}
	}
}
