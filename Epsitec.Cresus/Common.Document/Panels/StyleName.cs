using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Panels
{
	/// <summary>
	/// La classe StyleName permet de choisir le nom du style.
	/// </summary>
	[SuppressBundleSupport]
	public class StyleName : Abstract
	{
		public StyleName(Document document) : base(document)
		{
			this.label = new StaticText(this);
			this.label.Alignment = ContentAlignment.MiddleLeft;
			this.label.Text = Res.Strings.Panel.StyleName.Label.Name;

			this.field = new TextField(this);
			this.field.TextChanged += new EventHandler(this.HandleTextChanged);
			this.field.TabIndex = 1;
			this.field.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.field, Res.Strings.Panel.StyleName.Tooltip.Name);
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


		// Propriété -> widget.
		protected override void PropertyToWidgets()
		{
			base.PropertyToWidgets();

			this.ignoreChanged = true;
			this.field.Text = this.property.StyleName;
			this.ignoreChanged = false;
		}

		// Widgets -> propriété.
		protected override void WidgetsToProperty()
		{
			if ( this.property.StyleName != this.field.Text )
			{
				this.property.StyleName = this.field.Text;
				this.document.Notifier.NotifyPropertyChanged(this.property);
			}
		}


		// Met à jour la géométrie.
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
		
		// Une valeur a été changée.
		private void HandleTextChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			this.OnChanged();
		}

		// Met le focus par défaut.
		public void SetDefaultFocus()
		{
			this.field.SelectAll();
			this.field.Focus();
		}


		protected StaticText				label;
		protected TextField					field;
	}
}
