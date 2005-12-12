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
		
		public FontProperty(string face, string style, params string[] features)
		{
			if (features == null)
			{
				features = new string[0];
			}
			
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
				return PropertyType.CoreSetting;
			}
		}
		
		
		public string							FaceName
		{
			get
			{
				return this.face_name;
			}
		}
		
		public string							StyleName
		{
			get
			{
				return this.style_name;
			}
		}
		
		public string[]							Features
		{
			get
			{
				return this.features;
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
		
		public override void DeserializeFromText(TextContext context, string text, int pos, int length)
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
			
			//	Combine les noms des styles de manière avancée dès que la propriété
			//	contient des styles à ajouter/supprimer/inverser, avec une syntaxe
			//	du type "(+Bold)", "(-Bold)" ou "(!Bold)".
			
			if ((a.StyleName != null) &&
				(b.StyleName != null) &&
				(b.StyleName.Length > 0))
			{
				if (b.StyleName.IndexOfAny (new char[] { '+', '-', '!' }) != -1)
				{
					style_name = FontProperty.CombineStyles (a.StyleName, b.StyleName);
				}
			}
			
			System.Diagnostics.Debug.WriteLine (string.Format ("Combined '{0}' with '{1}' --> '{2}'", a.StyleName, b.StyleName, style_name));
			
			System.Collections.ArrayList features = new System.Collections.ArrayList ();
			
			if ((a.Features != null) &&
				(a.Features.Length > 0))
			{
				foreach (string feature in a.Features)
				{
					if (features.Contains (feature) == false)
					{
						features.Add (feature);
					}
				}
			}
			
			if ((b.Features != null) &&
				(b.Features.Length > 0))
			{
				foreach (string feature in b.Features)
				{
					if (features.Contains (feature) == false)
					{
						features.Add (feature);
					}
				}
			}
			
			FontProperty c = new FontProperty (face_name, style_name, (string[]) features.ToArray (typeof (string)));
			
			return c;
		}
		
		
		public static string CombineStyles(string a, string b)
		{
			//	Combine deux séries de noms de styles, en simplifiant d'éventuelles
			//	modifications "+Bold" et "-Bold" qui s'annuleraient.
			
			int  count_bold    = 0;
			int  count_italic  = 0;
			bool invert_bold   = false;
			bool invert_italic = false;
			
			System.Collections.ArrayList list   = new System.Collections.ArrayList ();
			System.Collections.ArrayList result = new System.Collections.ArrayList ();
			
			list.AddRange (a.Split (' '));
			list.AddRange (b.Split (' '));
			
			foreach (string element in list)
			{
				if (element.Length == 0)
				{
					continue;
				}
				
				switch (element)
				{
					case "Bold":	count_bold  = 1;	break;
					case "+Bold":	count_bold += 1;	break;
					case "-Bold":	count_bold -= 1;	break;
					
					case "Italic":	count_italic  = 1;	break;
					case "+Italic":	count_italic += 1;	break;
					case "-Italic":	count_italic -= 1;	break;
					
					case "!Bold":	invert_bold   = !invert_bold;	break;
					case "!Italic":	invert_italic = !invert_italic;	break;
					
					case "Regular":
					case "Normal":
					case "Roman":
						count_bold   = 0; invert_bold   = false;
						count_italic = 0; invert_italic = false;
						
						result.Add (element);
						
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
			
			while (count_bold-- > 0) result.Add ("+Bold");
			while (++count_bold < 0) result.Add ("-Bold");
			
			if (invert_bold) result.Add ("!Bold");
			
			//	Résume l'état des changements d'italique :
			
			while (count_italic-- > 0) result.Add ("+Italic");
			while (++count_italic < 0) result.Add ("-Italic");
			
			if (invert_italic) result.Add ("!Italic");
			
			return string.Join (" ", (string[]) result.ToArray (typeof (string)));
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
