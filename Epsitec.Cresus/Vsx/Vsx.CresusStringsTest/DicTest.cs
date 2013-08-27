using System;
using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.ResourceManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Roslyn.Services;

namespace Epsitec.Cresus.Strings
{
	public class Dic : IDictionary<string, object>
	{
		public Dic()
		{
			this.map = new Dictionary<string, object> ();
		}

		public Dic(Dictionary<string, object> map)
		{
			this.map = map;
		}

		#region IDictionary<string,object> Members

		public ICollection<string> Keys
		{
			get
			{
				return Dic.GetKeys (null, this.map).ToList ();
			}
		}

		public ICollection<object> Values
		{
			get
			{
				return Dic.Flatten(this.map).ToList();
			}
		}

		public object this[string key]
		{
			get
			{
				object value;
				if (this.TryGetValue(key, out value))
				{
					return value;
				}
				throw new KeyNotFoundException (key);
			}
			set
			{
				this.Add (key, value);
			}
		}

		public bool ContainsKey(string key)
		{
			var subkeys = key.Split ('.');
			var map = this.map;
			foreach (var subkey in subkeys)
			{
				if (map == null || !map.ContainsKey (subkey))
				{
					return false;
				}
				map = map[subkey] as Dictionary<string, object>;
			}
			return true;
		}

		public bool TryGetValue(string key, out object value)
		{
			return TryGetValue (key.Split ('.'), out value);
		}

		public void Add(string key, object value)
		{
			var subkeys = new HeadAndLast(key.Split ('.'));
			var map = this.map;
			foreach (var subkey in subkeys.Head)
			{
				map = map.GetOrAdd (subkey, _ => new Dictionary<string, object> ()) as Dictionary<string, object>;
			}
			map[subkeys.Last] = value;
		}

		public bool Remove(string key)
		{
			var subkeys = new Dic.HeadAndLast(key.Split ('.'));
			object value;
			if (this.TryGetValue (subkeys.Head, out value))
			{
				var map = value as IDictionary<string, object>;
				if (map != null)
				{
					return map.Remove (subkeys.Last);
				}
			}
			return false;
		}

		#endregion

		#region ICollection<KeyValuePair<string,object>> Members

		public void Add(KeyValuePair<string, object> item)
		{
			this.Add (item.Key, item.Value);
		}

		public void Clear()
		{
			this.map.Clear ();
		}

		public bool Contains(KeyValuePair<string, object> item)
		{
			object value;
			return this.TryGetValue (item.Key, out value) && object.Equals (value, item.Value);

		}

		public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
		{
			this.ToArray ().CopyTo (array, arrayIndex);
		}

		public int Count
		{
			get
			{
				return Keys.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public bool Remove(KeyValuePair<string, object> item)
		{
			var subkeys = new Dic.HeadAndLast (item.Key.Split ('.'));
			object value;
			if (this.TryGetValue (subkeys.Head, out value))
			{
				var map = value as IDictionary<string, object>;
				if (map != null && map.TryGetValue(subkeys.Last, out value) && object.Equals(value, item.Value))
				{
					return map.Remove (subkeys.Last);
				}
			}
			return false;
		}

		#endregion

		#region IEnumerable<KeyValuePair<string,object>> Members

		public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
		{
			return Dic.GetKeyValuePairs (null, this.map).GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator ();
		}

		#endregion


		private class HeadAndLast
		{
			public HeadAndLast(string[] array)
			{
				var lastIndex = array.Length - 1;
				this.Head = array.Take (lastIndex);
				this.Last = array[lastIndex];
			}

			public IEnumerable<string> Head
			{
				get;
				private set;
			}

			public string Last
			{
				get;
				private set;
			}
		}

		private static IEnumerable<string> GetKeys(string rootKey, IDictionary<string, object> map)
		{
			foreach (var kv in map)
			{
				var newMap = kv.Value as IDictionary<string, object>;
				var newKey = string.IsNullOrEmpty (rootKey) ? kv.Key : rootKey + '.' + kv.Key;
				if (newMap == null)
				{
					yield return newKey;
				}
				else
				{
					foreach (var key in GetKeys (newKey, newMap))
					{
						yield return key;
					}
				}
			}
		}

		private static IEnumerable<KeyValuePair<string, object>> GetKeyValuePairs(string rootKey, IDictionary<string, object> map)
		{
			foreach (var kv in map)
			{
				var newMap = kv.Value as IDictionary<string, object>;
				var newKey = string.IsNullOrEmpty (rootKey) ? kv.Key : rootKey + '.' + kv.Key;
				if (newMap == null)
				{
					yield return kv;
				}
				else
				{
					foreach (var kv1 in GetKeyValuePairs (newKey, newMap))
					{
						yield return kv1;
					}
				}
			}
		}

		private bool TryGetValue(IEnumerable<string> subkeys, out object value)
		{
			value = null;
			var map = this.map;
			foreach (var subkey in subkeys)
			{
				if (map == null || !map.TryGetValue (subkey, out value))
				{
					return false;
				}
				map = value as Dictionary<string, object>;
			}
			return true;
		}

		private static IEnumerable<object> Flatten(IDictionary<string, object> map)
		{
			foreach (var value in map.Values)
			{
				map = value as IDictionary<string, object>;
				if (map == null)
				{
					yield return value;
				}
				else
				{
					foreach (var v in Dic.Flatten (map))
					{
						yield return v;
					}
				}
			}

		}

		private readonly Dictionary<string, object> map;
	}

	[TestClass]
	public class DicTest
	{
		[TestMethod]
		public void Keys()
		{
			var dic = CreateDic1 ();
			var keys = dic.Keys;
		}

		[TestMethod]
		public void Values()
		{
			var dic = CreateDic1 ();
			var values = dic.Values;
		}

		[TestMethod]
		public void Indexer()
		{
			var dic = CreateDic1 ();

			var value1 = dic["Strings.Application.Name.fr"];
			Assert.AreEqual ("Strings.Application.Name.fr", value1);

			dic["Strings.Application.Name.fr"] = "new value";
			Assert.AreEqual (dic["Strings.Application.Name.fr"], "new value");

			dic["Strings.Application.Name.en"] = "english";
			Assert.AreEqual (dic["Strings.Application.Name.en"], "english");
		}

		[TestMethod]
		public void ContainsKey()
		{
			var dic = CreateDic1 ();
			Assert.IsTrue (dic.ContainsKey ("Strings"));
			Assert.IsTrue (dic.ContainsKey ("Strings.Application.Name.fr"));
			Assert.IsFalse (dic.ContainsKey ("Strings.Application.Name.fr.xxx"));
			Assert.IsFalse (dic.ContainsKey ("Strings.Application.Name.en"));
			Assert.IsFalse (dic.ContainsKey (string.Empty));
		}

		[TestMethod]
		public void TryGetValue()
		{
			var dic = CreateDic1 ();
			object value;
			Assert.IsTrue (dic.TryGetValue ("Strings.Application.Name.fr", out value));
			Assert.AreEqual (value, "Strings.Application.Name.fr");
			Assert.IsTrue (dic.TryGetValue ("Strings.Application.Name", out value));
			Assert.IsTrue (value is IDictionary<string, object>);

			Assert.IsFalse (dic.TryGetValue ("Strings.Application.Name.fr.xxx", out value));
			Assert.IsFalse (dic.TryGetValue ("Strings.Application.Name.en", out value));
			Assert.IsFalse (dic.TryGetValue (string.Empty, out value));
		}

		[TestMethod]
		public void Remove()
		{
			var dic = CreateDic1 ();
			var keys0 = dic.Keys;
			Assert.IsTrue (dic.Remove ("Strings.Application.Name.fr"));
			var keys1 = dic.Keys;
			var diff1 = keys0.Except (keys1).Single ();
			Assert.AreEqual ("Strings.Application.Name.fr", diff1);

			Assert.IsFalse (dic.Remove ("Strings.Application.Name.fr.xxx"));
			var keys2 = dic.Keys;
			var diff2 = keys1.Except (keys2);
			Assert.AreEqual (0, diff2.Count ());
		}

		[TestMethod]
		public void SolutionDic()
		{
			var dic = CreateDic2 ();
		}

		private static Dic CreateDic1()
		{
			var dic = new Dic ();
			dic.Add ("Strings.Application.Name.fr", "Strings.Application.Name.fr");
			dic.Add ("Strings.Application.Name.de", "Strings.Application.Name.de");
			dic.Add ("Strings.Root", "Strings.Root");
			return dic;
		}

		private static Dic CreateDic2()
		{
			var workspace = Workspace.LoadSolution (TestData.CresusGraphSolutionPath);
			var solution = workspace.CurrentSolution;
			var solutionResource = new SolutionResource (solution);
			var visitor = new MappingVisitor ();
			visitor.VisitSolution (solutionResource);
			return new Dic (visitor.map);
		}

		private class MappingVisitor : ResourceVisitor
		{
			public override ResourceNode VisitItem(ResourceItem item)
			{
				item = base.VisitItem (item) as ResourceItem;

				var subkeys = new string[] { this.bundle.Name }.Concat (item.Name.Split ('.'));

				var map = this.map;
				foreach (var subkey in subkeys)
				{
					map = map.GetOrAdd (subkey, key => new Dictionary<string, object> ()) as Dictionary<string, object>;
				}
				if (map.ContainsKey (this.bundle.Culture))
				{
					throw new InvalidOperationException ("Duplicate strings");
				}
				map[this.bundle.Culture] = item;
				return item;
			}

			public override ResourceNode VisitBundle(ResourceBundle bundle)
			{
				this.bundle = bundle;
				return base.VisitBundle (bundle);
			}

			public override ResourceNode VisitModule(ResourceModule module)
			{
				this.module = module;
				return base.VisitModule (module);
			}

			public override ResourceNode VisitProject(ProjectResource project)
			{
				this.project = project;
				return base.VisitProject (project);
			}

			public override ResourceNode VisitSolution(SolutionResource solution)
			{
				this.solution = solution;
				return base.VisitSolution (solution);
			}

			public readonly Dictionary<string, object> map = new Dictionary<string, object> ();

			private SolutionResource solution;
			private ProjectResource project;
			private ResourceModule module;
			private ResourceBundle bundle;
		}
	}
}
