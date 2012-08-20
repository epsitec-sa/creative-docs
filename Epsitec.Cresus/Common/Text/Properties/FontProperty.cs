//	Copyright © 2005-2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

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
			this.faceName  = face;
			this.styleName = style;
		}
		
		public FontProperty(string face, string style, params string[] features)
		{
			if (features == null)
			{
				features = Epsitec.Common.Types.Collections.EmptyArray<string>.Instance;
			}
			
			this.faceName   = face;
			this.styleName  = style;
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
				return this.faceName;
			}
		}
		
		public string							StyleName
		{
			get
			{
				return this.styleName;
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
				/**/				SerializerSupport.SerializeString (this.faceName),
				/**/				SerializerSupport.SerializeString (this.styleName),
				/**/				SerializerSupport.SerializeStringArray (this.features));
		}
		
		public override void DeserializeFromText(TextContext context, string text, int pos, int length)
		{
			string[] args = SerializerSupport.Split (text, pos, length);
			
			Debug.Assert.IsTrue (args.Length == 3);
			
			this.faceName  = SerializerSupport.DeserializeString (args[0]);
			this.styleName = SerializerSupport.DeserializeString (args[1]);
			this.features   = SerializerSupport.DeserializeStringArray (args[2]);
		}
		
		public override Property GetCombination(Property property)
		{
			Debug.Assert.IsTrue (property is Properties.FontProperty);
			
			FontProperty a = this;
			FontProperty b = property as FontProperty;
			
			string faceName  = ((b.FaceName == null)  || (b.FaceName.Length == 0))  ? a.FaceName  : b.FaceName;
			string styleName = ((b.StyleName == null) || (b.StyleName.Length == 0)) ? a.StyleName : b.StyleName;
			
			//	Combine les noms des styles de manière avancée dès que la propriété
			//	contient des styles à ajouter/supprimer/inverser, avec une syntaxe
			//	du type "(+Bold)", "(-Bold)" ou "(!Bold)".
			
			if ((a.StyleName != null) &&
				(b.StyleName != null) &&
				(b.StyleName.Length > 0))
			{
				if (b.StyleName.IndexOfAny (new char[] { '+', '-', '!' }) != -1)
				{
					styleName = FontProperty.CombineStyles (a.StyleName, b.StyleName);
				}
			}
			
//-			System.Diagnostics.Debug.WriteLine (string.Format ("Combined '{0}' with '{1}' --> '{2}'", a.StyleName, b.StyleName, styleName));
			
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
			
			FontProperty c = new FontProperty (faceName, styleName, (string[]) features.ToArray (typeof (string)));
			
			return c;
		}
		
		
		public static string CombineStyles(string a, string b)
		{
			//	Combine deux séries de noms de styles, en simplifiant d'éventuelles
			//	modifications "+Bold" et "-Bold" qui s'annuleraient.
			
			int  countBold    = 0;
			int  countItalic  = 0;
			bool invertBold   = false;
			bool invertItalic = false;
			
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
					case "Bold":	countBold  = 1;	break;
					case "+Bold":	countBold += 1;	break;
					case "-Bold":	countBold -= 1;	break;
					
					case "Italic":	countItalic  = 1;	break;
					case "+Italic":	countItalic += 1;	break;
					case "-Italic":	countItalic -= 1;	break;
					
					case "!Bold":	invertBold   = !invertBold;	break;
					case "!Italic":	invertItalic = !invertItalic;	break;
					
					case "Regular":
					case "Normal":
					case "Roman":
						countBold   = 0; invertBold   = false;
						countItalic = 0; invertItalic = false;
						
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
			
			while (countBold-- > 0) result.Add ("+Bold");
			while (++countBold < 0) result.Add ("-Bold");
			
			if (invertBold) result.Add ("!Bold");
			
			//	Résume l'état des changements d'italique :
			
			while (countItalic-- > 0) result.Add ("+Italic");
			while (++countItalic < 0) result.Add ("-Italic");
			
			if (invertItalic) result.Add ("!Italic");
			
			return string.Join (" ", (string[]) result.ToArray (typeof (string)));
		}
		
		
		public override void UpdateContentsSignature(IO.IChecksum checksum)
		{
			checksum.UpdateValue (this.faceName);
			checksum.UpdateValue (this.styleName);
			checksum.UpdateValue (this.features);
		}
		
		public override bool CompareEqualContents(object value)
		{
			return FontProperty.CompareEqualContents (this, value as FontProperty);
		}
		
		
		private static bool CompareEqualContents(FontProperty a, FontProperty b)
		{
			return a.faceName == b.faceName
				&& a.styleName == b.styleName
				&& Types.Comparer.Equal (a.features, b.features);
		}
		
		
		private string							faceName;
		private string							styleName;
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
