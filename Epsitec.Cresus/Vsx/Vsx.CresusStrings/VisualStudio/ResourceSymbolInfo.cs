using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Epsitec.Cresus.ResourceManagement;
using Microsoft.VisualStudio.Text;
using Roslyn.Compilers.Common;
using Roslyn.Compilers.CSharp;

namespace Epsitec.VisualStudio
{
	public class ResourceSymbolInfo
	{
		//public ResourceSymbolInfo(List<IReadOnlyDictionary<CultureInfo, ResourceItem>> resources, string symbolName, SyntaxNode node, CommonSyntaxToken token, ITrackingSpan applicableToSpan)
		public ResourceSymbolInfo(List<IReadOnlyDictionary<CultureInfo, ResourceItem>> resources, string symbolName, SyntaxNode node, CommonSyntaxToken token)
		{
			this.resources = resources;
			this.symbolName = symbolName;
			this.syntaxNode = node;
			this.syntaxToken = token;
		}

		public List<IReadOnlyDictionary<CultureInfo, ResourceItem>> Resources
		{
			get
			{
				return this.resources;
			}
		}

		public string SymbolName
		{
			get
			{
				return this.symbolName;
			}
		}

		public SyntaxNode SyntaxNode
		{
			get
			{
				return this.syntaxNode;
			}
		}

		public CommonSyntaxToken SyntaxToken
		{
			get
			{
				return this.syntaxToken;
			}
		}

		private readonly List<IReadOnlyDictionary<CultureInfo, ResourceItem>> resources;
		private readonly string symbolName;
		private readonly SyntaxNode syntaxNode;
		private readonly CommonSyntaxToken syntaxToken;
	}
}
