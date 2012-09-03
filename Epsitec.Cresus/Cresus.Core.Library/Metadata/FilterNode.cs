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
			: this (includeMode, activeMode)
		{
			this.filter = filter;
		}

		public FilterNode(EntityFilter filter, FilterIncludeMode includeMode, FilterActiveMode activeMode)
			: this (includeMode, activeMode)
		{
			this.filter = filter;
		}

		private FilterNode(FilterIncludeMode includeMode, FilterActiveMode activeMode)
		{
			this.filter = null;
			this.includeMode = includeMode;
			this.activeMode  = activeMode;
		}


		public FilterIncludeMode				IncludeMode
		{
			get
			{
				return this.includeMode;
			}
		}
		
		public FilterActiveMode					ActiveMode
		{
			get
			{
				return this.activeMode;
			}
		}
		
		public Filter							RichFilter
		{
			get
			{
				return this.filter as Filter;
			}
		}

		public EntityFilter						SimpleFilter
		{
			get
			{
				return this.filter as EntityFilter;
			}
		}

		public FilterNodeType					Type
		{
			get
			{
				if (this.filter == null)
				{
					return FilterNodeType.Undefined;
				}
				else if (this.RichFilter != null)
				{
					return FilterNodeType.RichFilter;
				}
				else if (this.SimpleFilter != null)
				{
					return FilterNodeType.SimpleFilter;
				}
				else
				{
					throw new System.NotSupportedException ("Cannot infer FilterNodeType from filter");
				}
			}
		}


		public XElement Save(string xmlNodeName)
		{
			XElement filter;

			switch (this.Type)
			{
				case FilterNodeType.SimpleFilter:
					filter = this.SimpleFilter.Save (Strings.SimpleFilter);
					break;

				case FilterNodeType.RichFilter:
					filter = this.RichFilter.Save (Strings.RichFilter);
					break;

				case FilterNodeType.Undefined:
					filter = null;
					break;

				default:
					throw new System.NotSupportedException (string.Format ("Invalid type {0}", this.Type.GetQualifiedName ()));
			}

			var xml = new XElement (xmlNodeName,
				new XAttribute (Strings.IncludeMode, InvariantConverter.ToString (EnumType.ConvertToInt (this.includeMode))),
				new XAttribute (Strings.ActiveMode, InvariantConverter.ToString (EnumType.ConvertToInt (this.activeMode))),
				filter);

			return xml;
		}

		public static FilterNode Restore(XElement xml)
		{
			var includeMode = xml.Attribute (Strings.IncludeMode).ToEnum (FilterIncludeMode.Inclusive);
			var activeMode  = xml.Attribute (Strings.ActiveMode).ToEnum (FilterActiveMode.Enabled);

			var richFilterXml   = xml.Element (Strings.RichFilter);
			var simpleFilterXml = xml.Element (Strings.SimpleFilter);

			if (richFilterXml != null)
			{
				var filter = Filter.Restore (richFilterXml);
				return new FilterNode (filter, includeMode, activeMode);
			}
			if (simpleFilterXml != null)
			{
				var filter = EntityFilter.Restore (simpleFilterXml);
				return new FilterNode (filter, includeMode, activeMode);
			}

			return new FilterNode (includeMode, activeMode);
		}


		#region Strings Class

		private static class Strings
		{
			public const string IncludeMode	 = "i";
			public const string ActiveMode   = "a";
			public const string RichFilter   = "rf";
			public const string SimpleFilter = "sf";
		}

		#endregion


		private readonly FilterIncludeMode		includeMode;
		private readonly FilterActiveMode		activeMode;
		private readonly object					filter;
	}
}