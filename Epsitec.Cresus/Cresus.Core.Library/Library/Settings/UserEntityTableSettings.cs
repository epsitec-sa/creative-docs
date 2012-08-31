//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

using Epsitec.Cresus.Core.Metadata;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.Library.Settings
{
	public sealed class UserEntityTableSettings
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

		public IList<ColumnRef<EntityColumnDisplay>>	Display
		{
			get
			{
				return this.display;
			}
		}


		
		public XElement Save(string xmlNodeName)
		{
			return new XElement (xmlNodeName,
				new XAttribute (Xml.EntityId, this.entityId.ToCompactString ()), 
				new XElement (Xml.SortColumnList, this.sort.Select (x => x.Save (Xml.SortColumnItem))),
				new XElement (Xml.DisplayColumnList, this.display.Select (x => x.Save (Xml.DisplayColumnItem))));
		}

		public static UserEntityTableSettings Restore(XElement xml)
		{
			/*
			 * <xx id="[123]">
			 *  <S>
			 *   <s id="..."><v ... /></s>
			 *   ...
			 *  </S>
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
			settings.display.AddRange (xml.Element (Xml.DisplayColumnList).Elements ().Select (x => ColumnRef.Restore<EntityColumnDisplay> (x)));
			
			return settings;
		}


		private static class Xml
		{
			public const string					EntityId		   = "id";
			public const string					SortColumnList     = "S";
			public const string					SortColumnItem     = "s";
			public const string					DisplayColumnList  = "D";
			public const string					DisplayColumnItem  = "d";
		}


		private readonly Druid entityId;
		private readonly List<ColumnRef<EntityColumnSort>>	sort;
		private readonly List<ColumnRef<EntityColumnDisplay>>	display;
	}
}
