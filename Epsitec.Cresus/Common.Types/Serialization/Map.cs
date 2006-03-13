//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types.Serialization
{
	/// <summary>
	/// The Map class is used to record Dependency Objects and associate them with
	/// unique identifiers. These identifiers are needed when serializing object
	/// graphs.
	/// </summary>
	public class Map
	{
		public Map()
		{
			this.idToObjectLookup.Add (null);
		}

		public IEnumerable<System.Type>			RecordedTypes
		{
			get
			{
				foreach (KeyValuePair<System.Type, List<int>> pair in this.typeInformation)
				{
					yield return pair.Key;
				}
			}
		}
		public IEnumerable<DependencyObject>	RecordedObjects
		{
			get
			{
				return this.idToObjectLookup;
			}
		}
		
		public void Register(DependencyObject obj)
		{
			if (obj == null)
			{
				//	Nothing to do: null objects are remembered as null. No need to
				//	record them.
			}
			else if (this.IsDefined (obj))
			{
				//	Nothing to do: the object has already been registered before.
			}
			else
			{
				int id = this.nextId++;
				System.Type type = obj.GetType ();
				
				this.idToObjectLookup[id]  = obj;
				this.objectToIdLookup[obj] = id;

				if (this.typeInformation.ContainsKey (type) == false)
				{
					this.typeInformation[type] = new List<int> ();
				}

				this.typeInformation[type].Add (id);
			}
		}

		public bool IsDefined(DependencyObject obj)
		{
			if (obj == null)
			{
				return true;
			}
			else if (this.objectToIdLookup.ContainsKey (obj))
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		public bool IsDefined(int id)
		{
			if ((id >= 0) && (id < this.idToObjectLookup.Count))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public int GetNullId()
		{
			return 0;
		}
		public int GetId(DependencyObject obj)
		{
			if (this.IsDefined (obj))
			{
				return this.objectToIdLookup[obj];
			}
			else
			{
				return -1;
			}
		}
		public DependencyObject GetObject(int id)
		{
			if (this.IsDefined (id))
			{
				return this.idToObjectLookup[id];
			}
			else
			{
				throw new System.Collections.Generic.KeyNotFoundException (string.Format ("Id {0} not found.", id));
			}
		}

		public IEnumerable<DependencyObject> GetObjects(System.Type type)
		{
			if (this.typeInformation.ContainsKey (type))
			{
				foreach (int id in this.typeInformation[type])
				{
					yield return this.idToObjectLookup[id];
				}
			}
		}
		
		List<DependencyObject>					idToObjectLookup = new List<DependencyObject> ();
		Dictionary<DependencyObject, int>		objectToIdLookup = new Dictionary<DependencyObject, int> ();
		Dictionary<System.Type, List<int>>		typeInformation = new Dictionary<System.Type, List<int>> ();
		int										nextId = 1;
	}
}
