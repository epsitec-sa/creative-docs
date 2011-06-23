//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.Widgets.Tiles
{
	public sealed class TileEntityDisplaySettings
	{
		public TileEntityDisplaySettings()
		{
			this.fields = new Dictionary<Druid, List<TileUserFieldDisplaySettings>> ();
		}


		public void Add(Druid field, TileUserFieldDisplaySettings settings)
		{
			List<TileUserFieldDisplaySettings> list;

			if (this.fields.TryGetValue (field, out list) == false)
			{
				list = new List<TileUserFieldDisplaySettings> ();
				this.fields[field] = list;
			}

			list.Add (settings);
		}

		public void AddRange(Druid field, IEnumerable<TileUserFieldDisplaySettings> settings)
		{
			List<TileUserFieldDisplaySettings> list;

			if (this.fields.TryGetValue (field, out list) == false)
			{
				list = new List<TileUserFieldDisplaySettings> ();
				this.fields[field] = list;
			}

			list.AddRange (settings);
		}

		public bool Remove(Druid field, TileUserFieldDisplaySettings settings)
		{
			List<TileUserFieldDisplaySettings> list;

			if (this.fields.TryGetValue (field, out list))
			{
				if (list.Remove (settings))
				{
					if (list.Count == 0)
					{
						this.fields.Remove (field);
					}

					return true;
				}
			}

			return false;
		}

		public int RemoveAll(Druid field, System.Predicate<TileUserFieldDisplaySettings> match)
		{
			List<TileUserFieldDisplaySettings> list;
			int count = 0;

			if (this.fields.TryGetValue (field, out list))
			{
				count = list.RemoveAll (match);

				if (list.Count == 0)
				{
					this.fields.Remove (field);
				}
			}

			return count;
		}


		public XElement Save(string xmlNodeName)
		{
			/*
			 * <xx>
			 *  <fields>
			 *   <f id="[ABC]">
			 *    <settings>
			 *     <s cat="n" uid="xxx" m="n"><f vis="n" edit="n"/></s>
			 *     <s cat="n" uid="yyy" m="n"><f vis="n" edit="n"/></s>
			 *    </settings>
			 *   </f>
			 *   <f id="[DEF]">
			 *    ...
			 *   </f>
			 *  </fields>
			 * </xx>
			 * 
			 */
			
			return new XElement (xmlNodeName,
				new XElement (Xml.FieldList,
					this.fields.Keys.OrderBy (x => x).Select (x => 
						new XElement (Xml.FieldItem,
							new XAttribute (Xml.FieldId, x.ToString ()),
							new XElement (Xml.SettingsList,
								this.fields[x].Select (s => s.Save (Xml.SettingsItem)))))));
		}

		public static TileEntityDisplaySettings Restore(XElement xml)
		{
			var entitySettings = new TileEntityDisplaySettings ();

			var xmlFieldList  = xml.Element (Xml.FieldList);
			var xmlFieldItems = xmlFieldList.Elements (Xml.FieldItem);

			foreach (var xmlFieldItem in xmlFieldItems)
			{
				var field    = Druid.Parse ((string) xmlFieldItem.Attribute (Xml.FieldId));
				var settings = xmlFieldItem.Element (Xml.SettingsList).Elements (Xml.SettingsItem).Select (x => TileUserFieldDisplaySettings.Restore (x));

				entitySettings.AddRange (field, settings);
			}

			return entitySettings;
		}


		private static class Xml
		{
			public const string FieldList		= "fields";
			public const string FieldItem		= "f";
			public const string FieldId			= "id";
			public const string SettingsList	= "settings";
			public const string SettingsItem	= "s";
		}

		
		private readonly Dictionary<Druid, List<TileUserFieldDisplaySettings>> fields;
	}
}
