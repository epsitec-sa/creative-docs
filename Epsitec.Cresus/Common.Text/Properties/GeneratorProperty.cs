//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// La classe GeneratorProperty définit un lien entre un fragment de texte
	/// et un générateur (qui produit du texte "automatique").
	/// </summary>
	public class GeneratorProperty : Property
	{
		public GeneratorProperty()
		{
			lock (GeneratorProperty.unique_id_lock)
			{
				this.unique_id = GeneratorProperty.next_unique_id++;
			}
		}
		
		public GeneratorProperty(string generator, int level) : this ()
		{
			this.generator = generator;
			this.level     = level;
		}
		
		
		public override WellKnownType			WellKnownType
		{
			get
			{
				return WellKnownType.Generator;
			}
		}
		
		public override PropertyType			PropertyType
		{
			get
			{
				return PropertyType.ExtraSetting;
			}
		}
		
		public override CombinationMode			CombinationMode
		{
			get
			{
				return CombinationMode.Accumulate;
			}
		}
		
		
		public string							Generator
		{
			get
			{
				return this.generator;
			}
		}
		
		public int								Level
		{
			get
			{
				return this.level;
			}
		}
		
		public long								UniqueId
		{
			//	Grâce au UniqueId, on garantit que deux propriétés décrivant un
			//	même générateur sont toujours distinctes; ceci évite que deux
			//	textes adjacents faisant référence au même générateur ne soient
			//	considérés que comme un texte unique.
			
			get
			{
				return this.unique_id;
			}
		}
		
		
		public static System.Collections.IComparer	Comparer
		{
			get
			{
				return new GeneratorComparer ();
			}
		}
		
		
		public override Property EmptyClone()
		{
			return new GeneratorProperty ();
		}
		
		public override void SerializeToText(System.Text.StringBuilder buffer)
		{
			SerializerSupport.Join (buffer,
				/**/				SerializerSupport.SerializeString (this.generator),
				/**/				SerializerSupport.SerializeInt (this.level),
				/**/				SerializerSupport.SerializeLong (this.unique_id));
		}

		public override void DeserializeFromText(TextContext context, string text, int pos, int length)
		{
			string[] args = SerializerSupport.Split (text, pos, length);
			
			Debug.Assert.IsTrue (args.Length == 3);
			
			string generator = SerializerSupport.DeserializeString (args[0]);
			int    level     = SerializerSupport.DeserializeInt (args[1]);
			long   unique_id = SerializerSupport.DeserializeLong (args[2]);
			
			this.generator = generator;
			this.level     = level;
			this.unique_id = unique_id;
			
			lock (GeneratorProperty.unique_id_lock)
			{
				if (GeneratorProperty.next_unique_id <= this.unique_id)
				{
					GeneratorProperty.next_unique_id = this.unique_id + 1;
				}
			}
		}
		
		public override Property GetCombination(Property property)
		{
			throw new System.NotImplementedException ();
		}

		
		public override void UpdateContentsSignature(IO.IChecksum checksum)
		{
			checksum.UpdateValue (this.generator);
			checksum.UpdateValue (this.level);
			checksum.UpdateValue (this.unique_id);
		}
		
		public override bool CompareEqualContents(object value)
		{
			return GeneratorProperty.CompareEqualContents (this, value as GeneratorProperty);
		}
		
		
		private static bool CompareEqualContents(GeneratorProperty a, GeneratorProperty b)
		{
			return a.generator == b.generator
				&& a.level == b.level
				&& a.unique_id == b.unique_id;
		}
		
		
		#region GeneratorComparer Class
		private class GeneratorComparer : System.Collections.IComparer
		{
			#region IComparer Members
			public int Compare(object x, object y)
			{
				Properties.GeneratorProperty px = x as Properties.GeneratorProperty;
				Properties.GeneratorProperty py = y as Properties.GeneratorProperty;
				
				if (px.level < py.level)
				{
					return -1;
				}
				if (px.level > py.level)
				{
					return 1;
				}
				
				int result = string.Compare (px.generator, py.generator);
				
				if (result == 0)
				{
					if (px.unique_id < py.unique_id)
					{
						return -1;
					}
					if (px.unique_id > py.unique_id)
					{
						return 1;
					}
				}
				
				return result;
			}
			#endregion
		}
		#endregion
		
		private static long						next_unique_id;
		private static object					unique_id_lock = new object ();
		
		private string							generator;
		private int								level;
		private long							unique_id;
	}
}
