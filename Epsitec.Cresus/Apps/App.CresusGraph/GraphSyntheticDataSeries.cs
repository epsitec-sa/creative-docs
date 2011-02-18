//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Graph.Data;
using Epsitec.Common.Support.Extensions;

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


		public override string Title
		{
			get
			{
				return base.Title ?? this.group.Name;
			}
			set
			{
				base.Title = value;
			}
		}

		public override ChartSeries ChartSeries
		{
			get
			{
				if (this.cache == null)
				{
					var func   = Functions.FunctionFactory.GetFunction (this.functionName);
					this.cache = this.group.SynthesizeChartSeries (this.group.Name, func);
				}

				return this.cache;
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

		public void Invalidate()
		{
			if (this.cache != null)
			{
				this.cache = null;
				this.Groups.ForEach (x => x.Invalidate ());
			}
		}
		
			
		private readonly GraphDataGroup group;
		private readonly string functionName;
		private ChartSeries cache;
	}
}
