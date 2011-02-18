//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Text.RegularExpressions;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// La classe RegexConstraint implémente une contrainte sur des chaînes, au
	/// moyen d'une expression régulière (qui peut être définie de diverses
	/// manières).
	/// </summary>
	
	[System.ComponentModel.TypeConverter (typeof (RegexConstraint.Converter))]
	
	public class RegexConstraint : Types.IDataConstraint
	{
		public RegexConstraint(Regex regex)
		{
			this.regex   = regex;
			this.pattern = null;
			this.options = RegexFactory.Options.None;
			this.preReg = PredefinedRegex.None;
		}
		
		public RegexConstraint(string pattern, RegexFactory.Options options)
		{
			this.regex   = RegexFactory.FromSimpleJoker (pattern, options);
			this.pattern = pattern;
			this.options = options;
			this.preReg = PredefinedRegex.None;
		}
		
		public RegexConstraint(PredefinedRegex regex)
		{
			this.regex   = RegexFactory.FromPredefinedRegex (regex);
			this.pattern = null;
			this.options = RegexFactory.Options.None;
			this.preReg = regex;
		}
		
		
		public Regex							Regex
		{
			get
			{
				return this.regex;
			}
		}
		
		public string							Pattern
		{
			get
			{
				return this.pattern;
			}
		}
		
		public RegexFactory.Options				PatternOptions
		{
			get
			{
				return this.options;
			}
		}
		
		public PredefinedRegex					PredefinedRegex
		{
			get
			{
				return this.preReg;
			}
		}
		
		
		#region IDataConstraint Members
		public bool IsValidValue(object value)
		{
			string text;
			
			if ((this.regex != null) &&
				(Types.InvariantConverter.Convert (value, out text)))
			{
				return this.regex.IsMatch (text);
			}
			
			return false;
		}
		#endregion
		
		public override string ToString()
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			if (this.pattern != null)
			{
				buffer.Append ("pat:");
				buffer.Append (this.options.ToString ());
				buffer.Append (":");
				buffer.Append (this.pattern);
			}
			else if (this.preReg != PredefinedRegex.None)
			{
				buffer.Append ("def:");
				buffer.Append (this.preReg.ToString ());
			}
			else if (this.regex != null)
			{
				buffer.Append ("exp:");
				buffer.Append (this.regex.ToString ());
			}
			else
			{
				throw new System.InvalidOperationException ("Cannot convert empty RegexConstraint to string.");
			}
			
			return buffer.ToString ();
		}
		
		public static RegexConstraint FromString(string value)
		{
			if ((value.Length < 4) ||
				(value[3] != ':'))
			{
				return null;
			}
			
			switch (value.Substring (0, 3))
			{
				case "pat": return RegexConstraint.FromStringPattern (value.Substring (4));
				case "def": return RegexConstraint.FromStringPredefined (value.Substring (4));
				case "exp": return RegexConstraint.FromStringRegex (value.Substring (4));
			}
			
			return null;
		}
		
		
		protected static RegexConstraint FromStringPattern(string value)
		{
			int pos = value.IndexOf (":");
			
			string argOptions = value.Substring (0, pos);
			string argPattern = value.Substring (pos+1);
			
			System.Enum options;
			
			Types.InvariantConverter.Convert (argOptions, typeof (RegexFactory.Options), out options);
			
			return new RegexConstraint (argPattern, (RegexFactory.Options) options);
		}
		
		protected static RegexConstraint FromStringPredefined(string value)
		{
			System.Enum predefined;
			
			Types.InvariantConverter.Convert (value, typeof (PredefinedRegex), out predefined);
			
			return new RegexConstraint ((PredefinedRegex) predefined);
		}
		
		protected static RegexConstraint FromStringRegex(string value)
		{
			RegexOptions options = RegexOptions.Compiled | RegexOptions.ExplicitCapture;
			
			return new RegexConstraint (new Regex (value, options));
		}
		
		
		#region Converter Class
		public class Converter : Epsitec.Common.Types.AbstractStringConverter
		{
			public override object ParseString(string value, System.Globalization.CultureInfo culture)
			{
				return RegexConstraint.FromString (value);
			}
			
			public override string ToString(object value, System.Globalization.CultureInfo culture)
			{
				RegexConstraint regex = (RegexConstraint) value;
				
				return regex.ToString ();
			}
		}
		#endregion
		
		private Regex							regex;
		private string							pattern;
		private RegexFactory.Options			options;
		private PredefinedRegex					preReg;
	}
}
