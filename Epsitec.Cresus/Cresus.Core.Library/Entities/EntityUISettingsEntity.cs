//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Library.Settings;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	partial class EntityUISettingsEntity
	{
		/// <summary>
		/// Gets the display settings. This will deserialize the settings when accessed
		/// for the first time.
		/// </summary>
		[System.Diagnostics.DebuggerBrowsable (System.Diagnostics.DebuggerBrowsableState.Never)]
		public UserEntityEditionSettings		EditionSettings
		{
			get
			{
				this.DeserializeSettingsIfNeeded ();
				return this.editionSettings;
			}
			set
			{
				this.DeserializeSettingsIfNeeded ();
				this.editionSettings = value ?? new UserEntityEditionSettings (Druid.Parse (this.EntityId));
			}
		}

		/// <summary>
		/// Gets the table settings. This will deserialize the settings when accessed
		/// for the first time.
		/// </summary>
		[System.Diagnostics.DebuggerBrowsable (System.Diagnostics.DebuggerBrowsableState.Never)]
		public UserEntityTableSettings			TableSettings
		{
			get
			{
				this.DeserializeSettingsIfNeeded ();
				return this.tableSettings;
			}
			set
			{
				this.DeserializeSettingsIfNeeded ();
				this.tableSettings = value ?? new UserEntityTableSettings (Druid.Parse (this.EntityId));
			}
		}


		public bool								HasSettings
		{
			get
			{
				return this.editionSettings != null
					|| this.tableSettings != null;
			}
		}

		
		public void PersistSettings(IBusinessContext context)
		{
			if (this.HasSettings)
			{
				if (this.SerializedSettings.IsNull ())
				{
					this.SerializedSettings = context.CreateEntity<XmlBlobEntity> ();
				}

				this.SerializeSettings ();
			}
		}


		private void SerializeSettings()
		{
			this.SerializedSettings.XmlData = new XElement (Xml.Settings,
				this.editionSettings == null ? null : this.editionSettings.Save (Xml.EditionSettings),
				this.tableSettings == null ? null : this.tableSettings.Save (Xml.TableSettings));
		}

		private void DeserializeSettingsIfNeeded()
		{
			if (this.HasSettings == false)
			{
				var xml = this.SerializedSettings.IsNull () ? null : this.SerializedSettings.XmlData;
				var entityId = Druid.Parse (this.EntityId);

				if (xml == null)
				{
					this.editionSettings = new UserEntityEditionSettings (entityId);
					this.tableSettings   = new UserEntityTableSettings (entityId);
				}
				else
				{
					this.editionSettings = UserEntityEditionSettings.Restore (xml.Element (Xml.EditionSettings));
					this.tableSettings   = UserEntityTableSettings.Restore (xml.Element (Xml.TableSettings));
				}
			}
		}


		private static class Xml
		{
			public const string					Settings		= "settings";
			public const string					EditionSettings	= "edition";
			public const string					TableSettings	= "table";
		}


		private UserEntityEditionSettings		editionSettings;
		private UserEntityTableSettings			tableSettings;
	}
}
