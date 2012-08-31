//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.Extensions;

namespace Epsitec.Cresus.Core.Metadata
{
	public class ColumnFilterRangeExpression : ColumnFilterExpression
	{
		public ColumnFilterRangeExpression()
		{

		}
		public ColumnFilterComparisonCode LowerBoundComparison
		{
			get
			{
				return this.lowerBoundComparison;
			}
			set
			{
				this.lowerBoundComparison = value;
			}
		}

		public ColumnFilterComparisonCode UpperBoundComparison
		{
			get
			{
				return this.upperBoundComparison;
			}
			set
			{
				this.upperBoundComparison = value;
			}
		}

		private ColumnFilterComparisonCode lowerBoundComparison;
		private ColumnFilterComparisonCode upperBoundComparison;
	}
}
