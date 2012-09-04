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
	public sealed class Filter : IFilter
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

		public IEnumerable<FilterNode>			ActiveNodes
		{
			get
			{
				return this.nodes.Where (x => x.ActiveMode != FilterActiveMode.Disabled);
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
				return this.ActiveNodes.All (x => x.IsValid);
			}
		}

		public Expression GetExpression(Expression parameter)
		{
			var list = this.ActiveNodes.ToList ();

			switch (list.Count)
			{
				case 0:
					return null;
				case 1:
					return list[0].GetExpression (parameter);
				default:
					return this.GetRecursiveExpression (parameter, list);
			}
		}

		#endregion

		
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


		/// <summary>
		/// Recursively gets the expression tree, build by combining all individual filter nodes
		/// using either logical <c>AND</c> or logical <c>OR</c>.
		/// </summary>
		/// <param name="parameter">The parameter.</param>
		/// <param name="list">The list of nodes.</param>
		/// <param name="index">The index into the list of nodes.</param>
		/// <returns>The combined filter expression.</returns>
		private Expression GetRecursiveExpression(Expression parameter, IList<FilterNode> list, int index = 0)
		{
			Expression left = list[index+0].GetExpression (parameter);
			Expression right;

			if (index+2 == list.Count)
			{
				right = list[index+1].GetExpression (parameter);
			}
			else
			{
				right = this.GetRecursiveExpression (parameter, list, index+1);
			}

			//	See http://stackoverflow.com/questions/457316/combining-two-expressions-expressionfunct-bool

			switch (this.CombineMode)
			{
				case FilterCombineMode.And:
					return Expression.AndAlso (left, right);
				case FilterCombineMode.Or:
					return Expression.OrElse (left, right);
			}

			throw new System.NotSupportedException (string.Format ("{0} not supported", this.CombineMode.GetQualifiedName ()));
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
