//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.OpenType
{
	/// <summary>
	/// Summary description for FontIdentity.
	/// </summary>
	public class FontIdentity
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
		
		
		public string							FullFontName
		{
			get
			{
				return this.ot_name.GetFullFontName ();
			}
		}
		
		
		public FontData							FontData
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
	}
}
