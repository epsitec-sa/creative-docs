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

		
		public void PersistSettings(BusinessContext context)
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
				this.dataSetSettings == null ? null : this.dataSetSettings.Save (Xml.DataSetSettings));
		}

		private void DeserializeSettingsIfNeeded()
		{
			if (this.HasSettings == false)
			{
				var xml = this.SerializedSettings.IsNull () ? null : this.SerializedSettings.XmlData;
				var dataSetCommandId = Druid.Parse (this.DataSetCommandId);

				this.dataSetSettings = xml == null
					? new UserDataSetSettings (dataSetCommandId)
					: UserDataSetSettings.Restore (xml.Element (Xml.DataSetSettings));
			}
		}


		private static class Xml
		{
			public const string					Settings		= "settings";
			public const string					DataSetSettings	= "dataSet";
		}


		private UserDataSetSettings			dataSetSettings;
	}
}
