﻿//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

using Epsitec.Common.Support;

namespace Epsitec.ModuleRepository
{
	[ServiceBehavior]
	public class ModuleRepositoryService : IModuleRepositoryService
	{
		#region IModuleRepositoryService Members

		public int GetNewModuleId(string moduleName, string developerName)
		{
			ModuleRecord record = null;

			try
			{
				lock (this.records)
				{
					record = this.records.Find (item => item.DeveloperName == developerName && item.ModuleState == ModuleState.FreeForReuse);

					if (record != null)
					{
						record.ModuleName = moduleName;
						record.ModuleState = ModuleState.InUse;

						return record.ModuleId;
					}
				}


				record = new ModuleRecord ();

				record.ModuleName = moduleName;
				record.DeveloperName = developerName;

				int moduleId = 0;
				int insertIndex = 0;

				lock (this.records)
				{
					for (int i = 0; i < this.records.Count-1; i++)
					{
						if (this.records[i].ModuleId+1 < this.records[i+1].ModuleId)
						{
							moduleId = this.records[i].ModuleId+1;
							insertIndex = i+1;
							break;
						}
					}

					if (moduleId == 0)
					{
						moduleId = 1000 + this.records.Count;
						insertIndex = this.records.Count;
					}

					record.ModuleId = moduleId;
					record.ModuleState = ModuleState.InUse;

					this.records.Insert (insertIndex, record);
				}

				return record.ModuleId;
			}
			finally
			{
				this.Persist ();
			}
		}

		public bool RecycleModuleId(int moduleId, string developerName)
		{
			lock (this.records)
			{
				ModuleRecord record = this.records.Find (item => item.ModuleId == moduleId);

				if (record == null)
				{
					return false;
				}
				else if (record.DeveloperName == developerName)
				{
					record.ModuleState = ModuleState.FreeForReuse;
					this.Persist ();
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		
		public ModuleDirectory CreateEmptyModule(int moduleId, string moduleLayerPrefix, string sourceNamespace)
		{
			ModuleRecord record = this.records.Find (item => item.ModuleId == moduleId);

			if (record == null)
			{
				return null;
			}

			ModuleDirectory directory = new ModuleDirectory (record.ModuleName);
			ResourceModuleInfo info = new ResourceModuleInfo ();

			info.FullId = new ResourceModuleId (record.ModuleName, "", record.ModuleId, ResourceModuleId.ConvertPrefixToLayer (moduleLayerPrefix));
			info.SourceNamespace = sourceNamespace;

			this.CreateModuleInfo (directory, info);

			return directory;
		}

		#endregion

		private void CreateModuleInfo(ModuleDirectory directory, ResourceModuleInfo info)
		{
			string comment = "";
			System.Xml.XmlDocument xml = ResourceModule.CreateXmlManifest (info, comment);
			System.IO.MemoryStream stream = new System.IO.MemoryStream ();
			xml.Save (stream);
			directory.AddFile (new ModuleFile () { Path = System.IO.Path.Combine (directory.Name, "module.info"), Data = stream.ToArray () });
		}

		private void Persist()
		{
			lock (this.records)
			{
				ModuleStore.Write (ModuleRepositoryService.storePath, this.records);
			}
		}

		private void Reload()
		{
			lock (this.records)
			{
				this.records.Clear ();
				
				if (System.IO.File.Exists (ModuleRepositoryService.storePath))
				{
					this.records.AddRange (ModuleStore.Read (ModuleRepositoryService.storePath));
				}
			}
		}

		static ModuleRepositoryService()
		{
			ModuleRepositoryService service = new ModuleRepositoryService ();
			service.Reload ();
		}

		private List<ModuleRecord> records = ModuleRepositoryService.globalRecords;

		private static List<ModuleRecord> globalRecords = new List<ModuleRecord> ();
		private static string storePath = @"S:\Epsitec.Cresus\module store.xml";
	}
}
