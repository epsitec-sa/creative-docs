using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epsitec.Cresus.ResourceManagement
{
	public class ResourceMapper : ResourceVisitor
	{
		public CompositeDictionary				Map
		{
			get
			{
				return this.map;
			}
		}
		public IEnumerable<IKey>				DuplicateKeys
		{
			get
			{
				return this.duplicateKeys;
			}
		}
		public IEnumerable<ResourceItemError>	ResourceItemErrors
		{
			get
			{
				return this.resourceItemErrors;
			}
		}
		public SolutionResource					SolutionResource
		{
			get
			{
				return this.solution;
			}
		}

		protected ProjectResource				CurrentProjectResource
		{
			get
			{
				return this.project;
			}
		}
		protected ResourceModule				CurrentResourceModule
		{
			get
			{
				return this.module;
			}
		}
		protected ResourceBundle				CurrentResourceBundle
		{
			get
			{
				return this.bundle;
			}
		}

		/// <summary>
		/// Creates a composite dictionary that contains resource items that
		/// match with the given resource item name tail. The composite key
		/// is a sequence of the resource item symbol and the culture.
		/// </summary>
		/// <param name="symbolTail"></param>
		/// <returns></returns>
		public CompositeDictionary MatchSymbolTail(string symbolTail)
		{
			//return this.MatchSymbolTail (Key.Create (symbolTail));

			var cultureFirstMap = new CompositeDictionary ();
			foreach (var cultureKey in this.map.FirstLevelKeys)
			{
				var symbolsOnlyMap = this.map[cultureKey] as Dictionary<IKey, object>;
				var symbolKeys = ResourceMapper.MatchedTailKeys (symbolsOnlyMap.Keys, symbolTail).ToList ();
				foreach (var symbolKey in symbolKeys)
				{
					cultureFirstMap[Key.Create (cultureKey, symbolKey)] = symbolsOnlyMap[symbolKey] as ResourceItem;
				}
			}
			var symbolFirstMap = new CompositeDictionary ();
			foreach (var cultureKey in cultureFirstMap.FirstLevelKeys)
			{
				var symbolsOnlyMap = CompositeDictionary.Create (cultureFirstMap[cultureKey]);
				foreach (var symbolKey in symbolsOnlyMap.Keys)
				{
					var symbol = string.Join (".", symbolKey.Select (i => i.ToString ()));
					symbolFirstMap[symbol, cultureKey] = symbolsOnlyMap[symbolKey];
				}
			}
			return symbolFirstMap;
		}

		/// <summary>
		/// Creates a composite dictionary that contains resource items that
		/// match with the given partial resource item name. The composite key
		/// is a sequence of the resource item symbol and the culture.
		/// </summary>
		/// <param name="symbolPart"></param>
		/// <returns></returns>
		public CompositeDictionary MatchSymbolPart(string symbolPart)
		{
			return this.MatchSymbolPart (Key.Create (symbolPart));
		}

		protected virtual ICompositeKey CreateItemAccessKey(ResourceItem item)
		{
			return Key.Create (this.GetItemAccessSubkeys (item));
		}

		#region ResourceVisitor Overrides

		public override ResourceNode VisitItem(ResourceItem item)
		{
			item = base.VisitItem (item) as ResourceItem;
			if (item is ResourceItemError)
			{
				this.resourceItemErrors.Add (item as ResourceItemError);
			}

			var key = this.CreateItemAccessKey (item);
			if (this.map.ContainsKey (key))
			{
				this.duplicateKeys.Add (key);
			}
			else
			{
				this.map[key] = item;
			}

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

		#endregion

		private static IEnumerable<IKey> MatchedTailKeys(IEnumerable<IKey> fullKeys, string symbolTail)
		{
			foreach (var fullKey in fullKeys)
			{
				if (ResourceMapper.MatchesTailKey (fullKey, symbolTail))
				{
					yield return fullKey;
				}
			}
		}

		private static bool MatchesTailKey(IKey fullKey, string symbolTail)
		{
			return true;
		}


		private static IEnumerable<IKey> MatchedTailKeys(IEnumerable<IKey> fullKeys, IKey tailKey)
		{
			foreach (var fullKey in fullKeys)
			{
				if (ResourceMapper.MatchesTailKey (fullKey, tailKey))
				{
					yield return fullKey;
				}
			}
		}

		private static IEnumerable<IKey> MatchedPartialKeys(IEnumerable<IKey> fullKeys, IKey tailKey)
		{
			foreach (var fullKey in fullKeys)
			{
				if (ResourceMapper.MatchesPartialKey (fullKey, tailKey))
				{
					yield return fullKey;
				}
			}
		}

		private static bool MatchesTailKey(IKey fullKey, IKey tailKey)
		{
			if (!tailKey.Any ())
			{
				return false;
			}
			var tailEnumerator = tailKey.Reverse ().GetEnumerator ();
			var fullEnumerator = fullKey.Reverse ().GetEnumerator ();

			while (tailEnumerator.MoveNext ())
			{
				if (!fullEnumerator.MoveNext ())
				{
					return false;
				}
				if (!object.Equals (tailEnumerator.Current, fullEnumerator.Current))
				{
					return false;
				}
			}
			return true;
		}

		private static bool MatchesPartialKey(IKey fullKey, IKey partKey)
		{
			if (!partKey.Any ())
			{
				return false;
			}
			var partEnumerator = partKey.Reverse ().GetEnumerator ();
			var fullEnumerator = fullKey.Reverse ().GetEnumerator ();

			if (partEnumerator.MoveNext () && fullEnumerator.MoveNext ())
			{
				while (!object.Equals (fullEnumerator.Current, partEnumerator.Current))
				{
					if (!fullEnumerator.MoveNext ())
					{
						return false;
					}
				}
				while (partEnumerator.MoveNext ())
				{
					if (!fullEnumerator.MoveNext ())
					{
						return false;
					}
					if (!object.Equals (partEnumerator.Current, fullEnumerator.Current))
					{
						return false;
					}
				}
			}
			return true;
		}

		private static CompositeDictionary ToSymbolFirstMap(CompositeDictionary cultureFirstMap)
		{
			var symbolFirstMap = new CompositeDictionary ();
			foreach (var cultureKey in cultureFirstMap.FirstLevelKeys)
			{
				var cultureMap = CompositeDictionary.Create (cultureFirstMap[cultureKey]);
				foreach (var symbolKey in cultureMap.Keys)
				{
					var symbolName = string.Join (".", symbolKey.Select (i => i.ToString ()));
					symbolFirstMap[symbolName, cultureKey] = cultureMap[symbolKey];
				}
			}
			return symbolFirstMap;
		}

		private CompositeDictionary MatchSymbolTail(IKey symbolTailKey)
		{
			var cultureFirstMap = new CompositeDictionary ();
			foreach (var cultureKey in this.map.FirstLevelKeys)
			{
				var symbolsOnlyMap = CompositeDictionary.Create (this.map[cultureKey]);
				var symbolKeys = ResourceMapper.MatchedTailKeys (symbolsOnlyMap.Keys, symbolTailKey).ToList ();
				foreach (var symbolKey in symbolKeys)
				{
					cultureFirstMap[Key.Create (cultureKey, symbolKey)] = symbolsOnlyMap[symbolKey] as ResourceItem;
				}
			}
			var symbolFirstMap = new CompositeDictionary ();
			foreach (var cultureKey in cultureFirstMap.FirstLevelKeys)
			{
				var symbolsOnlyMap = CompositeDictionary.Create (cultureFirstMap[cultureKey]);
				foreach (var symbolKey in symbolsOnlyMap.Keys)
				{
					var symbol = string.Join (".", symbolKey.Select (i => i.ToString ()));
					symbolFirstMap[symbol, cultureKey] = symbolsOnlyMap[symbolKey];
				}
			}
			return symbolFirstMap;
		}

		private CompositeDictionary MatchSymbolPart(IKey symbolPartKey)
		{
			var cultureFirstMap = new CompositeDictionary ();
			foreach (var cultureKey in this.map.FirstLevelKeys)
			{
				var symbolsOnlyMap = CompositeDictionary.Create (this.map[cultureKey]);
				var symbolKeys = ResourceMapper.MatchedPartialKeys (symbolsOnlyMap.Keys, symbolPartKey).ToList ();
				foreach (var symbolKey in symbolKeys)
				{
					cultureFirstMap[Key.Create (cultureKey, symbolKey)] = symbolsOnlyMap[symbolKey] as ResourceItem;
				}
			}
			var symbolFirstMap = new CompositeDictionary ();
			foreach (var cultureKey in cultureFirstMap.FirstLevelKeys)
			{
				var symbolsOnlyMap = CompositeDictionary.Create (cultureFirstMap[cultureKey]);
				foreach (var symbolKey in symbolsOnlyMap.Keys)
				{
					var symbol = string.Join (".", symbolKey.Select (i => i.ToString ()));
					symbolFirstMap[symbol, cultureKey] = symbolsOnlyMap[symbolKey];
				}
			}
			return symbolFirstMap;
		}

		/// <summary>
		/// Resource Item Access Key has following format
		///   bundle.Culture;module.ResourceNamespace;Res;bundle.Name;item.Name
		///   <example>fr-CH;Epsitec.Cisus;Res;Strings;Application.Name</example>
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		private IEnumerable<object> GetItemAccessSubkeys(ResourceItem item)
		{
			if (this.CurrentResourceBundle != null && this.CurrentResourceBundle.Culture != null)
			{
				yield return this.CurrentResourceBundle.Culture;
			}
			yield return string.Join(".", this.GetSymbolSubkeys (item));
		}

		private IEnumerable<string> GetSymbolSubkeys(ResourceItem item)
		{
			if (this.CurrentResourceModule != null && !string.IsNullOrEmpty (this.CurrentResourceModule.Info.ResourceNamespace))
			{
				yield return this.CurrentResourceModule.Info.ResourceNamespace;
			}
			yield return "Res";
			if (this.CurrentResourceBundle != null && !string.IsNullOrEmpty (this.CurrentResourceBundle.Name))
			{
				yield return this.CurrentResourceBundle.Name;
			}
			if (!string.IsNullOrEmpty (item.Name))
			{
				yield return item.Name;
			}
		}

		private readonly CompositeDictionary map = new CompositeDictionary ();
		private readonly HashSet<IKey> duplicateKeys = new HashSet<IKey> ();
		private readonly List<ResourceItemError> resourceItemErrors = new List<ResourceItemError> ();

		private SolutionResource solution;
		private ProjectResource project;
		private ResourceModule module;
		private ResourceBundle bundle;
	}
}
