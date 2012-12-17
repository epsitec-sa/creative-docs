//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Metadata;

using Epsitec.Cresus.DataLayer.Expressions;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;

[assembly: XmlNodeClass ("set", typeof (ColumnFilterSetExpression))]

namespace Epsitec.Cresus.Core.Metadata
{
	public class ColumnFilterSetExpression : ColumnFilterExpression
	{
		public ColumnFilterSetExpression()
		{
			this.values = new List<ColumnFilterConstant> ();
		}

		
		public ColumnFilterSetCode				Predicate
		{
			get
			{
				return this.predicate;
			}
			set
			{
				this.predicate = value;
			}
		}
		
		public IList<ColumnFilterConstant>		Values
		{
			get
			{
				return this.values;
			}
		}

		public override bool					IsValid
		{
			get
			{
				return this.Predicate != ColumnFilterSetCode.Undefined
					&& this.Values != null
					&& this.Values.All (v => v.IsValid);
			}
		}


		public override Expression GetExpression(AbstractEntity example, Expression parameter)
		{
			var method = this.Predicate == ColumnFilterSetCode.In
				? SqlMethods.IsInSetMethodInfo
				: SqlMethods.IsNotInSetMethodInfo;

			var values = this.Values
				.Select (v => ((ConstantExpression) v.GetExpression (example, parameter)).Value)
				.ToList ();

			var arguments = new List<Expression> ()
			{
				Expression.Convert (parameter, typeof (object)),
				Expression.Constant (values),
			};

			return Expression.Call (null, method, arguments);
		}

		public override XElement Save(string xmlNodeName)
		{
			var predicate = new XAttribute (Strings.Predicate, InvariantConverter.ToString (EnumType.ConvertToInt (this.Predicate)));

			var values = new XElement (Strings.Values,
				from v in this.Values
				select new XElement (Strings.Value, v.ToString ()));

			return new XElement (xmlNodeName, predicate, values);
		}

		
		public static new ColumnFilterSetExpression Restore(XElement xml)
		{
			var predicate = InvariantConverter.ToEnum (xml.Attribute (Strings.Predicate), ColumnFilterSetCode.Undefined);
			var values = xml
				.Element (Strings.Values)
				.Elements (Strings.Value)
				.Select (c => ColumnFilterConstant.Parse (c.Value));

			var columnFilterSetExpression = new ColumnFilterSetExpression ()
			{
				Predicate = predicate,
			};

			foreach (var value in values)
			{
				columnFilterSetExpression.Values.Add (value);
			}

			return columnFilterSetExpression;
		}


		#region Strings Class

		private static class Strings
		{
			public static readonly string					Predicate = "p";
			public static readonly string					Values = "V";
			public static readonly string					Value = "v";
		}

		#endregion


		private ColumnFilterSetCode					 predicate;
		private readonly IList<ColumnFilterConstant> values;
	}
}