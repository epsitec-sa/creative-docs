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
		
		private StyleList						style_list;
		private Internal.CharMarker				char_marker;
		private Markers							markers;
	}
}
