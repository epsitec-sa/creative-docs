//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Styles
{
	/// <summary>
	/// La classe PropertyContainer contient une série de propriétés de type
	/// Property.
	/// </summary>
	public abstract class PropertyContainer : IContentsSignature, IContentsSignatureUpdater, System.Collections.IEnumerable
	{
		public PropertyContainer()
		{
		}
		
		public PropertyContainer(System.Collections.ICollection properties)
		{
			this.Initialize (properties);
		}
		
		
		
		public Property							this[int index]
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
		
		public Property							this[System.Type type]
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
		
		public Property							this[Properties.WellKnownType type]
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
				return this.version;
			}
		}
		
		public int								UserCount
		{
			get
			{
				return this.userCount;
			}
		}
		
		
		public void IncrementUserCount()
		{
			Debug.Assert.IsInBounds (this.userCount+1, 1, BaseSettings.MaxUserCount-1);
			System.Threading.Interlocked.Increment (ref this.userCount);
		}
		
		public void DecrementUserCount()
		{
			Debug.Assert.IsInBounds (this.userCount, 1, BaseSettings.MaxUserCount-1);
			System.Threading.Interlocked.Decrement (ref this.userCount);
		}
		
		
		internal void Initialize(System.Collections.ICollection properties)
		{
			//	Insère les propriétés dans notre table interne. Les propriétés
			//	sont toujours triées en s'appuyant sur leur WellKnownType, ce
			//	qui permet une comparaison rapide.
			
			//	De plus, les propriétés les plus souvent utilisées ont une
			//	valeur WellKnownType plus faible, ce qui les place en tête du
			//	tableau et accélère la recherche.
			
			if (properties == null)
			{
				this.properties = new Property[0];
			}
			else
			{
				this.properties = new Property[properties.Count];
				properties.CopyTo (this.properties, 0);
				
				System.Array.Sort (this.properties, new PropertyComparer ());
			}
			
			this.Invalidate ();
		}
		
		internal virtual Property[] GetProperties()
		{
			if (this.properties == null)
			{
				return new Property[0];
			}
			else
			{
				return (Property[]) this.properties.Clone ();
			}
		}
		
		
		public void Invalidate()
		{
			this.version = 0;
			this.ClearContentsSignature ();
		}
		
		public virtual bool Update(long currentVersion)
		{
			if (this.version < currentVersion)
			{
				this.version = currentVersion;
				
				return true;
			}
			else
			{
				return false;
			}
		}
		
		
		public int GetGlyphForSpecialCode(ulong code)
		{
			if (this.properties != null)
			{
				for (int i = 0; i < this.properties.Length; i++)
				{
					if (this.properties[i].RequiresSpecialCodeProcessing)
					{
						return properties[i].GetGlyphForSpecialCode (code);
					}
				}
			}
			
			return -1;
		}
		
		public OpenType.Font GetFontForSpecialCode(TextContext context, ulong code)
		{
			if (this.properties != null)
			{
				for (int i = 0; i < this.properties.Length; i++)
				{
					if (this.properties[i].RequiresSpecialCodeProcessing)
					{
						return properties[i].GetFontForSpecialCode (context, code);
					}
				}
			}
			
			return null;
		}
		
		
		public bool Contains(Property property)
		{
			if (this.properties == null)
			{
				return false;
			}
			
			Properties.WellKnownType search = property.WellKnownType;
			
			for (int i = 0; i < this.properties.Length; i++)
			{
				Properties.WellKnownType found = this.properties[i].WellKnownType;
				
				if (found == search)
				{
					//	Trouvé une propriété du même type. Il faut encore vérifier
					//	que le contenu coïncide :
					
					if ((property.GetContentsSignature () == this.properties[i].GetContentsSignature ()) &&
						(property.CompareEqualContents (this.properties[i])))
					{
						return true;
					}
				}
				
				//	On peut s'arrêter dès que l'on trouve une propriété avec
				//	un WellKnownType plus grand que celui recherché, car la
				//	table est triée :
				
				if (found > search)
				{
					break;
				}
			}
			
			return false;
		}
		
		public bool Contains(Properties.WellKnownType type)
		{
			if (this.properties == null)
			{
				return false;
			}
			
			for (int i = 0; i < this.properties.Length; i++)
			{
				Properties.WellKnownType found = this.properties[i].WellKnownType;
				
				if (found == type)
				{
					return true;
				}
				
				//	On peut s'arrêter dès que l'on trouve une propriété avec
				//	un WellKnownType plus grand que celui recherché, car la
				//	table est triée :
				
				if (found > type)
				{
					break;
				}
			}
			
			return false;
		}
		
		
		public Property[] FindProperties(System.Type type)
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
			
			Property[] props = new Property[list.Count];
			list.CopyTo (props);
			
			return props;
		}
		
		public Property[] FindProperties(Properties.WellKnownType type)
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
			
			Property[] props = new Property[list.Count];
			list.CopyTo (props);
			
			return props;
		}
		
		public Property[] FindProperties(params Properties.WellKnownType[] types)
		{
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			if ((this.properties != null) &&
				(types.Length > 0))
			{
				Properties.WellKnownType maxType = Properties.WellKnownType.Undefined;
				
				for (int i = 0; i < types.Length; i++)
				{
					if (types[i] > maxType)
					{
						maxType = types[i];
					}
				}
				
				for (int i = 0; i < this.properties.Length; i++)
				{
					Properties.WellKnownType found = this.properties[i].WellKnownType;
					
					//	On peut s'arrêter dès que l'on trouve une propriété avec
					//	un WellKnownType plus grand que celui recherché, car la
					//	table est triée :
					
					if (found > maxType)
					{
						break;
					}
					
					for (int j = 0; j < types.Length; j++)
					{
						if (found == types[j])
						{
							list.Add (this.properties[i]);
							break;
						}
					}
				}
			}
			
			Property[] props = new Property[list.Count];
			list.CopyTo (props);
			
			return props;
		}
		
		
		public void Flatten(System.Collections.ArrayList list)
		{
			if (this.properties != null)
			{
				for (int i = 0; i < this.properties.Length; i++)
				{
					list.Add (this.properties[i]);
				}
			}
		}
		
		
		public Styles.PropertyContainer.Accumulator StartAccumulation()
		{
			return new Styles.PropertyContainer.Accumulator (this);
		}
		
		
		#region Accumulator Class
		public class Accumulator
		{
			public Accumulator() : this (null)
			{
			}
			
			public Accumulator(Styles.PropertyContainer host)
			{
				this.host    = host;
				this.hash    = new System.Collections.Hashtable ();
				this.special = false;
			}
			
			
			public bool							IsEmpty
			{
				get
				{
					return ((this.hash == null) || (this.hash.Count == 0))
						&& ((this.list == null) || (this.list.Count == 0));
				}
			}
			
			public bool							RequiresSpecialCodeProcessing
			{
				get
				{
					return this.special;
				}
			}
			
			public bool							SkipSymbolProperties
			{
				get
				{
					return this.skipSymbolProperties;
				}
				set
				{
					this.skipSymbolProperties = value;
				}
			}
			
			public Property[]					AccumulatedProperties
			{
				get
				{
					System.Collections.ArrayList list = new System.Collections.ArrayList ();
					
					if (this.list != null) list.AddRange (this.list);
					if (this.hash != null) list.AddRange (this.hash.Values);
					
					Property[] properties = new Property[list.Count];
					list.CopyTo (properties);
					
					return properties;
				}
			}
			
			
			public Accumulator Accumulate(System.Collections.IEnumerable source)
			{
				foreach (Property property in source)
				{
					this.Accumulate (property);
				}
				
				return this;
			}
			
			public Accumulator Accumulate(Styles.PropertyContainer source)
			{
				int n = source.CountProperties;
				
				for (int i = 0; i < n; i++)
				{
					this.Accumulate (source[i]);
				}
				
				return this;
			}
			
			public Accumulator Accumulate(Property property)
			{
				if ((this.skipSymbolProperties) &&
					(property.PropertyAffinity == Properties.PropertyAffinity.Symbol))
				{
					return this;
				}
				
				System.Type type = property.GetType ();
				
				if (property.CombinationMode == Properties.CombinationMode.Accumulate)
				{
					if (this.list == null)
					{
						this.list = new System.Collections.ArrayList ();
					}
					
					this.list.Add (property);
				}
				else
				{
					if (this.hash.Contains (type))
					{
						Property baseProp = this.hash[type] as Property;
						Property combProp = baseProp.GetCombination (property);
						
						this.hash[type] = combProp;
					}
					else
					{
						this.hash[type] = property;
					}
				}
				
				if (! this.special)
				{
					this.special = property.RequiresSpecialCodeProcessing;
				}
				
				return this;
			}
			
			
			public void Done()
			{
				if (this.host != null)
				{
					if (this.list != null)
					{
						this.list.AddRange (this.hash.Values);
						this.host.Initialize (this.list);
					}
					else
					{
						this.host.Initialize (this.hash.Values);
					}
				}
				
				this.host = null;
				this.hash = null;
				this.list = null;
			}
			
			
			Styles.PropertyContainer			host;
			System.Collections.Hashtable		hash;
			System.Collections.ArrayList		list;
			bool								special;
			bool								skipSymbolProperties;
		}
		#endregion
		
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
		
		#region IEnumerable Members
		public System.Collections.IEnumerator GetEnumerator()
		{
			return this.properties.GetEnumerator ();
		}
		#endregion
		
		internal void SerializeProperties(System.Text.StringBuilder buffer)
		{
			buffer.Append (SerializerSupport.SerializeInt (this.properties.Length));
			buffer.Append ("/");
			buffer.Append (SerializerSupport.SerializeInt (0));
			
			for (int i = 0; i < this.properties.Length; i++)
			{
				buffer.Append ("/");
				buffer.Append (SerializerSupport.SerializeString (Property.Serialize (this.properties[i])));
			}
		}
		
		internal void DeserializeProperties(TextContext context, int version, string[] source, ref int index)
		{
			int length = SerializerSupport.DeserializeInt (source[index++]);
			int users  = SerializerSupport.DeserializeInt (source[index++]);
			
			this.properties = new Property[length];
			this.userCount = users;
			
			for (int i = 0; i < length; i++)
			{
				string definition = SerializerSupport.DeserializeString (source[index++]);
				this.properties[i] = Property.Deserialize (context, version, definition);
			}
		}
		
		
		protected void ClearContentsSignature()
		{
			this.contentsSignature = 0;
		}
		
		
		public static bool CompareEqualContents(Styles.PropertyContainer a, Styles.PropertyContainer b)
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
				Property pa = a.properties[i];
				Property pb = b.properties[i];
				
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
				Property pa = a.properties[i];
				Property pb = b.properties[i];
				
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
				Property px = x as Property;
				Property py = y as Property;
				
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
					string xName = px.GetType ().Name;
					string yName = py.GetType ().Name;
					
					return string.Compare (xName, yName);
				}
				
				return 0;
			}
			#endregion
		}
		#endregion
		
		private long							version;
		private int								contentsSignature;
		
		private Property[]						properties;
		private int								userCount;
	}
}
