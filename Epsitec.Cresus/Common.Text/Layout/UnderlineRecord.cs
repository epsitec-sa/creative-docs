//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Layout
{
	/// <summary>
	/// La classe XlineRecord représente un changement de soulignement, biffé,
	/// etc. au sein d'un paragraphe.
	/// </summary>
	public class XlineRecord
	{
		public XlineRecord(RecordType record_type, int text_offset, System.Collections.ICollection xlines,
			/**/		   double x, double y, double ascender, double descender, int frame_index, bool is_visible)
		{
			int count = xlines == null ? 0 : xlines.Count;
			
			this.record_type = record_type;
			this.text_offset = text_offset;
			this.xlines      = new Properties.AbstractXlineProperty[count];
			
			if (xlines != null)
			{
				xlines.CopyTo (this.xlines, 0);
			}
			
			this.x = x;
			this.y = y;
			
			this.ascender  = ascender;
			this.descender = descender;
			
			this.frame_index = frame_index;
			this.is_visible  = is_visible;
		}
		
		
		public RecordType						Type
		{
			get
			{
				return this.record_type;
			}
		}
		
		public int								TextOffset
		{
			get
			{
				return this.text_offset;
			}
		}
		
		public Properties.AbstractXlineProperty[]	Xlines
		{
			get
			{
				return this.xlines;
			}
		}
		
		public int								FrameIndex
		{
			get
			{
				return this.frame_index;
			}
		}
		
		public bool								IsVisible
		{
			get
			{
				return this.is_visible;
			}
		}
		
		public double							X
		{
			get
			{
				return this.x;
			}
		}
		
		public double							Y
		{
			get
			{
				return this.y;
			}
		}
		
		public double							Ascender
		{
			get
			{
				return this.ascender;
			}
		}
		
		public double							Descender
		{
			get
			{
				return this.descender;
			}
		}
		
		
		#region RecordType Enumeration
		public enum RecordType
		{
			Change,
			
			LineStart,
			LineEnd
		}
		#endregion
		
		private RecordType						record_type;
		private int								text_offset;
		private Properties.AbstractXlineProperty[]	xlines;
		private int								frame_index;
		private bool							is_visible;
		private double							x, y;
		private double							ascender, descender;
	}
}
