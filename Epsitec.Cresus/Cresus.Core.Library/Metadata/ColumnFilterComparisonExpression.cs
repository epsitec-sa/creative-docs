//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Metadata;

using System.Linq.Expressions;
using System.Xml.Linq;

[assembly: XmlNodeClass ("comp", typeof (ColumnFilterComparisonExpression))]

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

		public override Expression GetExpression(AbstractEntity example, Expression parameter)
		{
			if (this.constant.IsNull)
			{
				var invalidNullComparison = this.comparison != ColumnFilterComparisonCode.Equal
					&& this.comparison != ColumnFilterComparisonCode.NotEqual;

				if (invalidNullComparison)
				{
					throw new System.NotSupportedException ("Cannot compare with NULL");
				}
			}

			var constantExpression = this.constant.GetExpression (example, parameter);

			return ColumnFilterExpression.Compare (parameter, this.comparison, constantExpression);
		}

		public override XElement Save(string xmlNodeName)
		{
			return new XElement (xmlNodeName,
				new XAttribute (Strings.ComparisonCode, InvariantConverter.ToString (EnumType.ConvertToInt (this.comparison))),
				new XAttribute (Strings.Value, this.constant.ToString ()));
		}

		
		public static new ColumnFilterComparisonExpression Restore(XElement xml)
		{
			return new ColumnFilterComparisonExpression ()
			{
				Comparison = InvariantConverter.ToEnum (xml.Attribute (Strings.ComparisonCode), ColumnFilterComparisonCode.Undefined),
				Constant   = ColumnFilterConstant.Parse (xml.Attribute (Strings.Value))
			};
		}


		#region Strings Class

		private static class Strings
		{
			public const string					ComparisonCode = "c";
			public const string					Value = "v";
		}

		#endregion


		private ColumnFilterComparisonCode		comparison;
		private ColumnFilterConstant			constant;
	}
}
