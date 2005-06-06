//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Styles
{
	/// <summary>
	/// Summary description for BasePropertyContainer.
	/// </summary>
	public abstract class BasePropertyContainer : IContentsSignature, IContentsSignatureUpdater, System.Collections.IEnumerable
	{
		public BasePropertyContainer()
		{
		}
		
		public BasePropertyContainer(System.Collections.ICollection properties)
		{
			this.Initialise (properties);
		}
		
		
		
		public Properties.BaseProperty			this[int index]
		{
			get
			{
				if (this.properties == null)
				{
					throw new System.IndexOutOfRangeException ("Index out of range.");
				}
				
				return this.properties[index];
			}
		}
		
		public Properties.BaseProperty			this[System.Type type]
		{
			get
			{
				if (this.properties == null)
				{
					return null;
				}
				
				for (int i = 0; i < this.properties.Length; i++)
				{
					if (this.properties[i].GetType () == type)
					{
						return this.properties[i];
					}
				}
				
				return null;
			}
		}
		
		public Properties.BaseProperty			this[Properties.WellKnownType type]
		{
			get
			{
				if (this.properties == null)
				{
					return null;
				}
				
				for (int i = 0; i < this.properties.Length; i++)
				{
					Properties.WellKnownType found = this.properties[i].WellKnownType;
					
					if (found == type)
					{
						return this.properties[i];
					}
					
					//	On peut s'arrêter dès que l'on trouve une propriété avec
					//	un WellKnownType plus grand que celui recherché, car la
					//	table est triée :
					
					if (found > type)
					{
						break;
					}
				}
				
				return null;
			}
		}
		
		
		public int								CountProperties
		{
			get
			{
				return (this.properties == null) ? 0 : this.properties.Length;
			}
		}
		
		public bool								IsEmpty
		{
			get
			{
				return (this.properties == null) || (this.properties.Length == 0);
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
		
		public int								CountUsers
		{
			get
			{
				return this.user_count;
			}
		}
		
		
		public void IncrementUserCount()
		{
			Debug.Assert.IsInBounds (this.user_count+1, 1, BaseStyle.MaxUserCount-1);
			this.user_count++;
		}
		
		public void DecrementUserCount()
		{
			Debug.Assert.IsInBounds (this.user_count, 1, BaseStyle.MaxUserCount-1);
			this.user_count--;
		}
		
		
		public virtual void Initialise(System.Collections.ICollection properties)
		{
			//	Insère les propriétés dans notre table interne. Les propriétés
			//	sont toujours triées en s'appuyant sur leur WellKnownType, ce
			//	qui permet une comparaison rapide.
			
			//	De plus, les propriétés les plus souvent utilisées ont une
			//	valeur WellKnownType plus faible, ce qui les place en tête du
			//	tableau et accélère la recherche.
			
			this.properties = new Properties.BaseProperty[properties.Count];
			properties.CopyTo (this.properties, 0);
			
			System.Array.Sort (this.properties, new PropertyComparer ());
			
			this.Invalidate ();
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
		
		
		public Properties.BaseProperty[] FindProperties(System.Type type)
		{
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			if (this.properties != null)
			{
				for (int i = 0; i < this.properties.Length; i++)
				{
					if (this.properties[i].GetType () == type)
					{
						list.Add (this.properties[i]);
					}
				}
			}
			
			Properties.BaseProperty[] props = new Properties.BaseProperty[list.Count];
			list.CopyTo (props);
			
			return props;
		}
		
		public Properties.BaseProperty[] FindProperties(Properties.WellKnownType type)
		{
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			if (this.properties != null)
			{
				for (int i = 0; i < this.properties.Length; i++)
				{
					Properties.WellKnownType found = this.properties[i].WellKnownType;
					
					//	On peut s'arrêter dès que l'on trouve une propriété avec
					//	un WellKnownType plus grand que celui recherché, car la
					//	table est triée :
					
					if (found > type)
					{
						break;
					}
					
					if (found == type)
					{
						list.Add (this.properties[i]);
					}
				}
			}
			
			Properties.BaseProperty[] props = new Properties.BaseProperty[list.Count];
			list.CopyTo (props);
			
			return props;
		}
		
		
		public Styles.BasePropertyContainer.Accumulator StartAccumulation()
		{
			return new Styles.BasePropertyContainer.Accumulator (this);
		}
		
		
		public class Accumulator
		{
			public Accumulator(Styles.BasePropertyContainer host)
			{
				this.host    = host;
				this.hash    = new System.Collections.Hashtable ();
				this.special = false;
			}
			
			
			public bool							RequiresSpecialCodeProcessing
			{
				get
				{
					return this.special;
				}
			}
			
			
			public Accumulator Accumulate(System.Collections.IEnumerable source)
			{
				foreach (Properties.BaseProperty property in source)
				{
					this.Accumulate (property);
				}
				
				return this;
			}
			
			public Accumulator Accumulate(Styles.BasePropertyContainer source)
			{
				int n = source.CountProperties;
				
				for (int i = 0; i < n; i++)
				{
					this.Accumulate (source[i]);
				}
				
				return this;
			}
			
			public Accumulator Accumulate(Properties.BaseProperty property)
			{
				System.Type type = property.GetType ();
				
				if (this.hash.Contains (type))
				{
					Properties.BaseProperty base_prop = this.hash[type] as Properties.BaseProperty;
					Properties.BaseProperty comb_prop = base_prop.GetCombination (property);
					
					this.hash[type] = comb_prop;
				}
				else
				{
					this.hash[type] = property;
				}
				
				if (! this.special)
				{
					this.special = property.RequiresSpecialCodeProcessing;
				}
				
				return this;
			}
			
			
			public void Done()
			{
				this.host.Initialise (this.hash.Values);
				
				this.host = null;
				this.hash = null;
			}
			
			
			Styles.BasePropertyContainer		host;
			System.Collections.Hashtable		hash;
			bool								special;
		}
		
		
		#region IContentsSignatureUpdater Members
		public virtual void UpdateContentsSignature(IO.IChecksum checksum)
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
		#endregion
		
		#region IContentsSignature Members
		public int GetContentsSignature()
		{
			//	Retourne la signature (CRC) correspondant au contenu du style.
			//	La signature exclut les réglages et l'index.
			
			//	Si la signature n'existe pas, il faut la calculer; on ne fait
			//	cela qu'à la demande, car le calcul de la signature peut être
			//	relativement onéreux :
			
			if (this.contents_signature == 0)
			{
				IO.IChecksum checksum = IO.Checksum.CreateAdler32 ();
				
				this.UpdateContentsSignature (checksum);
				
				int signature = (int) checksum.Value;
				
				//	La signature calculée pourrait être nulle; dans ce cas, on
				//	l'ajuste pour éviter d'interpréter cela comme une absence
				//	de signature :
				
				this.contents_signature = (signature == 0) ? 1 : signature;
			}
			
			return this.contents_signature;
		}
		#endregion
		
		#region IEnumerable Members
		public System.Collections.IEnumerator GetEnumerator()
		{
			return this.properties.GetEnumerator ();
		}
		#endregion
		
		protected void ClearContentsSignature()
		{
			this.contents_signature = 0;
		}
		
		
		public static bool CompareEqualContents(Styles.BasePropertyContainer a, Styles.BasePropertyContainer b)
		{
			if (a.properties == b.properties)
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
			
			int n = a.properties.Length;
			
			for (int i = 0; i < n; i++)
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
			
			for (int i = 0; i < n; i++)
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
		
		
		#region PropertyComparer Class
		private class PropertyComparer : System.Collections.IComparer
		{
			#region IComparer Members
			public int Compare(object x, object y)
			{
				Properties.BaseProperty px = x as Properties.BaseProperty;
				Properties.BaseProperty py = y as Properties.BaseProperty;
				
				Properties.WellKnownType wpx = px.WellKnownType;
				Properties.WellKnownType wpy = py.WellKnownType;
				
				if (wpx < wpy)
				{
					return -1;
				}
				if (wpx > wpy)
				{
					return 1;
				}
				
				if (wpx == Epsitec.Common.Text.Properties.WellKnownType.Other)
				{
					string x_name = px.GetType ().Name;
					string y_name = py.GetType ().Name;
					
					return string.Compare (x_name, y_name);
				}
				
				return 0;
			}
			#endregion
		}
		#endregion
		
		private long							version;
		private int								contents_signature;
		
		private Properties.BaseProperty[]		properties;
		private int								user_count;
	}
}
