//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Styles
{
	/// <summary>
	/// La classe SimpleStyle permet de décrire un style simple, constitué d'une
	/// défition de fonte et de paragraphe, plus quelques autres détails.
	/// </summary>
	public sealed class SimpleStyle : BaseStyle
	{
		public SimpleStyle()
		{
		}
		
//		public SimpleStyle(System.Collections.ICollection components)
//		{
//			this.Initialise (components);
//		}
		
		
		public override bool					IsRichStyle
		{
			get
			{
				return false;
			}
		}
		
		
		public long 							Version
		{
			get
			{
				if (this.version == 0)
				{
					this.Update ();
				}
				
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
		
		
		public void Invalidate()
		{
			this.version = 0;
			this.ClearContentsSignature ();
		}
		
		public bool Update()
		{
			//	Recalcule le numéro de version correspondant à ce style
			//	en se basant sur les versions des propriétés.
			
			bool changed = false;
			
			//	Retourne true si une modification a eu lieu.
			
			if ((this.properties != null) &&
				(this.properties.Length > 0))
			{
				long version = 0;
				
				for (int i = 0; i < this.properties.Length; i++)
				{
					version = System.Math.Max (version, this.properties[i].Version);
				}
				
				if (this.version != version)
				{
					this.version = version;
					this.ClearContentsSignature ();
					
					changed = true;
				}
			}
			else if (this.version > 0)
			{
				this.version = 0;
				this.ClearContentsSignature ();
				
				changed = true;
			}
			
			return changed;
		}
		
		
		
		
		public override void UpdateContentsSignature(IO.IChecksum checksum)
		{
			//	Calcule la signature en se basant exclusivement sur celle des
			//	propriétés.
			
			if ((this.properties != null) &&
				(this.properties.Length > 0))
			{
				for (int i = 0; i < this.properties.Length; i++)
				{
					checksum.UpdateValue (this.properties[i].GetContentsSignature ());
				}
			}
		}
		
		public override bool CompareEqualContents(object value)
		{
			return SimpleStyle.CompareEqualContents(this, value as Styles.SimpleStyle);
		}
		
		
		public SimpleStyle.Accumulator StartAccumulation()
		{
			return new SimpleStyle.Accumulator (this);
		}
		
		
		public class Accumulator
		{
			public Accumulator(Styles.SimpleStyle host)
			{
				this.host = host;
				this.hash = new System.Collections.Hashtable ();
			}
			
			
			public void Accumulate(Styles.SimpleStyle source)
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
			
			
			Styles.SimpleStyle					host;
			System.Collections.Hashtable		hash;
		}
		
		
		public static bool CompareEqualContents(Styles.SimpleStyle a, Styles.SimpleStyle b)
		{
			if ((a.properties == null) &&
				(b.properties == null))
			{
				return true;
			}
			if ((a.properties == null) ||
				(b.properties == null))
			{
				return false;
			}
			if (a.properties.Length != b.properties.Length)
			{
				return false;
			}
			
			for (int i = 0; i < a.properties.Length; i++)
			{
				Properties.BaseProperty pa = a.properties[i];
				Properties.BaseProperty pb = b.properties[i];
				
				if (pa.GetType () != pb.GetType ())
				{
					return false;
				}
				if (pa.GetContentsSignature () != pb.GetContentsSignature ())
				{
					return false;
				}
			}
			
			for (int i = 0; i < a.properties.Length; i++)
			{
				Properties.BaseProperty pa = a.properties[i];
				Properties.BaseProperty pb = b.properties[i];
				
				if (pa.CompareEqualContents (pb) == false)
				{
					return false;
				}
			}
			
			return true;
		}
		
		
		private long							version;
		private Properties.BaseProperty[]		properties;
	}
}
