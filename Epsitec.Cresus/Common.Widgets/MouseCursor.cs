namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe MouseCursor décrit un curseur de souris.
	/// </summary>
	public class MouseCursor
	{
		private MouseCursor(System.Windows.Forms.Cursor cursor)
		{
			this.cursor = cursor;
		}
		
		
		internal System.Windows.Forms.Cursor GetPlatformCursor()
		{
			return this.cursor;
		}
		
		public static MouseCursor			Default
		{
			get { return MouseCursor.AsArrow; }
		}
		
		public static MouseCursor			AsArrow
		{
			get { return MouseCursor.as_arrow; }
		}
		
		public static MouseCursor			AsHand
		{
			get { return MouseCursor.as_hand; }
		}
		
		public static MouseCursor			AsIBeam
		{
			get { return MouseCursor.as_I_beam; }
		}
		
		public static MouseCursor			AsHSplit
		{
			get { return MouseCursor.as_h_split; }
		}
		
		public static MouseCursor			AsVSplit
		{
			get { return MouseCursor.as_v_split; }
		}
		
		public static MouseCursor			AsCross
		{
			get { return MouseCursor.as_cross; }
		}
		
		public static MouseCursor			AsSizeAll
		{
			get { return MouseCursor.as_size_all; }
		}
		
		public static MouseCursor			AsWait
		{
			get { return MouseCursor.as_wait; }
		}
		
		
		private System.Windows.Forms.Cursor	cursor;
		
		
		private static MouseCursor			as_arrow   = new MouseCursor (System.Windows.Forms.Cursors.Arrow);
		private static MouseCursor			as_hand    = new MouseCursor (System.Windows.Forms.Cursors.Hand);
		private static MouseCursor			as_I_beam  = new MouseCursor (System.Windows.Forms.Cursors.IBeam);
		private static MouseCursor			as_h_split = new MouseCursor (System.Windows.Forms.Cursors.HSplit);
		private static MouseCursor			as_v_split = new MouseCursor (System.Windows.Forms.Cursors.VSplit);
		private static MouseCursor			as_cross    = new MouseCursor (System.Windows.Forms.Cursors.Cross);
		private static MouseCursor			as_size_all = new MouseCursor (System.Windows.Forms.Cursors.SizeAll);
		private static MouseCursor			as_wait    = new MouseCursor (System.Windows.Forms.Cursors.WaitCursor);
	}
}
