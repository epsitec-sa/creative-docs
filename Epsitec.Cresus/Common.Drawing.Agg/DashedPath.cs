//	Copyright © 2003-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
				AntiGrain.Path.SetDashOffset (this.agg_path, this.start);
			}
		}
		
		
		public void ResetDash()
		{
			this.CreateOnTheFly ();
			AntiGrain.Path.ResetDash (this.agg_path);
		}
		
		public void AddDash(double dash_length, double gap_length)
		{
			this.CreateOnTheFly ();
			AntiGrain.Path.AddDash (this.agg_path, dash_length, gap_length);
		}
		
		
		public Path GenerateDashedPath()
		{
			return this.GenerateDashedPath (this.default_zoom);
		}
		
		public Path GenerateDashedPath(double approximation_zoom)
		{
			if (this.IsEmpty)
			{
				return null;
			}
			
			Path path = new Path ();
			
			this.CreateOnTheFly ();
			path.InternalCreateNonEmpty ();
			
			AntiGrain.Path.AppendDashedPath (path.Handle, this.agg_path, approximation_zoom);
			
			return path;
		}
		
		
		private double							start;
	}
}
