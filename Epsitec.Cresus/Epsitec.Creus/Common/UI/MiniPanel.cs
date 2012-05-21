//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

//	******* EN CHANTIER *******

using Epsitec.Common.Types;
using Epsitec.Common.UI;

using System.Collections.Generic;

[assembly: DependencyClass (typeof (MiniPanel))]

namespace Epsitec.Common.UI
{
	//	TODO: terminer cette classe.
	
	//	L'idée est de pouvoir utiliser un Placeholder dont le contenu peut éventuellement
	//	s'élargir au besoin, en cours d'édition, même si son parent direct est trop petit.
	//	On place les widgets de l'interface graphique du Placeholder provisoirement dans
	//	un MiniPanel qui est un enfant du PanelStack.
	
	public class MiniPanel : Widgets.AbstractGroup
	{
		public MiniPanel()
		{
		}

		
		public PanelStack PanelStack
		{
			get
			{
				return this.panelStack;
			}
			set
			{
				if (this.panelStack != value)
				{
					this.Detach ();
					this.panelStack = value;
				}
			}
		}

		public void Attach()
		{
			if ((this.panelStack != null) &&
				(this.isAttached == false))
			{
				this.isAttached = true;
				this.panelStack.Add (this);
				this.panelStack.Window.FocusedWidgetChanged += this.HandleWindowFocusedWidgetChanged;
			}
		}

		public Drawing.Rectangle Aperture
		{
			get
			{
				return this.aperture;
			}
			set
			{
				if (this.aperture != value)
				{
					DependencyPropertyChangedEventArgs e = new DependencyPropertyChangedEventArgs ("Aperture", this.aperture, value);
					this.aperture = value;
					this.OnApertureChanged (e);
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.Detach ();
			}
			
			base.Dispose (disposing);
		}

		private void Detach()
		{
			if (this.panelStack != null)
			{
				this.panelStack.Window.FocusedWidgetChanged -= this.HandleWindowFocusedWidgetChanged;
				this.panelStack.Remove (this);
				this.panelStack = null;
			}
		}

		private void HandleWindowFocusedWidgetChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (this.ContainsKeyboardFocus)
			{
				if (this.keyboardFocus == false)
				{
					this.keyboardFocus = true;
				}
			}
			else
			{
				if (this.keyboardFocus == true)
				{
					this.keyboardFocus = false;
					this.OnLostKeyboardFocus ();
				}
			}
		}

		protected void OnLostKeyboardFocus()
		{
			if (this.LostKeyboardFocus != null)
			{
				this.LostKeyboardFocus (this);
			}

			this.Detach ();
		}

		protected void OnApertureChanged(DependencyPropertyChangedEventArgs e)
		{
			if (this.ApertureChanged != null)
			{
				this.ApertureChanged (this, e);
			}
		}

		public event Support.EventHandler LostKeyboardFocus;
		public event Support.EventHandler<DependencyPropertyChangedEventArgs> ApertureChanged;

		private Drawing.Rectangle aperture;
		private PanelStack panelStack;
		private bool keyboardFocus;
		private bool isAttached;
	}
}
