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
					this.module.Notifier.NotifySaveChanged();
				}
			}
		}


		#region Viewers
		public Viewer ActiveViewer
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

		public void AttachViewer(Viewer viewer)
		{
			//	Attache un nouveau visualisateur à ce module.
			this.attachViewers.Add(viewer);
		}

		public void DetachViewer(Viewer viewer)
		{
			//	Détache un visualisateur de ce module.
			this.attachViewers.Remove(viewer);
		}

		public System.Collections.ArrayList AttachViewers
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
		protected Viewer						activeViewer;
		protected System.Collections.ArrayList	attachViewers;
	}
}
