//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Metadata
{
	public class ColumnFilterRangeExpression : ColumnFilterExpression
	{
		public ColumnFilterRangeExpression()
		{

		}
		
		
		public ColumnFilterComparisonCode		LowerBoundComparison
		{
			get
			{
				return this.lowerBoundComparison;
			}
			set
			{
				switch (value)
				{
					case ColumnFilterComparisonCode.GreaterThan:
					case ColumnFilterComparisonCode.GreaterThanOrEqual:
						break;

					default:
						throw new System.ArgumentException (string.Format ("{0} not admissible for the lower bound comparison", value.GetQualifiedName ()));
				}
				
				this.lowerBoundComparison = value;
			}
		}

		public ColumnFilterComparisonCode		UpperBoundComparison
		{
			get
			{
				return this.upperBoundComparison;
			}
			set
			{
				switch (value)
				{
					case ColumnFilterComparisonCode.LessThan:
					case ColumnFilterComparisonCode.LessThanOrEqual:
						break;

					default:
						throw new System.ArgumentException (string.Format ("{0} not admissible for the upper bound comparison", value.GetQualifiedName ()));
				}
				
				this.upperBoundComparison = value;
			}
		}

		public ColumnFilterConstant				LowerBound
		{
			get
			{
				return this.lowerBound;
			}
			set
			{
				this.lowerBound = value;
			}
		}

		public ColumnFilterConstant				UpperBound
		{
			get
			{
				return this.upperBound;
			}
			set
			{
				this.upperBound = value;
			}
		}

		/// <summary>
		/// Determines whether the range is valid: both bounds must be valid and be
		/// of the same type, and a valid comparison must be defined for both bounds.
		/// </summary>
		/// <returns>
		///   <c>true</c> if this range is valid; otherwise, <c>false</c>.
		/// </returns>
		public override bool					IsValid
		{
			get
			{
				if ((this.lowerBound.IsValid) &&
					(this.upperBound.IsValid) &&
					(this.upperBound.Type == this.lowerBound.Type) &&
					(this.lowerBoundComparison != ColumnFilterComparisonCode.Undefined) &&
					(this.upperBoundComparison != ColumnFilterComparisonCode.Undefined))
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}


		public override Expression GetExpression(Expression parameter)
		{
			return Expression.And (
				ColumnFilterExpression.Comparison (parameter, this.lowerBoundComparison, this.lowerBound.GetExpression ()),
				ColumnFilterExpression.Comparison (parameter, this.upperBoundComparison, this.upperBound.GetExpression ()));
		}
		
		private ColumnFilterComparisonCode		lowerBoundComparison;
		private ColumnFilterComparisonCode		upperBoundComparison;
		private ColumnFilterConstant			lowerBound;
		private ColumnFilterConstant			upperBound;
	}
}
