using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Settings
{
	/// <summary>
	/// La classe Point contient un réglage numérique.
	/// </summary>
	public class Point : Abstract
	{
		public Point(Document document) : base(document)
		{
		}

		// Texte explicatif.
		public string TextX
		{
			get
			{
				return this.textX;
			}

			set
			{
				this.textX = value;
			}
		}

		// Texte explicatif.
		public string TextY
		{
			get
			{
				return this.textY;
			}

			set
			{
				this.textY = value;
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

					case "GridOffset":
						this.document.Modifier.ActiveViewer.DrawingContext.GridOffset = value;
						break;

					case "DuplicateMove":
						this.document.Modifier.DuplicateMove = value;
						break;
				}
			}
		}

		public double MinValue
		{
			get
			{
				return this.minValue;
			}

			set
			{
				this.minValue = value;
			}
		}

		public double MaxValue
		{
			get
			{
				return this.maxValue;
			}

			set
			{
				this.maxValue = value;
			}
		}

		public double Step
		{
			get
			{
				return this.step;
			}

			set
			{
				this.step = value;
			}
		}

		public double Resolution
		{
			get
			{
				return this.resolution;
			}

			set
			{
				this.resolution = value;
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


		// Indique quel est le widget qui édite ce réglage.
		public void TextField(int rank, TextFieldSlider widget)
		{
			if ( this.textField == null )
			{
				this.textField = new TextFieldSlider[2];
			}
			this.textField[rank] = widget;
		}

		// Met à jour la valeur du réglage.
		public override void UpdateValue()
		{
			this.textField[0].Value = (decimal) this.Value.X;
			this.textField[1].Value = (decimal) this.Value.Y;
		}

		
		#region Serialization
		// Sérialise le réglage.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			info.AddValue("Value", this.Value);
		}

		// Constructeur qui désérialise le réglage.
		protected Point(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.document = Document.ReadDocument;
			this.Value = (Drawing.Point) info.GetValue("Value", typeof(Drawing.Point));
		}
		#endregion


		protected string			textX;
		protected string			textY;
		protected double			minValue;
		protected double			maxValue;
		protected double			step;
		protected double			resolution;
		protected bool				link = true;
		protected TextFieldSlider[]	textField;
	}
}
