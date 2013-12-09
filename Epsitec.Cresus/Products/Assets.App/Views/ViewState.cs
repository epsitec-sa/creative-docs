//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public struct ViewState
	{
		public ViewState(ViewType viewType, ViewMode viewMode, PageType pageType, Timestamp? timestamp, Guid guid)
		{
			this.ViewType  = viewType;
			this.ViewMode  = viewMode;
			this.PageType  = pageType;
			this.Timestamp = timestamp;
			this.Guid      = guid;
		}

		public bool IsEmpty
		{
			get
			{
				return this.ViewType == ViewType.Unknown
					&& this.ViewMode == ViewMode.Unknown
					&& this.PageType == PageType.Unknown
					&& this.Timestamp == null
					&& this.Guid.IsEmpty;
			}
		}

		public static ViewState Empty = new ViewState (ViewType.Unknown, ViewMode.Unknown, PageType.Unknown, null, Guid.Empty);

		public readonly ViewType	ViewType;
		public readonly ViewMode	ViewMode;
		public readonly PageType	PageType;
		public readonly Timestamp?	Timestamp;
		public readonly Guid		Guid;
	}
}
