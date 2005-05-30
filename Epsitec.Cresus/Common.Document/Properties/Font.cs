using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Properties
{
	/// <summary>
	/// La classe Font repr�sente une propri�t� d'un objet graphique.
	/// </summary>
	[System.Serializable()]
	public class Font : Abstract
	{
		public Font(Document document, Type type) : base(document, type)
		{
		}

		protected override void Initialise()
		{
			this.fontName  = "Arial";
			if ( this.document.Type == DocumentType.Pictogram )
			{
				this.fontSize  = 2.0;
			}
			else
			{
				this.fontSize  = 12.0*Modifier.fontSizeScale;  // corps 12
			}
			this.fontColor = Drawing.Color.FromBrightness(0);
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

		public Drawing.Color FontColor
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

		// D�termine le nom de la propri�t� dans la liste (Lister).
		public string GetListName()
		{
			return this.fontName;
		}

		// Indique si une impression complexe est n�cessaire.
		public override bool IsComplexPrinting
		{
			get
			{
				if ( this.fontColor.A > 0.0 && this.fontColor.A < 1.0 )  return true;
				return false;
			}
		}

		// Indique si un changement de cette propri�t� modifie la bbox de l'objet.
		public override bool AlterBoundingBox
		{
			get { return true; }
		}

		// Effectue une copie de la propri�t�.
		public override void CopyTo(Abstract property)
		{
			base.CopyTo(property);
			Font p = property as Font;
			p.fontName  = this.fontName;
			p.fontSize  = this.fontSize;
			p.fontColor = this.fontColor;
		}

		// Compare deux propri�t�s.
		public override bool Compare(Abstract property)
		{
			if ( !base.Compare(property) )  return false;

			Font p = property as Font;
			if ( p.fontName  != this.fontName  )  return false;
			if ( p.fontSize  != this.fontSize  )  return false;
			if ( p.fontColor != this.fontColor )  return false;

			return true;
		}

		// Cr�e le panneau permettant d'�diter la propri�t�.
		public override Panels.Abstract CreatePanel(Document document)
		{
			return new Panels.Font(document);
		}


		// Retourne la fonte � utiliser.
		public Drawing.Font GetFont()
		{
			return Misc.GetFont(this.fontName);
		}


		// D�but du d�placement global de la propri�t�.
		public override void MoveGlobalStarting()
		{
			if ( !this.document.Modifier.ActiveViewer.SelectorAdaptText )  return;

			this.InsertOpletProperty();

			this.initialFontSize = this.fontSize;
		}
		
		// Effectue le d�placement global de la propri�t�.
		public override void MoveGlobalProcess(Selector selector)
		{
			if ( !this.document.Modifier.ActiveViewer.SelectorAdaptText )  return;

			double zoom = selector.GetTransformZoom;
			this.fontSize = this.initialFontSize*zoom;

			this.document.Notifier.NotifyPropertyChanged(this);
		}

		
		#region Serialization
		// S�rialise la propri�t�.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			info.AddValue("FontName", this.fontName);
			info.AddValue("FontSize", this.fontSize);
			info.AddValue("FontColor", this.fontColor);
		}

		// Constructeur qui d�s�rialise la propri�t�.
		protected Font(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.fontName = info.GetString("FontName");
			this.fontSize = info.GetDouble("FontSize");
			this.fontColor = (Drawing.Color) info.GetValue("FontColor", typeof(Drawing.Color));
		}
		#endregion

	
		protected string				fontName;
		protected double				fontSize;
		protected Drawing.Color			fontColor;
		protected double				initialFontSize;
	}
}
