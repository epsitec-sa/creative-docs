using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Pictogram.Data;

namespace Epsitec.Common.Pictogram.Widgets
{
	/// <summary>
	/// La classe PanelCombo permet de choisir un rang dans un TextFieldCombo.
	/// </summary>
	public class PanelCombo : AbstractPanel
	{
		public PanelCombo()
		{
			this.label = new StaticText(this);
			this.label.Alignment = Drawing.ContentAlignment.MiddleLeft;

			this.list = new TextFieldCombo(this);
			this.list.IsReadOnly = true;
			this.list.TextChanged += new EventHandler(this.ListChanged);
		}
		
		public PanelCombo(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.list.TextChanged -= new EventHandler(this.ListChanged);
			}
			
			base.Dispose(disposing);
		}


		// Propriété -> widget.
		public override void SetProperty(AbstractProperty property)
		{
			base.SetProperty(property);
			this.label.Text = this.text;

			PropertyCombo p = property as PropertyCombo;
			if ( p == null )  return;

			for ( int i=0 ; i<p.Count ; i++ )
			{
				this.list.Items.Add(p.Get(i));
			}
			this.list.SelectedIndex = p.Choice;
		}

		// Widget -> propriété.
		public override AbstractProperty GetProperty()
		{
			PropertyCombo p = new PropertyCombo();
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
			rect.Left += this.extendedZoneWidth;
			rect.Inflate(-5, -5);

			Drawing.Rectangle r = rect;
			r.Bottom = r.Top-20;
			r.Right = rect.Right-100;
			this.label.Bounds = r;

			r = rect;
			r.Left = r.Right-100;
			this.list.Bounds = r;
		}
		
		// Une valeur a été changée.
		private void ListChanged(object sender)
		{
			this.OnChanged();
		}


		protected StaticText				label;
		protected TextFieldCombo			list;
	}
}
