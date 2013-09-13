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
		public ResourceSymbolInfo(List<IReadOnlyDictionary<CultureInfo, ResourceItem>> resources, string symbolName, SyntaxNode node, CommonSyntaxToken token, ITrackingSpan applicableToSpan)
		{
			this.resources = resources;
			this.symbolName = symbolName;
			this.node = node;
			this.token = token;
			this.applicableToSpan = applicableToSpan;
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

		public SyntaxNode Node
		{
			get
			{
				return this.node;
			}
		}

		public CommonSyntaxToken Token
		{
			get
			{
				return this.token;
			}
		}

		public ITrackingSpan ApplicableToSpan
		{
			get
			{
				return this.applicableToSpan;
			}
		}

		private readonly List<IReadOnlyDictionary<CultureInfo, ResourceItem>> resources;
		private readonly string symbolName;
		private readonly SyntaxNode node;
		private readonly CommonSyntaxToken token;
		private readonly ITrackingSpan applicableToSpan;
	}
}
