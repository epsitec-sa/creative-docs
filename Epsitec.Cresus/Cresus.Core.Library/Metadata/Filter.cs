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
				throw new System.NotImplementedException ();
			}
		}

		public Expression GetExpression(Expression parameter)
		{
			throw new System.NotImplementedException ();
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
