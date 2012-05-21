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
			this.Initialize();
		}

		protected void Initialize()
		{
			this.conditionName = "";
			this.conditionState = false;

			switch ( this.name )
			{
				case "PageSize":
					this.textX = Res.Strings.Dialog.Point.PageSizeX;
					this.textY = Res.Strings.Dialog.Point.PageSizeY;
					this.factorMinValue = 0.01;  // 10mm
					this.factorMaxValue = 1.0;
					if ( this.document.Type != DocumentType.Pictogram )
					{
						this.link = false;
					}
					break;

				case "GridStep":
					this.textX = Res.Strings.Dialog.Point.GridStepX;
					this.textY = Res.Strings.Dialog.Point.GridStepY;
					this.factorMinValue = 0.0001;  // 0.1mm
					this.factorMaxValue = 0.1;  // 100mm
					this.doubler = true;
					break;

				case "GridSubdiv":
					this.textX = Res.Strings.Dialog.Point.GridSubdivX;
					this.textY = Res.Strings.Dialog.Point.GridSubdivY;
					this.integer = true;
					this.factorMinValue = 1.0;
					this.factorMaxValue = 10.0;
					break;

				case "GridOffset":
					this.textX = Res.Strings.Dialog.Point.GridOffsetX;
					this.textY = Res.Strings.Dialog.Point.GridOffsetY;
					this.factorMinValue = -0.1;
					this.factorMaxValue = 0.1;
					this.factorStep = 0.5;
					break;

				case "DuplicateMove":
					this.textX = Res.Strings.Dialog.Point.DuplicateMoveX;
					this.textY = Res.Strings.Dialog.Point.DuplicateMoveY;
					this.link = false;
					this.doubler = true;
					break;

				case "ArrowMove":
					this.textX = Res.Strings.Dialog.Point.ArrowMoveX;
					this.textY = Res.Strings.Dialog.Point.ArrowMoveY;
					this.link = true;
					this.factorMinValue = 0.0;
					this.doubler = true;
					break;
			}
		}

		public string TextX
		{
			//	Texte explicatif.
			get
			{
				return this.textX;
			}
		}

		public string TextY
		{
			//	Texte explicatif.
			get
			{
				return this.textY;
			}
		}

		public double ValueX
		{
			get
			{
				return this.Value.X;
			}

			set
			{
				Drawing.Point p = this.Value;

				if (this.ProportionalLink)
				{
					double prop = p.Y/p.X;
					p.X = value;
					p.Y = value*prop;
				}
				else
				{
					p.X = value;
					if (this.Link)
					{
						p.Y = p.X;
					}
				}

				this.Value = p;
			}
		}

		public double ValueY
		{
			get
			{
				return this.Value.Y;
			}

			set
			{
				Drawing.Point p = this.Value;

				if (this.ProportionalLink)
				{
					double prop = p.X/p.Y;
					p.Y = value;
					p.X = value*prop;
				}
				else
				{
					p.Y = value;
					if (this.Link)
					{
						p.X = p.Y;
					}
				}

				this.Value = p;
			}
		}

		public Drawing.Point Value
		{
			get
			{
				switch ( this.name )
				{
					case "PageSize":
						return new Drawing.Point(this.document.DocumentSize.Width, this.document.DocumentSize.Height);

					case "GridStep":
						return this.document.Modifier.ActiveViewer.DrawingContext.GridStep;

					case "GridSubdiv":
						return this.document.Modifier.ActiveViewer.DrawingContext.GridSubdiv;

					case "GridOffset":
						return this.document.Modifier.ActiveViewer.DrawingContext.GridOffset;

					case "DuplicateMove":
						return this.document.Modifier.DuplicateMove;

					case "ArrowMove":
						return this.document.Modifier.ArrowMove;
				}

				return new Drawing.Point(0,0);
			}

			set
			{
				switch ( this.name )
				{
					case "PageSize":
						this.document.DocumentSize = new Size(value.X, value.Y);
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

					case "ArrowMove":
						this.document.Modifier.ArrowMove = value;
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

		public bool ProportionalLink
		{
			get
			{
				return (this.Link && this.name == "PageSize");
			}
		}

		public bool Doubler
		{
			get
			{
				return this.doubler;
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
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	Sérialise le réglage.
			base.GetObjectData(info, context);
			info.AddValue("Link", this.Link);
			info.AddValue("Value", this.Value);
		}

		protected Point(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			//	Constructeur qui désérialise le réglage.
			this.link = false;
			this.Value = (Drawing.Point) info.GetValue("Value", typeof(Drawing.Point));
			this.Initialize();
			this.link = info.GetBoolean("Link");
		}
		#endregion


		protected string			textX;
		protected string			textY;
		protected double			factorMinValue = -1.0;
		protected double			factorMaxValue = 1.0;
		protected double			factorStep = 1.0;
		protected bool				link = true;
		protected bool				doubler = false;
		protected bool				integer = false;
	}
}
