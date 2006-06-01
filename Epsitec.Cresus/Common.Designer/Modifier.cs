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
			//	Indique si le module est � jour ou non.
			get
			{
				return this.isDirty;
			}

			set
			{
				if (this.isDirty != value)
				{
					this.isDirty = value;
					this.module.MainWindow.GetCommandState ("Save").Enable = this.isDirty;
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

			this.module.PanelsWrite();

			foreach (Viewers.Abstract viewer in this.attachViewers)
			{
				viewer.Update();
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

		public Druid Create(string name, string text)
		{
			//	Cr�e une nouvelle ressource dans la culture par d�faut du module.
			ResourceBundleCollection bundles = this.module.Bundles;
			ResourceBundle defaultBundle = bundles[ResourceLevel.Default];

			Druid druid = new Druid();  // TODO: [PA] comment g�n�rer un beau Panoramix unique et tout neuf ?
			ResourceBundle.Field newField = defaultBundle.CreateField(ResourceFieldType.Data);
			newField.SetDruid(druid);
			newField.SetName(name);
			newField.SetStringValue(text);
			newField.SetModificationId(0);
			defaultBundle.Add(newField);

			this.IsDirty = false;
			return druid;
		}

		public bool Move(string name, int direction)
		{
			//	D�place une ressource dans la culture par d�faut du module.
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
			//	Consid�re une ressource comme � jour dans toutes les cultures secondaires du module.
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
			//	Donne l'�tat de la commande ModificationAll.
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
			//	Cr�e une ressource secondaire, si n�cessaire.
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

		protected int GetDefaultIndex(string name)
		{
			//	Cherche l'index d'une ressource d'apr�s son nom.
			ResourceBundleCollection bundles = this.module.Bundles;
			ResourceBundle defaultBundle = bundles[ResourceLevel.Default];
			ResourceBundle.Field field = defaultBundle[name];
			return defaultBundle.IndexOf(field);
		}


		#region Viewers
		public Viewers.Abstract ActiveViewer
		{
			//	Un seul visualisateur privil�gi� peut �tre actif.
			get
			{
				return this.activeViewer;
			}
			set
			{
				this.activeViewer = value;
			}
		}

		public void AttachViewer(Viewers.Abstract viewer)
		{
			//	Attache un nouveau visualisateur � ce module.
			this.attachViewers.Add(viewer);
		}

		public void DetachViewer(Viewers.Abstract viewer)
		{
			//	D�tache un visualisateur de ce module.
			this.attachViewers.Remove(viewer);
		}

		public List<Viewers.Abstract> AttachViewers
		{
			//	Liste des visualisateurs attach�s au module.
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
