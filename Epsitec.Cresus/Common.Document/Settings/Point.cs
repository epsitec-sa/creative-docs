using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Settings
{
	/// <summary>
	/// La classe Point contient un réglage numérique.
	/// </summary>
	[System.Serializable()]
	public class Point : Abstract
	{
		public Point(Document document, string name) : base(document, name)
		{
			this.Initialise();
		}

		protected void Initialise()
		{
			switch ( this.name )
			{
				case "PageSize":
					this.textX = "Largeur";
					this.textY = "Hauteur";
					this.factorMinValue = 0.01;  // 10mm
					this.factorMaxValue = 1.0;
					if ( this.document.Type != DocumentType.Pictogram )
					{
						this.link = false;
					}
					break;

				case "GridStep":
					this.textX = "Pas horizontal";
					this.textY = "Pas vertical";
					this.factorMinValue = 0.001;  // 1mm
					this.factorMaxValue = 0.1;  // 100mm
					break;

				case "GridSubdiv":
					this.textX = "Subdivisions horizontales";
					this.textY = "Subdivisions verticales";
					this.integer = true;
					this.factorMinValue = 1.0;
					this.factorMaxValue = 10.0;
					break;

				case "GridOffset":
					this.textX = "Décalage horizontal";
					this.textY = "Décalage vertical";
					this.factorMinValue = -0.1;
					this.factorMaxValue = 0.1;
					this.factorStep = 0.5;
					break;

				case "DuplicateMove":
					this.textX = "Déplacement à droite";
					this.textY = "Déplacement en haut";
					this.link = false;
					break;
			}
		}

		// Texte explicatif.
		public string TextX
		{
			get
			{
				return this.textX;
			}
		}

		// Texte explicatif.
		public string TextY
		{
			get
			{
				return this.textY;
			}
		}

		public Drawing.Point Value
		{
			get
			{
				switch ( this.name )
				{
					case "PageSize":
						return new Drawing.Point(this.document.Size.Width, this.document.Size.Height);

					case "GridStep":
						return this.document.Modifier.ActiveViewer.DrawingContext.GridStep;

					case "GridSubdiv":
						return this.document.Modifier.ActiveViewer.DrawingContext.GridSubdiv;

					case "GridOffset":
						return this.document.Modifier.ActiveViewer.DrawingContext.GridOffset;

					case "DuplicateMove":
						return this.document.Modifier.DuplicateMove;
				}

				return new Drawing.Point(0,0);
			}

			set
			{
				switch ( this.name )
				{
					case "PageSize":
						this.document.Size = new Size(value.X, value.Y);
						break;

					case "GridStep":
						this.document.Modifier.ActiveViewer.DrawingContext.GridStep = value;
						break;

					case "GridSubdiv":
						this.document.Modifier.ActiveViewer.DrawingContext.GridSubdiv = value;
						break;

					case "GridOffset":
						this.document.Modifier.ActiveViewer.DrawingContext.GridOffset = value;
						break;

					case "DuplicateMove":
						this.document.Modifier.DuplicateMove = value;
						break;
				}
			}
		}

		public double FactorMinValue
		{
			get
			{
				return this.factorMinValue;
			}
		}

		public double FactorMaxValue
		{
			get
			{
				return this.factorMaxValue;
			}
		}

		public double FactorStep
		{
			get
			{
				return this.factorStep;
			}
		}

		public bool Link
		{
			get
			{
				return this.link;
			}

			set
			{
				this.link = value;
			}
		}

		public bool Integer
		{
			get
			{
				return this.integer;
			}
		}


		#region Serialization
		// Sérialise le réglage.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("Link", this.Link);
			info.AddValue("Value", this.Value);
		}

		// Constructeur qui désérialise le réglage.
		protected Point(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.link = false;
			this.Value = (Drawing.Point) info.GetValue("Value", typeof(Drawing.Point));
			this.Initialise();
			this.link = info.GetBoolean("Link");
		}
		#endregion


		protected string			textX;
		protected string			textY;
		protected double			factorMinValue = -1.0;
		protected double			factorMaxValue = 1.0;
		protected double			factorStep = 1.0;
		protected bool				link = true;
		protected bool				integer = false;
	}
}
