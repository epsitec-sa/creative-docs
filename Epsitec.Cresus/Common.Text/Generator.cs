//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
				return this.startVector == null ? new int[0] : this.startVector.Clone () as int[];
			}
			set
			{
				if (value == null)
				{
					this.startVector = null;
				}
				else if (! Types.Comparer.Equal (this.startVector, value))
				{
					this.startVector = value.Clone () as int[];
				}
			}
		}
		
		public string							GlobalPrefix
		{
			get
			{
				return this.globalPrefix;
			}
			set
			{
				this.globalPrefix = value;
			}
		}
		
		public Property[]						GlobalPrefixProperties
		{
			get
			{
				if ((this.globalPrefixProperties == null) ||
					(this.globalPrefixProperties.Length == 0))
				{
					return null;
				}
				else
				{
					return (Property[]) this.globalPrefixProperties.Clone ();
				}
			}
			set
			{
				if (this.globalPrefixProperties != value)
				{
					if ((value == null) ||
						(value.Length == 0))
					{
						this.globalPrefixProperties = value;
					}
					else
					{
						this.globalPrefixProperties = (Property[]) value.Clone ();
					}
				}
			}
		}
		
		public string							GlobalSuffix
		{
			get
			{
				return this.globalSuffix;
			}
			set
			{
				this.globalSuffix = value;
			}
		}
		
		public Property[]						GlobalSuffixProperties
		{
			get
			{
				if ((this.globalSuffixProperties == null) ||
					(this.globalSuffixProperties.Length == 0))
				{
					return null;
				}
				else
				{
					return (Property[]) this.globalSuffixProperties.Clone ();
				}
			}
			set
			{
				if (this.globalSuffixProperties != value)
				{
					if ((value == null) ||
						(value.Length == 0))
					{
						this.globalSuffixProperties = value;
					}
					else
					{
						this.globalSuffixProperties = (Property[]) value.Clone ();
					}
				}
			}
		}
		
		public string[]							UserData
		{
			get
			{
				return this.userData;
			}
			set
			{
				this.userData = value;
			}
		}
		
		public int								UserCount
		{
			get
			{
				return this.userCount;
			}
		}
		
		
		public Generator.Sequence				this[int index]
		{
			get
			{
				return this.sequences[index] as Sequence;
			}
		}

		public int								Count
		{
			get
			{
				return this.sequences.Count;
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
		
		public void Truncate(int count)
		{
			object[] copy = this.sequences.ToArray ();
			
			this.sequences.Clear ();
			
			if (count > copy.Length)
			{
				count = copy.Length;
			}
			
			for (int i = 0; i < count; i++)
			{
				this.sequences.Add (copy[i]);
			}
		}
		
		public Generator.Sequence Modify(int index, Generator.SequenceType type)
		{
			//	Modifie le type d'une séquence; si la séquence nécessite un argument
			//	spécial pour son initialisation (par exemple un caractère de puce à
			//	utiliser), il faudra encore appeler sequence.DefineSetupArgument sur
			//	l'instance de séquence retournée.
			
			Sequence oldSequence = this.sequences[index] as Sequence;
			Sequence newSequence = Generator.CreateSequence (type);
			
			newSequence.ValueProperties  = oldSequence.ValueProperties;
			newSequence.Prefix           = oldSequence.Prefix;
			newSequence.PrefixProperties = oldSequence.PrefixProperties;
			newSequence.Suffix           = oldSequence.Suffix;
			newSequence.SuffixProperties = oldSequence.SuffixProperties;
			newSequence.Casing           = oldSequence.Casing;
			newSequence.SuppressBefore   = oldSequence.SuppressBefore;
			newSequence.UserData         = oldSequence.UserData;
			
			this.sequences[index] = newSequence;
			
			return newSequence;
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
		
		public TextRange[] GenerateText(int[] ranks, int maxLevelCount, System.Globalization.CultureInfo culture)
		{
			System.Collections.ArrayList textRange = new System.Collections.ArrayList ();
			
			System.Diagnostics.Debug.Assert (maxLevelCount > 0);
			System.Diagnostics.Debug.Assert (maxLevelCount <= ranks.Length);
			
			if (this.sequences.Count == 0)
			{
				throw new System.InvalidOperationException ();
			}
			
			Sequence sequence = this.sequences[0] as Sequence;
			
			for (int i = 0; i < maxLevelCount; i++)
			{
				int index = System.Math.Min (i, this.sequences.Count - 1);
				
				sequence = this.sequences[index] as Sequence;
				
				if (sequence.SuppressBefore)
				{
					textRange.Clear ();
				}
				
				if ((sequence.Prefix != null) &&
					(sequence.Prefix.Length > 0))
				{
					textRange.Add (new TextRange (sequence.Prefix, sequence.PrefixProperties));
				}
				
				string text = sequence.GenerateText (ranks[i], culture);
				
				//	N'accepte pas de séquences qui ne génèrent aucun texte, car cela
				//	peut provoquer la disparition complète de la séquence. On force
				//	donc un espace de largeur nulle dans un tel cas.
				
				if (text.Length == 0)
				{
					text = "\u200b";
				}
				
				textRange.Add (new TextRange (text, sequence.ValueProperties));
				
				if ((sequence.Suffix != null) &&
					(sequence.Suffix.Length > 0))
				{
					textRange.Add (new TextRange (sequence.Suffix, sequence.SuffixProperties));
				}
			}
			
			if (this.globalPrefix != null)
			{
				textRange.Insert (0, new TextRange (this.globalPrefix, this.globalPrefixProperties));
			}
			
			if (this.globalSuffix != null)
			{
				textRange.Add (new TextRange (this.globalSuffix, this.globalSuffixProperties));
			}
			
			return TextRange.Simplify (textRange);
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
			int[]                startVector = this.StartVector;
			
			buffer.Append (SerializerSupport.SerializeString (this.name));
			buffer.Append ("/");
			buffer.Append (SerializerSupport.SerializeInt (sequences.Length));
			buffer.Append ("/");
			buffer.Append (SerializerSupport.SerializeInt (startVector.Length));
			
			foreach (Generator.Sequence sequence in sequences)
			{
				buffer.Append ("/");
				buffer.Append (SerializerSupport.SerializeString (Generator.SerializeToText (sequence)));
			}
			
			foreach (int start in startVector)
			{
				buffer.Append ("/");
				buffer.Append (SerializerSupport.SerializeInt (start));
			}
			
			buffer.Append ("/");
			buffer.Append (SerializerSupport.SerializeString (this.globalPrefix));
			buffer.Append ("/");
			buffer.Append (SerializerSupport.SerializeString (this.globalSuffix));
			buffer.Append ("/");
			buffer.Append (SerializerSupport.SerializeString (Property.SerializeProperties (this.globalPrefixProperties)));
			buffer.Append ("/");
			buffer.Append (SerializerSupport.SerializeString (Property.SerializeProperties (this.globalSuffixProperties)));
			buffer.Append ("/");
			buffer.Append (SerializerSupport.SerializeStringArray (this.userData));
		}
		
		public void Deserialize(TextContext context, int version, string[] args, ref int offset)
		{
			this.sequences.Clear ();
			
			this.name = SerializerSupport.DeserializeString (args[offset++]);

			int numSequences = SerializerSupport.DeserializeInt (args[offset++]);
			int numStarts    = SerializerSupport.DeserializeInt (args[offset++]);
			
			for (int i = 0; i < numSequences; i++)
			{
				Generator.Sequence sequence;
				
				Generator.DeserializeFromText (context, SerializerSupport.DeserializeString (args[offset++]), out sequence);
				
				this.Add (sequence);
			}
			
			this.startVector = new int[numStarts];
			
			for (int i = 0; i < numStarts; i++)
			{
				this.startVector[i] = SerializerSupport.DeserializeInt (args[offset++]);
			}
			
			if (version >= 4)
			{
				this.globalPrefix = SerializerSupport.DeserializeString (args[offset++]);
				this.globalSuffix = SerializerSupport.DeserializeString (args[offset++]);
			}
			
			if (version >= 5)
			{
				this.globalPrefixProperties = Property.DeserializeProperties (context, SerializerSupport.DeserializeString (args[offset++]));
				this.globalSuffixProperties = Property.DeserializeProperties (context, SerializerSupport.DeserializeString (args[offset++]));
				
				this.userData = SerializerSupport.DeserializeStringArray (args[offset++]);
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
			this.userCount++;
			System.Diagnostics.Debug.Assert (this.userCount > 0);
		}
		
		internal void DecrementUserCount()
		{
			System.Diagnostics.Debug.Assert (this.userCount > 0);
			this.userCount--;
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
					Properties.AutoTextProperty    autoTextProperty;
					Properties.GeneratorProperty   generatorProperty;
					Properties.MarginsProperty     marginsProperty = null;
					Properties.ManagedInfoProperty mInfoProperty  = null;
					
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
						
						generatorProperty = this.enumerator.GetGeneratorProperty (code);
						
						context.GetAutoText (code, out autoTextProperty);
						context.GetMargins (code, out marginsProperty);
						
						if (this.mpp != null)
						{
							context.GetManagedInfo (code, this.mpp.ManagerName, out mInfoProperty);
						}
						
						if ((mInfoProperty != null) &&
							(mInfoProperty.ManagerInfo != "auto"))
						{
							//	Mode spécifique :
							//
							//	- "cont" -----> continue indépendamment du contexte
							//	- "reset" ----> reprend au début
							//	- "set ..." --> reprend avec le numéro spécifié
							
							string mode = mInfoProperty.ManagerInfo;
							
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
								string[] args = mode.Substring (4).Split ('.');
								
								int[] vector = new int[args.Length];
								
								for (int i = 0; i < args.Length; i++)
								{
									int value;
									
									if (this.generator.Sequences[i].ParseText (args[i], out value))
									{
										vector[i] = value;
									}
									else
									{
										vector[i] = 1;
									}
								}
								
								this.series.Restart (vector);
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
						
						System.Diagnostics.Debug.Assert (generatorProperty != null);
						System.Diagnostics.Debug.Assert (generatorProperty.Generator == this.generator.Name);
						
						int    length = this.context.GetTextEndDistance (this.story, cursor, generatorProperty);
						int    level  = System.Math.Max (generatorProperty.Level, marginsProperty == null ? 0 : marginsProperty.Level);
						
						TextRange[] text = this.series.GetNextText (level);
						
						System.Diagnostics.Debug.Assert (length > 0);
						System.Diagnostics.Debug.Assert (text.Length > 0);
						System.Diagnostics.Debug.Assert (text[0].Text.Length > 0);
						
						//	Compte combien de textes ont été modifiés pendant cette
						//	opération :
						
						if (this.story.ReplaceTextSequence (cursor, length, autoTextProperty, generatorProperty, text))
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
			
			public void Restart(int[] vector)
			{
				this.vector = vector == null ? new int[0] : vector.Clone () as int[];
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
				
				TextRange[] textRange = this.generator.GenerateText (this.vector, level + 1, this.culture);
				
				this.level = level;
				
				return textRange;
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
					if ((this.valueProperties == null) ||
						(this.valueProperties.Length == 0))
					{
						return null;
					}
					else
					{
						return (Property[]) this.valueProperties.Clone ();
					}
				}
				set
				{
					if (this.valueProperties != value)
					{
						if ((value == null) ||
							(value.Length == 0))
						{
							this.valueProperties = value;
						}
						else
						{
							this.valueProperties = (Property[]) value.Clone ();
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
					if ((this.prefixProperties == null) ||
						(this.prefixProperties.Length == 0))
					{
						return null;
					}
					else
					{
						return (Property[]) this.prefixProperties.Clone ();
					}
				}
				set
				{
					if (this.prefixProperties != value)
					{
						if ((value == null) ||
							(value.Length == 0))
						{
							this.prefixProperties = value;
						}
						else
						{
							this.prefixProperties = (Property[]) value.Clone ();
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
					if ((this.suffixProperties == null) ||
						(this.suffixProperties.Length == 0))
					{
						return null;
					}
					else
					{
						return (Property[]) this.suffixProperties.Clone ();
					}
				}
				set
				{
					if (this.suffixProperties != value)
					{
						if ((value == null) ||
							(value.Length == 0))
						{
							this.suffixProperties = value;
						}
						else
						{
							this.suffixProperties = (Property[]) value.Clone ();
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
					return this.suppressBefore;
				}
				set
				{
					this.suppressBefore = value;
				}
			}
			
			public string[]						UserData
			{
				get
				{
					return this.userData;
				}
				set
				{
					this.userData = value;
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
			
			public abstract bool ParseText(string text, out int value);
			
			#region ISerializableAsText Members
			public void SerializeToText(System.Text.StringBuilder buffer)
			{
				string valueP  = Property.SerializeProperties (this.valueProperties);
				string prefixP = Property.SerializeProperties (this.prefixProperties);
				string suffixP = Property.SerializeProperties (this.suffixProperties);
				
				if ((valueP == null) &&
					(prefixP == null) &&
					(suffixP == null) &&
					(this.userData == null))
				{
					SerializerSupport.Join (buffer,
						/**/				SerializerSupport.SerializeString (this.prefix),
						/**/				SerializerSupport.SerializeString (this.suffix),
						/**/				SerializerSupport.SerializeInt ((int) this.casing | (this.suppressBefore ? 0x0100 : 0x0000)),
						/**/				SerializerSupport.SerializeString (this.GetSetupArgument ()));
				}
				else
				{
					SerializerSupport.Join (buffer,
						/**/				SerializerSupport.SerializeString (this.prefix),
						/**/				SerializerSupport.SerializeString (this.suffix),
						/**/				SerializerSupport.SerializeInt ((int) this.casing | (this.suppressBefore ? 0x0100 : 0x0000)),
						/**/				SerializerSupport.SerializeString (this.GetSetupArgument ()),
						/**/				SerializerSupport.SerializeString (valueP),
						/**/				SerializerSupport.SerializeString (prefixP),
						/**/				SerializerSupport.SerializeString (suffixP),
						/**/				SerializerSupport.SerializeStringArray (this.userData));
				}
			}
			
			public void DeserializeFromText(TextContext context, string text, int pos, int length)
			{
				string[] args = SerializerSupport.Split (text, pos, length);
				
				System.Diagnostics.Debug.Assert ((args.Length == 4) || (args.Length == 8));
				
				string   valueP   = null;
				string   prefixP  = null;
				string   suffixP  = null;
				string[] userData = null;
				
				if (args.Length == 8)
				{
					valueP   = SerializerSupport.DeserializeString (args[4]);
					prefixP  = SerializerSupport.DeserializeString (args[5]);
					suffixP  = SerializerSupport.DeserializeString (args[6]);
					userData = SerializerSupport.DeserializeStringArray (args[7]);
				}
				
				string prefix = SerializerSupport.DeserializeString (args[0]);
				string suffix = SerializerSupport.DeserializeString (args[1]);
				int    casing = SerializerSupport.DeserializeInt (args[2]);
				string setup  = SerializerSupport.DeserializeString (args[3]);
				
				this.prefix = prefix;
				this.suffix = suffix;
				this.casing = (Casing) (casing & 0x00ff);
				this.suppressBefore = (casing & 0x0100) != 0;
				
				this.valueProperties  = Property.DeserializeProperties (context, valueP);
				this.prefixProperties = Property.DeserializeProperties (context, prefixP);
				this.suffixProperties = Property.DeserializeProperties (context, suffixP);
				
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
			private Property[]					valueProperties;
			private Property[]					prefixProperties;
			private Property[]					suffixProperties;
			private string[]					userData;
			private bool						suppressBefore;
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
				
				case SequenceType.Empty:
					return new Internal.Sequences.Empty ();
					
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
		
		public static Generator.Sequence CreateSequence(SequenceType type, string prefix, string suffix, Casing casing, string setupArgument)
		{
			Generator.Sequence sequence = Generator.CreateSequence (type);
			
			sequence.DefineSetupArgument (setupArgument);
			sequence.Prefix = prefix;
			sequence.Suffix = suffix;
			sequence.Casing = casing;
			
			return sequence;
		}
		
		public static Generator.Sequence CreateSequence(SequenceType type, string prefix, string suffix, Casing casing, string setupArgument, bool suppressBefore)
		{
			Generator.Sequence sequence = Generator.CreateSequence (type);
			
			sequence.DefineSetupArgument (setupArgument);
			sequence.Prefix = prefix;
			sequence.Suffix = suffix;
			sequence.Casing = casing;
			sequence.SuppressBefore = suppressBefore;
			
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
			
			string sequName = sequence.GetType ().Name;
			
			System.Diagnostics.Debug.Assert (sequence.WellKnownType.ToString () == sequName);
			
			buffer.Append ("{");
			buffer.Append (sequName);
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
			
			int sepPos = text.IndexOf (':', pos, length);
			int endPos = pos + length;
			
			System.Diagnostics.Debug.Assert (sepPos > pos);
			
			string sequName = text.Substring (pos+1, sepPos - pos - 1);
			string typeName = string.Concat ("Epsitec.Common.Text.Internals.Sequences.", sequName, "Sequence");
			
			sequence = Generator.CreateSequence ((SequenceType) System.Enum.Parse (typeof (SequenceType), sequName));
			
			sepPos++;
			
			sequence.DeserializeFromText (context, text, sepPos, endPos - sepPos - 1);
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
			Roman,
			Empty
		}
		#endregion
		
		private System.Collections.ArrayList	sequences = new System.Collections.ArrayList ();
		private string							name;
		private int[]							startVector;
		private string							globalPrefix;
		private string							globalSuffix;
		private Property[]						globalPrefixProperties;
		private Property[]						globalSuffixProperties;
		private string[]						userData;
		private int								userCount;
	}
}
