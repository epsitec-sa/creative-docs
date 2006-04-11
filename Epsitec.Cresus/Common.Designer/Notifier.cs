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

			this.saveChanged = true;

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
			if (this.saveChanged)
			{
				this.OnSaveChanged();
				this.saveChanged = false;
			}
		}


		protected void OnSaveChanged()
		{
			if (this.SaveChanged != null)  // qq'un �coute ?
			{
				this.SaveChanged();
			}
		}




		public event SimpleEventHandler			SaveChanged;

		protected Module						module;
		protected bool							enable = true;
		protected bool							saveChanged;
	}
}
