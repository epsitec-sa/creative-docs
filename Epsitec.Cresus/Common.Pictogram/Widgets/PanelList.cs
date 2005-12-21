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
		public PanelList(Drawer drawer) : base(drawer)
		{
			this.label = new StaticText(this);
			this.label.Alignment = Drawing.ContentAlignment.MiddleLeft;

			this.list = new ScrollList(this);
			this.list.SelectedIndexChanged += new EventHandler(this.ListChanged);
			this.list.TabIndex = 1;
			this.list.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.list.SelectedIndexChanged -= new EventHandler(this.ListChanged);
				this.list = null;
				this.label = null;
			}
			
			base.Dispose(disposing);
		}

		public override double DefaultHeight
		{
			//	Retourne la hauteur standard.
			get
			{
				return 58;
			}
		}

		
		public override void SetProperty(AbstractProperty property)
		{
			//	Propriété -> widget.
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

		public override AbstractProperty GetProperty()
		{
			//	Widget -> propriété.
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


		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
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
		
		private void ListChanged(object sender)
		{
			//	Une valeur a été changée.
			this.OnChanged();
		}


		protected StaticText				label;
		protected ScrollList				list;
	}
}
