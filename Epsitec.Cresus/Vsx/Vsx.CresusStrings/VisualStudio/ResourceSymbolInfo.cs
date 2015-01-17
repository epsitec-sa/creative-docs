using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Epsitec.Cresus.ResourceManagement;
using Microsoft.VisualStudio.Text;
using Roslyn.Compilers;
using Roslyn.Compilers.Common;
using Roslyn.Compilers.CSharp;

namespace Epsitec.VisualStudio
{
	public sealed class ResourceSymbolInfo : IEquatable<ResourceSymbolInfo>
	{
		public ResourceSymbolInfo(ITextBuffer textBuffer, CommonSyntaxToken syntaxToken, SyntaxNode syntaxNode, string symbolName, List<MultiCultureResourceItem> resources)
		{
			this.textBuffer = textBuffer;
			this.syntaxToken = syntaxToken;
			this.syntaxNode = syntaxNode;
			this.symbolName = symbolName;
			this.resources = resources;
		}

		public ITextBuffer						TextBuffer
		{
			get
			{
				return this.textBuffer;
			}
		}

		public ITextSnapshot					Snapshot
		{
			get
			{
				return this.textBuffer.CurrentSnapshot;
			}
		}

		public CommonSyntaxToken				SyntaxToken
		{
			get
			{
				return this.syntaxToken;
			}
		}

		public SyntaxNode						SyntaxNode
		{
			get
			{
				return this.syntaxNode;
			}
		}

		public string							SymbolName
		{
			get
			{
				return this.symbolName;
			}
		}

		public TextSpan							TextSpan
		{
			get
			{
				return this.SyntaxNode.Span;
			}
		}

		public Span								Span
		{
			get
			{
				var textSpan = this.TextSpan;
				return Span.FromBounds (textSpan.Start, textSpan.End);
			}
		}

		public SnapshotSpan						SnapshotSpan
		{
			get
			{
				return new SnapshotSpan (this.Snapshot, this.Span);
			}
		}

		public List<MultiCultureResourceItem>	Resources
		{
			get
			{
				return this.resources;
			}
		}

		public static bool operator ==(ResourceSymbolInfo left, ResourceSymbolInfo right)
		{
			return object.Equals (left, right);
		}

		public static bool operator !=(ResourceSymbolInfo left, ResourceSymbolInfo right)
		{
			return !object.Equals (left, right);
		}

		#region Object Overrides

		public override int GetHashCode()
		{
			return this.TextSpan.GetHashCode ();
		}

		public override bool Equals(object obj)
		{
			return this.Equals (obj as ResourceSymbolInfo);
		}

		public override string ToString()
		{
			return symbolName;
		}

		#endregion

		#region IEquatable<ResourceSymbolInfo> Members

		public bool Equals(ResourceSymbolInfo other)
		{
			if (object.ReferenceEquals (this, other))
			{
				return true;
			}
			if (object.ReferenceEquals (other, null))
			{
				return false;
			}
			return this.TextSpan == other.TextSpan;
		}

		#endregion


		private readonly ITextBuffer textBuffer;
		private readonly CommonSyntaxToken syntaxToken;
		private readonly SyntaxNode syntaxNode;
		private readonly string symbolName;
		private readonly List<MultiCultureResourceItem> resources;
	}
}
