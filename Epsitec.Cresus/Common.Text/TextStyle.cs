//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// Summary description for TextStyle.
	/// </summary>
	public class TextStyle : IContentsSignature, IContentsComparer
	{
		public TextStyle()
		{
		}
		
		
		public long 							Version
		{
			get
			{
				return this.version;
			}
		}
		
		public Properties.BaseProperty[]		Properties
		{
			get
			{
				return this.properties;
			}
		}
		
		public static long						CurrentVersion
		{
			get
			{
				return TextStyle.current_version;
			}
		}
		
		
		public void Invalidate()
		{
			this.contents_signature = 0;
			this.version            = 0;
		}
		
		
		public static void ChangeCurrentVersion()
		{
			TextStyle.current_version++;
		}
		
		public void UpdateVersion()
		{
			this.version = TextStyle.CurrentVersion;
		}
		
		public void DefineVersion(long version)
		{
			this.version = version;
		}
		
		public TextStyle.Accumulator StartAccumulation()
		{
			return new TextStyle.Accumulator (this);
		}
		
		
		public class Accumulator
		{
			public Accumulator(TextStyle host)
			{
				this.host = host;
				this.hash = new System.Collections.Hashtable ();
			}
			
			
			public void Accumulate(TextStyle source)
			{
				Properties.BaseProperty[] props = source.Properties;
				
				for (int i = 0; i < props.Length; i++)
				{
					System.Type type = props[i].GetType ();
					
					if (this.hash.Contains (type))
					{
						Properties.BaseProperty base_prop = this.hash[type] as Properties.BaseProperty;
						Properties.BaseProperty comb_prop = base_prop.GetCombination (props[i]);
						
						this.hash[type] = comb_prop;
					}
					else
					{
						this.hash[type] = props[i];
					}
				}
			}
			
			public void Done()
			{
				int count = this.hash.Count;
				
				this.host.properties = new Properties.BaseProperty[count];
				this.hash.Values.CopyTo (this.host.properties, 0);
			}
			
			
			TextStyle							host;
			System.Collections.Hashtable		hash;
		}
		
		
		#region IContentsSignature Members
		public int GetContentsSignature()
		{
			if (this.contents_signature == 0)
			{
				int signature = 0;
				
				for (int i = 0; i < this.properties.Length; i++)
				{
					signature ^= this.properties[i].GetContentsSignature ();
				}
				
				//	La signature calculée pourrait être nulle; dans ce cas, on
				//	l'ajuste pour éviter d'interpréter cela comme une absence
				//	de signature :
				
				this.contents_signature = (signature == 0) ? 1 : signature;
			}
			
			return this.contents_signature;
		}
		#endregion
		
		#region IContentsComparer Members
		public bool CompareEqualContents(object value)
		{
			return TextStyle.CompareEqualContents (this, value as TextStyle);
		}
		#endregion
		
		public static bool CompareEqualContents(TextStyle a, TextStyle b)
		{
			//	TODO: compléter
			
			return true;
		}
		
		
		private int								contents_signature;
		private long							version;
		private Properties.BaseProperty[]		properties;
		
		private static long						current_version = 1;
	}
}
