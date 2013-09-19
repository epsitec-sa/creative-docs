using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;

namespace Epsitec.VisualStudio
{
	public class DocumentSourceManager : IDisposable
	{
		public DocumentSourceManager(ISolutionProvider solutionProvider)
		{
			this.solutionProvider = solutionProvider;
		}

		public DocumentSource ActiveDocumentSource
		{
			get
			{
				return this.activeDocument;
			}
		}

		internal void SetActiveDocument (EnvDTE.Document dteDocument, ITextBuffer textBuffer = null)
		{
			var id = dteDocument.FullName.ToLower ();
			this.activeDocument = this.documents.GetOrAdd (id, _ => new DocumentSource (this.solutionProvider, dteDocument));
			if (textBuffer != null)
			{
				this.activeDocument.TextBuffer = textBuffer;
			}
		}


		#region IDisposable Members

		public void Dispose()
		{
			foreach (var document in this.documents.Values)
			{
				document.Dispose ();
			}
		}

		#endregion


		private readonly ISolutionProvider solutionProvider;
		private readonly Dictionary<string, DocumentSource> documents = new Dictionary<string, DocumentSource> ();
		private DocumentSource activeDocument;
	}
}
