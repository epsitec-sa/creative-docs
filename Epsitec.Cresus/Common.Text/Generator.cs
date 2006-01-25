//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	using GeneratorEnumerator = Internal.GeneratorEnumerator;
	
	/// <summary>
	/// La classe Generator gère les générateurs de texte automatique.
	/// </summary>
	public sealed class Generator
	{
		internal Generator(string name)
		{
			this.name = name;
		}
		
		
		public string							Name
		{
			get
			{
				return this.name;
			}
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
		
		public int[]							StartVector
		{
			get
			{
				return this.start_vector == null ? new int[0] : this.start_vector.Clone () as int[];
			}
			set
			{
				if (value == null)
				{
					this.start_vector = null;
				}
				else if (! Types.Comparer.Equal (this.start_vector, value))
				{
					this.start_vector = value.Clone () as int[];
				}
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
			return this.GenerateText (ranks, ranks.Length, culture);
		}
		
		public string GenerateText(int[] ranks, int max_level_count, System.Globalization.CultureInfo culture)
		{
			System.Diagnostics.Debug.Assert (max_level_count > 0);
			System.Diagnostics.Debug.Assert (max_level_count <= ranks.Length);
			
			if (this.sequences.Count == 0)
			{
				throw new System.InvalidOperationException ();
			}
			
			Sequence sequence = this.sequences[0] as Sequence;
			
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			if (sequence.WellKnownType == SequenceType.Constant)
			{
				sequence.GenerateText (max_level_count, culture, buffer);
			}
			else
			{
				for (int i = 0; i < max_level_count; i++)
				{
					int index = System.Math.Min (i, this.sequences.Count - 1);
					
					sequence = this.sequences[index] as Sequence;
					
					sequence.GenerateText (ranks[i], culture, buffer);
				}
			}
			return buffer.ToString ();
		}
		
		
		public Generator.Series NewSeries(System.Globalization.CultureInfo culture)
		{
			return new Series (this, culture);
		}
		
		
		public int UpdateAllFields(TextStory story, Properties.ManagedParagraphProperty property, System.Globalization.CultureInfo culture)
		{
			TextProcessor    processor = new TextProcessor (story);
			Generator.Series series    = this.NewSeries (culture);
			TextUpdater      updater   = new TextUpdater (story, this, property, series);
			
			processor.Process (new TextProcessor.Iterator (updater.Iterate));
			
			return updater.ChangeCount;
		}
		
		
		public void Serialize(System.Text.StringBuilder buffer)
		{
			Generator.Sequence[] sequences    = this.Sequences;
			int[]                start_vector = this.StartVector;
			
			buffer.Append (SerializerSupport.SerializeString (this.name));
			buffer.Append ("/");
			buffer.Append (SerializerSupport.SerializeInt (sequences.Length));
			buffer.Append ("/");
			buffer.Append (SerializerSupport.SerializeInt (start_vector.Length));
			
			foreach (Generator.Sequence sequence in sequences)
			{
				buffer.Append ("/");
				buffer.Append (SerializerSupport.SerializeString (Generator.SerializeToText (sequence)));
			}
			
			foreach (int start in start_vector)
			{
				buffer.Append ("/");
				buffer.Append (SerializerSupport.SerializeInt (start));
			}
		}
		
		public void Deserialize(TextContext context, int version, string[] args, ref int offset)
		{
			this.name = SerializerSupport.DeserializeString (args[offset++]);

			int num_sequences = SerializerSupport.DeserializeInt (args[offset++]);
			int num_starts    = SerializerSupport.DeserializeInt (args[offset++]);
			
			for (int i = 0; i < num_sequences; i++)
			{
				Generator.Sequence sequence;
				
				Generator.DeserializeFromText (context, SerializerSupport.DeserializeString (args[offset++]), out sequence);
				
				this.Add (sequence);
			}
			
			this.start_vector = new int[num_starts];
			
			for (int i = 0; i < num_starts; i++)
			{
				this.start_vector[i] = SerializerSupport.DeserializeInt (args[offset++]);
			}
		}
		
		
		#region TextUpdater Class
		class TextUpdater
		{
			public TextUpdater(TextStory story, Generator generator, Properties.ManagedParagraphProperty property, Generator.Series series)
			{
				this.story      = story;
				this.context    = story.TextContext;
				this.text       = story.TextTable;
				this.generator  = generator;
				this.mpp        = property;
				this.series     = series;
				this.enumerator = new GeneratorEnumerator (story, this.mpp, this.generator.Name);
			}
			
			
			public int							ChangeCount
			{
				get
				{
					return this.count;
				}
			}
			
			public void Iterate(out TextProcessor.Status status)
			{
				if (this.enumerator.MoveNext ())
				{
					Properties.GeneratorProperty   generator_property;
					Properties.MarginsProperty     margins_property = null;
					Properties.ManagedInfoProperty m_info_property  = null;
					
					TextContext        context = this.story.TextContext;
					Cursors.TempCursor cursor  = this.enumerator.Cursor;
					Internal.CursorId  id      = cursor.CursorId;
					int                pos     = this.text.GetCursorPosition (id);
					
					if (pos < this.story.TextLength)
					{
						ulong code = this.text.ReadChar (id);
						
						//	Retrouve la propriété associée à notre générateur, ce
						//	qui permet ensuite de déterminer la longueur du texte
						//	à remplacer :
						
						generator_property = this.enumerator.GetGeneratorProperty (code);
						
						context.GetMargins (code, out margins_property);
						
						if (this.mpp != null)
						{
							context.GetManagedInfo (code, this.mpp.ManagerName, out m_info_property);
						}
						
						if ((m_info_property != null) &&
							(m_info_property.ManagerInfo != "auto"))
						{
							//	Mode spécifique :
							//
							//	- "cont" -----> continue indépendamment du contexte
							//	- "set ..." --> reprend avec le numéro spécifié
							
							string mode = m_info_property.ManagerInfo;
							
							if (mode == "cont")
							{
								//	Continue normalement la séquence, indépendamment
								//	des recommendations de l'énumérateur.
							}
							else if (mode.StartsWith ("set "))
							{
								//	TODO: réinitialiser le vecteur de départ
							}
							else
							{
								throw new System.NotSupportedException (string.Format ("ManagerInfo '{0}' not supported", mode));
							}
						}
						else
						{
							//	Mode automatique : recommence la numérotation comme
							//	recommandé par l'énumérateur...
							
							if (this.enumerator.RestartGenerator)
							{
								this.series.Restart ();
							}
						}
						
						System.Diagnostics.Debug.Assert (generator_property != null);
						System.Diagnostics.Debug.Assert (generator_property.Generator == this.generator.Name);
						
						int    length = this.context.GetTextEndDistance (this.story, cursor, generator_property);
						int    level  = System.Math.Max (generator_property.Level, margins_property == null ? 0 : margins_property.Level);
						
						string text = this.series.GetNextText (level);
						
						System.Diagnostics.Debug.Assert (length > 0);
						System.Diagnostics.Debug.Assert (text.Length > 0);
						
						//	Compte combien de textes ont été modifiés pendant cette
						//	opération :
						
						if (this.story.ReplaceText (cursor, length, text))
						{
							this.count++;
						}
						
						status = TextProcessor.Status.Continue;
						return;
					}
				}
				
				status = TextProcessor.Status.Abort;
			}
			
			
			private TextStory					story;
			private TextContext					context;
			private Internal.TextTable			text;
			private Generator					generator;
			Properties.ManagedParagraphProperty mpp;
			private Generator.Series			series;
			private GeneratorEnumerator			enumerator;
			private int							count;
		}
		#endregion
		
		#region Series Class
		public class Series
		{
			public Series(Generator generator, System.Globalization.CultureInfo culture)
			{
				this.generator = generator;
				this.vector    = generator.StartVector;	//	clone modifiable
				this.level     = -1;
				this.culture   = culture;
			}
			
			
			public void Restart()
			{
				this.vector = this.generator.StartVector;
				this.level  = -1;
			}
			
			public string GetNextText(int level)
			{
				this.GrowToLevel (level);
				
				if (this.level == -1)
				{
					//	Première génération de texte. Utilise le vecteur de
					//	départ.
				}
				else if (this.level < level)
				{
					//	On doit générer plus de niveaux que précédemment; il
					//	faut donc mettre à zéro les nouveaux niveaux :
					
					for (int i = this.level + 1; i <= level; i++)
					{
						this.vector[i] = 1;
					}
				}
				else if (this.level > level)
				{
					//	On repasse au niveau supérieur. Il faut incrémenter le
					//	dernier numéro actif :
					
					this.vector[level]++;
				}
				else
				{
					//	On reste au même niveau; il faut aussi incrémenter le
					//	dernier numéro actif :
					
					this.vector[level]++;
				}
				
				string text = this.generator.GenerateText (this.vector, level + 1, this.culture);
				
				this.level = level;
				
				return text;
			}
			
			
			private void GrowToLevel(int level)
			{
				if (level >= this.vector.Length)
				{
					int   size = level + 1;
					int[] copy = new int[size];
					
					for (int i = 0; i < this.vector.Length; i++)
					{
						copy[i] = this.vector[i];
					}
					for (int i = this.vector.Length; i < size; i++)
					{
						copy[i] = 1;
					}
					
					this.vector = copy;
				}
			}
			
			
			private Generator					generator;
			private int							level;
			private int[]						vector;
			System.Globalization.CultureInfo	culture;
		}
		#endregion
		
		#region Sequence Class
		public abstract class Sequence : ISerializableAsText
		{
			protected Sequence()
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
			
			
			public void DefineSetupArgument(string argument)
			{
				this.Setup (argument);
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
			
			public void DeserializeFromText(TextContext context, string text, int pos, int length)
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
				
				case SequenceType.Constant:
					return new Internal.Sequences.Constant ();
				
				default:
					throw new System.NotSupportedException (string.Format ("SequenceType {0} not supported", type));
			}
		}
		
		public static Generator.Sequence CreateSequence(SequenceType type, string prefix, string suffix)
		{
			Generator.Sequence sequence = Generator.CreateSequence (type);
			
			sequence.Prefix = prefix;
			sequence.Suffix = suffix;
			
			return sequence;
		}
		
		public static Generator.Sequence CreateSequence(SequenceType type, string prefix, string suffix, Casing casing)
		{
			Generator.Sequence sequence = Generator.CreateSequence (type);
			
			sequence.Prefix = prefix;
			sequence.Suffix = suffix;
			sequence.Casing = casing;
			
			return sequence;
		}
		
		public static Generator.Sequence CreateSequence(SequenceType type, string prefix, string suffix, Casing casing, string setup_argument)
		{
			Generator.Sequence sequence = Generator.CreateSequence (type);
			
			sequence.DefineSetupArgument (setup_argument);
			sequence.Prefix = prefix;
			sequence.Suffix = suffix;
			sequence.Casing = casing;
			
			return sequence;
		}
		
		
		public static string SerializeToText(Sequence sequence)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			Generator.SerializeToText (buffer, sequence);
			return buffer.ToString ();
		}
		
		public static void SerializeToText(System.Text.StringBuilder buffer, Sequence sequence)
		{
			System.Diagnostics.Debug.Assert (sequence != null);
			
			string sequ_name = sequence.GetType ().Name;
			
			System.Diagnostics.Debug.Assert (sequence.WellKnownType.ToString () == sequ_name);
			
			buffer.Append ("{");
			buffer.Append (sequ_name);
			buffer.Append (":");
			
			sequence.SerializeToText (buffer);
			
			buffer.Append ("}");
		}
		
		public static void DeserializeFromText(TextContext context, string text, out Sequence sequence)
		{
			Generator.DeserializeFromText (context, text, 0, text.Length, out sequence);
		}
		
		public static void DeserializeFromText(TextContext context, string text, int pos, int length, out Sequence sequence)
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
		
		
		#region Casing Enumeration
		public enum Casing
		{
			Default,
			Lower,
			Upper
		}
		#endregion
		
		#region SequenceType Enumeration
		public enum SequenceType
		{
			None,
			
			Alphabetic,
			Numeric,
			Constant
		}
		#endregion
		
		private System.Collections.ArrayList	sequences = new System.Collections.ArrayList ();
		private string							name;
		private int[]							start_vector;
	}
}
