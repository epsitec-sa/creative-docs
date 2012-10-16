//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

using Epsitec.Cresus.Core.Metadata;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.Library.Settings
{
	public sealed class UserEntityTableSettings : IXmlNodeClass
	{
		public UserEntityTableSettings(Druid entityId)
		{
			this.entityId = entityId;
			this.sort     = new List<ColumnRef<EntityColumnSort>> ();
			this.display  = new List<ColumnRef<EntityColumnDisplay>> ();
		}

		
		public Druid									EntityId
		{
			get
			{
				return this.entityId;
			}
		}

		public IList<ColumnRef<EntityColumnSort>>		Sort
		{
			get
			{
				return this.sort;
			}
		}

		public EntityFilter								Filter
		{
			get
			{
				return this.filter;
			}
			set
			{
				this.filter = value;
			}
		}

		public IList<ColumnRef<EntityColumnDisplay>>	Display
		{
			get
			{
				return this.display;
			}
		}


		#region IXmlNodeClass Members

		public XElement Save(string xmlNodeName)
		{
			var xmlFilter = this.filter == null
				? null
				: this.filter.Save (Xml.FilterItem);

			return new XElement (xmlNodeName,
				new XAttribute (Xml.EntityId, this.entityId.ToCompactString ()),
				new XElement (Xml.SortColumnList, this.sort.Select (x => x.Save (Xml.SortColumnItem))),
				xmlFilter,
				new XElement (Xml.DisplayColumnList, this.display.Select (x => x.Save (Xml.DisplayColumnItem))));
		}

		#endregion

		public static UserEntityTableSettings Restore(XElement xml)
		{
			if (xml == null)
			{
				return null;
			}

			/*
			 * <xx id="[123]">
			 *  <S>
			 *   <s id="..."><v ... /></s>
			 *   ...
			 *  </S>
			 *  <f ...>
			 *   ...
			 *  </f>
			 *  <D>
			 *   <d id="..."><v ... /></d>
			 *   ...
			 *  </D>
			 * </xx>
			 * 
			 */

			var entityId = Druid.Parse (xml.Attribute (Xml.EntityId));
			var settings = new UserEntityTableSettings (entityId);

			settings.sort.AddRange (xml.Element (Xml.SortColumnList).Elements ().Select (x => ColumnRef.Restore<EntityColumnSort> (x)));

			var xmlFilter = xml.Element (Xml.FilterItem);

			if (xmlFilter != null)
			{
				settings.filter = (EntityFilter.Restore (xmlFilter));
			}

			settings.display.AddRange (xml.Element (Xml.DisplayColumnList).Elements ().Select (x => ColumnRef.Restore<EntityColumnDisplay> (x)));

			return settings;
		}


		private static class Xml
		{
			public const string					EntityId		   = "id";
			public const string					SortColumnList     = "S";
			public const string					SortColumnItem     = "s";
			public const string					FilterItem		   = "f";
			public const string					DisplayColumnList  = "D";
			public const string					DisplayColumnItem  = "d";
		}


		private readonly Druid									entityId;
		private readonly List<ColumnRef<EntityColumnSort>>		sort;
		private readonly List<ColumnRef<EntityColumnDisplay>>	display;
		private EntityFilter									filter;
	}
}
