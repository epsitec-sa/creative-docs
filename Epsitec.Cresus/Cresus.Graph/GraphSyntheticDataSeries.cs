//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Graph.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph
{
	public class GraphSyntheticDataSeries : GraphDataSeries
	{
		public GraphSyntheticDataSeries(GraphDataGroup group, string functionName)
			: base ()
		{
			this.group = group;
			this.functionName = functionName;
		}


		public override ChartSeries ChartSeries
		{
			get
			{
				var func   = Functions.FunctionFactory.GetFunction (this.functionName);
				var series = this.group.SynthesizeChartSeries (this.Label, func);

				return series;
			}
		}

		public GraphDataGroup SourceGroup
		{
			get
			{
				return this.group;
			}
		}

		public bool Enabled
		{
			get;
			set;
		}

		public string FunctionName
		{
			get
			{
				return this.functionName;
			}
		}

		private readonly GraphDataGroup group;
		private readonly string functionName;
	}
}
