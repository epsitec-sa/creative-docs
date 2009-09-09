//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph
{
	public class GraphDataCategory
	{
		public GraphDataCategory(int index)
		{
			this.index = index;
		}


		public string Name
		{
			get;
			set;
		}

		public int Index
		{
			get
			{
				return this.index;
			}
		}


		public static GraphDataCategory Generic
		{
			get
			{
				return new GraphDataCategory (0);
			}
		}


		private readonly int index;
	}
}
