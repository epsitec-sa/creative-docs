using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Panels
{
	/// <summary>
	/// La classe LayerName permet de choisir le nom d'un calque.
	/// </summary>
	[SuppressBundleSupport]
	public class LayerName : Abstract
	{
		public LayerName(Document document) : base(document)
		{
			this.label = new StaticText(this);
			this.label.Alignment = ContentAlignment.MiddleLeft;
			this.label.Text = Res.Strings.Panel.LayerName.Label.Name;

			this.field = new TextField(this);
			this.field.TextChanged += new EventHandler(this.HandleTextChanged);
			this.field.TabIndex = 1;
			this.field.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.field, Res.Strings.Panel.LayerName.Tooltip.Name);
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.field.TextChanged -= new EventHandler(this.HandleTextChanged);
				this.field = null;
				this.label = null;
			}
			
			base.Dispose(disposing);
		}


		// Propri�t� -> widget.
		protected override void PropertyToWidgets()
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			int sel = context.CurrentLayer;

			string text = this.document.Modifier.LayerName(sel);
			if ( this.field.Text != text )
			{
				this.ignoreChanged = true;
				this.field.Text = text;
				this.ignoreChanged = false;
			}
		}

		// Widgets -> propri�t�.
		protected override void WidgetsToProperty()
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			int sel = context.CurrentLayer;

			if ( this.document.Modifier.LayerName(sel) != this.field.Text )
			{
				this.document.Modifier.LayerName(sel, this.field.Text);
			}
		}


		// Met � jour la g�om�trie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.field == null )  return;

			Rectangle rect = this.Client.Bounds;
			rect.Deflate(this.extendedZoneWidth, 0);
			rect.Deflate(5);

			Rectangle r = rect;
			r.Bottom = r.Top-20;
			r.Right = rect.Right-110;
			this.label.Bounds = r;

			r = rect;
			r.Left = r.Right-110;
			this.field.Bounds = r;
		}
		
		// Une valeur a �t� chang�e.
		private void HandleTextChanged(object sender)
		{
			if ( this.ignoreChanged )  return;

			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			int sel = context.CurrentLayer;
			this.document.Modifier.LayerName(sel, this.field.Text);
		}

		// Met le focus par d�faut.
		public void SetDefaultFocus()
		{
			this.field.SelectAll();
			this.field.Focus();
		}


		protected StaticText				label;
		protected TextField					field;
	}
}
