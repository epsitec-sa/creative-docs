//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.Metadata
{
	public sealed class Filter
	{
		public Filter()
		{
			this.nodes = new List<FilterNode> ();
		}

		
		public FilterCombineMode				CombineMode
		{
			get;
			set;
		}

		public IList<FilterNode>				Nodes
		{
			get
			{
				return this.nodes;
			}
		}

		public FormattedText					Name
		{
			get;
			set;
		}

		public FormattedText					Description
		{
			get;
			set;
		}


		public XElement Save(string xmlNodeName)
		{
			throw new System.NotImplementedException ();
		}

		public static Filter Restore(XElement xml)
		{
			throw new System.NotImplementedException ();
		}

		
		private readonly List<FilterNode>		nodes;
	}
}
