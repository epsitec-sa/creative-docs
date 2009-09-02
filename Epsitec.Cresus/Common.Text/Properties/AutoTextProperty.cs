//	Copyright � 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// La classe AutoTextProperty permet de d�crire un morceau de texte
	/// g�n�r� automatiquement (avec styles particuliers); comparer avec
	/// GeneratorProperty qui g�n�re aussi du texte.
	/// Attention: cette propri�t� requiert un traitement sp�cial de la part
	/// de TextContext.GetPropertiesQuickAndDirty.
	/// </summary>
	public class AutoTextProperty : Property
	{
		public AutoTextProperty()
		{
			lock (AutoTextProperty.uniqueIdLock)
			{
				this.uniqueId = AutoTextProperty.nextUniqueId++;
			}
		}
		
		public AutoTextProperty(string tag) : this ()
		{
			this.tag = tag;
		}
		
		
		public override WellKnownType			WellKnownType
		{
			get
			{
				return WellKnownType.AutoText;
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
		
		
		public string							Tag
		{
			get
			{
				return this.tag;
			}
		}
		
		public long								UniqueId
		{
			//	Gr�ce au UniqueId, on garantit que deux propri�t�s d�crivant un
			//	m�me g�n�rateur sont toujours distinctes; ceci �vite que deux
			//	textes adjacents faisant r�f�rence au m�me g�n�rateur ne soient
			//	consid�r�s que comme un texte unique.
			
			get
			{
				return this.uniqueId;
			}
		}
		
		
		public override Property EmptyClone()
		{
			return new AutoTextProperty ();
		}
		
		public override void SerializeToText(System.Text.StringBuilder buffer)
		{
			SerializerSupport.Join (buffer,
				/**/				SerializerSupport.SerializeString (this.tag),
				/**/				SerializerSupport.SerializeLong (this.uniqueId));
		}

		public override void DeserializeFromText(TextContext context, string text, int pos, int length)
		{
			string[] args = SerializerSupport.Split (text, pos, length);
			
			System.Diagnostics.Debug.Assert (args.Length == 2);
			
			string tag       = SerializerSupport.DeserializeString (args[0]);
			long   uniqueId  = SerializerSupport.DeserializeLong (args[1]);
			
			this.tag       = tag;
			this.uniqueId  = uniqueId;
			
			lock (AutoTextProperty.uniqueIdLock)
			{
				if (AutoTextProperty.nextUniqueId <= this.uniqueId)
				{
					AutoTextProperty.nextUniqueId = this.uniqueId + 1;
				}
			}
		}
		
		public override Property GetCombination(Property property)
		{
			throw new System.NotImplementedException ();
		}

		
		public override void UpdateContentsSignature(IO.IChecksum checksum)
		{
			checksum.UpdateValue (this.tag);
			checksum.UpdateValue (this.uniqueId);
		}
		
		public override bool CompareEqualContents(object value)
		{
			return AutoTextProperty.CompareEqualContents (this, value as AutoTextProperty);
		}
		
		
		public static AutoTextProperty Find(Property[] properties, string tag)
		{
			foreach (Property property in properties)
			{
				if (property.WellKnownType == WellKnownType.AutoText)
				{
					AutoTextProperty autoText = property as AutoTextProperty;
					
					if (autoText.Tag == tag)
					{
						return autoText;
					}
				}
			}
			
			return null;
		}
		
		
		private static bool CompareEqualContents(AutoTextProperty a, AutoTextProperty b)
		{
			return a.tag == b.tag
				&& a.uniqueId == b.uniqueId;
		}
		
		
		
		private static long						nextUniqueId;
		private static object					uniqueIdLock = new object ();
		
		private string							tag;
		private long							uniqueId;
	}
}
