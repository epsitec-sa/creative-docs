namespace Epsitec.Common.Drawing
{
	using XmlAttribute = System.Xml.Serialization.XmlAttributeAttribute;
	
	[System.Serializable]
	
	public class Shape
	{
		public Shape()
		{
		}

		
		public Path Path
		{
			get
			{
				return this.path;
			}

			set
			{
				this.path = value;
			}
		}

		public bool Surface
		{
			get
			{
				return this.surface;
			}

			set
			{
				this.surface = value;
			}
		}
		
		public bool Stroke
		{
			get
			{
				return this.stroke;
			}

			set
			{
				this.stroke = value;
			}
		}
		
		public double LineWidth
		{
			get
			{
				return this.lineWidth;
			}

			set
			{
				this.lineWidth = value;
			}
		}
		
		public JoinStyle LineJoin
		{
			get
			{
				return this.lineJoin;
			}

			set
			{
				this.lineJoin = value;
			}
		}
		
		public CapStyle LineCap
		{
			get
			{
				return this.lineCap;
			}

			set
			{
				this.lineCap = value;
			}
		}
		
		public bool LineDash
		{
			get
			{
				return this.lineDash;
			}

			set
			{
				this.lineDash = value;
			}
		}
		
		public double LineDashPen1
		{
			get
			{
				return this.lineDashPen1;
			}

			set
			{
				this.lineDashPen1 = value;
			}
		}
		
		public double LineDashGap1
		{
			get
			{
				return this.lineDashGap1;
			}

			set
			{
				this.lineDashGap1 = value;
			}
		}
		
		public double LineDashPen2
		{
			get
			{
				return this.lineDashPen2;
			}

			set
			{
				this.lineDashPen2 = value;
			}
		}
		
		public double LineDashGap2
		{
			get
			{
				return this.lineDashGap2;
			}

			set
			{
				this.lineDashGap2 = value;
			}
		}
		
		
		protected Path							path = null;
		protected bool							surface = false;
		protected bool							stroke = false;
		protected double						lineWidth = 0.0;
		protected JoinStyle						lineJoin = JoinStyle.Round;
		protected CapStyle						lineCap = CapStyle.Round;
		protected bool							lineDash = false;
		protected double						lineDashPen1 = 0.0;
		protected double						lineDashGap1 = 0.0;
		protected double						lineDashPen2 = 0.0;
		protected double						lineDashGap2 = 0.0;
		protected double						lineMiterLimit = 5.0;
	}
}
