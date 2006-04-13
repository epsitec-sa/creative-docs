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
			this.attachViewers = new System.Collections.ArrayList();
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
					this.module.Notifier.NotifySaveChanged();
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

		public void Duplicate(string name, string newName)
		{
			//	Duplique une ressource dans toutes les cultures du module.
			ResourceBundleCollection bundles = this.module.Bundles;
			ResourceBundle defaultBundle = bundles[ResourceLevel.Default];
			int index = this.GetDefaultIndex(name);
			foreach (ResourceBundle bundle in bundles)
			{
				ResourceBundle.Field field = bundle[name];
				if (field == null || field.Name == null)  continue;
				// TODO: v�rifier si c'est bien ainsi qu'il faut cr�er un ResourceBundle.Field !
				ResourceBundle.Field newField = new ResourceBundle.Field(bundle, field.Xml);
				newField.SetName(newName);

				if (bundle == defaultBundle)
				{
					bundle.Insert(index+1, newField);
				}
				else
				{
					bundle.Add(newField);
				}
			}
			this.IsDirty = false;
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
				bundle[name].SetName(newName);
			}
			this.IsDirty = false;
		}

		public void CreateIfNecessary(ResourceBundle bundle, string name)
		{
			//	Cr�e une ressource secondaire, si n�cessaire.
			ResourceBundle.Field field = bundle[name];
			if (field == null || field.Name == null)
			{
				// TODO: comment cr�er un nouveau ResourceBundle.Field ???
				ResourceBundle.Field newField = new ResourceBundle.Field(bundle, bundle.XmlDocument);
				newField.SetName(name);
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
		public Viewer ActiveViewer
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

		public void AttachViewer(Viewer viewer)
		{
			//	Attache un nouveau visualisateur � ce module.
			this.attachViewers.Add(viewer);
		}

		public void DetachViewer(Viewer viewer)
		{
			//	D�tache un visualisateur de ce module.
			this.attachViewers.Remove(viewer);
		}

		public System.Collections.ArrayList AttachViewers
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
		protected Viewer						activeViewer;
		protected System.Collections.ArrayList	attachViewers;
	}
}
