using Epsitec.Common.Widgets;
using Epsitec.Common.Pictogram.Data;

namespace Epsitec.Common.Pictogram.Widgets
{
	/// <summary>
	/// La classe AbstractPanel est la classe de base pour tous les panels.
	/// </summary>
	public class AbstractPanel : Epsitec.Common.Widgets.Widget
	{
		public AbstractPanel()
		{
			this.extendedButton = new ArrowButton(this);
			this.extendedButton.ButtonStyle = ButtonStyle.Icon;
			this.extendedButton.Direction = Direction.Down;
			this.extendedButton.Clicked += new MessageEventHandler(this.ExtendedButtonClicked);

			this.colorBlack = Drawing.Color.FromName("WindowFrame");
		}
		
		public AbstractPanel(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.extendedButton.Clicked -= new MessageEventHandler(this.ExtendedButtonClicked);
			}
			
			base.Dispose(disposing);
		}

		
		// Retourne la hauteur standard.
		public override double DefaultHeight
		{
			get
			{
				return 30;
			}
		}

		// Indique si ce panneau poss�de 2 hauteurs diff�rentes.
		public virtual bool IsNormalAndExtended()
		{
			return this.isNormalAndExtended;
		}

		// Indique si le panneau repr�sente des propri�t�s contradictoires.
		public bool Multi
		{
			get
			{
				return this.multi;
			}

			set
			{
				if ( this.multi != value )
				{
					this.multi = value;
					this.Invalidate();
				}
			}
		}


		// Propri�t� -> widget.
		public virtual void SetProperty(AbstractProperty property)
		{
			this.type                = property.Type;
			this.text                = property.Text;
			this.backgroundIntensity = property.BackgroundIntensity;
			this.extendedSize        = property.ExtendedSize;

			this.extendedButton.Direction = this.extendedSize ? Direction.Up : Direction.Down;
		}

		// Widget -> propri�t�.
		public virtual AbstractProperty GetProperty()
		{
			return null;
		}

		protected void GetProperty(AbstractProperty property)
		{
			property.Type                = this.type;
			property.Text                = this.text;
			property.BackgroundIntensity = this.backgroundIntensity;
			property.ExtendedSize        = this.extendedSize;
		}

		// Retourne le type de la propri�t� �dit�e par le panneau.
		public virtual PropertyType PropertyType
		{
			get
			{
				return this.type;
			}
		}


		// Met � jour la g�om�trie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.extendedButton == null )  return;

			Drawing.Rectangle rTop = this.Client.Bounds;
			rTop.Left += 1;
			rTop.Width = this.extendedZoneWidth-2;
			rTop.Top -= 8;
			rTop.Bottom = rTop.Top-13;
			this.extendedButton.Bounds = rTop;
			this.extendedButton.SetVisible(this.isNormalAndExtended);
		}


		// D�s�lectionne toutes les origines de couleurs possibles.
		public virtual void OriginColorDeselect()
		{
		}

		// S�lectionne l'origine de couleur.
		public virtual void OriginColorSelect(int rank)
		{
		}

		// Retourne le rang de la couleur d'origine.
		public virtual int OriginColorRank()
		{
			return -1;
		}

		// Modifie la couleur d'origine.
		public virtual void OriginColorChange(Drawing.Color color)
		{
		}

		// Donne la couleur d'origine.
		public virtual Drawing.Color OriginColorGet()
		{
			return Drawing.Color.FromBrightness(0);
		}


		// G�n�re un �v�nement pour dire que �a a chang�.
		protected virtual void OnChanged()
		{
			if ( this.Changed != null )  // qq'un �coute ?
			{
				this.Changed(this);
			}
		}

		public event EventHandler Changed;


		// G�n�re un �v�nement pour dire que la couleur d'origine a chang�.
		protected virtual void OnOriginColorChanged()
		{
			if ( this.OriginColorChanged != null )  // qq'un �coute ?
			{
				this.OriginColorChanged(this);
			}
		}

		public event EventHandler OriginColorChanged;


		// Le bouton pour �tendre/r�duire le panneau a �t� cliqu�.
		private void ExtendedButtonClicked(object sender, MessageEventArgs e)
		{
			this.extendedSize = !this.extendedSize;
			this.OnExtendedChanged();
		}

		// G�n�re un �v�nement pour dire que la hauteur a chang�.
		protected virtual void OnExtendedChanged()
		{
			if ( this.ExtendedChanged != null )  // qq'un �coute ?
			{
				this.ExtendedChanged(this);
			}
		}

		public event EventHandler ExtendedChanged;


		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Epsitec.Common.Widgets.Adorner.Factory.Active;

			Drawing.Rectangle rect  = new Drawing.Rectangle(0, 0, this.Client.Width, this.Client.Height);
			
			graphics.AddFilledRectangle(rect);
#if false
			Drawing.Color color = adorner.ColorWindow;
			color = Drawing.Color.FromRGB(color.R*this.backgroundIntensity, color.G*this.backgroundIntensity, color.B*this.backgroundIntensity);
#else
			Drawing.Color color = Drawing.Color.FromARGB(1.0-this.backgroundIntensity, 0.5,0.5,0.5);
#endif
			graphics.RenderSolid(color);

			if ( this.multi )
			{
				Drawing.Rectangle part = rect;
				part.Width = this.extendedZoneWidth;
				graphics.AddFilledRectangle(part);
				graphics.RenderSolid(Drawing.Color.FromRGB(1,0,0));
			}

			rect.Inflate(-0.5, -0.5);

			graphics.AddLine(rect.Left+this.extendedZoneWidth, rect.Bottom-0.5, rect.Left+this.extendedZoneWidth, rect.Top+0.5);
			graphics.RenderSolid(adorner.ColorBorder);

			graphics.AddLine(rect.Left-0.5, rect.Bottom, rect.Right+0.5, rect.Bottom);
			graphics.RenderSolid(adorner.ColorBorder);
		}


		protected Drawing.Color				colorBlack;
		protected double					backgroundIntensity;
		protected bool						extendedSize;
		protected PropertyType				type;
		protected string					text;
		protected bool						isNormalAndExtended = false;
		protected double					extendedZoneWidth = 15;
		protected ArrowButton				extendedButton;
		protected bool						multi = false;
	}
}
