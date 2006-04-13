using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer
{
	public delegate void SimpleEventHandler();

	/// <summary>
	/// Notifications centralis�es.
	/// </summary>
	public class Notifier
	{
		public Notifier(Module module)
		{
			this.module = module;
		}

		public void Dispose()
		{
		}


		public bool Enable
		{
			//	Etat du notificateur.
			get
			{
				return this.enable;
			}

			set
			{
				this.enable = value;
			}
		}

		public void NotifyAllChanged()
		{
			//	Indique que tout a chang�.
			if ( !this.enable )  return;

			this.modeChanged = true;
			this.saveChanged = true;
			this.infoAccessChanged = true;

			this.NotifyAsync();
		}

		public void NotifyModeChanged()
		{
			//	Indique que les informations sur le document ont chang�.
			//	Nom du document, taille, etc.
			if ( !this.enable )  return;
			this.modeChanged = true;
			this.NotifyAsync();
		}

		public void NotifySaveChanged()
		{
			//	Indique que les informations sur le document ont chang�.
			//	Nom du document, taille, etc.
			if ( !this.enable )  return;
			this.saveChanged = true;
			this.NotifyAsync();
		}

		public void NotifyInfoAccessChanged()
		{
			//	Indique que les informations sur le document ont chang�.
			//	Nom du document, taille, etc.
			if ( !this.enable )  return;
			this.infoAccessChanged = true;
			this.NotifyAsync();
		}


		protected void NotifyAsync()
		{
			//	Notifie qu'il faudra faire le GenerateEvents lorsque Windows
			//	aura le temps.
			if ( this.module.Modifier.ActiveViewer == null )  return;
			Window window = this.module.Modifier.ActiveViewer.Window;
			if ( window == null )  return;
			window.AsyncNotify();
		}

		
		public void GenerateEvents()
		{
			//	G�n�re tous les �v�nements pour informer des changements, en fonction
			//	des NotifyXYZ fait pr�c�demment.
			if (this.modeChanged)
			{
				this.OnModeChanged();
				this.modeChanged = false;
			}

			if (this.saveChanged)
			{
				this.OnSaveChanged();
				this.saveChanged = false;
			}

			if (this.infoAccessChanged)
			{
				this.OnInfoAccessChanged();
				this.infoAccessChanged = false;
			}
		}


		protected void OnModeChanged()
		{
			if (this.ModeChanged != null)  // qq'un �coute ?
			{
				this.ModeChanged();
			}
		}

		protected void OnSaveChanged()
		{
			if (this.SaveChanged != null)  // qq'un �coute ?
			{
				this.SaveChanged();
			}
		}

		protected void OnInfoAccessChanged()
		{
			if (this.InfoAccessChanged != null)  // qq'un �coute ?
			{
				this.InfoAccessChanged();
			}
		}


		public event SimpleEventHandler			ModeChanged;
		public event SimpleEventHandler			SaveChanged;
		public event SimpleEventHandler			InfoAccessChanged;

		protected Module						module;
		protected bool							enable = true;
		protected bool							modeChanged;
		protected bool							saveChanged;
		protected bool							infoAccessChanged;
	}
}
