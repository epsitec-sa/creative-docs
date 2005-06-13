//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// Summary description for BaseProperty.
	/// </summary>
	public abstract class BaseProperty : IContentsSignature, IContentsSignatureUpdater, IContentsComparer, ISerializableAsText
	{
		protected BaseProperty()
		{
		}
		
		
		public virtual long 					Version
		{
			get
			{
				return this.version;
			}
		}
		
		public abstract WellKnownType			WellKnownType
		{
			get;
		}
		
		public abstract PropertyType			PropertyType
		{
			get;
		}
		
		public virtual CombinationMode			CombinationMode
		{
			get
			{
				return CombinationMode.Combine;
			}
		}
		
		public virtual bool						RequiresSpecialCodeProcessing
		{
			get
			{
				return false;
			}
		}
		
		
		public void Invalidate()
		{
			this.contents_signature = 0;
			this.version            = 0;
		}
		
		public void UpdateVersion()
		{
			this.version = StyleVersion.Current;
		}
		
		
		public virtual int GetGlyphForSpecialCode(ulong code)
		{
			return -1;
		}
		
		public abstract Properties.BaseProperty GetCombination(Properties.BaseProperty property);
		
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
		public abstract void DeserializeFromText(Context context, string text, int pos, int length);
		#endregion
		
		public override string ToString()
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			this.SerializeToText (buffer);
			return buffer.ToString ();
		}
		
		
		public static bool CompareEqualContents(Properties.BaseProperty[] a, Properties.BaseProperty[] b)
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
				Properties.BaseProperty pa = a[i];
				Properties.BaseProperty pb = b[i];
				
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
				Properties.BaseProperty pa = a[i];
				Properties.BaseProperty pb = b[i];
				
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
				Properties.BaseProperty pa = a[i] as Properties.BaseProperty;
				Properties.BaseProperty pb = b[i] as Properties.BaseProperty;
				
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
				Properties.BaseProperty pa = a[i] as Properties.BaseProperty;
				Properties.BaseProperty pb = b[i] as Properties.BaseProperty;
				
				if (pa.CompareEqualContents (pb) == false)
				{
					return false;
				}
			}
			
			return true;
		}
		
		
		public static void SerializeToText(System.Text.StringBuilder buffer, BaseProperty property)
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
		
		public static void DeserializeFromText(Context context, string text, int pos, int length, out BaseProperty property)
		{
			System.Diagnostics.Debug.Assert (text[pos+0] == '{');
			System.Diagnostics.Debug.Assert (text[pos+length-1] == '}');
			
			int sep_pos = text.IndexOf (':', pos, length);
			int end_pos = pos + length;
			
			System.Diagnostics.Debug.Assert (sep_pos > pos);
			
			string prop_name = text.Substring (pos+1, sep_pos - pos - 1);
			string type_name = string.Concat ("Epsitec.Common.Text.Properties.", prop_name, "Property");
			
			System.Runtime.Remoting.ObjectHandle handle = System.Activator.CreateInstance (typeof (BaseProperty).Assembly.FullName, type_name);
			
			property = handle.Unwrap () as BaseProperty;
			
			sep_pos++;
			
			property.DeserializeFromText (context, text, sep_pos, end_pos - sep_pos - 1);
		}
		
		protected void DefineVersion(long version)
		{
			this.version = version;
		}
		
		
		private int								contents_signature;
		private long							version;
	}
}
