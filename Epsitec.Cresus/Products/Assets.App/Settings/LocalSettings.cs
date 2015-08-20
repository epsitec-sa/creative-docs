//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Data.Helpers;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.Export;
using Epsitec.Cresus.Assets.Server.Expression;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Settings
{
	/// <summary>
	/// C'est ici que sont concentrés tous les réglages liés au mandat, à l'utilisateur
	/// et à la UI.
	/// </summary>
	public static class LocalSettings
	{
		static LocalSettings()
		{
			LocalSettings.UILanguage   = "fr";
			LocalSettings.DataLanguage = "fr";

			LocalSettings.columnsStates = new Dictionary<string, ColumnsState> ();
			LocalSettings.searchInfos = new Dictionary<SearchKind, SearchInfo> ();
			LocalSettings.createAssetDefaultGroups = new Dictionary<Guid, Guid> ();
			LocalSettings.hiddenWarnings = new HashSet<string> ();

			LocalSettings.Initialize (Timestamp.Now.Date);
		}

		public static void Initialize(System.DateTime defaultDate)
		{
			LocalSettings.columnsStates.Clear ();
			LocalSettings.searchInfos.Clear ();
			LocalSettings.createAssetDefaultGroups.Clear ();

			var defaultTimestamp = new Timestamp (defaultDate, 0);

			LocalSettings.CreateMandatDate     = new System.DateTime (defaultTimestamp.Date.Year, 1, 1);
			LocalSettings.CreateAssetDate      = new System.DateTime (defaultTimestamp.Date.Year, 1, 1);
			LocalSettings.AmortizationDateFrom = new System.DateTime (defaultTimestamp.Date.Year, 1, 1);
			LocalSettings.AmortizationDateTo   = new System.DateTime (defaultTimestamp.Date.Year, 12, 31);
			LocalSettings.LockedDate           = defaultTimestamp.Date;
			LocalSettings.DefaultMandatDate    = defaultTimestamp.Date;
			LocalSettings.DefaultFreeDate      = defaultTimestamp.Date;

			LocalSettings.ExportFormat = ExportFormat.Pdf;

			LocalSettings.ExportTxtProfile  = TextExportProfile.TxtProfile;
			LocalSettings.ExportCsvProfile  = TextExportProfile.CsvProfile;
			LocalSettings.ExportXmlProfile  = XmlExportProfile.Default;
			LocalSettings.ExportYamlProfile = YamlExportProfile.Default;
			LocalSettings.ExportJsonProfile = JsonExportProfile.Default;
			LocalSettings.ExportPdfProfile  = PdfExportProfile.Default;

			FolderItem item = FileManager.GetFolderItem (FolderId.VirtualMyDocuments, FolderQueryMode.NoIcons);
			LocalSettings.ExportTxtFilename  = System.IO.Path.Combine (item.FullPath, "export.txt");
			LocalSettings.ExportCsvFilename  = System.IO.Path.Combine (item.FullPath, "export.csv");
			LocalSettings.ExportXmlFilename  = System.IO.Path.Combine (item.FullPath, "export.xml");
			LocalSettings.ExportYamlFilename = System.IO.Path.Combine (item.FullPath, "export.yaml");
			LocalSettings.ExportJsonFilename = System.IO.Path.Combine (item.FullPath, "export.json");
			LocalSettings.ExportPdfFilename  = System.IO.Path.Combine (item.FullPath, "export.pdf");

			LocalSettings.AccountsImportFilename = null;

			LocalSettings.SplitterAssetsEventPos    = 190;
			LocalSettings.SplitterAssetsMultiplePos = 180;

			LocalSettings.ShowAccountsWarnings = false;

			LocalSettings.AccountCategories = AccountCategory.Actif |
											  AccountCategory.Passif |
											  AccountCategory.Charge |
											  AccountCategory.Produit |
											  AccountCategory.Exploitation |
											  AccountCategory.Revenu |
											  AccountCategory.Depense |
											  AccountCategory.Recette;

			LocalSettings.ExpressionSimulationParams = ExpressionSimulationParams.Default;
		}


		#region Languages
		public static string					UILanguage
		{
			get
			{
				return LocalSettings.uiLanguage;
			}
			set
			{
				LocalSettings.lastUILanguage = LocalSettings.uiLanguage;
				LocalSettings.uiLanguage = value;
			}
		}

		public static string					DataLanguage
		{
			get
			{
				return LocalSettings.dataLanguage;
			}
			set
			{
				LocalSettings.lastDataLanguage = LocalSettings.dataLanguage;
				LocalSettings.dataLanguage = value;
			}
		}

		public static string					LastUILanguage
		{
			get
			{
				return LocalSettings.lastUILanguage;
			}
		}

		public static string					LastDataLanguage
		{
			get
			{
				return LocalSettings.lastDataLanguage;
			}
		}
		#endregion


		#region Columns state
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
		#endregion


		#region Search info
		public static SearchInfo GetSearchInfo(SearchKind kind)
		{
			SearchInfo info;
			if (LocalSettings.searchInfos.TryGetValue (kind, out info))
			{
				return info;
			}
			else
			{
				return SearchInfo.Default;
			}
		}

		public static void SetSearchInfo(SearchKind kind, SearchInfo info)
		{
			LocalSettings.searchInfos[kind] = info;
		}
		#endregion


		#region Create group
		public static void AddCreateGroup(Guid parentGroupGuid, Guid selectedGroupGuid)
		{
			LocalSettings.createAssetDefaultGroups[parentGroupGuid] = selectedGroupGuid;
		}

		public static Guid GetCreateGroup(Guid parentGroupGuid)
		{
			Guid guid;
			if (LocalSettings.createAssetDefaultGroups.TryGetValue (parentGroupGuid, out guid))
			{
				return guid;
			}
			else
			{
				return Guid.Empty;
			}
		}
		#endregion


		#region Hidden warnings
		public static IEnumerable<Warning> GetVisibleWarnings(DataAccessor accessor)
		{
			//	Retourne les avertissements, en masquant ceux qui sont cachés par l'utilisateur.
			return WarningsLogic.GetWarnings (accessor, LocalSettings.ShowAccountsWarnings)
				.Where (x => !LocalSettings.IsHiddenWarnings (x.PersistantUniqueId));
		}

		public static void ClearHiddenWarnings()
		{
			LocalSettings.hiddenWarnings.Clear ();
		}

		public static void AddHiddenWarnings(string persistantUniqueId)
		{
			LocalSettings.hiddenWarnings.Add (persistantUniqueId);
		}

		public static bool IsHiddenWarnings(string persistantUniqueId)
		{
			return LocalSettings.hiddenWarnings.Contains (persistantUniqueId);
		}

		public static bool HasHiddenWarnings()
		{
			return LocalSettings.hiddenWarnings.Any ();
		}
		#endregion


		#region Serialize
		public static void Serialize(System.Xml.XmlWriter writer)
		{
			writer.WriteStartDocument ();
			writer.WriteStartElement (X.LocalSettings);

			writer.WriteElementString (X.DocumentVersion, DataMandat.SerializationVersion);

			LocalSettings.SerializeColumnsState (writer);
			LocalSettings.SerializeSearchInfo (writer);
			LocalSettings.SerializeCreateAssetDefaultGroups (writer);
			LocalSettings.SerializeHiddenWarnings (writer);

			writer.WriteElementString (X.UILanguage,           LocalSettings.UILanguage);
			writer.WriteElementString (X.DataLanguage,         LocalSettings.DataLanguage);
			writer.WriteElementString (X.CreateMandatDate,     LocalSettings.CreateMandatDate.ToStringIO ());
			writer.WriteElementString (X.CreateAssetDate,      LocalSettings.CreateAssetDate.ToStringIO ());
			writer.WriteElementString (X.AmortizationDateFrom, LocalSettings.AmortizationDateFrom.ToStringIO ());
			writer.WriteElementString (X.AmortizationDateTo,   LocalSettings.AmortizationDateTo.ToStringIO ());
			writer.WriteElementString (X.LockedDate,           LocalSettings.LockedDate.ToStringIO ());
			writer.WriteElementString (X.DefaultMandatDate,    LocalSettings.DefaultMandatDate.ToStringIO ());
			writer.WriteElementString (X.DefaultFreeDate,      LocalSettings.DefaultFreeDate.ToStringIO ());

			writer.WriteElementString (X.ExportFormat, LocalSettings.ExportFormat.ToStringIO ());
			
			LocalSettings.ExportTxtProfile .Serialize (writer, X.ExportTxtProfile);
			LocalSettings.ExportCsvProfile .Serialize (writer, X.ExportCsvProfile);
			LocalSettings.ExportXmlProfile .Serialize (writer, X.ExportXmlProfile);
			LocalSettings.ExportYamlProfile.Serialize (writer, X.ExportYamlProfile);
			LocalSettings.ExportJsonProfile.Serialize (writer, X.ExportJsonProfile);
			LocalSettings.ExportPdfProfile .Serialize (writer, X.ExportPdfProfile);

			writer.WriteElementString (X.ExportTxtFilename,  LocalSettings.ExportTxtFilename.ToStringIO ());
			writer.WriteElementString (X.ExportCsvFilename,  LocalSettings.ExportCsvFilename.ToStringIO ());
			writer.WriteElementString (X.ExportXmlFilename,  LocalSettings.ExportXmlFilename.ToStringIO ());
			writer.WriteElementString (X.ExportYamlFilename, LocalSettings.ExportYamlFilename.ToStringIO ());
			writer.WriteElementString (X.ExportJsonFilename, LocalSettings.ExportJsonFilename.ToStringIO ());
			writer.WriteElementString (X.ExportPdfFilename,  LocalSettings.ExportPdfFilename.ToStringIO ());

			writer.WriteElementString (X.AccountsImportFilename, LocalSettings.AccountsImportFilename);

			writer.WriteElementString (X.SplitterAssetsEventPos, LocalSettings.SplitterAssetsEventPos.ToStringIO ());
			writer.WriteElementString (X.SplitterAssetsMultiplePos, LocalSettings.SplitterAssetsMultiplePos.ToStringIO ());

			writer.WriteElementString (X.ShowAccountsWarnings, LocalSettings.ShowAccountsWarnings.ToStringIO ());

			writer.WriteElementString (X.AccountCategories, LocalSettings.AccountCategories.ToStringIO ());

			LocalSettings.ExpressionSimulationParams.Serialize (writer, X.ExpressionSimulationParams);

			writer.WriteEndElement ();
			writer.WriteEndDocument ();
		}

		private static void SerializeColumnsState(System.Xml.XmlWriter writer)
		{
			var columnsStates = LocalSettings.columnsStates.Where (x => !x.Value.IsEmpty).ToArray ();

			if (columnsStates.Any ())
			{
				writer.WriteStartElement (X.ColumnsState);

				foreach (var pair in columnsStates)
				{
					pair.Value.Serialize (writer, pair.Key);
				}

				writer.WriteEndElement ();
			}
		}

		private static void SerializeSearchInfo(System.Xml.XmlWriter writer)
		{
			if (LocalSettings.searchInfos.Any ())
			{
				writer.WriteStartElement (X.SearchInfo);

				foreach (var pair in LocalSettings.searchInfos)
				{
					pair.Value.Serialize (writer, pair.Key.ToStringIO ());
				}

				writer.WriteEndElement ();
			}
		}

		private static void SerializeCreateAssetDefaultGroups(System.Xml.XmlWriter writer)
		{
			if (LocalSettings.createAssetDefaultGroups.Any ())
			{
				writer.WriteStartElement (X.CreateAssetDefaultGroups);

				foreach (var pair in LocalSettings.createAssetDefaultGroups)
				{
					writer.WriteStartElement (X.Group);

					writer.WriteStringAttribute (X.Attr.Parent, pair.Key.ToStringIO ());
					writer.WriteStringAttribute (X.Attr.Selection, pair.Value.ToStringIO ());

					writer.WriteEndElement ();
				}

				writer.WriteEndElement ();
			}
		}

		private static void SerializeHiddenWarnings(System.Xml.XmlWriter writer)
		{
			if (LocalSettings.hiddenWarnings.Any ())
			{
				writer.WriteStartElement (X.HiddenWarnings);

				foreach (var persistantUniqueId in LocalSettings.hiddenWarnings)
				{
					writer.WriteStartElement (X.Warning);
					writer.WriteStringAttribute (X.Attr.PersistantUniqueId, persistantUniqueId);
					writer.WriteEndElement ();
				}

				writer.WriteEndElement ();
			}
		}
		#endregion


		#region Deserialization
		public static void Deserialize(System.Xml.XmlReader reader)
		{
			while (reader.Read ())
			{
				if (reader.NodeType == System.Xml.XmlNodeType.Element)
				{
					if (reader.Name == X.LocalSettings)
					{
						LocalSettings.DeserializeLocalSettings (reader);
					}
				}
				else if (reader.NodeType == System.Xml.XmlNodeType.EndElement)
				{
					break;
				}
			}
		}

		private static void DeserializeLocalSettings(System.Xml.XmlReader reader)
		{
			while (reader.Read ())
			{
				if (reader.NodeType == System.Xml.XmlNodeType.Element)
				{
					switch (reader.Name)
					{
						case X.DocumentVersion:
							var version = reader.ReadElementContentAsString ();
							break;

						case X.ColumnsState:
							LocalSettings.DeserializeColumnsState (reader);
							break;

						case X.SearchInfo:
							LocalSettings.DeserializeSearchInfo (reader);
							break;

						case X.CreateAssetDefaultGroups:
							LocalSettings.DeserializeCreateAssetDefaultGroups (reader);
							break;

						case X.HiddenWarnings:
							LocalSettings.DeserializeHiddenWarnings (reader);
							break;

						case X.UILanguage:
							LocalSettings.UILanguage = reader.ReadElementContentAsString ();
							break;

						case X.DataLanguage:
							LocalSettings.DataLanguage = reader.ReadElementContentAsString ();
							break;

						case X.CreateMandatDate:
							LocalSettings.CreateMandatDate = reader.ReadElementContentAsString ().ParseDate ();
							break;

						case X.CreateAssetDate:
							LocalSettings.CreateAssetDate = reader.ReadElementContentAsString ().ParseDate ();
							break;

						case X.AmortizationDateFrom:
							LocalSettings.AmortizationDateFrom = reader.ReadElementContentAsString ().ParseDate ();
							break;

						case X.AmortizationDateTo:
							LocalSettings.AmortizationDateTo = reader.ReadElementContentAsString ().ParseDate ();
							break;

						case X.LockedDate:
							LocalSettings.LockedDate = reader.ReadElementContentAsString ().ParseDate ();
							break;

						case X.DefaultMandatDate:
							LocalSettings.DefaultMandatDate = reader.ReadElementContentAsString ().ParseDate ();
							break;

						case X.DefaultFreeDate:
							LocalSettings.DefaultFreeDate = reader.ReadElementContentAsString ().ParseDate ();
							break;

						case X.ExportFormat:
							LocalSettings.ExportFormat = reader.ReadElementContentAsString ().ParseType<ExportFormat> ();
							break;

						case X.ExportTxtProfile:
							LocalSettings.ExportTxtProfile = new TextExportProfile (reader);
							break;

						case X.ExportCsvProfile:
							LocalSettings.ExportCsvProfile = new TextExportProfile (reader);
							break;

						case X.ExportXmlProfile:
							LocalSettings.ExportXmlProfile = new XmlExportProfile (reader);
							break;

						case X.ExportYamlProfile:
							LocalSettings.ExportYamlProfile = new YamlExportProfile (reader);
							break;

						case X.ExportJsonProfile:
							LocalSettings.ExportJsonProfile = new JsonExportProfile (reader);
							break;

						case X.ExportPdfProfile:
							LocalSettings.ExportPdfProfile = new PdfExportProfile (reader);
							break;

						case X.ExportTxtFilename:
							LocalSettings.ExportTxtFilename = reader.ReadElementContentAsString ();
							break;

						case X.ExportCsvFilename:
							LocalSettings.ExportCsvFilename = reader.ReadElementContentAsString ();
							break;

						case X.ExportXmlFilename:
							LocalSettings.ExportXmlFilename = reader.ReadElementContentAsString ();
							break;

						case X.ExportYamlFilename:
							LocalSettings.ExportYamlFilename = reader.ReadElementContentAsString ();
							break;

						case X.ExportJsonFilename:
							LocalSettings.ExportJsonFilename = reader.ReadElementContentAsString ();
							break;

						case X.ExportPdfFilename:
							LocalSettings.ExportPdfFilename = reader.ReadElementContentAsString ();
							break;

						case X.AccountsImportFilename:
							LocalSettings.AccountsImportFilename = reader.ReadElementContentAsString ();
							break;

						case X.SplitterAssetsEventPos:
							LocalSettings.SplitterAssetsEventPos = reader.ReadElementContentAsString ().ParseInt ();
							break;

						case X.SplitterAssetsMultiplePos:
							LocalSettings.SplitterAssetsMultiplePos = reader.ReadElementContentAsString ().ParseInt ();
							break;

						case X.ShowAccountsWarnings:
							LocalSettings.ShowAccountsWarnings = reader.ReadElementContentAsString ().ParseBool ();
							break;

						case X.AccountCategories:
							LocalSettings.AccountCategories = reader.ReadElementContentAsString ().ParseType<AccountCategory> ();
							break;

						case X.ExpressionSimulationParams:
							LocalSettings.ExpressionSimulationParams = new ExpressionSimulationParams (reader);
							break;
					}
				}
				else if (reader.NodeType == System.Xml.XmlNodeType.EndElement)
				{
					break;
				}
			}
		}

		private static void DeserializeColumnsState(System.Xml.XmlReader reader)
		{
			LocalSettings.columnsStates.Clear ();

			while (reader.Read ())
			{
				if (reader.NodeType == System.Xml.XmlNodeType.Element)
				{
					var c = new ColumnsState (reader);
					LocalSettings.columnsStates.Add (reader.Name, c);
				}
				else if (reader.NodeType == System.Xml.XmlNodeType.EndElement)
				{
					break;
				}
			}
		}

		private static void DeserializeSearchInfo(System.Xml.XmlReader reader)
		{
			LocalSettings.searchInfos.Clear ();

			while (reader.Read ())
			{
				if (reader.NodeType == System.Xml.XmlNodeType.Element)
				{
					var c = new SearchInfo (reader);
					var k = IOHelpers.ParseType<SearchKind> (reader.Name);
					LocalSettings.searchInfos.Add (k, c);
				}
				else if (reader.NodeType == System.Xml.XmlNodeType.EndElement)
				{
					break;
				}
			}
		}

		private static void DeserializeCreateAssetDefaultGroups(System.Xml.XmlReader reader)
		{
			LocalSettings.searchInfos.Clear ();

			while (reader.Read ())
			{
				if (reader.NodeType == System.Xml.XmlNodeType.Element)
				{
					var k = IOHelpers.ParseGuid (reader[X.Attr.Parent]);
					var v = IOHelpers.ParseGuid (reader[X.Attr.Selection]);
					LocalSettings.createAssetDefaultGroups.Add (k, v);
				}
				else if (reader.NodeType == System.Xml.XmlNodeType.EndElement)
				{
					break;
				}
			}
		}

		private static void DeserializeHiddenWarnings(System.Xml.XmlReader reader)
		{
			LocalSettings.hiddenWarnings.Clear ();

			while (reader.Read ())
			{
				if (reader.NodeType == System.Xml.XmlNodeType.Element)
				{
					LocalSettings.hiddenWarnings.Add (reader[X.Attr.PersistantUniqueId]);
				}
				else if (reader.NodeType == System.Xml.XmlNodeType.EndElement)
				{
					break;
				}
			}
		}
		#endregion


		private static string						uiLanguage;
		private static string						dataLanguage;
		private static string						lastUILanguage;
		private static string						lastDataLanguage;

		public static System.DateTime				CreateMandatDate;
		public static System.DateTime				CreateAssetDate;
		public static System.DateTime				AmortizationDateFrom;
		public static System.DateTime				AmortizationDateTo;
		public static System.DateTime				LockedDate;
		public static System.DateTime				DefaultMandatDate;
		public static System.DateTime				DefaultFreeDate;

		public static ExportFormat					ExportFormat;
		public static TextExportProfile				ExportTxtProfile;
		public static TextExportProfile				ExportCsvProfile;
		public static XmlExportProfile				ExportXmlProfile;
		public static YamlExportProfile				ExportYamlProfile;
		public static JsonExportProfile				ExportJsonProfile;
		public static PdfExportProfile				ExportPdfProfile;
		public static string						ExportTxtFilename;
		public static string						ExportCsvFilename;
		public static string						ExportXmlFilename;
		public static string						ExportYamlFilename;
		public static string						ExportJsonFilename;
		public static string						ExportPdfFilename;
		public static string						AccountsImportFilename;

		public static int							SplitterAssetsEventPos;
		public static int							SplitterAssetsMultiplePos;

		public static bool							ShowAccountsWarnings;

		public static AccountCategory				AccountCategories;

		public static ExpressionSimulationParams	ExpressionSimulationParams;

		private static readonly Dictionary<string, ColumnsState> columnsStates;
		private static readonly Dictionary<SearchKind, SearchInfo> searchInfos;
		private static readonly Dictionary<Guid, Guid> createAssetDefaultGroups;
		private static readonly HashSet<string>		hiddenWarnings;
	}
}
