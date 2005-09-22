//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// La classe FontProperty décrit une fonte (famille + style).
	/// </summary>
	public class FontProperty : Property
	{
		public FontProperty()
		{
		}
		
		public FontProperty(string face, string style)
		{
			this.face_name  = face;
			this.style_name = style;
		}
		
		public FontProperty(string face, string style, string[] features)
		{
			this.face_name  = face;
			this.style_name = style;
			this.features   = features.Clone () as string[];
		}
		
		
		public override WellKnownType			WellKnownType
		{
			get
			{
				return WellKnownType.Font;
			}
		}
		
		public override PropertyType			PropertyType
		{
			get
			{
				return PropertyType.Style;
			}
		}
		
		
		public string							FaceName
		{
			get
			{
				return this.face_name;
			}
			set
			{
				if (this.face_name != value)
				{
					this.face_name = value;
					this.Invalidate ();
				}
			}
		}
		
		public string							StyleName
		{
			get
			{
				return this.style_name;
			}
			set
			{
				if (this.style_name != value)
				{
					this.style_name = value;
					this.Invalidate ();
				}
			}
		}
		
		public string[]							Features
		{
			get
			{
				return this.features;
			}
			set
			{
				if (! Types.Comparer.Equal (this.features, value))
				{
					this.features = value.Clone () as string[];
					this.Invalidate ();
				}
			}
		}
		
		
		public override Property EmptyClone()
		{
			return new FontProperty ();
		}
		
		public override void SerializeToText(System.Text.StringBuilder buffer)
		{
			SerializerSupport.Join (buffer,
				/**/				SerializerSupport.SerializeString (this.face_name),
				/**/				SerializerSupport.SerializeString (this.style_name),
				/**/				SerializerSupport.SerializeStringArray (this.features));
		}
		
		public override void DeserializeFromText(Context context, string text, int pos, int length)
		{
			string[] args = SerializerSupport.Split (text, pos, length);
			
			Debug.Assert.IsTrue (args.Length == 3);
			
			this.face_name  = SerializerSupport.DeserializeString (args[0]);
			this.style_name = SerializerSupport.DeserializeString (args[1]);
			this.features   = SerializerSupport.DeserializeStringArray (args[2]);
		}
		
		public override Property GetCombination(Property property)
		{
			Debug.Assert.IsTrue (property is Properties.FontProperty);
			
			FontProperty a = this;
			FontProperty b = property as FontProperty;
			
			string face_name  = ((b.FaceName == null)  || (b.FaceName.Length == 0))  ? a.FaceName  : b.FaceName;
			string style_name = ((b.StyleName == null) || (b.StyleName.Length == 0)) ? a.StyleName : b.StyleName;
			
			if ((a.StyleName != null) &&
				(b.StyleName != null) &&
				(b.StyleName.Length > 0))
			{
				if (b.StyleName.IndexOf ("(") != -1)
				{
					style_name = FontProperty.CombineStyles (a.StyleName, b.StyleName);
				}
			}
			
			System.Diagnostics.Debug.WriteLine (string.Format ("Combined <{0}> with <{1}> --> <{2}>", a.StyleName, b.StyleName, style_name));
			
			FontProperty c = new FontProperty (face_name, style_name);
			
			//	TODO: gérer 'features'
			
			c.DefineVersion (System.Math.Max (a.Version, b.Version));
			
			return c;
		}
		
		
		public static string CombineStyles(string a, string b)
		{
			int  count_bold    = 0;
			int  count_italic  = 0;
			bool invert_bold   = false;
			bool invert_italic = false;
			
			System.Collections.ArrayList list   = new System.Collections.ArrayList ();
			System.Collections.ArrayList result = new System.Collections.ArrayList ();
			
			FontProperty.SplitStyle (a, list);
			FontProperty.SplitStyle (b, list);
			
			foreach (string element in list)
			{
				if (element.Length == 0)
				{
					continue;
				}
				
				switch (element)
				{
					case "Regular":
					case "Normal":
					case "Roman":
						count_bold    = 0;
						count_italic  = 0;
						invert_bold   = false;
						invert_italic = false;
						result.Add (element);
						break;
					
					case "Bold":
					case "+Bold":
						count_bold++;
						break;
					case "-Bold":
						count_bold--;
						break;
					case "!Bold":
						invert_bold = !invert_bold;
						break;
					
					case "Italic":
					case "+Italic":
						count_italic++;
						break;
					case "-Italic":
						count_italic--;
						break;
					case "!Italic":
						invert_italic = !invert_italic;
						break;
					
					default:
						if (result.Contains (element) == false)
						{
							result.Add (element);
						}
						break;
				}
			}
			
			//	Résume l'état des changements de graisse :
			
			while (count_bold > 0)
			{
				result.Add ("(+Bold)");
				count_bold--;
			}
			while (count_bold < 0)
			{
				result.Add ("(-Bold)");
				count_bold++;
			}
			if (invert_bold)
			{
				result.Add ("(!Bold)");
			}
			
			//	Résume l'état des changements d'italique :
			
			while (count_italic > 0)
			{
				result.Add ("(+Italic)");
				count_italic--;
			}
			while (count_italic < 0)
			{
				result.Add ("(-Italic)");
				count_italic++;
			}
			if (invert_italic)
			{
				result.Add ("(!Italic)");
			}
			
			string[] elements = (string[]) result.ToArray (typeof (string));
			
			return string.Join (" ", elements);
		}
		
		public static void SplitStyle(string style, System.Collections.ArrayList list)
		{
			int pos = style.IndexOf ('(');
			int end = style.IndexOf (')');
			
			if ((pos == -1) &&
				(end == -1))
			{
				list.Add (style.Trim ());
				return;
			}
			
			while (style.Length > 0)
			{
				System.Diagnostics.Debug.Assert (pos > -1);
				System.Diagnostics.Debug.Assert (end > -1);
				
				if (pos > 0)
				{
					list.Add (style.Substring (0, pos).Trim ());
					
					style = style.Substring (pos);
					
					end -= pos;
					pos  = 0;
				}
				
				if (end > 1)
				{
					list.Add (style.Substring (1, end-1).Trim ());
				}
				
				style = style.Substring (end+1).Trim ();
				
				pos = style.IndexOf ('(');
				end = style.IndexOf (')');
				
				if ((pos == -1) &&
					(end == -1))
				{
					break;
				}
			}
			
			if (style.Length > 0)
			{
				list.Add (style.Trim ());
			}
		}
		
		
		public override void UpdateContentsSignature(IO.IChecksum checksum)
		{
			checksum.UpdateValue (this.face_name);
			checksum.UpdateValue (this.style_name);
			checksum.UpdateValue (this.features);
		}
		
		public override bool CompareEqualContents(object value)
		{
			return FontProperty.CompareEqualContents (this, value as FontProperty);
		}
		
		
		private static bool CompareEqualContents(FontProperty a, FontProperty b)
		{
			return a.face_name == b.face_name
				&& a.style_name == b.style_name
				&& Types.Comparer.Equal (a.features, b.features);
		}
		
		
		private string							face_name;
		private string							style_name;
		private string[]						features;
	}
	
	public enum FontStyle : byte
	{
		Normal,
		Italic,
		Oblique,
		
		Other,
	}
	
	public enum FontWeight : byte
	{
		Normal,
		Light,
		Bold,
		
		Other,
	}
}
