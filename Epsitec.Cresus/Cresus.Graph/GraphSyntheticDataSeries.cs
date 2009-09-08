//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Graph.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph
{
	public class GraphSyntheticDataSeries : GraphDataSeries
	{
		public GraphSyntheticDataSeries(GraphDataGroup group)
		{
			this.group = group;
		}


		public GraphDataGroup SourceGroup
		{
			get
			{
				return this.group;
			}
		}


		private readonly GraphDataGroup group;
	}
}
