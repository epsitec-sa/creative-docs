//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Cresus.Core.Entities;

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

		public TreeNode(int groupIndex)
		{
			//	Crée un noeud sans entité, juste avec un GroupIndex.
			this.GroupIndex = groupIndex;

			this.childrens = new List<TreeNode> ();
		}

		public TreeNode(AbstractDocumentItemEntity entity)
		{
			//	Crée une feuille avec entité et sans GroupIndex.
			//	Cette feuille n'aura jamais d'enfants.
			this.Entity = entity;

			this.childrens = new List<TreeNode> ();
		}


		public int GroupIndex
		{
			//	Chemin complet du noeud (contrairement à Entity.GroupIndex d'une feuille), seulement pour les noeuds.
			get
			{
				System.Diagnostics.Debug.Assert (this.Entity == null);  // bien un noeud ?
				return this.groupIndex;
			}
			set
			{
				System.Diagnostics.Debug.Assert (this.Entity == null);  // bien un noeud ?
				this.groupIndex = value;
			}
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

		public List<TreeNode> Childrens
		{
			get
			{
				if (this.Entity != null)  // feuille ?
				{
					//	Une feuille ne peut jamais avoir d'enfants.
					System.Diagnostics.Debug.Assert (this.childrens.Count == 0);
				}

				return this.childrens;
			}
		}


		private readonly List<TreeNode>	childrens;
		private int groupIndex;
	}
}
