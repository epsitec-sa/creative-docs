//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support;
using Epsitec.Cresus.Assets.Data;

namespace Epsitec.Cresus.Assets.App.Settings
{
	/// <summary>
	/// C'est ici que sont concentrés tous les réglages liés à l'utilisateur et à la UI.
	/// Il faudra être capable de (dé)sérialiser tout ça !
	/// </summary>
	public static class LocalSettings
	{
		static LocalSettings()
		{
			LocalSettings.Initialize (Timestamp.Now.Date);
		}

		public static void Initialize(System.DateTime defaultDate)
		{
			var defaultTimestamp = new Timestamp (defaultDate, 0);

			LocalSettings.CreateMandatDate     = new System.DateTime (defaultTimestamp.Date.Year, 1, 1);
			LocalSettings.CreateAssetDate      = new System.DateTime (defaultTimestamp.Date.Year, 1, 1);
			LocalSettings.AmortizationDateFrom = new System.DateTime (defaultTimestamp.Date.Year, 1, 1);
			LocalSettings.AmortizationDateTo   = new System.DateTime (defaultTimestamp.Date.Year, 12, 31);
			LocalSettings.LockedDate           = defaultTimestamp.Date;
			LocalSettings.DefaultMandatDate    = defaultTimestamp.Date;
			LocalSettings.DefaultFreeDate      = defaultTimestamp.Date;

			FolderItem item = FileManager.GetFolderItem (FolderId.VirtualMyDocuments, FolderQueryMode.NoIcons);
			LocalSettings.ExportFilename = System.IO.Path.Combine (item.FullPath, "export.csv");
		}

		public static string Serialize()
		{
			return null;  // TODO:
		}

		public static void Deserialize(string data)
		{
			// TODO:
		}


		public static System.DateTime			CreateMandatDate;
		public static System.DateTime			CreateAssetDate;
		public static System.DateTime			AmortizationDateFrom;
		public static System.DateTime			AmortizationDateTo;
		public static System.DateTime			LockedDate;
		public static System.DateTime			DefaultMandatDate;
		public static System.DateTime			DefaultFreeDate;
		public static string					ExportFilename;
	}
}
