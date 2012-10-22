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
	public sealed class Filter : IFilter, IXmlNodeClass
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

		public IEnumerable<IFilter>				ActiveFilters
		{
			get
			{
				return this.nodes.Where (x => x.ActiveMode != FilterActiveMode.Disabled).Cast<IFilter> ();
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


		#region IFilter Members

		public bool IsValid
		{
			get
			{
				return this.ActiveFilters.All (x => x.IsValid);
			}
		}

		public Expression GetExpression(AbstractEntity example, Expression parameter)
		{
			return Filter.GetExpression (this.ActiveFilters, example, parameter, this.CombineMode);
		}

		#endregion

		#region IXmlNodeClass Members

		public XElement Save(string xmlNodeName)
		{
			var xml = new XElement (xmlNodeName,
				new XAttribute (Strings.CombineMode, InvariantConverter.ToString (EnumType.ConvertToInt (this.CombineMode))),
				new XAttribute (Strings.Name, this.Name.ToString ()),
				new XAttribute (Strings.Description, this.Description.ToString ()),
				new XElement (Strings.FilterNodeList,
					this.nodes.Select (x => x.Save (Strings.FilterNodeItem))));

			return xml;
		}

		#endregion

		public static Filter Restore(XElement xml)
		{
			var list   = xml.Element (Strings.FilterNodeList).Elements ();
			var filter = new Filter ()
			{
				CombineMode = xml.Attribute (Strings.CombineMode).ToEnum (FilterCombineMode.And),
				Name        = Xml.GetFormattedText (xml.Attribute (Strings.Name)),
				Description = Xml.GetFormattedText (xml.Attribute (Strings.Description)),
			};

			filter.Nodes.AddRange (list.Select (x => FilterNode.Restore (x)));

			return filter;
		}


		public static Expression GetExpression(IEnumerable<IFilter> filters, AbstractEntity example, Expression parameter, FilterCombineMode mode)
		{
			return Filter.GetExpression (filters.Select (x => x.GetExpression (example, parameter)), mode);
		}

		/// <summary>
		/// Produces the expression tree, built by combining all individual filter expressions
		/// using either logical <c>AND</c> or logical <c>OR</c>.
		/// </summary>
		/// <param name="expressions">The expressions.</param>
		/// <param name="mode">The mode.</param>
		/// <returns>
		/// The combined filter expression.
		/// </returns>
		public static Expression GetExpression(IEnumerable<Expression> expressions, FilterCombineMode mode)
		{
			//	See http://stackoverflow.com/questions/457316/combining-two-expressions-expressionfunct-bool

			switch (mode)
			{
				case FilterCombineMode.And:
					return ExpressionAnalyzer.CombineExpressions (expressions, (left, right) => Expression.AndAlso (left, right));
				case FilterCombineMode.Or:
					return ExpressionAnalyzer.CombineExpressions (expressions, (left, right) => Expression.OrElse (left, right));
			}

			throw new System.NotSupportedException (string.Format ("{0} not supported", mode.GetQualifiedName ()));
		}


		#region Strings Class

		private static class Strings
		{
			public const string CombineMode    = "c";
			public const string Name           = "n";
			public const string Description    = "d";
			public const string FilterNodeList = "F";
			public const string FilterNodeItem = "f";
		}

		#endregion

		
		private readonly List<FilterNode>		nodes;
	}
}