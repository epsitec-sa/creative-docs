//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe Context décrit un contexte (pour la désérialisation) lié à
	/// un environnement 'texte'.
	/// </summary>
	public class Context
	{
		public Context()
		{
			this.style_list  = new StyleList ();
			this.char_marker = new Internal.CharMarker ();
			
			this.char_marker.Add (Context.Markers.TagSelected);
			this.char_marker.Add (Context.Markers.TagRequiresSpellChecking);
			
			this.markers = new Context.Markers (this.char_marker);
			 
			this.font_collection = new OpenType.FontCollection ();
			this.font_collection.Initialize ();
			
			this.font_cache = new System.Collections.Hashtable ();
		}
		
		
		public StyleList						StyleList
		{
			get
			{
				return this.style_list;
			}
		}
		
		internal Internal.CharMarker			CharMarker
		{
			get
			{
				return this.char_marker;
			}
		}
		
		public Context.Markers					Marker
		{
			get
			{
				return this.markers;
			}
		}
		
		
		public void GetFont(ulong code, out OpenType.Font font, out double font_size)
		{
			code &= 0xffffffff00000000ul;
			
			long current_style_version = this.style_list.InternalStyleTable.Version;
			
			if ((this.get_font_last_style_version == current_style_version) &&
				(this.get_font_last_code == code))
			{
				font      = this.get_font_last_font;
				font_size = this.get_font_last_font_size;
				
				return;
			}
			
			Styles.SimpleStyle style = this.style_list[code];
			
			Styles.LocalSettings local_settings = style.GetLocalSettings (code);
			Styles.ExtraSettings extra_settings = style.GetExtraSettings (code);
			
			Properties.FontProperty     font_p      = style[Properties.WellKnownType.Font] as Properties.FontProperty;
			Properties.FontSizeProperty font_size_p = style[Properties.WellKnownType.FontSize] as Properties.FontSizeProperty;
			
			string font_face  = font_p.FaceName;
			string font_style = font_p.StyleName;
			string font_full  = string.Concat (font_face, "/", font_style);
			
			font_size  = font_size_p.PointSize;
			
			font = this.font_cache[font_full] as OpenType.Font;
			
			if (font == null)
			{
				font = this.font_collection.CreateFont (font_face, font_style);
				this.font_cache[font_full] = font;
			}
			
			this.get_font_last_style_version = current_style_version;
			this.get_font_last_code          = code;
			this.get_font_last_font          = font;
			this.get_font_last_font_size     = font_size;
		}
		
		
		#region Markers Class
		public class Markers
		{
			internal Markers(Internal.CharMarker marker)
			{
				this.requires_spell_checking = marker[Markers.TagRequiresSpellChecking];
				this.selected                = marker[Markers.TagSelected];
			}
			
			
			public ulong						Selected
			{
				get
				{
					return this.selected;
				}
			}
			
			public ulong						RequiresSpellChecking
			{
				get
				{
					return this.requires_spell_checking;
				}
			}
			
			
			private ulong						selected;
			private ulong						requires_spell_checking;
			
			public static string				TagSelected					= "Selected";
			public static string				TagRequiresSpellChecking	= "RequiresSpellChecking";
		}
		#endregion
		
		private StyleList						style_list;
		private Internal.CharMarker				char_marker;
		private Markers							markers;
		private OpenType.FontCollection			font_collection;
		private System.Collections.Hashtable	font_cache;
		
		private long							get_font_last_style_version;
		private ulong							get_font_last_code;
		private OpenType.Font					get_font_last_font;
		private double							get_font_last_font_size;
	}
}
