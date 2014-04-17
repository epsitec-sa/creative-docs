﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class ReportsViewState : AbstractViewState, System.IEquatable<AbstractViewState>
	{
		public string							SelectedReportId;


		#region IEquatable<AbstractViewState> Members
		public override bool Equals(AbstractViewState other)
		{
			if (!base.Equals (other))
			{
				return false;
			}

			var o = other as ReportsViewState;

			if (o == null)
			{
				return false;
			}

			return this.ViewType         == o.ViewType
				&& this.SelectedReportId == o.SelectedReportId;
		}
		#endregion


		protected override string GetDescription(DataAccessor accessor)
		{
			if (!string.IsNullOrEmpty (this.SelectedReportId))
			{
				return ReportsView.GetReportName (this.SelectedReportId);
			}

			return null;
		}
	}
}
