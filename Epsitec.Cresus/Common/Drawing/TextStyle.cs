//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Drawing
{
	/// <summary>
	/// La classe TextStyle définit le style d'un texte (paragraphe) qui peut dériver
	/// d'un style par défaut ou de n'importe quel autre style.
	/// </summary>
	
	[System.ComponentModel.TypeConverter (typeof (TextStyle.Converter))]

	public sealed class TextStyle : System.ICloneable, IReadOnly, IReadOnlyLock, System.IDisposable
	{
		public TextStyle()
			: this (TextStyle.Default)
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
			this.fontSize        = 0;
			
			this.fontColor       = RichColor.Empty;
			this.anchorColor     = Color.Empty;
			this.waveColor       = Color.Empty;
			
			this.alignment       = ContentAlignment.None;
			this.breakMode       = TextBreakMode.None;
			this.justifMode      = TextJustifMode.Undefined;
			this.showLineBreaks  = ThreeState.None;
			this.showTabMarks    = ThreeState.None;
			
			this.language        = null;

			this.defaultTabWidth = 0.0;
			this.tabs            = new List<Tab> ();

			this.parent.Changed += this.HandleParentChanged;
		}
		
		
		private TextStyle(int __unused__)
		{
			this.font            = Font.DefaultFont;
			this.fontSize        = Font.DefaultFontSize;
			
			this.fontColor       = new RichColor (1.0, 0.0, 0.0, 0.0, 1.0);
			this.anchorColor     = new Color (0, 0, 1);
			this.waveColor       = new Color (1, 0, 0);
			
			this.alignment       = ContentAlignment.TopLeft;
			this.breakMode       = TextBreakMode.Ellipsis | TextBreakMode.SingleLine;
			this.justifMode      = TextJustifMode.None;
			this.showLineBreaks  = ThreeState.False;
			this.showTabMarks    = ThreeState.False;
			
			this.language        = "";

			this.defaultTabWidth = 40;
			this.tabs            = new List<Tab> ();
			
			this.isDefaultStyle  = true;
			this.isReadOnly      = true;
		}
		
		
		public bool								IsDefaultStyle
		{
			get
			{
				return this.isDefaultStyle;
			}
		}


		#region IReadOnly Members

		public bool								IsReadOnly
		{
			get
			{
				return this.isReadOnly;
			}
		}

		#endregion
		
		
		public TextStyle						Parent
		{
			get
			{
				return this.parent;
			}
		}


		//	UnderlineThickness
		//	UnderlineOffset

		public Font Font
		{
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
					this.OnChanged ("Font");
				}
			}
		}
		
		public double							FontSize
		{
			get
			{
				return (this.fontSize == 0) ? this.parent.FontSize : this.fontSize;
			}
			set
			{
				this.CheckForDefaultStyle ();
				
				if (this.fontSize != value)
				{
					this.fontSize = value;
					this.OnChanged ("FontSize");
				}
			}
		}
		
		public RichColor						FontRichColor
		{
			get
			{
				return (this.fontColor.IsEmpty) ? new RichColor(this.parent.FontColor) : this.fontColor;
			}
			set
			{
				this.CheckForDefaultStyle ();
				
				if (this.fontColor != value)
				{
					this.fontColor = value;
					this.OnChanged ("FontColor");
				}
			}
		}
		
		public Color							FontColor
		{
			get
			{
				return (this.fontColor.IsEmpty) ? this.parent.FontColor : this.fontColor.Basic;
			}
			set
			{
				this.CheckForDefaultStyle ();
				
				if (this.fontColor.Basic != value)
				{
					this.fontColor = new RichColor (value);
					this.OnChanged ("FontColor");
				}
			}
		}
		
		public Color							AnchorColor
		{
			get
			{
				return (this.anchorColor.IsEmpty) ? this.parent.AnchorColor : this.anchorColor;
			}
			set
			{
				this.CheckForDefaultStyle ();
				
				if (this.anchorColor != value)
				{
					this.anchorColor = value;
					this.OnChanged ("AnchorColor");
				}
			}
		}
		
		public Color							WaveColor
		{
			get
			{
				return (this.waveColor.IsEmpty) ? this.parent.WaveColor : this.waveColor;
			}
			set
			{
				this.CheckForDefaultStyle ();
				
				if (this.waveColor != value)
				{
					this.waveColor = value;
					this.OnChanged ("WaveColor");
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
					this.OnChanged ("Alignment");
				}
			}
		}
		
		public TextBreakMode					BreakMode
		{
			get
			{
				return (this.breakMode == TextBreakMode.None) ? this.parent.BreakMode : this.breakMode;
			}

			set
			{
				this.CheckForDefaultStyle ();
				
				if (this.breakMode != value)
				{
					this.breakMode = value;
					this.OnChanged ("BreakMode");
				}
			}
		}
		
		public TextJustifMode					JustifMode
		{
			get
			{
				return (this.justifMode == TextJustifMode.Undefined) ? this.parent.JustifMode : this.justifMode;
			}
			set
			{
				this.CheckForDefaultStyle ();
				
				if (this.justifMode != value)
				{
					this.justifMode = value;
					this.OnChanged ("JustifMode");
				}
			}
		}
		
		public bool								ShowLineBreaks
		{
			get
			{
				if (this.showLineBreaks == ThreeState.None)
				{
					return this.parent.ShowLineBreaks;
				}
				
				switch (this.showLineBreaks)
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
				
				if (this.showLineBreaks != test)
				{
					this.showLineBreaks = test;
					this.OnChanged ("ShowLineBreaks");
				}
			}
		}
		
		public bool								ShowTabMarks
		{
			get
			{
				if (this.showTabMarks == ThreeState.None)
				{
					return this.parent.ShowTabMarks;
				}
				
				switch (this.showTabMarks)
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
				
				if (this.showTabMarks != test)
				{
					this.showTabMarks = test;
					this.OnChanged ("ShowTabMarks");
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
					this.OnChanged ("Language");
				}
			}
		}


		public double							DefaultTabWidth
		{
			get
			{
				return (this.defaultTabWidth == 0.0) ? this.parent.DefaultTabWidth : this.defaultTabWidth;
			}
			set
			{
				this.CheckForDefaultStyle ();
				
				if (this.defaultTabWidth != value)
				{
					this.defaultTabWidth = value;
					this.OnChanged ("DefaultTabWidth");
				}
			}
		}
		
		public static TextStyle					Default
		{
			get
			{
				return TextStyle.defaultStyle;
			}
		}
		
		
		
		public int TabInsert(Tab tab)
		{
			this.CheckForDefaultStyle ();
			int rank = this.tabs.Count;
			this.tabs.Add (tab);
			this.OnChanged ("Tabs");
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
			this.OnChanged ("Tabs");
		}
		
		public void TabRemoveAt(int rank)
		{
			System.Diagnostics.Debug.Assert (rank >= 0);
			System.Diagnostics.Debug.Assert (rank < this.tabs.Count);
			this.CheckForDefaultStyle ();
			this.tabs.RemoveAt (rank);
			this.OnChanged ("Tabs");
		}
		
		public Tab GetTab(int rank)
		{
			System.Diagnostics.Debug.Assert (rank >= 0);
			System.Diagnostics.Debug.Assert (rank < this.tabs.Count);
			return new Tab (this.tabs[rank]);
		}

		public Tab[] GetTabs()
		{
			return this.tabs.ToArray ();
		}

		public void SetTabs(IEnumerable<Tab> src)
		{
			this.CheckForDefaultStyle ();
			this.tabs.Clear ();
			this.tabs.AddRange (src);
			this.OnChanged ("Tabs");
		}
		
		public void SetTabPosition(int rank, double pos)
		{
			System.Diagnostics.Debug.Assert (rank >= 0);
			System.Diagnostics.Debug.Assert (rank < this.tabs.Count);
			Tab tab = this.tabs[rank];
			this.tabs[rank] = new Tab (pos, tab.Type, tab.Line);
			this.OnChanged ("Tabs");
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

				Tab tab = new Tab (pos, TextTabType.Left, TextTabLine.None);
				return tab;
			}
			else
			{
				return bestTab;
			}
		}

		public void RedefineParent(TextStyle parent)
		{
			this.CheckForDefaultStyle ();

			if (parent == null)
			{
				parent = TextStyle.Default;
			}
			if (parent == this.parent)
			{
				return;
			}
			
			//	Check that the caller is not building a circular parent relationship.
			
			TextStyle iter = parent;

			while (iter != null)
			{
				if (iter == this)
				{
					throw new System.ArgumentException ("Circular definition");
				}

				iter = iter.parent;
			}

			if (!this.parent.IsDefaultStyle)
			{
				this.parent.Changed -= this.HandleParentChanged;
			}
			
			this.parent = parent;
			this.parent.Changed += this.HandleParentChanged;
			
			this.OnChanged ("Parent");
		}

		#region IReadOnlyLock Members

		public void Lock()
		{
			this.isReadOnly = true;
		}

		public void Unlock()
		{
			this.isReadOnly = false;
		}

		#endregion
		
		#region IDisposable Members

		public void Dispose()
		{
			this.parent.Changed -= this.HandleParentChanged;
		}

		#endregion
		
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
				
			if (this.fontSize != 0)
			{
				if (buffer.Length > 0) buffer.Append (";");
					
				buffer.Append ("<Size=");
				buffer.Append (this.fontSize.ToString (culture));
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
						string[] fontArgs = data.Split ('/');
							
						System.Diagnostics.Debug.Assert (fontArgs.Length == 3);
							
						string face  = fontArgs[0];
						string style = fontArgs[1];
						string optic = fontArgs[2];
							
						this.Font = Font.GetFont (face, style, optic);
						break;
					}
						
					case "Size":
						this.FontSize = System.Double.Parse (data, culture);
						break;

						//	TODO: ajouter les autres propriétés ici...
				}
			}
		}


		public static void DefineDefaultFont(Font font)
		{
			TextStyle.defaultStyle.font = font ?? Font.DefaultFont;
		}

		public static void DefineDefaultFontSize(double fontSize)
		{
			TextStyle.defaultStyle.fontSize = (double.IsNaN (fontSize) || fontSize <= 0) ? Font.DefaultFontSize : fontSize;
		}
		
		public static void DefineDefaultFontColor(Drawing.Color color)
		{
			TextStyle.defaultStyle.fontColor = new RichColor (color);
		}
		
		
		private void CheckForDefaultStyle ()
		{
			if (this.isDefaultStyle)
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
			
			that.parent         = this.isDefaultStyle ? TextStyle.defaultStyle : this.parent;
			that.font           = this.font;
			that.fontSize       = this.fontSize;
			that.fontColor          = this.fontColor;
			that.anchorColor    = this.anchorColor;
			that.waveColor      = this.waveColor;
			that.alignment      = this.alignment;
			that.breakMode      = this.breakMode;
			that.justifMode     = this.justifMode;
			that.showLineBreaks = this.showLineBreaks;
			that.showTabMarks   = this.showTabMarks;
			that.language       = this.language;
			that.tabs           = new List<Tab> (this.tabs);
			
			return that;
		}

		private void HandleParentChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			this.OnChanged (e.PropertyName);
		}

		private void OnChanged(string propertyName)
		{
			this.changedListeners.Invoke (this, new DependencyPropertyChangedEventArgs (propertyName));
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
		public struct Tab : System.IEquatable<Tab>
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


		public event EventHandler<DependencyPropertyChangedEventArgs> Changed
		{
			add
			{
				this.changedListeners.Add (value);
			}
			remove
			{
				this.changedListeners.Remove (value);
			}
		}

		private WeakEventListeners				changedListeners;


		private static readonly TextStyle		defaultStyle = new TextStyle (0);
		
		private readonly bool					isDefaultStyle;
		private bool							isReadOnly;
		private TextStyle						parent;
		
		private Font							font;
		private double							fontSize;
		private RichColor						fontColor;
		private Color							anchorColor;
		private Color							waveColor;
		private ContentAlignment				alignment;
		private TextBreakMode					breakMode;
		private TextJustifMode					justifMode;
		private ThreeState						showLineBreaks;
		private ThreeState						showTabMarks;
		private string							language;
		private double							defaultTabWidth;
		private List<Tab>						tabs;
	}
}
