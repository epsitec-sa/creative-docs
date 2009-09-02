//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Layout
{
	/// <summary>
	/// La classe XlineRecord représente un changement de soulignement, biffé,
	/// etc. au sein d'un paragraphe.
	/// </summary>
	public class XlineRecord
	{
		public XlineRecord(RecordType recordType, int textOffset, System.Collections.ICollection xlines, Properties.FontColorProperty textColor,
			/**/		   double x, double y, double ascender, double descender, int frameIndex, bool isVisible)
		{
			int count = xlines == null ? 0 : xlines.Count;
			
			this.recordType  = recordType;
			this.textOffset  = textOffset;
			this.xlines      = new Properties.AbstractXlineProperty[count];
			
			if (xlines != null)
			{
				xlines.CopyTo (this.xlines, 0);
			}
			
			this.textColor = textColor;
			
			this.x = x;
			this.y = y;
			
			this.ascender  = ascender;
			this.descender = descender;
			
			this.frameIndex = frameIndex;
			this.isVisible  = isVisible;
		}
		
		
		public RecordType						Type
		{
			get
			{
				return this.recordType;
			}
		}
		
		public int								TextOffset
		{
			get
			{
				return this.textOffset;
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
				return this.frameIndex;
			}
		}
		
		public bool								IsVisible
		{
			get
			{
				return this.isVisible;
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
		
		public Properties.FontColorProperty		TextColor
		{
			get
			{
				return this.textColor;
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
		
		private RecordType						recordType;
		private int								textOffset;
		private Properties.AbstractXlineProperty[]	xlines;
		private int								frameIndex;
		private bool							isVisible;
		private double							x, y;
		private double							ascender, descender;
		private Properties.FontColorProperty	textColor;
	}
}
