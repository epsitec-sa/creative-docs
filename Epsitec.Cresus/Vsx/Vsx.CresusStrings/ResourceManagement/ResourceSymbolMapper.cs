using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Epsitec.Cresus.ResourceManagement
{
	/// <summary>
	/// Organize Cresus Strings resources as a two levels dictionary of ResourceItemContext.
	/// The first key is the semantic symbol of the resource in C# syntax
	/// The second key is the culture associated with the resource item
	/// </summary>
	/// <example>
	/// <code>
	/// var mapper = new ResourceSymbolMapper();
	/// mapper.VisitSolution(solutionResource);
	/// var resourceItemContext = mapper.SymbolTable["Epsitec.Cresus.Strings.Message.MoreThanPiccolo"][CultureInfo.CurrentUICulture];
	/// </code>
	/// </example>
	public class ResourceSymbolMapper : ResourceVisitor
	{
		/// <summary>
		/// <example>
		/// <code>
		/// var mapper = new ResourceSymbolMapper();
		/// mapper.VisitSolution(solutionResource);
		/// var memo = mapper.SymbolTable[symbol][culture];
		/// </code>
		/// </example>
		/// </summary>
		//public IReadOnlyDictionary<string, Dictionary<CultureInfo, ResourceItemContext>> SymbolTable
		//{
		//	get
		//	{
		//		return this.symbolTable;
		//	}
		//}

		/// <summary>
		/// <example>
		/// <code>
		/// var mapper = new ResourceSymbolMapper();
		/// mapper.VisitSolution(solutionResource);
		/// resourceTables = mapper.FindAll();
		/// resources = resourceTables.SelectMany(kv => kv.Values);
		/// </code>
		/// </example>
		/// </summary>
		public IEnumerable<IReadOnlyDictionary<CultureInfo, ResourceItem>> FindAll()
		{
			return this.symbolTable.Values;
		}

		/// <summary>
		/// <example>
		/// <code>
		/// var mapper = new ResourceSymbolMapper();
		/// mapper.VisitSolution(solutionResource);
		/// resourceTables = mapper.FindPartial("Strings.Message");
		/// resources = resourceTables.SelectMany(kv => kv.Values);
		/// </code>
		/// </example>
		/// </summary>
		public IEnumerable<IReadOnlyDictionary<CultureInfo, ResourceItem>> FindPartial(string symbolPart, CancellationToken cancellationToken)
		{
			// Prefix pattern
			// (?<=			: match prefix but exclude it (throws away backtracking references)
			//    \.|^		: dot character or beginning of string
			// )
			// Suffix pattern
			// (?=			: match suffix but exclude it (throws away backtracking references)
			//    \.|$		: dot character or end of string
			// )
			var pattern = @"(?<=\.|^)" + Regex.Escape (symbolPart) + @"(?=\.|$)";
			foreach (var kv in this.symbolTable)
			{
				cancellationToken.ThrowIfCancellationRequested ();
				if (Regex.IsMatch (kv.Key, pattern))
				{
					cancellationToken.ThrowIfCancellationRequested ();
					yield return kv.Value;
				}
			}
		}

		/// <summary>
		/// <example>
		/// <code>
		/// var mapper = new ResourceSymbolMapper();
		/// mapper.VisitSolution(solutionResource);
		/// resourceTables = mapper.FindTail("Dialog.Tooltip.Close");
		/// resources = resourceTables.SelectMany(kv => kv.Values);
		/// </code>
		/// </example>
		/// </summary>
		public IEnumerable<IReadOnlyDictionary<CultureInfo, ResourceItem>> FindTail(string symbolTail, CancellationToken cancellationToken)
		{
			// Prefix pattern
			// (?<=			: match prefix but exclude it (throws away backtracking references)
			//    \.|^		: dot character or beginning of string
			// )
			// Suffix pattern
			// (?=			: match suffix but exclude it (throws away backtracking references)
			//    $			: end of string
			// )
			var pattern = @"(?<=\.|^)" + Regex.Escape (symbolTail) + @"(?=$)";
			foreach (var kv in this.symbolTable)
			{
				cancellationToken.ThrowIfCancellationRequested ();
				if (Regex.IsMatch (kv.Key, pattern))
				{
					cancellationToken.ThrowIfCancellationRequested ();
					yield return kv.Value;
				}
			}
		}

		#region ResourceVisitor Overrides

		public override ResourceNode VisitItem(ResourceItem item)
		{
			item = base.VisitItem (item) as ResourceItem;
			var symbol = item.SymbolName;

			Dictionary<CultureInfo, ResourceItem> cultureTable = null;
			if (!this.symbolTable.TryGetValue (symbol, out cultureTable))
			{
				cultureTable = new Dictionary<CultureInfo, ResourceItem> ();
				this.symbolTable[symbol] = cultureTable;
			}

			var culture = this.bundle.Culture;
			ResourceItem oldItem;
			if (cultureTable.TryGetValue (culture, out oldItem))
			{
				// TODO: process duplicate items
				// add memo { project, module, bundle, item } to duplicateTable[symbol][culture] list
			}
			else
			{
				cultureTable[culture] = item;
			}

			//if (item is ResourceItemError)
			//{
			//	this.resourceItemErrors.Add (item as ResourceItemError);
			//}

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

		// var resourceItemContext = this.symbolTable[symbol][culture]
		private readonly Dictionary<string, Dictionary<CultureInfo, ResourceItem>> symbolTable = new Dictionary<string, Dictionary<CultureInfo, ResourceItem>> ();

		private SolutionResource solution;
		private ProjectResource project;
		private ResourceModule module;
		private ResourceBundle bundle;
	}
}
