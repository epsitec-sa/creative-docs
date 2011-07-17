//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Support;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.Core.Helpers;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Business;

namespace Epsitec.Cresus.Core.Controllers.BusinessDocumentControllers
{
	public class TreeNode
	{
		public TreeNode()
		{
			//	Crée un noeud sans entité et sans GroupIndex connu.
			this.childrens = new List<TreeNode> ();
		}

		public TreeNode(int debugRank, int groupIndex)
		{
			//	Crée un noeud sans entité, juste avec un GroupIndex.
			this.DebugRank = debugRank;
			this.FullGroupIndex = groupIndex;

			this.childrens = new List<TreeNode> ();
		}

		public TreeNode(int debugRank, AbstractDocumentItemEntity entity)
		{
			//	Crée un noeud avec entité et GroupIndex.
			this.DebugRank = debugRank;
			this.Entity = entity;
			this.FullGroupIndex = -1;

			this.childrens = new List<TreeNode> ();
		}


		public int FullGroupIndex
		{
			get;
			set;
		}

		public AbstractDocumentItemEntity Entity
		{
			get;
			private set;
		}

		public TreeNode Parent
		{
			get;
			set;
		}

		public int Deep
		{
			get;
			set;
		}

		public int DebugRank
		{
			get;
			private set;
		}

		public List<TreeNode> Childrens
		{
			get
			{
				return this.childrens;
			}
		}


		public string DebugNodeContent()
		{
			if (this.Entity == null)
			{
				return string.Format ("Empty node: DebugRank={0} Deep={1} GroupIndex={2} Childrens={3}\r\n", this.DebugRank, this.Deep, this.FullGroupIndex, this.childrens.Count);
			}
			else
			{
				return string.Format ("Entity node: DebugRank={0} Deep={1} GroupIndex={2} Type={3}\r\n", this.DebugRank, this.Deep, this.Entity.GroupIndex, this.Entity.GetType ().ToString ());
			}
		}


		private readonly List<TreeNode>	childrens;
	}
}
