//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.Extensions;

using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Metadata
{
	public class ColumnFilterComparisonExpression : ColumnFilterExpression
	{
		public ColumnFilterComparisonExpression()
		{

		}
		
		
		public ColumnFilterComparisonCode		Comparison
		{
			get
			{
				return this.comparison;
			}
			set
			{
				this.comparison = value;
			}
		}

		public ColumnFilterConstant				Constant
		{
			get
			{
				return this.constant;
			}
			set
			{
				this.constant = value;
			}
		}


		public override bool					IsValid
		{
			get
			{
				if ((this.comparison != ColumnFilterComparisonCode.Undefined) &&
					(this.constant.IsValid))
				{
					return true;
				}
				else if (this.constant.IsNull)
				{
					//	NULL can only be used for equality/non equality and produces
					//	specific IS NULL and IS NOT NULL SQL expressions at the very
					//	low level.

					if ((this.comparison == ColumnFilterComparisonCode.Equal) ||
						(this.comparison != ColumnFilterComparisonCode.NotEqual))
					{
						return true;
					}
				}
				return false;
			}
		}

		public override Expression GetExpression(Expression parameter)
		{
			if (this.constant.IsNull)
			{
				if (this.comparison == ColumnFilterComparisonCode.Equal)
				{
					return ColumnFilterExpression.IsNull (parameter);
				}
				
				if (this.comparison == ColumnFilterComparisonCode.NotEqual)
				{
					return ColumnFilterExpression.IsNotNull (parameter);
				}

				throw new System.NotSupportedException ("Cannot compare with NULL");
			}

			return ColumnFilterExpression.Comparison (parameter, this.comparison, this.constant.GetExpression ());
		}


		private ColumnFilterComparisonCode		comparison;
		private ColumnFilterConstant			constant;
	}
}
