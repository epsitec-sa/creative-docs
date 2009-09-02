//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// La classe GeneratorProperty définit un lien entre un fragment de texte
	/// et un générateur (qui produit du texte "automatique").
	/// Attention: cette propriété requiert un traitement spécial de la part
	/// de TextContext.GetPropertiesQuickAndDirty.
	/// </summary>
	public class GeneratorProperty : Property
	{
		public GeneratorProperty()
		{
		}
		
		public GeneratorProperty(string generator, int level, long uniqueId)
		{
			this.generator = generator;
			this.level     = level;
			this.uniqueId  = uniqueId;
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
				return this.uniqueId;
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
				/**/				SerializerSupport.SerializeLong (this.uniqueId));
		}

		public override void DeserializeFromText(TextContext context, string text, int pos, int length)
		{
			string[] args = SerializerSupport.Split (text, pos, length);
			
			Debug.Assert.IsTrue (args.Length == 3);
			
			string generator = SerializerSupport.DeserializeString (args[0]);
			int    level     = SerializerSupport.DeserializeInt (args[1]);
			long   uniqueId  = SerializerSupport.DeserializeLong (args[2]);
			
			this.generator = generator;
			this.level     = level;
			this.uniqueId  = uniqueId;
		}
		
		public override Property GetCombination(Property property)
		{
			throw new System.NotImplementedException ();
		}

		
		public override void UpdateContentsSignature(IO.IChecksum checksum)
		{
			checksum.UpdateValue (this.generator);
			checksum.UpdateValue (this.level);
			checksum.UpdateValue (this.uniqueId);
		}
		
		public override bool CompareEqualContents(object value)
		{
			return GeneratorProperty.CompareEqualContents (this, value as GeneratorProperty);
		}
		
		
		private static bool CompareEqualContents(GeneratorProperty a, GeneratorProperty b)
		{
			return a.generator == b.generator
				&& a.level == b.level
				&& a.uniqueId == b.uniqueId;
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
					if (px.uniqueId < py.uniqueId)
					{
						return -1;
					}
					if (px.uniqueId > py.uniqueId)
					{
						return 1;
					}
				}
				
				return result;
			}
			#endregion
		}
		#endregion
		
		private string							generator;
		private int								level;
		private long							uniqueId;
	}
}
