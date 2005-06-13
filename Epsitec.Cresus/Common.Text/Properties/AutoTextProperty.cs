//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// La classe AutoTextProperty permet de décrire un morceau de texte
	/// généré automatiquement (avec styles particuliers); comparer avec
	/// GeneratorProperty qui génère aussi du texte.
	/// </summary>
	public class AutoTextProperty : BaseProperty
	{
		public AutoTextProperty()
		{
			this.unique_id = System.Threading.Interlocked.Increment (ref AutoTextProperty.next_unique_id);
		}
		
		public AutoTextProperty(string source, string parameter) : this ()
		{
			this.source    = source;
			this.parameter = parameter;
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
		
		
		public string							Source
		{
			get
			{
				return this.source;
			}
		}
		
		public string							Parameter
		{
			get
			{
				return this.parameter;
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
		
		
		public override void SerializeToText(System.Text.StringBuilder buffer)
		{
			SerializerSupport.Join (buffer,
				/**/				SerializerSupport.SerializeString (this.source),
				/**/				SerializerSupport.SerializeString (this.parameter));
		}

		public override void DeserializeFromText(Context context, string text, int pos, int length)
		{
			string[] args = SerializerSupport.Split (text, pos, length);
			
			Debug.Assert.IsTrue (args.Length == 2);
			
			string source    = SerializerSupport.DeserializeString (args[0]);
			string parameter = SerializerSupport.DeserializeString (args[1]);
			
			this.source    = source;
			this.parameter = parameter;
		}
		
		public override Properties.BaseProperty GetCombination(Properties.BaseProperty property)
		{
			throw new System.NotImplementedException ();
		}

		
		public override void UpdateContentsSignature(IO.IChecksum checksum)
		{
			checksum.UpdateValue (this.source);
			checksum.UpdateValue (this.parameter);
			checksum.UpdateValue (this.unique_id);
		}
		
		public override bool CompareEqualContents(object value)
		{
			return AutoTextProperty.CompareEqualContents (this, value as AutoTextProperty);
		}
		
		
		private static bool CompareEqualContents(AutoTextProperty a, AutoTextProperty b)
		{
			return a.source == b.source
				&& a.parameter == b.parameter
				&& a.unique_id == b.unique_id;
		}
		
		
		
		private static long						next_unique_id;
		
		private string							source;
		private string							parameter;
		private readonly long					unique_id;
	}
}
