//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	/// <summary>
	/// Accès en lecture à des données quelconques constituées de GuidNode/LevelNode/TreeNode.
	/// </summary>
	public abstract class AbstractNodesGetter<T>
		where T : struct
	{
		public IEnumerable<T> Nodes
		{
			get
			{
				for (int i=0; i<this.Count; i++)
				{
					yield return this[i];
				}
			}
		}

		public virtual int Count
		{
			get
			{
				return 0;
			}
		}

		public virtual T this[int row]
		{
			get
			{
				return new T ();
			}
		}
	}
}
