using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Pictogram.Data;

namespace Epsitec.Common.Pictogram.Widgets
{
	/// <summary>
	/// La classe PanelList permet de choisir un rang dans une liste.
	/// </summary>
	public class PanelList : AbstractPanel
	{
		public PanelList()
		{
			this.label = new StaticText(this);
			this.label.Alignment = Drawing.ContentAlignment.MiddleLeft;

			this.list = new ScrollList(this);
			this.list.SelectedIndexChanged += new EventHandler(this.ListChanged);
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

		
		// Propri�t� -> widget.
		public override void SetProperty(AbstractProperty property)
		{
			base.SetProperty(property);
			this.label.Text = this.text;

			PropertyList p = property as PropertyList;
			if ( p == null )  return;

			for ( int i=0 ; i<p.Count ; i++ )
			{
				this.list.Items.Add(p.Get(i));
			}
			this.list.SelectedIndex = p.Choice;
			this.list.ShowSelectedLine(ScrollListShow.Middle);
		}

		// Widget -> propri�t�.
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


		// Met � jour la g�om�trie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.list == null )  return;

			Drawing.Rectangle rect = this.Client.Bounds;
			rect.Left += this.extendedZoneWidth;
			rect.Inflate(-5, -5);

			Drawing.Rectangle r = rect;
			r.Bottom = r.Top-20;
			r.Right = rect.Right-100;
			this.label.Bounds = r;

			r = rect;
			r.Left = r.Right-100;
			this.list.Bounds = r;
			this.list.ShowSelectedLine(ScrollListShow.Middle);
		}
		
		// Une valeur a �t� chang�e.
		private void ListChanged(object sender)
		{
			this.OnChanged();
		}


		protected StaticText				label;
		protected ScrollList				list;
	}
}
