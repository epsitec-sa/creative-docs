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
			this.attachViewers = new List<Viewer.Abstract>();
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


		public void Save()
		{
			//	Enregistre toutes les cultures du module.
			ResourceBundleCollection bundles = this.module.Bundles;
			foreach (ResourceBundle bundle in bundles)
			{
				this.module.ResourceManager.SetBundle(bundle, ResourceSetMode.UpdateOnly);
			}
			this.IsDirty = false;
		}

		public void Delete(string name)
		{
			//	Supprime une ressource dans toutes les cultures du module.
			ResourceBundleCollection bundles = this.module.Bundles;
			foreach (ResourceBundle bundle in bundles)
			{
				bundle.Remove(name);
			}
			this.IsDirty = false;
		}

		public void Duplicate(string name, string newName, bool duplicate)
		{
			//	Duplique une ressource dans toutes les cultures du module.
			ResourceBundleCollection bundles = this.module.Bundles;
			ResourceBundle defaultBundle = bundles[ResourceLevel.Default];
			int index = this.GetDefaultIndex(name);
			foreach (ResourceBundle bundle in bundles)
			{
				ResourceBundle.Field field = bundle[name];
				ResourceBundle.Field newField = bundle.CreateField(ResourceFieldType.Data);
				newField.SetName(newName);
				if (duplicate)
				{
					if (field == null || field.Name == null)  continue;
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
		}

		public bool Move(string name, int direction)
		{
			//	Déplace une ressource dans la culture par défaut du module.
			ResourceBundleCollection bundles = this.module.Bundles;
			ResourceBundle defaultBundle = bundles[ResourceLevel.Default];
			int index = this.GetDefaultIndex(name);
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

		public void Rename(string name, string newName)
		{
			//	Renomme une ressource dans toutes les cultures du module.
			ResourceBundleCollection bundles = this.module.Bundles;
			foreach (ResourceBundle bundle in bundles)
			{
				ResourceBundle.Field field = bundle[name];
				if (field == null || field.Name == null)  continue;
				bundle[name].SetName(newName);
			}
			this.IsDirty = false;
		}

		public void ModificationClearAll(string name)
		{
			//	Considère une ressource comme à jour dans toutes les cultures secondaires du module.
			ResourceBundleCollection bundles = this.module.Bundles;
			ResourceBundle defaultBundle = bundles[ResourceLevel.Default];
			int id = defaultBundle[name].ModificationId;
			foreach (ResourceBundle bundle in bundles)
			{
				if (bundle != defaultBundle)
				{
					bundle[name].SetModificationId(id);
				}
			}
			this.IsDirty = false;
		}

		public bool IsModificationAll(string name)
		{
			//	Donne l'état de la commande ModificationAll.
			ResourceBundleCollection bundles = this.module.Bundles;
			ResourceBundle defaultBundle = bundles[ResourceLevel.Default];
			int id = defaultBundle[name].ModificationId;
			int count = 0;
			foreach (ResourceBundle bundle in bundles)
			{
				if (bundle != defaultBundle)
				{
					if (bundle[name].ModificationId < id)
					{
						count ++;
					}
				}
			}
			return (count != bundles.Count-1);
		}

		public void CreateIfNecessary(ResourceBundle bundle, string name, int id)
		{
			//	Crée une ressource secondaire, si nécessaire.
			ResourceBundle.Field field = bundle[name];
			if (field == null || field.Name == null)
			{
				ResourceBundle.Field newField = bundle.CreateField(ResourceFieldType.Data);
				newField.SetName(name);

				if (id != -1)
				{
					newField.SetModificationId(id);
				}

				bundle.Add(newField);
			}
		}

		public bool IsExistingName(string baseName)
		{
			//	Indique si un nom existe.
			ResourceBundleCollection bundles = this.module.Bundles;
			ResourceBundle defaultBundle = bundles[ResourceLevel.Default];

			ResourceBundle.Field field = defaultBundle[baseName];
			return (field != null && field.Name != null);
		}

		public string GetDuplicateName(string baseName)
		{
			//	Retourne le nom à utiliser lorsqu'un nom existant est dupliqué.
			ResourceBundleCollection bundles = this.module.Bundles;
			ResourceBundle defaultBundle = bundles[ResourceLevel.Default];

			int numberLength = 0;
			while (baseName.Length > 0)
			{
				char last = baseName[baseName.Length-1-numberLength];
				if (last >= '0' && last <= '9')
				{
					numberLength ++;
				}
				else
				{
					break;
				}
			}

			int nextNumber = 2;
			if (numberLength > 0)
			{
				nextNumber = int.Parse(baseName.Substring(baseName.Length-numberLength))+1;
				baseName = baseName.Substring(0, baseName.Length-numberLength);
			}

			string newName = baseName;
			for (int i=nextNumber; i<nextNumber+100; i++)
			{
				newName = string.Concat(baseName, i.ToString(System.Globalization.CultureInfo.InvariantCulture));
				ResourceBundle.Field field = defaultBundle[newName];
				if ( field == null || field.Name == null )  break;
			}

			return newName;
		}

		protected int GetDefaultIndex(string name)
		{
			//	Cherche l'index d'une ressource d'après son nom.
			ResourceBundleCollection bundles = this.module.Bundles;
			ResourceBundle defaultBundle = bundles[ResourceLevel.Default];
			ResourceBundle.Field field = defaultBundle[name];
			return defaultBundle.IndexOf(field);
		}


		#region Viewers
		public Viewer.Abstract ActiveViewer
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

		public void AttachViewer(Viewer.Abstract viewer)
		{
			//	Attache un nouveau visualisateur à ce module.
			this.attachViewers.Add(viewer);
		}

		public void DetachViewer(Viewer.Abstract viewer)
		{
			//	Détache un visualisateur de ce module.
			this.attachViewers.Remove(viewer);
		}

		public List<Viewer.Abstract> AttachViewers
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
		protected Viewer.Abstract				activeViewer;
		protected List<Viewer.Abstract>			attachViewers;
	}
}
