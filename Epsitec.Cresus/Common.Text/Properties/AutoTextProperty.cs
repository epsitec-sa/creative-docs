//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// La classe AutoTextProperty permet de décrire un morceau de texte
	/// généré automatiquement (avec styles particuliers); comparer avec
	/// GeneratorProperty qui génère aussi du texte.
	/// </summary>
	public class AutoTextProperty : Property
	{
		public AutoTextProperty()
		{
			lock (AutoTextProperty.unique_id_lock)
			{
				this.unique_id = AutoTextProperty.next_unique_id++;
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
			//	Grâce au UniqueId, on garantit que deux propriétés décrivant un
			//	même générateur sont toujours distinctes; ceci évite que deux
			//	textes adjacents faisant référence au même générateur ne soient
			//	considérés que comme un texte unique.
			
			get
			{
				return this.unique_id;
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
				/**/				SerializerSupport.SerializeLong (this.unique_id));
		}

		public override void DeserializeFromText(TextContext context, string text, int pos, int length)
		{
			string[] args = SerializerSupport.Split (text, pos, length);
			
			System.Diagnostics.Debug.Assert (args.Length == 2);
			
			string tag       = SerializerSupport.DeserializeString (args[0]);
			long   unique_id = SerializerSupport.DeserializeLong (args[1]);
			
			this.tag       = tag;
			this.unique_id = unique_id;
			
			lock (AutoTextProperty.unique_id_lock)
			{
				if (AutoTextProperty.next_unique_id <= this.unique_id)
				{
					AutoTextProperty.next_unique_id = this.unique_id + 1;
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
			checksum.UpdateValue (this.unique_id);
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
					AutoTextProperty auto_text = property as AutoTextProperty;
					
					if (auto_text.Tag == tag)
					{
						return auto_text;
					}
				}
			}
			
			return null;
		}
		
		
		private static bool CompareEqualContents(AutoTextProperty a, AutoTextProperty b)
		{
			return a.tag == b.tag
				&& a.unique_id == b.unique_id;
		}
		
		
		
		private static long						next_unique_id;
		private static object					unique_id_lock = new object ();
		
		private string							tag;
		private long							unique_id;
	}
}
