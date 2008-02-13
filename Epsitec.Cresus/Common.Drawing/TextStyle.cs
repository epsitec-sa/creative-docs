//	Copyright © 2004-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System;

namespace Epsitec.Common.Drawing
{
	/// <summary>
	/// La classe TextStyle définit le style d'un texte (paragraphe) qui peut dériver
	/// d'un style par défaut ou de n'importe quel autre style.
	/// </summary>
	
	[System.ComponentModel.TypeConverter (typeof (TextStyle.Converter))]
	
	public sealed class TextStyle : System.ICloneable
	{
		public TextStyle() : this (null)
		{
		}
		
		public TextStyle(TextStyle parent)
		{
			if (parent == null)
			{
				parent = TextStyle.Default;
			}
			
			this.parent          = parent;
			this.font            = null;
			this.size            = 0;
			
			this.color           = RichColor.Empty;
			this.color_anchor    = Color.Empty;
			this.color_wave      = Color.Empty;
			
			this.alignment       = ContentAlignment.None;
			this.break_mode      = TextBreakMode.None;
			this.justif_mode     = TextJustifMode.Undefined;
			this.show_line_break = ThreeState.None;
			this.show_tab_marks        = ThreeState.None;
			
			this.language        = null;

			this.def_tab_width   = 0.0;
			this.tabs            = new List<Tab> ();
		}
		
		
		private TextStyle(int __unused__)
		{
			this.font            = Font.DefaultFont;
			this.size            = Font.DefaultFontSize;
			
			this.color           = new RichColor (1.0, 0.0, 0.0, 0.0, 1.0);
			this.color_anchor    = new Color (0, 0, 1);
			this.color_wave      = new Color (1, 0, 0);
			
			this.alignment       = ContentAlignment.TopLeft;
			this.break_mode      = TextBreakMode.Ellipsis | TextBreakMode.SingleLine;
			this.justif_mode     = TextJustifMode.None;
			this.show_line_break = ThreeState.False;
			this.show_tab_marks  = ThreeState.False;
			
			this.language        = "";

			this.def_tab_width   = 40;
			this.tabs            = new List<Tab> ();
		}
		
		static TextStyle()
		{
			TextStyle.default_style = new TextStyle (0);
			TextStyle.default_style.is_default_style = true;
		}
		
		
		public bool								IsDefaultStyle
		{
			get
			{
				return this.is_default_style;
			}
		}
		
		public TextStyle						Parent
		{
			get
			{
				return this.parent;
			}
		}
		
		
		public Font								Font
		{
			//	UnderlineThickness
			//	UnderlineOffset
			
			get
			{
				return (this.font == null) ? this.parent.Font : this.font;
			}
			set
			{
				this.CheckForDefaultStyle ();
				
				if (value != this.font)
				{
					this.font = value;
					this.OnChanged ();
				}
			}
		}
		
		public double							Size
		{
			get
			{
				return (this.size == 0) ? this.parent.Size : this.size;
			}
			set
			{
				this.CheckForDefaultStyle ();
				
				if (this.size != value)
				{
					this.size = value;
					this.OnChanged ();
				}
			}
		}
		
		public RichColor						RichColor
		{
			get
			{
				return (this.color.IsEmpty) ? new RichColor(this.parent.Color) : this.color;
			}
			set
			{
				this.CheckForDefaultStyle ();
				
				if (this.color != value)
				{
					this.color = value;
					this.OnChanged ();
				}
			}
		}
		
		public Color							Color
		{
			get
			{
				return (this.color.IsEmpty) ? this.parent.Color : this.color.Basic;
			}
			set
			{
				this.CheckForDefaultStyle ();
				
				if (this.color.Basic != value)
				{
					this.color.Basic = value;
					this.OnChanged ();
				}
			}
		}
		
		public Color							AnchorColor
		{
			get
			{
				return (this.color_anchor.IsEmpty) ? this.parent.AnchorColor : this.color_anchor;
			}
			set
			{
				this.CheckForDefaultStyle ();
				
				if (this.color_anchor != value)
				{
					this.color_anchor = value;
					this.OnChanged ();
				}
			}
		}
		
		public Color							WaveColor
		{
			get
			{
				return (this.color_wave.IsEmpty) ? this.parent.WaveColor : this.color_wave;
			}
			set
			{
				this.CheckForDefaultStyle ();
				
				if (this.color_wave != value)
				{
					this.color_wave = value;
					this.OnChanged ();
				}
			}
		}
		
		public ContentAlignment					Alignment
		{
			get
			{
				return (this.alignment == ContentAlignment.None) ? this.parent.Alignment : this.alignment;
			}
			set
			{
				this.CheckForDefaultStyle ();
				
				if (this.alignment != value)
				{
					this.alignment = value;
					this.OnChanged ();
				}
			}
		}
		
		public TextBreakMode					BreakMode
		{
			get
			{
				return (this.break_mode == TextBreakMode.None) ? this.parent.BreakMode : this.break_mode;
			}

			set
			{
				this.CheckForDefaultStyle ();
				
				if (this.break_mode != value)
				{
					this.break_mode = value;
					this.OnChanged ();
				}
			}
		}
		
		public TextJustifMode					JustifMode
		{
			get
			{
				return (this.justif_mode == TextJustifMode.Undefined) ? this.parent.JustifMode : this.justif_mode;
			}
			set
			{
				this.CheckForDefaultStyle ();
				
				if (this.justif_mode != value)
				{
					this.justif_mode = value;
					this.OnChanged ();
				}
			}
		}
		
		public bool								ShowLineBreaks
		{
			get
			{
				if (this.show_line_break == ThreeState.None)
				{
					return this.parent.ShowLineBreaks;
				}
				
				switch (this.show_line_break)
				{
					case ThreeState.True:	return true;
					case ThreeState.False:	return false;
				}
				
				throw new System.InvalidOperationException ("TextStyle has invalid ShowLineBreak state.");
			}
			set
			{
				this.CheckForDefaultStyle ();
				
				ThreeState test = (value ? ThreeState.True : ThreeState.False);
				
				if (this.show_line_break != test)
				{
					this.show_line_break = test;
					this.OnChanged ();
				}
			}
		}
		
		public bool								ShowTabMarks
		{
			get
			{
				if (this.show_tab_marks == ThreeState.None)
				{
					return this.parent.ShowTabMarks;
				}
				
				switch (this.show_tab_marks)
				{
					case ThreeState.True:	return true;
					case ThreeState.False:	return false;
				}
				
				throw new System.InvalidOperationException ("TextStyle has invalid ShowTab state.");
			}
			set
			{
				this.CheckForDefaultStyle();
				
				ThreeState test = (value ? ThreeState.True : ThreeState.False);
				
				if (this.show_tab_marks != test)
				{
					this.show_tab_marks = test;
					this.OnChanged ();
				}
			}
		}
		
		public string							Language
		{
			get
			{
				return (this.language == null) ? this.parent.Language : this.language;
			}
			set
			{
				this.CheckForDefaultStyle ();
				
				if (this.language != value)
				{
					this.language = value;
					this.OnChanged ();
				}
			}
		}


		public double							DefaultTabWidth
		{
			get
			{
				return (this.def_tab_width == 0.0) ? this.parent.DefaultTabWidth : this.def_tab_width;
			}
			set
			{
				this.CheckForDefaultStyle ();
				
				if (this.def_tab_width != value)
				{
					this.def_tab_width = value;
					this.OnChanged ();
				}
			}
		}
		
		public static TextStyle					Default
		{
			get
			{
				return TextStyle.default_style;
			}
		}
		
		
		
		public int TabInsert(Tab tab)
		{
			this.CheckForDefaultStyle ();
			int rank = this.tabs.Count;
			this.tabs.Add (tab);
			this.OnChanged ();
			return rank;
		}
		
		public int TabCount
		{
			get
			{
				return this.tabs.Count;
			}
		}
		
		public void TabRemove(Tab tab)
		{
			this.tabs.Remove (tab);
			this.OnChanged ();
		}
		
		public void TabRemoveAt(int rank)
		{
			System.Diagnostics.Debug.Assert (rank >= 0);
			System.Diagnostics.Debug.Assert (rank < this.tabs.Count);
			this.CheckForDefaultStyle ();
			this.tabs.RemoveAt (rank);
			this.OnChanged ();
		}
		
		public Tab GetTab(int rank)
		{
			System.Diagnostics.Debug.Assert (rank >= 0);
			System.Diagnostics.Debug.Assert (rank < this.tabs.Count);
			return new Tab (this.tabs[rank]);
		}

		public Tab[] GetTabArray()
		{
			return this.tabs.ToArray ();
		}

		public void TabCopyTo(TextStyle dst)
		{
			dst.CheckForDefaultStyle ();
			dst.tabs.Clear();
			foreach (Tab tab in this.tabs)
			{
				dst.tabs.Add (new Tab (tab));
			}
		}
		
		public void TabCopyTo(out Tab[] dst)
		{
			dst = new Tab[this.tabs.Count];
			for (int i=0; i<this.tabs.Count; i++)
			{
				dst[i] = this.tabs[i];
			}
		}

		public void TabCopyFrom(Tab[] src)
		{
			this.CheckForDefaultStyle ();
			this.tabs.Clear ();
			for (int i=0; i<src.Length; i++)
			{
				this.tabs.Add (new Tab (src[i]));
			}
			this.OnChanged ();
		}
		
		public void SetTabPosition(int rank, double pos)
		{
			System.Diagnostics.Debug.Assert (rank >= 0);
			System.Diagnostics.Debug.Assert (rank < this.tabs.Count);
			Tab tab = this.tabs[rank];
			this.tabs[rank] = new Tab (pos, tab.Type, tab.Line);
			this.OnChanged ();
		}

		public Tab FindTabAfterPosition(double pos)
		{
			//	Cherche la position du prochain tabulateur après une position donnée.
			double lastPos = 0.0;
			double bestDist = 1000000;
			Tab bestTab = Tab.Empty;
			foreach (Tab tab in this.tabs)
			{
				lastPos = System.Math.Max (lastPos, tab.Pos);

				if (pos+0.001 >= tab.Pos)
					continue;

				double dist = tab.Pos - pos;
				if (bestDist > dist)
				{
					bestDist = dist;
					bestTab = tab;
				}
			}

			if (bestTab.IsEmpty)
			{
				double def = this.DefaultTabWidth;
				pos -= lastPos;
				pos = System.Math.Ceiling((pos+1)/def)*def;
				pos += lastPos;

				Tab tab = new Tab (pos, TextTabType.Right, TextTabLine.None);
				return tab;
			}
			else
			{
				return bestTab;
			}
		}

		
		#region ICloneable Members
		object System.ICloneable.Clone()
		{
			return this.Clone ();
		}
		#endregion
		
		
		public TextStyle Clone()
		{
			//	Crée une copie parfaite du TextStyle. L'astuce des deux méthodes
			//	ci-après permet de réalise un "Clone" qui marche aussi pour des
			//	classes dérivées, lesquelles doivent simplement surcharger ces
			//	méthodes pour (1) allouer le bon objet et (2) copier les champs
			//	supplémentaires.
			
			return this.CloneCopyToNewObject (this.CloneNewObject ()) as TextStyle;
		}
		
		
		public override string ToString()
		{
			return this.ToString (System.Globalization.CultureInfo.InvariantCulture);
		}

		public string ToString(System.Globalization.CultureInfo culture)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			if (this.font != null)
			{
				if (buffer.Length > 0) buffer.Append (";");
					
				buffer.Append ("<Font=");
				buffer.Append (this.font.FaceName);
				buffer.Append ("/");
				buffer.Append (this.font.StyleName);
				buffer.Append ("/");
				buffer.Append (this.font.OpticalName);
				buffer.Append (">");
			}
				
			if (this.size != 0)
			{
				if (buffer.Length > 0) buffer.Append (";");
					
				buffer.Append ("<Size=");
				buffer.Append (this.size.ToString (culture));
				buffer.Append (">");
			}
				
			//	TODO: ajouter tout ce qui manque...
				
			return buffer.ToString ();
		}
		
		
		public void Parse(string value)
		{
			this.Parse (value, System.Globalization.CultureInfo.InvariantCulture);
		}
		
		public void Parse(string value, System.Globalization.CultureInfo culture)
		{
			string[]  args = Support.Utilities.Split (value, ';');
				
			foreach (string arg in args)
			{
				int n   = arg.Length;
				int pos = arg.IndexOf ("=");
					
				System.Diagnostics.Debug.Assert (arg[0] == '<');
				System.Diagnostics.Debug.Assert (arg[n-1] == '>');
				System.Diagnostics.Debug.Assert (pos > 1);
					
				string name = arg.Substring (1, pos-1);
				string data = arg.Substring (pos+1, n-pos-2);
					
				switch (name)
				{
					case "Font":
					{
						string[] font_args = data.Split ('/');
							
						System.Diagnostics.Debug.Assert (font_args.Length == 3);
							
						string face  = font_args[0];
						string style = font_args[1];
						string optic = font_args[2];
							
						this.Font = Font.GetFont (face, style, optic);
						break;
					}
						
					case "Size":
						this.Size = System.Double.Parse (data, culture);
						break;

						//	TODO: ajouter les autres propriétés ici...
				}
			}
		}
	
		
		public static void DefineDefaultRichColor(Drawing.RichColor color)
		{
			TextStyle.default_style.color = color;
		}
		
		public static void DefineDefaultColor(Drawing.Color color)
		{
			TextStyle.default_style.color.Basic = color;
		}
		
		
		private void CheckForDefaultStyle ()
		{
			if (this.is_default_style)
			{
				throw new System.InvalidOperationException ("TextStyle.Default cannot be modified.");
			}
		}


		private object CloneNewObject()
		{
			return new TextStyle ();
		}

		private object CloneCopyToNewObject(object o)
		{
			TextStyle that = o as TextStyle;
			
			//	Copie tous les éléments en utilisant "this" comme modèle. Il y a juste le
			//	parent pour lequel nous devons faire attention.
			
			that.parent          = this.is_default_style ? TextStyle.default_style : this.parent;
			that.font            = this.font;
			that.size            = this.size;
			that.color           = this.color;
			that.color_anchor    = this.color_anchor;
			that.color_wave      = this.color_wave;
			that.alignment       = this.alignment;
			that.break_mode      = this.break_mode;
			that.justif_mode     = this.justif_mode;
			that.show_line_break = this.show_line_break;
			that.show_tab_marks        = this.show_tab_marks;
			that.language        = this.language;
			that.tabs            = new List<Tab> (this.tabs);
			
			return that;
		}

		private void OnChanged()
		{
			if (this.Changed != null)
			{
				this.Changed (this);
			}
		}

		#region ThreeState Enumeration

		private enum ThreeState
		{
			None,								//	état non défini
			False,
			True
		}

		#endregion

		#region Converter Class

		internal class Converter : Epsitec.Common.Types.AbstractStringConverter
		{
			public override object ParseString(string value, System.Globalization.CultureInfo culture)
			{
				TextStyle that = new TextStyle ();
				that.Parse (value, culture);
				return that;
			}
			
			public override string ToString(object value, System.Globalization.CultureInfo culture)
			{
				TextStyle that = value as TextStyle;
				return that.ToString (culture);
			}
		}

		#endregion

		#region Tab Structure

		[System.Serializable]
		public struct Tab : IEquatable<Tab>
		{
			public Tab(Tab model)
			{
				this.Pos  = model.Pos;
				this.Type = model.Type;
				this.Line = model.Line;
			}

			public Tab(double pos, TextTabType type, TextTabLine line)
			{
				this.Pos = pos;
				this.Type = type;
				this.Line = line;
			}


			public static readonly Tab		Empty = new Tab (double.NaN, TextTabType.None, TextTabLine.None);
			
			public readonly double			Pos;
			public readonly TextTabType		Type;
			public readonly TextTabLine		Line;

			public bool IsEmpty
			{
				get
				{
					return double.IsNaN (this.Pos);
				}
			}

			#region IEquatable<Tab> Members

			public bool Equals(Tab other)
			{
				if ((double.IsNaN (this.Pos)) &&
					(double.IsNaN (other.Pos)))
				{
					//	Same...
				}
				else if (this.Pos != other.Pos)
				{
					return false;
				}

				return this.Type == other.Type
					&& this.Line == other.Line;
			}

			#endregion

			public override bool Equals(object obj)
			{
				if (obj is Tab)
				{
					return this.Equals ((Tab) obj);
				}
				else
				{
					return false;
				}
			}

			public override int GetHashCode()
			{
				return this.Pos.GetHashCode () ^  this.Type.GetHashCode () ^ this.Line.GetHashCode ();
			}
		}

		#endregion

		public event Support.EventHandler		Changed;
		
		
		private static readonly TextStyle		default_style;
		
		private bool							is_default_style;
		private TextStyle						parent;
		
		private Font							font;
		private double							size;
		private RichColor						color;
		private Color							color_anchor;
		private Color							color_wave;
		private ContentAlignment				alignment;
		private TextBreakMode					break_mode;
		private TextJustifMode					justif_mode;
		private ThreeState						show_line_break;
		private ThreeState						show_tab_marks;
		private string							language;
		private double							def_tab_width;
		private List<Tab>						tabs;
	}
}
