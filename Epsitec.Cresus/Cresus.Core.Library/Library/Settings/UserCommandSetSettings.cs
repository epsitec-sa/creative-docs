//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types.Collections;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.Library.Settings
{
	/// <summary>
	/// The <c>UserCommandSetSettings</c> class represents two collections of <see cref="UserCommandSettings"/>;
	/// one for the enabled commands and one for the disabled commands.
	/// </summary>
	public sealed class UserCommandSetSettings
	{
		public UserCommandSetSettings()
		{
			this.commands = new Dictionary<Druid, Records> ();
		}


		public void Add(UserCommandSettings settings, bool enable)
		{
			Druid commandId = settings.CommandId;
			Records records;

			if (this.commands.TryGetValue (commandId, out records))
			{
				this.Remove (settings);
			}
			else
			{
				records = new Records ();
				this.commands[commandId] = records;
			}

			records[enable].Add (settings);
		}

		public void AddRange(IEnumerable<UserCommandSettings> settings, bool enable)
		{
			foreach (var item in settings)
			{
				this.Add (item, enable);
			}
		}

		public bool Remove(UserCommandSettings settings)
		{
			Druid commandId = settings.CommandId;
			Records records;

			if (this.commands.TryGetValue (commandId, out records))
			{
				if ((records.EnabledCommands.Remove (settings)) ||
					(records.DisabledCommands.Remove (settings)))
				{
					if (records.IsEmpty)
					{
						this.commands.Remove (commandId);
					}
					return true;
				}
			}
			
			return false;
		}

		public IEnumerable<UserCommandSettings> GetEnabledCommands()
		{
			return this.commands.OrderBy (x => x.Key).SelectMany (x => x.Value.EnabledCommands);
		}

		public IEnumerable<UserCommandSettings> GetEnabledCommands(Druid commandId)
		{
			Records records;
			
			if (this.commands.TryGetValue (commandId, out records))
			{
				return records.EnabledCommands;
			}
			else
			{
				return EmptyEnumerable<UserCommandSettings>.Instance;
			}
		}

		public IEnumerable<UserCommandSettings> GetDisabledCommands()
		{
			return this.commands.OrderBy (x => x.Key).SelectMany (x => x.Value.DisabledCommands);
		}
		
		public IEnumerable<UserCommandSettings> GetDisabledCommands(Druid commandId)
		{
			Records records;

			if (this.commands.TryGetValue (commandId, out records))
			{
				return records.DisabledCommands;
			}
			else
			{
				return EmptyEnumerable<UserCommandSettings>.Instance;
			}
		}



		public XElement Save(string xmlNodeName)
		{
			/*
			 * <xx>
			 *  <E>
			 *   <c id="[ABC]">
			 *    <S>
			 *     <s cat="n" uid="xxx" />
			 *     <s cat="m" uid="yyy" />
			 *    </S>
			 *   </c>
			 *   <c id="[DEF]">
			 *    ...
			 *   </c>
			 *  </E>
			 *  <D>
			 *   <c id="[GHI]">
			 *    ...
			 *   </c>
			 *  </D>
			 * </xx>
			 * 
			 */

			return new XElement (xmlNodeName,
				new XElement (Xml.EnabledList,
					this.commands.Keys.OrderBy (x => x).Select (x =>
						new XElement (Xml.CommandItem,
							new XAttribute (Xml.CommandId, x.ToString ()),
							new XElement (Xml.SettingsList,
								this.commands[x].EnabledCommands.Select (s => s.Save (Xml.SettingsItem)))))),
				new XElement (Xml.DisabledList,
					this.commands.Keys.OrderBy (x => x).Select (x =>
						new XElement (Xml.CommandItem,
							new XAttribute (Xml.CommandId, x.ToString ()),
							new XElement (Xml.SettingsList,
								this.commands[x].DisabledCommands.Select (s => s.Save (Xml.SettingsItem)))))));
		}

		public static UserCommandSetSettings Restore(XElement xml)
		{
			var commandSettings = new UserCommandSetSettings ();

			var xmlEnabledList  = xml.Element (Xml.EnabledList);
			var xmlDisabledList = xml.Element (Xml.DisabledList);

			var xmlEnabledItems  = xmlEnabledList.Elements (Xml.CommandItem);
			var xmlDisabledItems = xmlDisabledList.Elements (Xml.CommandItem);

			UserCommandSetSettings.DeserializeCommandSettings (commandSettings, xmlEnabledItems, enable: true);
			UserCommandSetSettings.DeserializeCommandSettings (commandSettings, xmlDisabledItems, enable: false);

			return commandSettings;
		}


		private static void DeserializeCommandSettings(UserCommandSetSettings commandSettings, IEnumerable<XElement> xmlEnabledItems, bool enable)
		{
			foreach (var xmlCommandItem in xmlEnabledItems)
			{
				var command  = Druid.Parse ((string) xmlCommandItem.Attribute (Xml.CommandId));
				var settings = xmlCommandItem.Element (Xml.SettingsList).Elements (Xml.SettingsItem).Select (x => UserCommandSettings.Restore (command, x));

				commandSettings.AddRange (settings, enable);
			}
		}

		#region Xml Class

		private static class Xml
		{
			public const string EnabledList		= "E";
			public const string DisabledList	= "D";
			public const string CommandItem		= "c";
			public const string CommandId		= "id";
			public const string SettingsList	= "S";
			public const string SettingsItem	= "s";
		}

		#endregion

		#region Records Class

		private sealed class Records
		{
			public Records()
			{
				this.enabled  = new List<UserCommandSettings> ();
				this.disabled = new List<UserCommandSettings> ();
			}

			public bool IsEmpty
			{
				get
				{
					return (this.enabled.Count == 0)
						&& (this.disabled.Count == 0);
				}
			}

			public IList<UserCommandSettings> this[bool enable]
			{
				get
				{
					return enable ? this.enabled : this.disabled;
				}
			}

			public IList<UserCommandSettings> EnabledCommands
			{
				get
				{
					return this.enabled;
				}
			}

			public IList<UserCommandSettings> DisabledCommands
			{
				get
				{
					return this.disabled;
				}
			}

			private readonly List<UserCommandSettings> enabled;
			private readonly List<UserCommandSettings> disabled;
		}

		#endregion


		private readonly Dictionary<Druid, Records> commands;
	}
}
