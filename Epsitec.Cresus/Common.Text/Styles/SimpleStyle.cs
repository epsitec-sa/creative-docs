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
		
		public SimpleStyle(System.Collections.ICollection properties) : base (properties)
		{
		}
		
		
		public override void UpdateContentsSignature(IO.IChecksum checksum)
		{
			base.UpdateContentsSignature (checksum);
		}
		
		public override bool CompareEqualContents(object value)
		{
			return Styles.BasePropertyContainer.CompareEqualContents (this, value as Styles.SimpleStyle);
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
				int n = source.CountProperties;
				
				for (int i = 0; i < n; i++)
				{
					System.Type type = source[i].GetType ();
					
					if (this.hash.Contains (type))
					{
						Properties.BaseProperty base_prop = this.hash[type] as Properties.BaseProperty;
						Properties.BaseProperty comb_prop = base_prop.GetCombination (source[i]);
						
						this.hash[type] = comb_prop;
					}
					else
					{
						this.hash[type] = source[i];
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
		
		
		private Properties.BaseProperty[]		properties;
	}
}
