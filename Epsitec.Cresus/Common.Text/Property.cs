//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe Property sert de base aux diverses propri�t�s.
	/// </summary>
	public abstract class Property : IContentsSignature, IContentsSignatureUpdater, IContentsComparer, ISerializableAsText
	{
		protected Property()
		{
		}
		
		
		public virtual long 						Version
		{
			get
			{
				if (this.version == 0)
				{
					this.version = StyleVersion.Default.Current;
				}
				
				return this.version;
			}
		}
		
		public abstract Properties.WellKnownType	WellKnownType
		{
			get;
		}
		
		public abstract Properties.PropertyType		PropertyType
		{
			get;
		}
		
		public virtual Properties.PropertyAffinity	PropertyAffinity
		{
			get
			{
				return Properties.PropertyAffinity.Text;
			}
		}
		
		public virtual Properties.CombinationMode	CombinationMode
		{
			get
			{
				return Properties.CombinationMode.Combine;
			}
		}
		
		public virtual bool							RequiresSpecialCodeProcessing
		{
			get
			{
				return false;
			}
		}
		
		public virtual bool							RequiresUniformParagraph
		{
			get
			{
				return false;
			}
		}
		
		
		public void Invalidate()
		{
			if (this.version != 0)
			{
				StyleVersion.Default.Change ();
			}
			
			this.contents_signature = 0;
			this.version            = 0;
		}
		
		
		public virtual int GetGlyphForSpecialCode(ulong code)
		{
			return -1;
		}
		
		public virtual OpenType.Font GetFontForSpecialCode(TextContext context, ulong code)
		{
			return null;
		}
		
		
		public abstract Property GetCombination(Property property);
		public abstract Property EmptyClone();
		
		#region IContentsSignatureUpdater Members
		public abstract void UpdateContentsSignature(IO.IChecksum checksum);
		#endregion
		
		#region IContentsComparer Members
		public abstract bool CompareEqualContents(object value);
		#endregion
		
		#region IContentsSignature Members
		public int GetContentsSignature()
		{
			if (this.contents_signature == 0)
			{
				IO.IChecksum checksum = IO.Checksum.CreateAdler32 ();
				
				this.UpdateContentsSignature (checksum);
				
				int signature = (int) checksum.Value;
				
				//	La signature calcul�e pourrait �tre nulle; dans ce cas, on
				//	l'ajuste pour �viter d'interpr�ter cela comme une absence
				//	de signature :
				
				this.contents_signature = (signature == 0) ? 1 : signature;
			}
			
			return this.contents_signature;
		}
		#endregion
		
		#region ISerializableAsText Members
		public abstract void SerializeToText(System.Text.StringBuilder buffer);
		public abstract void DeserializeFromText(TextContext context, string text, int pos, int length);
		#endregion
		
		public string SerializeToText()
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			this.SerializeToText (buffer);
			return buffer.ToString ();
		}
		
		public void DeserializeFromText(TextContext context, string text)
		{
			this.DeserializeFromText (context, text, 0, text.Length);
		}
		
		
		public override string ToString()
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			this.SerializeToText (buffer);
			return buffer.ToString ();
		}
		
		
		public static bool CompareEqualContents(Property a, Property b)
		{
			if (a == b)
			{
				return true;
			}
			
			if ((a == null) ||
				(b == null))
			{
				return false;
			}
			
			return a.CompareEqualContents (b);
		}
		
		public static bool CompareEqualContents(Property[] a, Property[] b)
		{
			if (a == b)
			{
				return true;
			}
			if (((a == null) && (b.Length == 0)) ||
				((b == null) && (a.Length == 0)))
			{
				return true;
			}
			if ((a == null) ||
				(b == null))
			{
				return false;
			}
			if (a.Length != b.Length)
			{
				return false;
			}
			
			int n = a.Length;
			
			for (int i = 0; i < n; i++)
			{
				Property pa = a[i];
				Property pb = b[i];
				
				if (pa.GetType () != pb.GetType ())
				{
					return false;
				}
				if (pa.GetContentsSignature () != pb.GetContentsSignature ())
				{
					return false;
				}
			}
			
			for (int i = 0; i < n; i++)
			{
				Property pa = a[i];
				Property pb = b[i];
				
				if (pa.CompareEqualContents (pb) == false)
				{
					return false;
				}
			}
			
			return true;
		}
		
		public static bool CompareEqualContents(System.Collections.IList a, System.Collections.IList b)
		{
			if (a == b)
			{
				return true;
			}
			if (((a == null) && (b.Count == 0)) ||
				((b == null) && (a.Count == 0)))
			{
				return true;
			}
			if ((a == null) ||
				(b == null))
			{
				return false;
			}
			if (a.Count != b.Count)
			{
				return false;
			}
			
			int n = a.Count;
			
			for (int i = 0; i < n; i++)
			{
				Property pa = a[i] as Property;
				Property pb = b[i] as Property;
				
				if (pa.GetType () != pb.GetType ())
				{
					return false;
				}
				if (pa.GetContentsSignature () != pb.GetContentsSignature ())
				{
					return false;
				}
			}
			
			for (int i = 0; i < n; i++)
			{
				Property pa = a[i] as Property;
				Property pb = b[i] as Property;
				
				if (pa.CompareEqualContents (pb) == false)
				{
					return false;
				}
			}
			
			return true;
		}
		
		
		public static void SerializeToText(System.Text.StringBuilder buffer, Property property)
		{
			System.Diagnostics.Debug.Assert (property != null);
			
			string type_name = property.GetType ().Name;
			string prop_name = type_name.Substring (0, type_name.Length - 8);	//	"XxxProperty" --> "Xxx"
			
			System.Diagnostics.Debug.Assert (property.WellKnownType.ToString () == prop_name);
			System.Diagnostics.Debug.Assert (type_name.Substring (prop_name.Length) == "Property");
			
			buffer.Append ("{");
			buffer.Append (prop_name);
			buffer.Append (":");
			
			property.SerializeToText (buffer);
			
			buffer.Append ("}");
		}
		
		public static void DeserializeFromText(TextContext context, string text, out Property property)
		{
			Property.DeserializeFromText (context, text, 0, text.Length, out property);
		}
		
		public static void DeserializeFromText(TextContext context, string text, int pos, int length, out Property property)
		{
			System.Diagnostics.Debug.Assert (text[pos+0] == '{');
			System.Diagnostics.Debug.Assert (text[pos+length-1] == '}');
			
			int sep_pos = text.IndexOf (':', pos, length);
			int end_pos = pos + length;
			
			System.Diagnostics.Debug.Assert (sep_pos > pos);
			
			string prop_name = text.Substring (pos+1, sep_pos - pos - 1);
			
			Property template = Property.templates[prop_name] as Property;
			
			if (template == null)
			{
				//	Nous n'avons pas encore de propri�t� mod�le que l'on puisse
				//	cl�ner; il faut donc l'instancier dynamiquement :
				
				string type_name = string.Concat ("Epsitec.Common.Text.Properties.", prop_name, "Property");
				
				System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly ();
				System.Type                type     = assembly.GetType (type_name);
				
				template = System.Activator.CreateInstance (type) as Property;
				
				Property.templates[prop_name] = template;
			}
			
			property = template.EmptyClone ();
			
			sep_pos++;
			
			property.DeserializeFromText (context, text, sep_pos, end_pos - sep_pos - 1);
		}
		
		
		public static Property[] Filter(System.Collections.ICollection properties, Properties.PropertyFilter filter)
		{
			switch (filter)
			{
				case Properties.PropertyFilter.UniformOnly:
					return Property.FilterUniformParagraph (properties);
				
				case Properties.PropertyFilter.NonUniformOnly:
					return Property.FilterNonUniformParagraph (properties);
					
				case Properties.PropertyFilter.All:
					if (properties is System.Array)
					{
						return (Property[]) properties;
					}
					else
					{
						Property[] copy = new Property[properties.Count];
						properties.CopyTo (copy, 0);
						return copy;
					}
				
				default:
					throw new System.InvalidOperationException (string.Format ("Filter {0} not supported", filter));
			}
		}
		
		
		private static Property[] FilterUniformParagraph(System.Collections.ICollection properties)
		{
			int count = 0;
			
			foreach (Property property in properties)
			{
				if (property.RequiresUniformParagraph)
				{
					count++;
				}
			}
			
			Property[] filtered = new Property[count];
			
			int index = 0;
			
			foreach (Property property in properties)
			{
				if (property.RequiresUniformParagraph)
				{
					filtered[index++] = property;
				}
			}
			
			System.Diagnostics.Debug.Assert (index == count);
			
			return filtered;
		}
		
		private static Property[] FilterNonUniformParagraph(System.Collections.ICollection properties)
		{
			int count = 0;
			
			foreach (Property property in properties)
			{
				if (! property.RequiresUniformParagraph)
				{
					count++;
				}
			}
			
			Property[] filtered = new Property[count];
			
			int index = 0;
			
			foreach (Property property in properties)
			{
				if (! property.RequiresUniformParagraph)
				{
					filtered[index++] = property;
				}
			}
			
			System.Diagnostics.Debug.Assert (index == count);
			
			return filtered;
		}
		
		
		protected void DefineVersion(long version)
		{
			this.version = version;
		}
		
		
		private int								contents_signature;
		private long							version;
		
		static System.Collections.Hashtable		templates = new System.Collections.Hashtable ();
	}
}
