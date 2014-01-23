//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public struct ViewState
	{
		public ViewState(ViewType viewType, string data)
		{
			this.ViewType = viewType;
			this.Data     = data;
		}

		public bool IsEmpty
		{
			get
			{
				return this.ViewType == ViewType.Unknown
					&& string.IsNullOrEmpty (this.Data);
			}
		}


		public static bool operator ==(ViewState a, ViewState b)
		{
			return (a.ViewType == b.ViewType)
				&& (a.Data     == b.Data);
		}

		public static bool operator !=(ViewState a, ViewState b)
		{
			return (a.ViewType  != b.ViewType)
				|| (a.Data      != b.Data);
		}


		public static ViewState Empty = new ViewState (ViewType.Unknown, null);

		public readonly ViewType	ViewType;
		public readonly string		Data;
	}
}
