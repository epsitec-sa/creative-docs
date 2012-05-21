using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Properties
{
	/// <summary>
	/// La classe Font représente une propriété d'un objet graphique.
	/// </summary>
	[System.Serializable()]
	public class Font : Abstract
	{
		public Font(Document document, Type type) : base(document, type)
		{
		}

		protected override void Initialize()
		{
			base.Initialize ();
			this.fontName = "Arial";

			if ( this.document.Type == DocumentType.Pictogram )
			{
				this.fontSize = 2.0;
			}
			else
			{
				this.fontSize = 12.0*Modifier.FontSizeScale;  // corps 12
			}

			this.fontColor = Drawing.RichColor.FromCmyk(0.0, 0.0, 0.0, 1.0);
		}

		public string FontName
		{
			get
			{
				return this.fontName;
			}

			set
			{
				if ( this.fontName != value )
				{
					this.NotifyBefore();
					this.fontName = value;
					this.NotifyAfter();
				}
			}
		}

		public double FontSize
		{
			get
			{
				return this.fontSize;
			}

			set
			{
				if ( this.fontSize != value )
				{
					this.NotifyBefore();
					this.fontSize = value;
					this.NotifyAfter();
				}
			}
		}

		public Drawing.RichColor FontColor
		{
			get
			{
				return this.fontColor;
			}

			set
			{
				if ( this.fontColor != value )
				{
					this.NotifyBefore();
					this.fontColor = value;
					this.NotifyAfter();
				}
			}
		}

		public override bool IsComplexPrinting
		{
			//	Indique si une impression complexe est nécessaire.
			get
			{
				if ( this.fontColor.A > 0.0 && this.fontColor.A < 1.0 )  return true;
				return false;
			}
		}

		public override void PutStyleBrief(System.Text.StringBuilder builder)
		{
			//	Construit le texte résumé d'un style pour une propriété.
			this.PutStyleBriefPrefix(builder);

			builder.Append(this.fontName);
			builder.Append(", ");
			builder.Append((this.fontSize/Modifier.FontSizeScale).ToString("F1"));
			builder.Append(", ");
			builder.Append(Misc.GetColorNiceName(this.fontColor));
			
			this.PutStyleBriefPostfix(builder);
		}

		public override bool AlterBoundingBox
		{
			//	Indique si un changement de cette propriété modifie la bbox de l'objet.
			get { return true; }
		}

		public override void CopyTo(Abstract property)
		{
			//	Effectue une copie de la propriété.
			base.CopyTo(property);
			Font p = property as Font;
			p.fontName  = this.fontName;
			p.fontSize  = this.fontSize;
			p.fontColor = this.fontColor;
		}

		public override bool Compare(Abstract property)
		{
			//	Compare deux propriétés.
			if ( !base.Compare(property) )  return false;

			Font p = property as Font;
			if ( p.fontName  != this.fontName  )  return false;
			if ( p.fontSize  != this.fontSize  )  return false;
			if ( p.fontColor != this.fontColor )  return false;

			return true;
		}

		public override Panels.Abstract CreatePanel(Document document)
		{
			//	Crée le panneau permettant d'éditer la propriété.
			Panels.Abstract.StaticDocument = document;
			return new Panels.Font(document);
		}


		public Drawing.Font GetFont()
		{
			//	Retourne la fonte à utiliser.
			return Misc.GetFont(this.fontName);
		}


		public override void MoveGlobalStarting()
		{
			//	Début du déplacement global de la propriété.
			if ( !this.document.Modifier.ActiveViewer.SelectorAdaptText )  return;

			this.InsertOpletProperty();

			this.initialFontSize = this.fontSize;
		}
		
		public override void MoveGlobalProcess(Selector selector)
		{
			//	Effectue le déplacement global de la propriété.
			if ( !this.document.Modifier.ActiveViewer.SelectorAdaptText )  return;

			double scale = selector.GetTransformScale;
			this.fontSize = this.initialFontSize*scale;

			this.document.Notifier.NotifyPropertyChanged(this);
		}

		
		public override PDF.Type TypeComplexSurfacePDF(IPaintPort port)
		{
			//	Donne le type PDF de la surface complexe.
			Drawing.Color c = port.GetFinalColor(this.fontColor.Basic);

			if ( c.A == 0.0 )
			{
				return PDF.Type.None;
			}

			if ( c.A < 1.0 )
			{
				return PDF.Type.TransparencyRegular;
			}

			return PDF.Type.OpaqueRegular;
		}


		public override bool ChangeColorSpace(ColorSpace cs)
		{
			//	Modifie l'espace des couleurs.
			this.NotifyBefore();
			this.fontColor.ColorSpace = cs;
			this.NotifyAfter();
			this.document.Notifier.NotifyPropertyChanged(this);
			return true;
		}

		public override bool ChangeColor(double adjust, bool stroke)
		{
			//	Modifie les couleurs.
			if ( stroke )  return false;

			this.NotifyBefore();
			this.fontColor.ChangeBrightness(adjust);
			this.NotifyAfter();
			this.document.Notifier.NotifyPropertyChanged(this);
			return true;
		}

		
		#region Serialization
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	Sérialise la propriété.
			base.GetObjectData(info, context);

			info.AddValue("FontName", this.fontName);
			info.AddValue("FontSize", this.fontSize);
			info.AddValue("FontColor", this.fontColor);
		}

		protected Font(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			//	Constructeur qui désérialise la propriété.
			this.fontName = info.GetString("FontName");
			this.fontSize = info.GetDouble("FontSize");

			if ( this.document.IsRevisionGreaterOrEqual(1,0,22) )
			{
				this.fontColor = (Drawing.RichColor) info.GetValue("FontColor", typeof(Drawing.RichColor));
			}
			else
			{
				Drawing.Color c = (Drawing.Color) info.GetValue("FontColor", typeof(Drawing.Color));
				this.fontColor = new RichColor(c);
			}
		}
		#endregion

	
		protected string				fontName;
		protected double				fontSize;
		protected Drawing.RichColor		fontColor;
		protected double				initialFontSize;
	}
}
