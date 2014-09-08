//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;

namespace Epsitec.Cresus.Assets.App.NodeGetters
{
	public struct ReportNode
	{
		public ReportNode(string description1, string description2, string sortableDescription, Guid guid)
		{
			this.Description1        = description1;
			this.Description2        = description2;
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
				return string.IsNullOrEmpty (this.Description1)
					&& string.IsNullOrEmpty (this.Description2)
					&& string.IsNullOrEmpty (this.SortableDescription)
					&& this.Guid.IsEmpty;
			}
		}

		public static ReportNode Empty = new ReportNode (null, null, null, Guid.Empty);

		public readonly string				Description1;
		public readonly string				Description2;
		public readonly string				SortableDescription;
		public readonly Guid				Guid;
	}
}
