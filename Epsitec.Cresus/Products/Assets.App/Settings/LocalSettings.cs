//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.Export;

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
			LocalSettings.columnsStates = new Dictionary<string, ColumnsState> ();

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
			var filename = System.IO.Path.Combine (item.FullPath, "export.txt");
			LocalSettings.ExportInstructions = new ExportInstructions (ExportFormat.Txt, filename);

			LocalSettings.ExportTxtProfile  = TextExportProfile.TxtProfile;
			LocalSettings.ExportCsvProfile  = TextExportProfile.CsvProfile;
			LocalSettings.ExportXmlProfile  = XmlExportProfile.Default;
			LocalSettings.ExportYamlProfile = YamlExportProfile.Default;
			LocalSettings.ExportJsonProfile = JsonExportProfile.Default;
		}


		public static ColumnsState GetColumnsState(string name)
		{
			ColumnsState columnsState;
			if (LocalSettings.columnsStates.TryGetValue (name, out columnsState))
			{
				return columnsState;
			}
			else
			{
				return ColumnsState.Empty;
			}
		}

		public static void SetColumnsState(string name, ColumnsState columnsState)
		{
			LocalSettings.columnsStates[name] = columnsState;
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

		public static ExportInstructions		ExportInstructions;
		public static TextExportProfile			ExportTxtProfile;
		public static TextExportProfile			ExportCsvProfile;
		public static XmlExportProfile			ExportXmlProfile;
		public static YamlExportProfile			ExportYamlProfile;
		public static JsonExportProfile			ExportJsonProfile;

		private static readonly Dictionary<string, ColumnsState> columnsStates;
	}
}
