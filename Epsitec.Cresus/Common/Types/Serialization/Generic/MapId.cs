//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types.Serialization.Generic
{
	/// <summary>
	/// The generic MapId class is used to record objects and associate them with
	/// unique identifiers. These identifiers are needed when serializing object
	/// graphs.
	/// </summary>
	public class MapId<T> where T : class
	{
		public MapId()
		{
			this.idToValueLookup.Add (null);
			this.types.Add (null);
		}

		public IEnumerable<System.Type>			RecordedTypes
		{
			get
			{
				return this.types;
			}
		}
		public IEnumerable<T>					RecordedValues
		{
			get
			{
				return this.idToValueLookup;
			}
		}

		public int								TypeCount
		{
			get
			{
				return this.types.Count;
			}
		}
		public int								ValueCount
		{
			get
			{
				return this.idToValueLookup.Count;
			}
		}
		
		
		public bool Record(T value)
		{
			//	Record the value and associate an id to the object.
			//	Record also the value's type.
			
			//	Return true if the object leads to a new definition,
			//	false if the object was already known.
			
			if (value == null)
			{
				//	Nothing to do: null values are remembered as null. No need to
				//	record them.
				
				return false;
			}
			else if (this.IsValueDefined (value))
			{
				//	Nothing to do: the value has already been registered before.

				return false;
			}
			else
			{
				int id = this.idToValueLookup.Count;
				System.Type type = value.GetType ();
				
				this.idToValueLookup.Add (value);
				this.valueToIdLookup[value] = id;

				if (this.typeInformation.ContainsKey (type) == false)
				{
					this.types.Add (type);
					this.typeInformation[type] = new List<int> ();
				}

				this.typeInformation[type].Add (id);
				
				return true;
			}
		}
		public void RecordType(System.Type type)
		{
			//	Record the specified type, if it was not yet known.
			
			if (this.typeInformation.ContainsKey (type) == false)
			{
				this.types.Add (type);
				this.typeInformation[type] = new List<int> ();
			}
		}

		public bool IsValueDefined(T value)
		{
			if (value == null)
			{
				return true;
			}
			else if (this.valueToIdLookup.ContainsKey (value))
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		public bool IsIdDefined(int id)
		{
			if ((id >= 0) && (id < this.idToValueLookup.Count))
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		public bool IsTypeDefined(System.Type type)
		{
			return this.typeInformation.ContainsKey (type);
		}

		public int GetNullId()
		{
			return 0;
		}
		public int GetId(T value)
		{
			if (value == null)
			{
				return 0;
			}
			else if (this.IsValueDefined (value))
			{
				return this.valueToIdLookup[value];
			}
			else
			{
				return -1;
			}
		}
		public T GetValue(int id)
		{
			if (this.IsIdDefined (id))
			{
				return this.idToValueLookup[id];
			}
			else
			{
				throw new System.Collections.Generic.KeyNotFoundException (string.Format ("Id {0} not found.", id));
			}
		}
		public System.Type GetType(int index)
		{
			return this.types[index];
		}
		public int GetTypeIndex(System.Type type)
		{
			return this.types.IndexOf (type);
		}

		public IEnumerable<T> GetValues(System.Type type)
		{
			List<int> list;
			if (this.typeInformation.TryGetValue (type, out list))
			{
				foreach (int id in list)
				{
					yield return this.idToValueLookup[id];
				}
			}
		}
		
		List<T>									idToValueLookup = new List<T> ();
		Dictionary<T, int>						valueToIdLookup = new Dictionary<T, int> ();
		List<System.Type>						types = new List<System.Type> ();
		Dictionary<System.Type, List<int>>		typeInformation = new Dictionary<System.Type, List<int>> ();
	}
}
