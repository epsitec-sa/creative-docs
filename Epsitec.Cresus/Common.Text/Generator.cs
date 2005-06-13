//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe Generator gère les générateurs de texte automatique.
	/// </summary>
	public class Generator
	{
		public Generator()
		{
		}
		
		
		public Generator.Sequence[]				Sequences
		{
			get
			{
				Sequence[] value = new Sequence[this.sequences.Count];
				this.sequences.CopyTo (value);
				return value;
			}
		}
		
		
		public void Add(Generator.Sequence sequence)
		{
			this.sequences.Add (sequence);
		}
		
		public void AddRange(System.Collections.ICollection sequences)
		{
			this.sequences.AddRange (sequences);
		}
		
		
		public string GenerateText(int[] ranks, System.Globalization.CultureInfo culture)
		{
			if (this.sequences.Count == 0)
			{
				throw new System.InvalidOperationException ();
			}
			
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			for (int i = 0; i < ranks.Length; i++)
			{
				int      index    = System.Math.Min (i, this.sequences.Count - 1);
				Sequence sequence = this.sequences[index] as Sequence;
				
				sequence.GenerateText (ranks[i], culture, buffer);
			}
			
			return buffer.ToString ();
		}
		
		
		#region Sequence Class
		public abstract class Sequence : ISerializableAsText
		{
			public Sequence()
			{
			}
			
			
			public string						Prefix
			{
				get
				{
					return this.prefix;
				}
				set
				{
					this.prefix = value;
				}
			}
			
			public string						Suffix
			{
				get
				{
					return this.suffix;
				}
				set
				{
					this.suffix = value;
				}
			}
			
			public Casing						Casing
			{
				get
				{
					return this.casing;
				}
				set
				{
					this.casing = value;
				}
			}
			
			public abstract SequenceType		WellKnownType
			{
				get;
			}
			
			
			public void GenerateText(int rank, System.Globalization.CultureInfo culture, System.Text.StringBuilder buffer)
			{
				if (this.prefix != null)
				{
					buffer.Append (this.prefix);
				}
				
				string text = this.GetRawText (rank, culture);
				
				switch (this.casing)
				{
					case Casing.Default: buffer.Append (text);					 break;
					case Casing.Lower:	 buffer.Append (text.ToLower (culture)); break;
					case Casing.Upper:	 buffer.Append (text.ToUpper (culture)); break;
				}
				
				if (this.suffix != null)
				{
					buffer.Append (this.suffix);
				}
			}
			
			
			#region ISerializableAsText Members
			public void SerializeToText(System.Text.StringBuilder buffer)
			{
				SerializerSupport.Join (buffer,
					/**/				SerializerSupport.SerializeString (this.prefix),
					/**/				SerializerSupport.SerializeString (this.suffix),
					/**/				SerializerSupport.SerializeInt ((int) this.casing),
					/**/				SerializerSupport.SerializeString (this.GetSetupArgument ()));
			}
			
			public void DeserializeFromText(Context context, string text, int pos, int length)
			{
				string[] args = SerializerSupport.Split (text, pos, length);
				
				System.Diagnostics.Debug.Assert (args.Length == 4);
				
				string prefix = SerializerSupport.DeserializeString (args[0]);
				string suffix = SerializerSupport.DeserializeString (args[1]);
				int    casing = SerializerSupport.DeserializeInt (args[2]);
				string setup  = SerializerSupport.DeserializeString (args[3]);
				
				this.prefix = prefix;
				this.suffix = suffix;
				this.casing = (Casing) casing;
				
				this.Setup (setup);
			}
			#endregion
			
			protected abstract string GetRawText(int rank, System.Globalization.CultureInfo culture);
			protected virtual string GetSetupArgument()
			{
				return null;
			}
			
			protected virtual void Setup(string argument)
			{
			}
			
			
			private string						prefix;
			private string						suffix;
			private Casing						casing;
		}
		#endregion
		
		public static Generator.Sequence CreateSequence(SequenceType type)
		{
			switch (type)
			{
				case SequenceType.Alphabetic:
					return new Internal.Sequences.Alphabetic ();
				
				case SequenceType.Numeric:
					return new Internal.Sequences.Numeric ();
				
				default:
					throw new System.NotSupportedException (string.Format ("SequenceType {0} not supported", type));
			}
		}
		
		public static void SerializeToText(System.Text.StringBuilder buffer, Sequence sequence)
		{
			System.Diagnostics.Debug.Assert (sequence != null);
			
			string type_name = sequence.GetType ().Name;
			string sequ_name = type_name.Substring (0, type_name.Length - 8);	//	"XxxSequence" --> "Xxx"
			
			System.Diagnostics.Debug.Assert (sequence.WellKnownType.ToString () == sequ_name);
			System.Diagnostics.Debug.Assert (type_name.Substring (sequ_name.Length) == "Sequence");
			
			buffer.Append ("{");
			buffer.Append (sequ_name);
			buffer.Append (":");
			
			sequence.SerializeToText (buffer);
			
			buffer.Append ("}");
		}
		
		public static void DeserializeFromText(Context context, string text, int pos, int length, out Sequence sequence)
		{
			System.Diagnostics.Debug.Assert (text[pos+0] == '{');
			System.Diagnostics.Debug.Assert (text[pos+length-1] == '}');
			
			int sep_pos = text.IndexOf (':', pos, length);
			int end_pos = pos + length;
			
			System.Diagnostics.Debug.Assert (sep_pos > pos);
			
			string sequ_name = text.Substring (pos+1, sep_pos - pos - 1);
			string type_name = string.Concat ("Epsitec.Common.Text.Internals.Sequences.", sequ_name, "Sequence");
			
			sequence = Generator.CreateSequence ((SequenceType) System.Enum.Parse (typeof (SequenceType), sequ_name));
			
			sep_pos++;
			
			sequence.DeserializeFromText (context, text, sep_pos, end_pos - sep_pos - 1);
		}
		
		
		public enum Casing
		{
			Default,
			Lower,
			Upper
		}
		
		public enum SequenceType
		{
			None,
			
			Alphabetic,
			Numeric
		}
		
		
		private System.Collections.ArrayList	sequences = new System.Collections.ArrayList ();
	}
}
