//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.OpenType
{
	/// <summary>
	/// Summary description for FontIdentity.
	/// </summary>
	public sealed class FontIdentity
	{
		public FontIdentity(Table_name open_type_name_table, object system_record)
		{
			this.ot_name = open_type_name_table;
			this.record  = system_record;
		}
		
		
		public string							LocaleFaceName
		{
			get
			{
				return this.GetName (NameId.FontFamily);
			}
		}
		
		public string							LocaleStyleName
		{
			get
			{
				return this.GetName (NameId.FontSubfamily);
			}
		}
		
		
		public string							InvariantFaceName
		{
			get
			{
				return this.GetName (NameId.FontFamily, 1033);
			}
		}
		
		public string							InvariantStyleName
		{
			get
			{
				return this.GetName (NameId.FontSubfamily, 1033);
			}
		}
		
		
		public string							FullName
		{
			get
			{
				return this.ot_name.GetFullFontName ();
			}
		}
		
		
		internal FontData						FontData
		{
			get
			{
				lock (this)
				{
					if (this.font_data == null)
					{
						byte[] data = Platform.Win32.LoadFontData (this.record);
						
						this.font_data = data == null ? null : new FontData (data);
					}
				}
				
				return this.font_data;
			}
		}
		
		public int								FontWeight
		{
			get
			{
				return Platform.Win32.GetFontWeight (this.record);
			}
		}
		
		public bool								FontIsItalic
		{
			get
			{
				return Platform.Win32.GetFontItalic (this.record) != 0;
			}
		}
		
		
		public static System.Collections.IComparer Comparer
		{
			get
			{
				return new FontComparer ();
			}
		}
		
		
		public Platform.IFontHandle GetFontHandle(int size)
		{
			SizeInfo info = this.GetSizeInfo (size);
			
			return info == null ? null : info.Handle;
		}
		
		public SizeInfo GetSizeInfo(int size)
		{
			lock (this)
			{
				if (this.font_sizes == null)
				{
					this.font_sizes = new System.Collections.Hashtable ();
				}
				
				if (this.font_sizes.Contains (size) == false)
				{
					this.font_sizes[size] = new SizeInfo (size, Platform.Win32.GetFontHandle (this.record, size));
				}
			}
			
			return this.font_sizes[size] as SizeInfo;
		}
		
		
		#region FontComparer Class
		private class FontComparer : System.Collections.IComparer
		{
			#region IComparer Members
			public int Compare(object x, object y)
			{
				FontIdentity fx = x as FontIdentity;
				FontIdentity fy = y as FontIdentity;
				
				int comp = string.Compare (fx.InvariantFaceName, fy.InvariantFaceName);
				
				if (comp == 0)
				{
					comp = string.Compare (fx.InvariantStyleName, fy.InvariantStyleName);
				}
				
				return comp;
			}
			#endregion
		}
		#endregion
		
		#region SizeInfo Class
		public class SizeInfo
		{
			public SizeInfo(int size, Platform.IFontHandle handle)
			{
				this.point_size   = size;
				this.font_handle  = handle;
				this.glyph_widths = new int[4][];
			}
			
			
			public Platform.IFontHandle			Handle
			{
				get
				{
					return this.font_handle;
				}
			}
			
			public int							PointSize
			{
				get
				{
					return this.point_size;
				}
			}
			
			
			public int GetGlyphWidth(int glyph)
			{
				if (glyph >= 0xffff)
				{
					return 0;
				}
				
				int block = glyph / 64;
				int index = glyph % 64;
				
				if (block > this.glyph_widths.Length)
				{
					int[][] old_widths = this.glyph_widths;
					int[][] new_widths = new int[block+1][];
					
					for (int i = 0; i < old_widths.Length; i++)
					{
						new_widths[i] = old_widths[i];
					}
					
					this.glyph_widths = new_widths;
				}
				
				if (this.glyph_widths[block] == null)
				{
					this.glyph_widths[block] = new int[64];
					
					Platform.Win32.FillFontWidths (this.font_handle, block*64, 64, this.glyph_widths[block], null, null);
				}
				
				return this.glyph_widths[block][index];
			}
			
			
			int									point_size;
			Platform.IFontHandle				font_handle;
			int[][]								glyph_widths;
		}
		#endregion
		
		private string GetName(NameId id)
		{
			System.Globalization.CultureInfo info = System.Globalization.CultureInfo.CurrentCulture;
			
			int locale_id = (info.LCID & 0x03ff) + 1024;
			
			string name = this.GetName (id, locale_id);
			
			return (name == null) ? this.GetName (id, 1033) : name;
		}
		
		private string GetName(NameId id, int locale_id)
		{
			string name;
			
			name = this.ot_name.GetUnicodeName (locale_id, id, PlatformId.Microsoft);
			
			if (name == null)
			{
				name = this.ot_name.GetLatinName (locale_id, id, PlatformId.Microsoft);
			}
			
			return name;
		}
		
		
		private Table_name						ot_name;
		private object							record;
		private FontData						font_data;
		private System.Collections.Hashtable	font_sizes;
	}
}
