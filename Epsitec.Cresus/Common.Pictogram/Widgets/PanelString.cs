using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Pictogram.Data;

namespace Epsitec.Common.Pictogram.Widgets
{
	/// <summary>
	/// La classe PanelString permet de choisir une chaîne de caractères.
	/// </summary>
	
	[SuppressBundleSupport]
	
	public class PanelString : AbstractPanel
	{
		public PanelString(Drawer drawer) : base(drawer)
		{
			this.label = new StaticText(this);
			this.label.Alignment = Drawing.ContentAlignment.MiddleLeft;

			this.fieldSingle = new TextField(this);
			this.fieldSingle.TextChanged += new EventHandler(this.HandleTextChanged);
			this.fieldSingle.TabIndex = 1;
			this.fieldSingle.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.fieldMulti = new TextFieldMulti(this);
			this.fieldMulti.TextChanged += new EventHandler(this.HandleTextChanged);
			this.fieldMulti.TabIndex = 2;
			this.fieldMulti.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.isNormalAndExtended = true;
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.fieldSingle.TextChanged -= new EventHandler(this.HandleTextChanged);
				this.fieldMulti.TextChanged -= new EventHandler(this.HandleTextChanged);

				this.label = null;
				this.fieldSingle = null;
				this.fieldMulti = null;
			}
			
			base.Dispose(disposing);
		}


		public override double DefaultHeight
		{
			//	Retourne la hauteur standard.
			get
			{
				return ( this.extendedSize ? 80 : 30 );
			}
		}

		public override bool ExtendedSize
		{
			//	Indique si le panneau est réduit (petite hauteur) ou étendu (grande hauteur).
			get
			{
				return this.extendedSize;
			}

			set
			{
				if ( this.extendedSize != value )
				{
					if ( value )
					{
						this.fieldMulti.Text = this.fieldSingle.Text;
					}
					else
					{
						this.fieldSingle.Text = this.fieldMulti.Text;
					}
				}

				base.ExtendedSize = value;

				this.fieldSingle.SetVisible(!this.extendedSize);
				this.fieldMulti.SetVisible(this.extendedSize);
			}
		}

		public override void SetProperty(AbstractProperty property)
		{
			//	Propriété -> widget.
			base.SetProperty(property);
			this.label.Text = this.textStyle;

			PropertyString p = property as PropertyString;
			if ( p == null )  return;

			if ( this.extendedSize )
			{
				this.fieldMulti.Text = p.String;
			}
			else
			{
				this.fieldSingle.Text = p.String;
			}
		}

		public override AbstractProperty GetProperty()
		{
			//	Widget -> propriété.
			PropertyString p = new PropertyString();
			base.GetProperty(p);

			if ( this.extendedSize )
			{
				p.String = this.fieldMulti.Text;
			}
			else
			{
				p.String = this.fieldSingle.Text;
			}
			return p;
		}


		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.fieldSingle == null )  return;

			Drawing.Rectangle rect = this.Client.Bounds;
			rect.Deflate(this.extendedZoneWidth, 0);
			rect.Deflate(5);

			Drawing.Rectangle r = rect;
			r.Bottom = r.Top-20;
			r.Right = rect.Right-110;
			this.label.Bounds = r;

			r = rect;
			r.Left = r.Right-110;
			this.fieldSingle.Bounds = r;
			this.fieldMulti.Bounds = r;

			this.fieldSingle.SetVisible(!this.extendedSize);
			this.fieldMulti.SetVisible(this.extendedSize);
		}
		
		private void HandleTextChanged(object sender)
		{
			//	Une valeur a été changée.
			this.OnChanged();
		}


		protected StaticText				label;
		protected TextField					fieldSingle;
		protected TextFieldMulti			fieldMulti;
	}
}
