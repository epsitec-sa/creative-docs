//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets.Helpers
{
	/// <summary>
	/// VisualTreeSnapshot.
	/// </summary>
	public class VisualTreeSnapshot
	{
		public VisualTreeSnapshot()
		{
		}
		
		
		public void Record(Visual visual, Types.Property property)
		{
			if (property == null)
			{
				//	Enregistre l'état de toutes les propriétés héritées.
				
				System.Type type = visual.GetType ();
				
				foreach (Types.Property prop in visual.ObjectType.GetProperties ())
				{
					VisualPropertyMetadata metadata = prop.GetMetadata (type) as VisualPropertyMetadata;
					
					if ((metadata != null) &&
						(metadata.InheritsValue))
					{
						Key key = new Key (visual, property, this.data.Count);
						this.data[key] = visual.GetValue (property);
					}
				}
			}
			else
			{
				//	Enregistre uniquement l'état de la propriété spécifée.
				
				Key key = new Key (visual, property, this.data.Count);
				this.data[key] = visual.GetValue (property);
			}
		}
		
		public void InvalidateDifferent()
		{
			Key[] keys = new Key[this.data.Count];
			
			this.data.Keys.CopyTo (keys, 0);
			System.Array.Sort (keys, new Key.Comparer ());
			
			for (int i = 0; i < keys.Length; i++)
			{
				Key key = keys[i];
				
				object old_value = this.data[key];
				object new_value = key.Visual.GetValue (key.Property);
				
				if (old_value == new_value)
				{
					//	C'est exactement la même valeur -- on ne signale donc rien ici.
				}
				else if ((old_value == null) || (! old_value.Equals (new_value)))
				{
					key.Visual.InvalidateProperty (key.Property, old_value, new_value);
				}
			}
		}
		
		
		#region Key Class
		private class Key
		{
			public Key(Visual visual, Types.Property property, int rank)
			{
				this.visual   = visual;
				this.property = property;
				this.rank     = rank;
			}
			
			
			public Visual						Visual
			{
				get
				{
					return this.visual;
				}
			}
			
			public Types.Property				Property
			{
				get
				{
					return this.property;
				}
			}
			
			public int							Rank
			{
				get
				{
					return this.rank;
				}
			}
			
			
			public override int GetHashCode()
			{
				return this.visual.GetHashCode () ^ this.property.GetHashCode ();
			}
			
			public override bool Equals(object obj)
			{
				Key key = obj as Key;
				
				return (this.visual == key.visual) && (this.property == key.property);
			}
			
			
			public class Comparer : System.Collections.IComparer
			{
				public int Compare(object x, object y)
				{
					Key kx = x as Key;
					Key ky = y as Key;
					
					return kx.Rank - ky.Rank;
				}
			}
			
			
			private Visual						visual;
			private Types.Property				property;
			private int							rank;
		}
		#endregion
		
		private System.Collections.Hashtable	data = new System.Collections.Hashtable ();
	}
}
