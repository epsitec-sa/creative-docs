//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.NodeGetters
{
	public struct EntryNode
	{
		public EntryNode(Guid entryGuid, Guid assetGuid, string assetName, System.DateTime? date, string debit, string credit, string stamp, string title, decimal? value, int level, NodeType type)
		{
			this.EntryGuid = entryGuid;
			this.AssetGuid = assetGuid;
			this.AssetName = assetName;
			this.Date      = date;
			this.Debit     = debit;
			this.Credit    = credit;
			this.Stamp     = stamp;
			this.Title     = title;
			this.Value     = value;
			this.Level     = level;
			this.Type      = type;
		}

		public bool IsEmpty
		{
			get
			{
				return this.Level == -1
					&& this.Type == NodeType.None;
			}
		}

		public static EntryNode Empty = new EntryNode (Guid.Empty, Guid.Empty, null, System.DateTime.MaxValue, null, null, null, null, null, -1, NodeType.None);

		public readonly Guid				EntryGuid;
		public readonly Guid				AssetGuid;
		public readonly string				AssetName;
		public readonly System.DateTime?	Date;
		public readonly string				Debit;
		public readonly string				Credit;
		public readonly string				Stamp;
		public readonly string				Title;
		public readonly decimal?			Value;
		public readonly int					Level;
		public readonly NodeType			Type;
	}
}
