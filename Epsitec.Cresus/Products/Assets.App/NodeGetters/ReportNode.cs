//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;

namespace Epsitec.Cresus.Assets.App.NodeGetters
{
	public struct ReportNode
	{
		public ReportNode(string description, string sortableDescription, Guid guid)
		{
			this.Description         = description;
			this.SortableDescription = sortableDescription;
			this.Guid                = guid;
		}

		public bool IsTitle
		{
			get
			{
				return this.Guid.IsEmpty;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return string.IsNullOrEmpty (this.Description)
					&& string.IsNullOrEmpty (this.SortableDescription)
					&& this.Guid.IsEmpty;
			}
		}

		public static ReportNode Empty = new ReportNode (null, null, Guid.Empty);

		public readonly string				Description;
		public readonly string				SortableDescription;
		public readonly Guid				Guid;
	}
}
