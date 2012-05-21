//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Drawing
{
	/// <summary>
	/// The DashedPath class represents a ... dashed path.
	/// </summary>
	public class DashedPath : Path
	{
		public DashedPath()
		{
		}
		
		
		public double							DashOffset
		{
			get
			{
				return this.start;
			}
			set
			{
				this.CreateOnTheFly ();
				this.start = value;
				AntiGrain.Path.SetDashOffset (this.aggPath, this.start);
			}
		}
		
		
		public void ResetDash()
		{
			this.CreateOnTheFly ();
			AntiGrain.Path.ResetDash (this.aggPath);
		}
		
		public void AddDash(double dashLength, double gapLength)
		{
			this.CreateOnTheFly ();
			AntiGrain.Path.AddDash (this.aggPath, dashLength, gapLength);
		}
		
		
		public Path GenerateDashedPath()
		{
			return this.GenerateDashedPath (this.defaultZoom);
		}
		
		public Path GenerateDashedPath(double approximationZoom)
		{
			if (this.IsEmpty)
			{
				return null;
			}
			
			Path path = new Path ();
			
			this.CreateOnTheFly ();
			path.InternalCreateNonEmpty ();
			
			AntiGrain.Path.AppendDashedPath (path.Handle, this.aggPath, approximationZoom);
			
			return path;
		}
		
		
		private double							start;
	}
}
