//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

using Epsitec.Cresus.Core.Library.Settings;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	partial class DataSetUISettingsEntity
	{
		/// <summary>
		/// Gets the table settings. This will deserialize the settings when accessed
		/// for the first time.
		/// </summary>
		[System.Diagnostics.DebuggerBrowsable (System.Diagnostics.DebuggerBrowsableState.Never)]
		public UserDataSetSettings			DataSetSettings
		{
			get
			{
				this.DeserializeSettingsIfNeeded ();
				return this.dataSetSettings;
			}
			set
			{
				this.DeserializeSettingsIfNeeded ();
				this.dataSetSettings = value ?? new UserDataSetSettings (Druid.Parse (this.DataSetCommandId));
			}
		}


		public bool								HasSettings
		{
			get
			{
				return this.dataSetSettings != null;
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
				this.dataSetSettings == null
					? null
					: this.dataSetSettings.Save (Xml.DataSetSettings)
			);

			var newValue = DataSetUISettingsEntity.XmlToByteArray (xml);

			if (!ArrayEqualityComparer<byte>.Instance.Equals(newValue, this.SerializedSettings))
			{
				this.SerializedSettings = newValue;
			}
		}

		private void DeserializeSettingsIfNeeded()
		{
			if (this.HasSettings == false)
			{
				var xml = DataSetUISettingsEntity.ByteArrayToXml (this.SerializedSettings);
				var dataSetCommandId = Druid.Parse (this.DataSetCommandId);

				this.dataSetSettings = xml == null
					? new UserDataSetSettings (dataSetCommandId)
					: UserDataSetSettings.Restore (xml.Element (Xml.DataSetSettings));
			}
		}


		public static byte[] XmlToByteArray(XElement xml)
		{
			if (xml == null)
			{
				return null;
			}
			
			var str = xml.ToString(SaveOptions.DisableFormatting);
			var encoding = new System.Text.UTF8Encoding ();

			return encoding.GetBytes (str);
		}

		public static XElement ByteArrayToXml(byte[] data)
		{
			if (data == null)
			{
				return null;
			}

			var encoding = new System.Text.UTF8Encoding ();
			var str = encoding.GetString (data);

			return XElement.Parse (str);
		}


		private static class Xml
		{
			public const string					Settings		= "settings";
			public const string					DataSetSettings	= "dataSet";
		}


		private UserDataSetSettings			dataSetSettings;
	}
}
