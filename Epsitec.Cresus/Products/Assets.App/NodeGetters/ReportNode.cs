//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;

namespace Epsitec.Cresus.Assets.App.NodeGetters
{
	public struct ReportNode
	{
		public ReportNode(string title)
		{
			this.Title = title;
			this.Guid  = Guid.Empty;
		}

		public ReportNode(Guid guid)
		{
			this.Title = null;
			this.Guid  = guid;
		}

		public bool IsTitle
		{
			get
			{
				return !string.IsNullOrEmpty (this.Title);
			}
		}

		public bool IsEmpty
		{
			get
			{
				return string.IsNullOrEmpty (this.Title)
					&& this.Guid.IsEmpty;
			}
		}

		public static ReportNode Empty = new ReportNode (null);

		public readonly string				Title;
		public readonly Guid				Guid;
	}
}
