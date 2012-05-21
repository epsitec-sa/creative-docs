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
		public enum ModificationState
		{
			Normal,			//	défini normalement
			Empty,			//	vide (fond rouge)
			Modified,		//	modifié (fond jaune)
		}


		public Modifier(Module module)
		{
			this.module = module;
			this.attachViewers = new List<Viewers.Abstract>();
		}

		public void Dispose()
		{
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

		public ResourceAccess.Type ActiveResourceType
		{
			//	Retourne le type du visualisateur actif.
			get
			{
				if (this.activeViewer == null)
				{
					return ResourceAccess.Type.Unknown;
				}
				else
				{
					return this.activeViewer.ResourceType;
				}
			}
		}

		public Viewers.Entities EntitiesViewer
		{
			get
			{
				foreach (var viewer in this.attachViewers)
				{
					if (viewer is Viewers.Entities)
					{
						return viewer as Viewers.Entities;
					}
				}

				return null;
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
		protected Viewers.Abstract				activeViewer;
		protected List<Viewers.Abstract>		attachViewers;
	}
}
