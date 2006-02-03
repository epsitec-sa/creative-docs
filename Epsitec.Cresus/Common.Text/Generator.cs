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
		
		public string							GlobalPrefix
		{
			get
			{
				return this.global_prefix;
			}
			set
			{
				this.global_prefix = value;
			}
		}
		
		public Property[]						GlobalPrefixProperties
		{
			get
			{
				if ((this.global_prefix_properties == null) ||
					(this.global_prefix_properties.Length == 0))
				{
					return null;
				}
				else
				{
					return (Property[]) this.global_prefix_properties.Clone ();
				}
			}
			set
			{
				if (this.global_prefix_properties != value)
				{
					if ((value == null) ||
						(value.Length == 0))
					{
						this.global_prefix_properties = value;
					}
					else
					{
						this.global_prefix_properties = (Property[]) value.Clone ();
					}
				}
			}
		}
		
		public string							GlobalSuffix
		{
			get
			{
				return this.global_suffix;
			}
			set
			{
				this.global_suffix = value;
			}
		}
		
		public Property[]						GlobalSuffixProperties
		{
			get
			{
				if ((this.global_suffix_properties == null) ||
					(this.global_suffix_properties.Length == 0))
				{
					return null;
				}
				else
				{
					return (Property[]) this.global_suffix_properties.Clone ();
				}
			}
			set
			{
				if (this.global_suffix_properties != value)
				{
					if ((value == null) ||
						(value.Length == 0))
					{
						this.global_suffix_properties = value;
					}
					else
					{
						this.global_suffix_properties = (Property[]) value.Clone ();
					}
				}
			}
		}
		
		public string[]							UserData
		{
			get
			{
				return this.user_data;
			}
			set
			{
				this.user_data = value;
			}
		}
		
		public int								UserCount
		{
			get
			{
				return this.user_count;
			}
		}
		
		
		public Generator.Sequence				this[int index]
		{
			get
			{
				return this.sequences[index] as Sequence;
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
		
		
		public string GenerateTextString(int[] ranks, System.Globalization.CultureInfo culture)
		{
			TextRange[] ranges = this.GenerateText (ranks, culture);
			
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			foreach (TextRange range in ranges)
			{
				buffer.Append (range.Text);
			}
			
			return buffer.ToString ();
		}
		
		public TextRange[] GenerateText(int[] ranks, System.Globalization.CultureInfo culture)
		{
			return this.GenerateText (ranks, ranks.Length, culture);
		}
		
		public TextRange[] GenerateText(int[] ranks, int max_level_count, System.Globalization.CultureInfo culture)
		{
			System.Collections.ArrayList text_range = new System.Collections.ArrayList ();
			
			System.Diagnostics.Debug.Assert (max_level_count > 0);
			System.Diagnostics.Debug.Assert (max_level_count <= ranks.Length);
			
			if (this.sequences.Count == 0)
			{
				throw new System.InvalidOperationException ();
			}
			
			Sequence sequence = this.sequences[0] as Sequence;
			
			for (int i = 0; i < max_level_count; i++)
			{
				int index = System.Math.Min (i, this.sequences.Count - 1);
				
				sequence = this.sequences[index] as Sequence;
				
				if (sequence.SuppressBefore)
				{
					text_range.Clear ();
				}
				
				if ((sequence.Prefix != null) &&
					(sequence.Prefix.Length > 0))
				{
					text_range.Add (new TextRange (sequence.Prefix, sequence.PrefixProperties));
				}
				
				string text = sequence.GenerateText (ranks[i], culture);
				
				if (text.Length > 0)
				{
					text_range.Add (new TextRange (text, sequence.ValueProperties));
				}
				
				if ((sequence.Suffix != null) &&
					(sequence.Suffix.Length > 0))
				{
					text_range.Add (new TextRange (sequence.Suffix, sequence.SuffixProperties));
				}
			}
			
			if (this.global_prefix != null)
			{
				text_range.Insert (0, new TextRange (this.global_prefix, this.global_prefix_properties));
			}
			
			if (this.global_suffix != null)
			{
				text_range.Add (new TextRange (this.global_suffix, this.global_suffix_properties));
			}
			
			return TextRange.Simplify (text_range);
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
			
			buffer.Append ("/");
			buffer.Append (SerializerSupport.SerializeString (this.global_prefix));
			buffer.Append ("/");
			buffer.Append (SerializerSupport.SerializeString (this.global_suffix));
			buffer.Append ("/");
			buffer.Append (SerializerSupport.SerializeString (Property.SerializeProperties (this.global_prefix_properties)));
			buffer.Append ("/");
			buffer.Append (SerializerSupport.SerializeString (Property.SerializeProperties (this.global_suffix_properties)));
			buffer.Append ("/");
			buffer.Append (SerializerSupport.SerializeStringArray (this.user_data));
		}
		
		public void Deserialize(TextContext context, int version, string[] args, ref int offset)
		{
			this.sequences.Clear ();
			
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
			
			if (version >= 4)
			{
				this.global_prefix = SerializerSupport.DeserializeString (args[offset++]);
				this.global_suffix = SerializerSupport.DeserializeString (args[offset++]);
			}
			
			if (version >= 5)
			{
				this.global_prefix_properties = Property.DeserializeProperties (context, SerializerSupport.DeserializeString (args[offset++]));
				this.global_suffix_properties = Property.DeserializeProperties (context, SerializerSupport.DeserializeString (args[offset++]));
				
				this.user_data = SerializerSupport.DeserializeStringArray (args[offset++]);
			}
		}
		
		
		internal string Save()
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			this.Serialize (buffer);
			return buffer.ToString ();
		}
		
		internal void Restore(TextContext context, string state)
		{
			string[] args = state.Split ('/');
			
			int version = TextContext.SerializationVersion;
			int offset  = 0;
			
			this.Deserialize (context, version, args, ref offset);
		}
		
		internal void DefineName(string name)
		{
			this.name = name;
		}
		
		
		internal void IncrementUserCount()
		{
			this.user_count++;
			System.Diagnostics.Debug.Assert (this.user_count > 0);
		}
		
		internal void DecrementUserCount()
		{
			System.Diagnostics.Debug.Assert (this.user_count > 0);
			this.user_count--;
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
					Properties.AutoTextProperty    auto_text_property;
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
						
						context.GetAutoText (code, out auto_text_property);
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
							//	- "reset" ----> reprend au début
							//	- "set ..." --> reprend avec le numéro spécifié
							
							string mode = m_info_property.ManagerInfo;
							
							if (mode == "cont")
							{
								//	Continue normalement la séquence, indépendamment
								//	des recommendations de l'énumérateur.
							}
							else if (mode == "reset")
							{
								this.series.Restart ();
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
						
						TextRange[] text = this.series.GetNextText (level);
						
						System.Diagnostics.Debug.Assert (length > 0);
						System.Diagnostics.Debug.Assert (text.Length > 0);
						System.Diagnostics.Debug.Assert (text[0].Text.Length > 0);
						
						//	Compte combien de textes ont été modifiés pendant cette
						//	opération :
						
						if (this.story.ReplaceTextSequence (cursor, length, auto_text_property, generator_property, text))
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
		
		#region TextRange Class
		public class TextRange
		{
			public TextRange(string text)
			{
				this.text = text;
			}
			
			public TextRange(string text, System.Collections.ICollection properties)
			{
				this.text       = text;
				this.properties = ((properties == null) || (properties.Count == 0)) ? null : new Property[properties.Count];
				
				if (this.properties != null)
				{
					properties.CopyTo (this.properties, 0);
				}
			}
			
			public TextRange(string text, params Property[] properties)
			{
				this.text       = text;
				this.properties = properties;
			}
			
			
			public string						Text
			{
				get
				{
					return this.text;
				}
			}
			
			public Property[]					Properties
			{
				get
				{
					return this.properties;
				}
			}
			
			public int							PropertyCount
			{
				get
				{
					if (this.properties == null)
					{
						return 0;
					}
					else
					{
						return this.properties.Length;
					}
				}
			}
			
			
			public static TextRange[] Simplify(System.Collections.ICollection ranges)
			{
				System.Collections.ArrayList list = new System.Collections.ArrayList ();
				TextRange last = null;
				
				foreach (TextRange range in ranges)
				{
					if ((range.text == null) ||
						(range.text.Length == 0))
					{
						continue;
					}
					
					if ((last == null) ||
						(Property.CompareEqualContents (last.properties, range.properties) == false))
					{
						list.Add (range);
						last = range;
					}
					else
					{
						last.text = string.Concat (last.text, range.text);
					}
				}
				
				return (TextRange[]) list.ToArray (typeof (TextRange));
			}
			
			
			private Property[]					properties;
			private string						text;
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
			
			public string GetNextTextString(int level)
			{
				TextRange[] ranges = this.GetNextText (level);
			
				System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
				foreach (TextRange range in ranges)
				{
					buffer.Append (range.Text);
				}
			
				return buffer.ToString ();
			}
			
			public TextRange[] GetNextText(int level)
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
				
				TextRange[] text_range = this.generator.GenerateText (this.vector, level + 1, this.culture);
				
				this.level = level;
				
				return text_range;
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
			
			
			public Property[]					ValueProperties
			{
				get
				{
					if ((this.value_properties == null) ||
						(this.value_properties.Length == 0))
					{
						return null;
					}
					else
					{
						return (Property[]) this.value_properties.Clone ();
					}
				}
				set
				{
					if (this.value_properties != value)
					{
						if ((value == null) ||
							(value.Length == 0))
						{
							this.value_properties = value;
						}
						else
						{
							this.value_properties = (Property[]) value.Clone ();
						}
					}
				}
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
			
			public Property[]					PrefixProperties
			{
				get
				{
					if ((this.prefix_properties == null) ||
						(this.prefix_properties.Length == 0))
					{
						return null;
					}
					else
					{
						return (Property[]) this.prefix_properties.Clone ();
					}
				}
				set
				{
					if (this.prefix_properties != value)
					{
						if ((value == null) ||
							(value.Length == 0))
						{
							this.prefix_properties = value;
						}
						else
						{
							this.prefix_properties = (Property[]) value.Clone ();
						}
					}
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
			
			public Property[]					SuffixProperties
			{
				get
				{
					if ((this.suffix_properties == null) ||
						(this.suffix_properties.Length == 0))
					{
						return null;
					}
					else
					{
						return (Property[]) this.suffix_properties.Clone ();
					}
				}
				set
				{
					if (this.suffix_properties != value)
					{
						if ((value == null) ||
							(value.Length == 0))
						{
							this.suffix_properties = value;
						}
						else
						{
							this.suffix_properties = (Property[]) value.Clone ();
						}
					}
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
			
			public bool							SuppressBefore
			{
				get
				{
					return this.suppress_before;
				}
				set
				{
					this.suppress_before = value;
				}
			}
			
			public string[]						UserData
			{
				get
				{
					return this.user_data;
				}
				set
				{
					this.user_data = value;
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
			
			public string GenerateText(int rank, System.Globalization.CultureInfo culture)
			{
				string text = this.GetRawText (rank, culture);
				
				switch (this.casing)
				{
					case Casing.Default:								break;
					case Casing.Lower:	 text = text.ToLower (culture); break;
					case Casing.Upper:	 text = text.ToUpper (culture); break;
				}
				
				return text;
			}
			
			
			#region ISerializableAsText Members
			public void SerializeToText(System.Text.StringBuilder buffer)
			{
				string value_p  = Property.SerializeProperties (this.value_properties);
				string prefix_p = Property.SerializeProperties (this.prefix_properties);
				string suffix_p = Property.SerializeProperties (this.suffix_properties);
				
				if ((value_p == null) &&
					(prefix_p == null) &&
					(suffix_p == null) &&
					(this.user_data == null))
				{
					SerializerSupport.Join (buffer,
						/**/				SerializerSupport.SerializeString (this.prefix),
						/**/				SerializerSupport.SerializeString (this.suffix),
						/**/				SerializerSupport.SerializeInt ((int) this.casing | (this.suppress_before ? 0x0100 : 0x0000)),
						/**/				SerializerSupport.SerializeString (this.GetSetupArgument ()));
				}
				else
				{
					SerializerSupport.Join (buffer,
						/**/				SerializerSupport.SerializeString (this.prefix),
						/**/				SerializerSupport.SerializeString (this.suffix),
						/**/				SerializerSupport.SerializeInt ((int) this.casing | (this.suppress_before ? 0x0100 : 0x0000)),
						/**/				SerializerSupport.SerializeString (this.GetSetupArgument ()),
						/**/				SerializerSupport.SerializeString (value_p),
						/**/				SerializerSupport.SerializeString (prefix_p),
						/**/				SerializerSupport.SerializeString (suffix_p),
						/**/				SerializerSupport.SerializeStringArray (this.user_data));
				}
			}
			
			public void DeserializeFromText(TextContext context, string text, int pos, int length)
			{
				string[] args = SerializerSupport.Split (text, pos, length);
				
				System.Diagnostics.Debug.Assert ((args.Length == 4) || (args.Length == 8));
				
				string   value_p   = null;
				string   prefix_p  = null;
				string   suffix_p  = null;
				string[] user_data = null;
				
				if (args.Length == 8)
				{
					value_p   = SerializerSupport.DeserializeString (args[4]);
					prefix_p  = SerializerSupport.DeserializeString (args[5]);
					suffix_p  = SerializerSupport.DeserializeString (args[6]);
					user_data = SerializerSupport.DeserializeStringArray (args[7]);
				}
				
				string prefix = SerializerSupport.DeserializeString (args[0]);
				string suffix = SerializerSupport.DeserializeString (args[1]);
				int    casing = SerializerSupport.DeserializeInt (args[2]);
				string setup  = SerializerSupport.DeserializeString (args[3]);
				
				this.prefix = prefix;
				this.suffix = suffix;
				this.casing = (Casing) (casing & 0x00ff);
				this.suppress_before = (casing & 0x0100) != 0;
				
				this.value_properties  = Property.DeserializeProperties (context, value_p);
				this.prefix_properties = Property.DeserializeProperties (context, prefix_p);
				this.suffix_properties = Property.DeserializeProperties (context, suffix_p);
				
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
			private Property[]					value_properties;
			private Property[]					prefix_properties;
			private Property[]					suffix_properties;
			private string[]					user_data;
			private bool						suppress_before;
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
				
				case SequenceType.Roman:
					return new Internal.Sequences.Roman ();
				
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
		
		public static Generator.Sequence CreateSequence(SequenceType type, string prefix, string suffix, Casing casing, string setup_argument, bool suppress_before)
		{
			Generator.Sequence sequence = Generator.CreateSequence (type);
			
			sequence.DefineSetupArgument (setup_argument);
			sequence.Prefix = prefix;
			sequence.Suffix = suffix;
			sequence.Casing = casing;
			sequence.SuppressBefore = suppress_before;
			
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
			Constant,
			Roman
		}
		#endregion
		
		private System.Collections.ArrayList	sequences = new System.Collections.ArrayList ();
		private string							name;
		private int[]							start_vector;
		private string							global_prefix;
		private string							global_suffix;
		private Property[]						global_prefix_properties;
		private Property[]						global_suffix_properties;
		private string[]						user_data;
		private int								user_count;
	}
}
