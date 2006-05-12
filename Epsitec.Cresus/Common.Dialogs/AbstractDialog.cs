//	Copyright © 2004-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// La classe AbstractDialog implémente l'interface IDialog et offre les
	/// méthodes de base pour ouvrir et fermer un dialogue.
	/// </summary>
	public abstract class AbstractDialog : DependencyObject, IDialog
	{
		public AbstractDialog()
		{
		}
		
		
		public void OpenDialog()
		{
			if (this.IsReady)
			{
				if (this.Window == null)
				{
					throw new System.InvalidOperationException ("Cannot show window.");
				}
				
				Widgets.Window owner = this.Owner;
				
				if (owner != null)
				{
					Drawing.Rectangle owner_bounds  = owner.WindowBounds;
					Drawing.Rectangle dialog_bounds = this.Window.WindowBounds;
					
					if (owner.IsMinimized)
					{
						owner_bounds = Widgets.ScreenInfo.AllScreens[0].Bounds;
					}
					
					double ox = System.Math.Floor (owner_bounds.Left + (owner_bounds.Width - dialog_bounds.Width) / 2);
					double oy = System.Math.Floor (owner_bounds.Top  - (owner_bounds.Height - dialog_bounds.Height) / 3 - dialog_bounds.Height);
					
					dialog_bounds.Location = new Drawing.Point (ox, oy);
					
					this.Window.WindowBounds = dialog_bounds;
				}
				
				this.OnDialogOpening ();
				
				if (this.is_modal)
				{
					this.Window.WindowShown += new Support.EventHandler (this.HandleWindowShown);
					this.Window.ShowDialog ();
					this.Window.WindowShown -= new Support.EventHandler (this.HandleWindowShown);
				}
				else
				{
					this.Window.Show ();
					this.HandleWindowShown (this.Window);
				}
			}
		}
		
		public void CloseDialog()
		{
			if (this.IsReady)
			{
				if (this.Window != null)
				{
					if (this.Window.IsActive)
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
					
					this.Window.Hide ();
					
					if (this.is_modal)
					{
						this.Window.Close ();
					}
					
					this.Window.AsyncDispose ();
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
				if ((this.Window != null) &&
					(this.Window.Owner != value))
				{
					this.Window.Owner = value;
					this.OnOwnerChanged ();
				}
			}
		}
		
		public DialogResult						Result
		{
			get
			{
				return this.result;
			}
		}
		
		public virtual Widgets.Window			DispatchWindow
		{
			get
			{
				Widgets.Window window = this.Window;
				Widgets.Window owner  = window.Owner;
				
				return owner == null ? window : owner;
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
		
		public bool								IsModal
		{
			get
			{
				return this.is_modal;
			}
			set
			{
				this.is_modal = value;
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
		protected bool							is_modal = true;
		protected DialogResult					result = DialogResult.None;
	}
}
