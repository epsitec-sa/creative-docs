//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Metadata;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;

[assembly:XmlNodeClass ("range", typeof (ColumnFilterRangeExpression))]

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


		public override Expression GetExpression(AbstractEntity example, Expression parameter)
		{
			return Expression.AndAlso (
				ColumnFilterExpression.Compare (parameter, this.lowerBoundComparison, this.lowerBound.GetExpression (example, parameter)),
				ColumnFilterExpression.Compare (parameter, this.upperBoundComparison, this.upperBound.GetExpression (example, parameter)));
		}

		public override XElement Save(string xmlNodeName)
		{
			return new XElement (xmlNodeName,
				new XAttribute (Strings.LowerBoundComparisonCode, InvariantConverter.ToString (EnumType.ConvertToInt (this.lowerBoundComparison))),
				new XAttribute (Strings.UpperBoundComparisonCode, InvariantConverter.ToString (EnumType.ConvertToInt (this.upperBoundComparison))),
				new XAttribute (Strings.LowerBoundValue, this.lowerBound.ToString ()),
				new XAttribute (Strings.UpperBoundValue, this.upperBound.ToString ()));
		}


		public static new ColumnFilterRangeExpression Restore(XElement xml)
		{
			return new ColumnFilterRangeExpression ()
			{
				LowerBoundComparison = InvariantConverter.ToEnum (xml.Attribute (Strings.LowerBoundComparisonCode), ColumnFilterComparisonCode.Undefined),
				UpperBoundComparison = InvariantConverter.ToEnum (xml.Attribute (Strings.UpperBoundComparisonCode), ColumnFilterComparisonCode.Undefined),
				LowerBound = ColumnFilterConstant.Parse (xml.Attribute (Strings.LowerBoundValue)),
				UpperBound = ColumnFilterConstant.Parse (xml.Attribute (Strings.UpperBoundValue))
			};
		}

		#region Strings Class

		private static class Strings
		{
			public const string LowerBoundComparisonCode = "lc";
			public const string UpperBoundComparisonCode = "uc";
			public const string LowerBoundValue = "lv";
			public const string UpperBoundValue = "uv";
		}

		#endregion


		private ColumnFilterComparisonCode		lowerBoundComparison;
		private ColumnFilterComparisonCode		upperBoundComparison;
		private ColumnFilterConstant			lowerBound;
		private ColumnFilterConstant			upperBound;
	}
}
