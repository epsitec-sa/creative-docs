namespace Epsitec.Common.Drawing
{
	public struct Size
	{
		public Size(double width, double height)
		{
			this.width  = width;
			this.height = height;
		}
		
		public Size(System.Drawing.SizeF size)
		{
			this.width  = size.Width;
			this.height = size.Height;
		}
		
		public Size(System.Drawing.Size size)
		{
			this.width  = size.Width;
			this.height = size.Height;
		}
		
		public double					Width
		{
			get { return this.width; }
			set { this.width = value; }
		}
		
		public double					Height
		{
			get { return this.height; }
			set { this.height = value; }
		}
		
		public bool						IsEmpty
		{
			get { return this.width == 0 && this.height == 0; }
		}
		
		public static readonly Size		Empty;
		
		public Point ToPoint()
		{
			return new Point (this.width, this.height);
		}
		
		public override bool Equals(object obj)
		{
			if ((obj == null) &&
				(obj.GetType () != typeof (Size)))
			{
				return false;
			}
			
			Size s = (Size) obj;
			
			return (s.width == this.width) && (s.height == this.height);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}
		
		public override string ToString()
		{
			return System.String.Format ("{{Width={0}, Height={1}}}", this.width, this.height);
		}
		
		public static bool operator ==(Size a, Size b)
		{
			return (a.width == b.width) && (a.height == b.height);
		}
		
		public static bool operator !=(Size a, Size b)
		{
			return (a.width != b.width) || (a.height != b.height);
		}
		
		private double					width, height;
	}
}
