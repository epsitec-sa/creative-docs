//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe Property sert de base aux diverses propriétés.
	/// </summary>
	public abstract class Property : IContentsSignature, IContentsSignatureUpdater, IContentsComparer, ISerializableAsText
	{
		protected Property()
		{
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
		
		
		internal string								InternalName
		{
			get
			{
				return this.internalName;
			}
			set
			{
				if (value == "")
				{
					value = null;
				}
				
				if (this.internalName != value)
				{
					this.internalName = value;
				}
			}
		}
		
		
		public void Invalidate()
		{
			this.contentsSignature = 0;
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
			if (this.contentsSignature == 0)
			{
				IO.IChecksum checksum = IO.Checksum.CreateAdler32 ();
				
				this.UpdateContentsSignature (checksum);
				
				int signature = (int) checksum.Value;
				
				//	La signature calculée pourrait être nulle; dans ce cas, on
				//	l'ajuste pour éviter d'interpréter cela comme une absence
				//	de signature :
				
				this.contentsSignature = (signature == 0) ? 1 : signature;
			}
			
			return this.contentsSignature;
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
			
			if (a.WellKnownType != b.WellKnownType)
			{
				return false;
			}
			
			if (a.GetContentsSignature () != b.GetContentsSignature ())
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
				
				if (pa.WellKnownType != pb.WellKnownType)
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
				
				if (pa.WellKnownType != pb.WellKnownType)
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
		
		
		public static string Serialize(Property property)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			Property.SerializeToText (buffer, property);
			return buffer.ToString ();
		}
		
		public static Property Deserialize(TextContext context, int version, string text)
		{
			Property property;
			Property.DeserializeFromText (context, text, out property);
			return property;
		}
		
		
		public static string SerializeProperties(Property[] properties)
		{
			if (properties == null)
			{
				return null;
			}
			
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			buffer.Append (SerializerSupport.SerializeInt (properties.Length));
			buffer.Append ("/");
			buffer.Append (TextContext.SerializationVersion);
			
			for (int i = 0; i < properties.Length; i++)
			{
				buffer.Append ("/");
				buffer.Append (SerializerSupport.SerializeString (Property.Serialize (properties[i])));
			}
			
			return buffer.ToString ();
		}
		
		public static Property[] DeserializeProperties(TextContext context, string text)
		{
			if (text == null)
			{
				return null;
			}
			
			string[] args = text.Split ('/');
			
			int offset  = 0;
			int count   = SerializerSupport.DeserializeInt (args[offset++]);
			int version = SerializerSupport.DeserializeInt (args[offset++]);
			
			Property[] properties = new Property[count];
			
			for (int i = 0; i < count; i++)
			{
				properties[i] = Property.Deserialize (context, version, SerializerSupport.DeserializeString (args[offset++]));
			}
			
			return properties;
		}
		
		
		public static void SerializeToText(System.Text.StringBuilder buffer, Property property)
		{
			System.Diagnostics.Debug.Assert (property != null);
			
			string typeName = property.GetType ().Name;
			string propName = typeName.Substring (0, typeName.Length - 8);	//	"XxxProperty" --> "Xxx"
			
			System.Diagnostics.Debug.Assert (property.WellKnownType.ToString () == propName);
			System.Diagnostics.Debug.Assert (typeName.Substring (propName.Length) == "Property");
			
			buffer.Append ("{");
			buffer.Append (propName);
			
			if ((property.internalName != null) &&
				(property.internalName.Length > 0))
			{
				buffer.Append (",");
				buffer.Append (property.internalName);
			}
			
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
			
			int sepPos = text.IndexOf (':', pos, length);
			int endPos = pos + length;
			
			System.Diagnostics.Debug.Assert (sepPos > pos);
			
			string propName = text.Substring (pos+1, sepPos - pos - 1);
			string intlName = null;
			
			if (propName.IndexOf (',') > 0)
			{
				string[] split = propName.Split (',');
				
				propName = split[0];
				intlName = split[1];
			}
			
			Property template = Property.templates[propName] as Property;
			
			if (template == null)
			{
				//	Nous n'avons pas encore de propriété modèle que l'on puisse
				//	clôner; il faut donc l'instancier dynamiquement :
				
				string typeName = string.Concat ("Epsitec.Common.Text.Properties.", propName, "Property");
				
				System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly ();
				System.Type                type     = assembly.GetType (typeName);
				
				template = System.Activator.CreateInstance (type) as Property;
				
				Property.templates[propName] = template;
			}
			
			property = template.EmptyClone ();
			property.internalName = intlName;
			
			sepPos++;
			
			property.DeserializeFromText (context, text, sepPos, endPos - sepPos - 1);
		}
		
		
		public static Property[] Filter(IEnumerable<Property> properties, Properties.WellKnownType type)
		{
			int count = 0;
			
			foreach (Property property in properties)
			{
				if (property.WellKnownType == type)
				{
					count++;
				}
			}
			
			Property[] filtered = new Property[count];
			
			if (count > 0)
			{
				int index = 0;
				
				foreach (Property property in properties)
				{
					if (property.WellKnownType == type)
					{
						filtered[index++] = property;
					}
				}
				
				System.Diagnostics.Debug.Assert (index == count);
			}
			
			return filtered;
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
		
		
		
		private int								contentsSignature;
		private string							internalName;
		
		static System.Collections.Hashtable		templates = new System.Collections.Hashtable ();
	}
}
