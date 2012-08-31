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
	public struct FilterNode
	{
		public FilterNode(Filter filter, FilterIncludeMode includeMode, FilterActiveMode activeMode)
		{
			this.filter      = filter;
			this.includeMode = includeMode;
			this.activeMode  = activeMode;
		}

		public FilterNode(EntityFilter filter, FilterIncludeMode includeMode, FilterActiveMode activeMode)
		{
			this.filter      = filter;
			this.includeMode = includeMode;
			this.activeMode  = activeMode;
		}


		public FilterIncludeMode IncludeMode
		{
			get
			{
				return this.includeMode;
			}
		}
		public FilterActiveMode ActiveMode
		{
			get
			{
				return this.activeMode;
			}
		}
		public Filter RichFilter
		{
			get
			{
				return this.filter as Filter;
			}
		}

		public EntityFilter SimpleFilter
		{
			get
			{
				return this.filter as EntityFilter;
			}
		}


		private readonly FilterIncludeMode		includeMode;
		private readonly FilterActiveMode		activeMode;
		private readonly object					filter;
	}
}
