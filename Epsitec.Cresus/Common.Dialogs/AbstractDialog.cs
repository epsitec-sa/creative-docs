//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// La classe AbstractDialog implémente l'interface IDialog et offre les
	/// méthodes de base pour ouvrir et fermer un dialogue.
	/// </summary>
	public abstract class AbstractDialog : IDialog
	{
		public AbstractDialog()
		{
		}
		
		
		public void OpenDialog()
		{
			if (this.IsReady)
			{
				if (this.window == null)
				{
					throw new System.InvalidOperationException ("Cannot show window.");
				}
				
				Widgets.Window owner = this.Owner;
				
				if (owner != null)
				{
					Drawing.Rectangle owner_bounds  = owner.WindowBounds;
					Drawing.Rectangle dialog_bounds = this.window.WindowBounds;
					
					double ox = System.Math.Floor (owner_bounds.Left + (owner_bounds.Width - dialog_bounds.Width) / 2);
					double oy = System.Math.Floor (owner_bounds.Top  - (owner_bounds.Height - dialog_bounds.Height) / 3 - dialog_bounds.Height);
					
					dialog_bounds.Location = new Drawing.Point (ox, oy);
					
					this.window.WindowBounds = dialog_bounds;
				}
				
				this.OnDialogOpening ();
				this.window.WindowShown += new Support.EventHandler (this.HandleWindowShown);
				this.window.ShowDialog ();
				this.window.WindowShown -= new Support.EventHandler (this.HandleWindowShown);
			}
		}
		
		public void CloseDialog()
		{
			if (this.IsReady)
			{
				if (this.window != null)
				{
					if (this.window.IsActive)
					{
						//	Si la fenêtre est active, il faut faire attention à rendre d'abord
						//	le parent actif, avant de cacher la fenêtre, pour éviter que le focus
						//	ne parte dans le décor.
						
						Widgets.Window owner = this.Owner;
						
						if (owner != null)
						{
							owner.MakeActive ();
						}
					}
					
					this.window.Hide ();
					this.window.CommandDispatcher.Dispose ();
					this.window.CommandDispatcher = null;
					this.window.AsyncDispose ();
				}
			}
		}
		
		
		public virtual Widgets.Window			Window
		{
			get
			{
				return this.window;
			}
		}
		
		public virtual Widgets.Window			Owner
		{
			get
			{
				if (this.window != null)
				{
					return this.window.Owner;
				}
				
				return null;
			}
			set
			{
				if ((this.window != null) &&
					(this.window.Owner != value))
				{
					this.window.Owner = value;
					this.OnOwnerChanged ();
				}
			}
		}
		
		
		public bool								IsVisible
		{
			get
			{
				if (this.window != null)
				{
					return this.window.IsVisible;
				}
				
				return false;
			}
		}
		
		public virtual bool						IsReady
		{
			get
			{
				return true;
			}
		}
		
		
		
		protected virtual void OnDialogOpening()
		{
			if (this.DialogOpening != null)
			{
				this.DialogOpening (this);
			}
		}
		
		protected virtual void OnDialogOpened()
		{
			if (this.DialogOpened != null)
			{
				this.DialogOpened (this);
			}
		}
		
		protected virtual void OnOwnerChanged()
		{
		}
		
		
		private void HandleWindowShown(object sender)
		{
			this.OnDialogOpened ();
		}
		
		
		
		public event Support.EventHandler		DialogOpening;
		public event Support.EventHandler		DialogOpened;
		
		protected Widgets.Window				window;
	}
}
