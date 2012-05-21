//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.CorePlugIn.WorkflowDesigner.Widgets
{
	/// <summary>
	/// La classe EntitySubView est un bouton pour choisir une sous-vue, avec possibilité
	/// de drag &amp; drop entre EntitySubView.
	/// </summary>
	public class EntitySubView : Button, Epsitec.Common.Widgets.Behaviors.IDragBehaviorHost
	{
		public EntitySubView()
		{
			this.dragBehavior = new Epsitec.Common.Widgets.Behaviors.DragBehavior (this, true, true);
		}
		
		public EntitySubView(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
			}
			
			base.Dispose(disposing);
		}


		#region IDragBehaviorHost Members
		public Point DragLocation
		{
			get
			{
				//	Pas utile ici:
				return Point.Zero;
			}
		}

		
		public bool OnDragBegin(Point cursor)
		{
			//	Crée un échantillon utilisable pour l'opération de drag & drop (il
			//	va représenter visuellement l'échantillon de couleur). On le place
			//	dans un DragWindow et hop.
			EntitySubView widget = new EntitySubView();
			widget.AutoFocus = false;
			
			//	Signale à l'échantillon qui est la cause du drag. On aurait très
			//	bien pu ajouter une variable à EntitySubView, mais ça paraît du
			//	gaspillage de mémoire d'avoir cette variable inutilisée pour
			//	tous les EntitySubView. Alors on utilise une "propriété" :
			EntitySubView.SetDragHost(widget, this);

			double w = this.ActualWidth*0.5;
			double h = this.ActualHeight*0.5;
			
			this.dragTarget = null;
			this.dragOrigin = this.MapClientToScreen(new Point(-w/2, -h/2));
			this.dragWindow = new DragWindow();
			this.dragWindow.Alpha = 0.5;
			this.dragWindow.DefineWidget(widget, new Size(w, h), Margins.Zero);
			this.dragWindow.WindowLocation = this.dragOrigin + cursor;
			this.dragWindow.Owner = this.Window;
			this.dragWindow.FocusWidget (widget);
			this.dragWindow.Show();
			
			this.OnDragStarting();
			return true;
		}

		public void OnDragging(DragEventArgs e)
		{
			this.dragWindow.WindowLocation = this.dragOrigin + e.Offset;
			
			EntitySubView cs = this.SearchDropTarget(e.ToPoint);
			if (cs != this.dragTarget)
			{
				this.DragHilite(this.dragTarget, this, false);
				this.dragTarget = cs;
				this.DragHilite(this.dragTarget, this, true);
			}
		}

		public void OnDragEnd()
		{
			this.DragHilite(this, this.dragTarget, false);
			this.DragHilite(this.dragTarget, this, false);
			
			if (this.dragTarget != null)
			{
				if (this.dragTarget != this)
				{
					this.dragTarget.OnDragEnding();
				}
				
				this.dragWindow.Hide();
				this.dragWindow.Dispose();
				this.dragWindow = null;
			}
			else
			{
				this.dragWindow.DissolveAndDisposeWindow();
				this.dragWindow = null;
			}
		}


		protected EntitySubView SearchDropTarget(Point mouse)
		{
			//	Cherche un widget EntitySubView destinataire du drag & drop.
			mouse = this.MapClientToRoot(mouse);
			Widget root   = this.Window.Root;
			Widget widget = root.FindChild (mouse, WidgetChildFindMode.SkipHidden | WidgetChildFindMode.Deep | WidgetChildFindMode.SkipDisabled);
			return widget as EntitySubView;
		}

		protected void DragHilite(EntitySubView dst, EntitySubView src, bool enable)
		{
			//	Met en évidence le widget EntitySubView destinataire du drag & drop.
			if (dst == null || src == null)  return;

			if (enable)
			{
				if (!dst.Text.StartsWith("<b>"))
				{
					dst.Text = string.Concat("<b><font size=\"150%\">", dst.Text, "</font></b>");
				}
			}
			else
			{
				string text = dst.Text;

				if (text.Length == 3+18 + 1 + 7+4)
				{
					text = text.Substring(3+18, 1);
				}

				dst.Text = text;
			}
		}

		
		public static void SetDragHost(DependencyObject o, EntitySubView value)
		{
			o.SetValue(EntitySubView.DragHostProperty, value);
		}

		public static EntitySubView GetDragHost(DependencyObject o)
		{
			return o.GetValue(EntitySubView.DragHostProperty) as EntitySubView;
		}
		
		public static readonly DependencyProperty DragHostProperty = DependencyProperty.RegisterAttached("DragHost", typeof(EntitySubView), typeof(EntitySubView));
		#endregion


		protected override void ProcessMessage(Message message, Point pos)
		{
			//	Gestion d'un événement.
			EntitySubView dragHost = EntitySubView.GetDragHost(this);
			
			//	Est-ce que l'événement clavier est reçu dans un échantillon en
			//	cours de drag dans un DragWindow ? C'est possible, car le focus
			//	clavier change quand on montre le DragWindow.
			if (dragHost != null && message.IsKeyType)
			{
				//	Signalons l'événement clavier à l'auteur du drag :
				dragHost.ProcessMessage(message, pos);
				return;
			}
			
			if ( !this.dragBehavior.ProcessMessage(message, pos) )
			{
				base.ProcessMessage(message, pos);
			}
		}


		#region
		protected virtual void OnDragStarting()
		{
			//	Génère un événement pour dire que le drag a commencé.
			var handler = this.GetUserEventHandler("DragStarting");
			if (handler != null)
			{
				handler(this);
			}
		}

		public event EventHandler DragStarting
		{
			add
			{
				this.AddUserEventHandler("DragStarting", value);
			}
			remove
			{
				this.RemoveUserEventHandler("DragStarting", value);
			}
		}

		protected virtual void OnDragEnding()
		{
			//	Génère un événement pour dire que le drag est terminé.
			var handler = this.GetUserEventHandler("DragEnding");
			if (handler != null)
			{
				handler(this);
			}
		}

		public event EventHandler DragEnding
		{
			add
			{
				this.AddUserEventHandler("DragEnding", value);
			}
			remove
			{
				this.RemoveUserEventHandler("DragEnding", value);
			}
		}
		#endregion


		private readonly Epsitec.Common.Widgets.Behaviors.DragBehavior	dragBehavior;

		private DragWindow						dragWindow;
		private Point							dragOrigin;
		private EntitySubView					dragTarget;
	}
}
