using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer
{
	/// <summary>
	/// Permet de modifier les ressources d'un module.
	/// </summary>
	public class Modifier
	{
		public Modifier(Module module)
		{
			this.module = module;
			this.attachViewers = new List<Viewers.Abstract>();
		}

		public void Dispose()
		{
		}


		public bool IsDirty
		{
			//	Indique si le module est à jour ou non.
			get
			{
				return this.isDirty;
			}

			set
			{
				if (this.isDirty != value)
				{
					this.isDirty = value;
					this.module.MainWindow.GetCommandState("Save").Enable = this.isDirty;
				}
			}
		}


		public void Save(Module.BundleType type)
		{
			//	Enregistre toutes les cultures du module.
			ResourceBundleCollection bundles = this.module.Bundles(type);
			foreach (ResourceBundle bundle in bundles)
			{
				this.module.ResourceManager.SetBundle(bundle, ResourceSetMode.UpdateOnly);
			}

			this.module.PanelsWrite();

			foreach (Viewers.Abstract viewer in this.attachViewers)
			{
				viewer.Update();
			}

			this.IsDirty = false;
		}

		public void Delete(Module.BundleType type, Druid druid)
		{
			//	Supprime une ressource dans toutes les cultures du module.
			ResourceBundleCollection bundles = this.module.Bundles(type);
			foreach (ResourceBundle bundle in bundles)
			{
				int index = bundle.IndexOf(druid);
				if (index >= 0)
				{
					bundle.Remove(index);
				}
			}
			this.IsDirty = false;
		}

		public Druid Duplicate(Module.BundleType type, Druid actualDruid, string newName, bool duplicate)
		{
			//	Duplique une ressource dans toutes les cultures du module.
			ResourceBundleCollection bundles = this.module.Bundles(type);
			ResourceBundle defaultBundle = bundles[ResourceLevel.Default];

			int index = this.GetDefaultIndex(type, actualDruid);
			Druid newDruid = this.CreateUniqueDruid(type);

			foreach (ResourceBundle bundle in bundles)
			{
				ResourceBundle.Field newField = bundle.CreateField(ResourceFieldType.Data);
				newField.SetDruid(newDruid);
				newField.SetName(newName);

				if (duplicate)
				{
					ResourceBundle.Field field = bundle[actualDruid];
					if (field.IsEmpty)  continue;
					newField.SetStringValue(field.AsString);
					newField.SetAbout(field.About);
				}
				else
				{
					newField.SetStringValue("");
				}

				if (bundle == defaultBundle)
				{
					newField.SetModificationId(1);
					bundle.Insert(index+1, newField);
				}
				else
				{
					newField.SetModificationId(0);
					bundle.Add(newField);
				}
			}

			this.IsDirty = false;
			return newDruid;
		}

		public Druid Create(Module.BundleType type, string name, string text)
		{
			//	Crée une nouvelle ressource dans la culture par défaut du module.
			ResourceBundleCollection bundles = this.module.Bundles(type);
			System.Globalization.CultureInfo culture = this.module.ResourceManager.ActiveCulture;
			ResourceBundle actualBundle = bundles[ResourceLevel.Localized, culture];
			ResourceBundle defaultBundle = bundles[ResourceLevel.Default];

			if (actualBundle == null)
			{
				actualBundle = defaultBundle;
			}

			Druid druid = this.CreateUniqueDruid(type);

			if (defaultBundle != actualBundle)
			{
				ResourceBundle.Field defaultField = defaultBundle.CreateField(ResourceFieldType.Data);
				defaultField.SetDruid(druid);
				defaultField.SetName(name);
				defaultField.SetModificationId(0);
				defaultBundle.Add(defaultField);
			}

			ResourceBundle.Field newField = actualBundle.CreateField(ResourceFieldType.Data);
			newField.SetDruid(druid);
			newField.SetName(name);
			newField.SetStringValue(text);
			newField.SetModificationId(0);
			actualBundle.Add(newField);

			this.IsDirty = false;
			return druid;
		}

		protected Druid CreateUniqueDruid(Module.BundleType type)
		{
			//	Crée un nouveau druid unique.
			ResourceBundleCollection bundles = this.module.Bundles(type);
			ResourceBundle defaultBundle = bundles[ResourceLevel.Default];

			int moduleId = defaultBundle.Module.Id;
			int developerId = 0;  // [PA] provisoire
			int localId = 0;

			foreach (ResourceBundle.Field field in defaultBundle.Fields)
			{
				Druid druid = field.Druid;

				if (druid.IsValid && druid.Developer == developerId && druid.Local >= localId)
				{
					localId = druid.Local+1;
				}
			}

			return new Druid(moduleId, developerId, localId);
		}

		public bool Move(Module.BundleType type, Druid druid, int direction)
		{
			//	Déplace une ressource dans la culture par défaut du module.
			ResourceBundleCollection bundles = this.module.Bundles(type);
			ResourceBundle defaultBundle = bundles[ResourceLevel.Default];
			int index = this.GetDefaultIndex(type, druid);
			if (index+direction < 0 || index+direction >= defaultBundle.FieldCount)
			{
				return false;
			}

			ResourceBundle.Field field = defaultBundle[index];
			defaultBundle.Remove(index);
			defaultBundle.Insert(index+direction, field);
			this.IsDirty = false;
			return true;
		}

		public void Rename(Module.BundleType type, Druid druid, string newName)
		{
			//	Renomme une ressource dans toutes les cultures du module.
			ResourceBundleCollection bundles = this.module.Bundles(type);
			foreach (ResourceBundle bundle in bundles)
			{
				ResourceBundle.Field field = bundle[druid];
				if (field.IsValid)
				{
					field.SetName(newName);
				}
			}
			this.IsDirty = false;
		}

		public void ModificationClearAll(Module.BundleType type, Druid druid)
		{
			//	Considère une ressource comme à jour dans toutes les cultures secondaires du module.
			ResourceBundleCollection bundles = this.module.Bundles(type);
			ResourceBundle defaultBundle = bundles[ResourceLevel.Default];
			int id = defaultBundle[druid].ModificationId;
			foreach (ResourceBundle bundle in bundles)
			{
				if (bundle != defaultBundle && !bundle[druid].IsEmpty)
				{
					bundle[druid].SetModificationId(id);
				}
			}
			this.IsDirty = false;
		}

		public bool IsModificationAll(Module.BundleType type, Druid druid)
		{
			//	Donne l'état de la commande ModificationAll.
			ResourceBundleCollection bundles = this.module.Bundles(type);
			ResourceBundle defaultBundle = bundles[ResourceLevel.Default];
			int id = defaultBundle[druid].ModificationId;
			int count = 0;
			foreach (ResourceBundle bundle in bundles)
			{
				if (bundle != defaultBundle && !bundle[druid].IsEmpty)
				{
					if (bundle[druid].ModificationId < id)
					{
						count ++;
					}
				}
			}
			return (count != bundles.Count-1);
		}

		public void CreateIfNecessary(Module.BundleType type, ResourceBundle secondaryBundle, Druid druid)
		{
			//	Crée une ressource secondaire, si nécessaire.
			ResourceBundleCollection bundles = this.module.Bundles(type);
			ResourceBundle defaultBundle = bundles[ResourceLevel.Default];

			ResourceBundle.Field field = secondaryBundle[druid];
			if (field.IsEmpty)
			{
				ResourceBundle.Field defaultField = defaultBundle[druid];
				ResourceBundle.Field newField = secondaryBundle.CreateField(ResourceFieldType.Data);
				newField.SetName(defaultField.Name);
				newField.SetDruid(druid);
				newField.SetModificationId(defaultField.ModificationId);

				secondaryBundle.Add(newField);
			}
		}

		protected int GetDefaultIndex(Module.BundleType type, Druid druid)
		{
			//	Cherche l'index d'une ressource d'après son druid.
			ResourceBundleCollection bundles = this.module.Bundles(type);
			ResourceBundle defaultBundle = bundles[ResourceLevel.Default];
			ResourceBundle.Field field = defaultBundle[druid];
			return defaultBundle.IndexOf(field);
		}


		#region Viewers
		public Viewers.Abstract ActiveViewer
		{
			//	Un seul visualisateur privilégié peut être actif.
			get
			{
				return this.activeViewer;
			}
			set
			{
				this.activeViewer = value;
			}
		}

		public Module.BundleType ActiveBundleType
		{
			get
			{
				if (this.activeViewer == null)
				{
					return Module.BundleType.Unknow;
				}
				else
				{
					return this.activeViewer.BundleType;
				}
			}
		}

		public void AttachViewer(Viewers.Abstract viewer)
		{
			//	Attache un nouveau visualisateur à ce module.
			this.attachViewers.Add(viewer);
		}

		public void DetachViewer(Viewers.Abstract viewer)
		{
			//	Détache un visualisateur de ce module.
			this.attachViewers.Remove(viewer);
		}

		public List<Viewers.Abstract> AttachViewers
		{
			//	Liste des visualisateurs attachés au module.
			get
			{
				return this.attachViewers;
			}
		}
		#endregion

		
		protected Module						module;
		protected bool							isDirty = false;
		protected Viewers.Abstract				activeViewer;
		protected List<Viewers.Abstract>		attachViewers;
	}
}
