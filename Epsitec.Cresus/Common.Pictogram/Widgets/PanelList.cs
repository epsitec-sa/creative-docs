using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Pictogram.Data;

namespace Epsitec.Common.Pictogram.Widgets
{
	/// <summary>
	/// La classe PanelList permet de choisir un rang dans une liste.
	/// </summary>
	
	[SuppressBundleSupport]
	
	public class PanelList : AbstractPanel
	{
		public PanelList()
		{
			this.label = new StaticText(this);
			this.label.Alignment = Drawing.ContentAlignment.MiddleLeft;

			this.list = new ScrollList(this);
			this.list.SelectedIndexChanged += new EventHandler(this.ListChanged);
			this.list.TabIndex = 1;
			this.list.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
		}
		
		public PanelList(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.list.SelectedIndexChanged -= new EventHandler(this.ListChanged);
			}
			
			base.Dispose(disposing);
		}

		// Retourne la hauteur standard.
		public override double DefaultHeight
		{
			get
			{
				return 58;
			}
		}

		
		// Propriété -> widget.
		public override void SetProperty(AbstractProperty property)
		{
			base.SetProperty(property);
			this.label.Text = this.textStyle;

			PropertyList p = property as PropertyList;
			if ( p == null )  return;

			for ( int i=0 ; i<p.Count ; i++ )
			{
				this.list.Items.Add(p.GetName(i));
			}
			this.list.SelectedIndex = p.Choice;
			this.list.ShowSelected(ScrollShowMode.Center);
		}

		// Widget -> propriété.
		public override AbstractProperty GetProperty()
		{
			PropertyList p = new PropertyList();
			base.GetProperty(p);

			p.Choice = this.list.SelectedIndex;
			p.Clear();
			for ( int i=0 ; i<this.list.Items.Count ; i++ )
			{
				p.Add(this.list.Items[i]);
			}

			return p;
		}


		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.list == null )  return;

			Drawing.Rectangle rect = this.Client.Bounds;
			rect.Deflate(this.extendedZoneWidth, 0);
			rect.Deflate(5);

			Drawing.Rectangle r = rect;
			r.Bottom = r.Top-20;
			r.Right = rect.Right-100;
			this.label.Bounds = r;

			r = rect;
			r.Left = r.Right-100;
			this.list.Bounds = r;
			this.list.ShowSelected(ScrollShowMode.Center);
		}
		
		// Une valeur a été changée.
		private void ListChanged(object sender)
		{
			this.OnChanged();
		}


		protected StaticText				label;
		protected ScrollList				list;
	}
}
