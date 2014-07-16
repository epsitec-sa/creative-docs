//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;

namespace Epsitec.Cresus.Assets.Server.NodeGetters
{
	/// <summary>
	///	Noeud correspondant à une ligne d'un TreeTable. S'il s'agit d'un groupe
	///	compacté, on stocke les valeurs des totaux des objets sous-jacents.
	/// </summary>
	public struct CumulNode
	{
		public CumulNode(Guid guid, BaseType baseType, int level, decimal? ratio, NodeType type, int? groupIndex)
		{
			this.Guid       = guid;
			this.BaseType   = baseType;
			this.Level      = level;
			this.Ratio      = ratio;
			this.Type       = type;
			this.GroupIndex = groupIndex;

			this.cumuls = new Dictionary<ObjectField, decimal> ();
		}

		public Dictionary<ObjectField, decimal> Cumuls
		{
			get
			{
				return this.cumuls;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.Guid.IsEmpty
					&& this.Level == -1
					&& this.Type == NodeType.None;
			}
		}

		public static CumulNode Empty = new CumulNode (Guid.Empty, BaseType.Assets, -1, null, NodeType.None, null);

		public readonly Guid				Guid;
		public readonly BaseType			BaseType;
		public readonly int					Level;
		public readonly decimal?			Ratio;
		public readonly NodeType			Type;
		public readonly int?				GroupIndex;

		private readonly Dictionary<ObjectField, decimal> cumuls;
	}
}
