//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

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


		public bool								HasSettings
		{
			get
			{
				return this.editionSettings != null;
			}
		}

		
		public void PersistSettings()
		{
			if (this.HasSettings)
			{
				this.SerializeSettings ();
			}
		}


		private void SerializeSettings()
		{
			var xml = new XElement (Xml.Settings,
				this.editionSettings == null
					? null
					: this.editionSettings.Save (Xml.EditionSettings)
			);

			var newValue = DataSetUISettingsEntity.XmlToByteArray (xml);

			if (!ArrayEqualityComparer<byte>.Instance.Equals (newValue, this.SerializedSettings))
			{
				this.SerializedSettings = newValue;
			}
		}

		private void DeserializeSettingsIfNeeded()
		{
			if (this.HasSettings == false)
			{
				var xml = DataSetUISettingsEntity.ByteArrayToXml (this.SerializedSettings);
				var entityId = Druid.Parse (this.EntityId);

				this.editionSettings = xml == null
					? new UserEntityEditionSettings (entityId)
					: UserEntityEditionSettings.Restore (xml.Element (Xml.EditionSettings));
			}
		}


		private static class Xml
		{
			public const string					Settings		= "settings";
			public const string					EditionSettings	= "edition";
		}


		private UserEntityEditionSettings		editionSettings;
	}
}
