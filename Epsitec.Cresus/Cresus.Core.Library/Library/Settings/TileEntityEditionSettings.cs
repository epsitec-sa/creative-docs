//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.Library.Settings
{
	public sealed class TileEntityEditionSettings
	{
		public TileEntityEditionSettings()
		{
			this.fields = new Dictionary<Druid, List<UserFieldEditionSettings>> ();
		}


		public void Add(Druid field, UserFieldEditionSettings settings)
		{
			List<UserFieldEditionSettings> list;

			if (this.fields.TryGetValue (field, out list) == false)
			{
				list = new List<UserFieldEditionSettings> ();
				this.fields[field] = list;
			}

			list.Add (settings);
		}

		public void AddRange(Druid field, IEnumerable<UserFieldEditionSettings> settings)
		{
			List<UserFieldEditionSettings> list;

			if (this.fields.TryGetValue (field, out list) == false)
			{
				list = new List<UserFieldEditionSettings> ();
				this.fields[field] = list;
			}

			list.AddRange (settings);
		}

		public bool Remove(Druid field, UserFieldEditionSettings settings)
		{
			List<UserFieldEditionSettings> list;

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

		public int RemoveAll(Druid field, System.Predicate<UserFieldEditionSettings> match)
		{
			List<UserFieldEditionSettings> list;
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


		public IEnumerable<System.Tuple<Druid, IEnumerable<UserFieldEditionSettings>>> GetAllFieldSettings(System.Predicate<UserFieldEditionSettings> match = null)
		{
			if (match == null)
			{
				return this.fields.Select (x => new System.Tuple<Druid, IEnumerable<UserFieldEditionSettings>> (x.Key, x.Value));
			}
			else
			{
				return this.fields.Select (x => new System.Tuple<Druid, IEnumerable<UserFieldEditionSettings>> (x.Key, x.Value.Where (s => match (s))));
			}
		}
		
		
		public XElement Save(string xmlNodeName)
		{
			/*
			 * <xx>
			 *  <F>
			 *   <f id="[ABC]">
			 *    <S>
			 *     <s cat="n" uid="xxx" m="n"><f v="n" e="n" /></s>		//	field settings (full)
			 *     <s m="n"><f v="n" e="n" /></s>						//	field settings, specific, without user
			 *     <s><f v="n" e="n" /></s>								//	field settings, inclusive, without user
			 *    </S>
			 *   </f>
			 *   <f id="[DEF]">
			 *    ...
			 *   </f>
			 *  </F>
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

		public static TileEntityEditionSettings Restore(XElement xml)
		{
			var entitySettings = new TileEntityEditionSettings ();

			var xmlFieldList  = xml.Element (Xml.FieldList);
			var xmlFieldItems = xmlFieldList.Elements (Xml.FieldItem);

			foreach (var xmlFieldItem in xmlFieldItems)
			{
				var field    = Druid.Parse ((string) xmlFieldItem.Attribute (Xml.FieldId));
				var settings = xmlFieldItem.Element (Xml.SettingsList).Elements (Xml.SettingsItem).Select (x => UserFieldEditionSettings.Restore (x));

				entitySettings.AddRange (field, settings);
			}

			return entitySettings;
		}


		private static class Xml
		{
			public const string FieldList		= "F";
			public const string FieldItem		= "f";
			public const string FieldId			= "id";
			public const string SettingsList	= "S";
			public const string SettingsItem	= "s";
		}

		
		private readonly Dictionary<Druid, List<UserFieldEditionSettings>> fields;
	}
}
