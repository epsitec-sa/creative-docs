using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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

		/// <summary>
		/// </summary>
		/// <remarks>
		/// <see cref="DocumentSourceManager.ActiveDteDocument"/> and <see cref="DocumentSourceManager.ActiveTextBuffer"/> cooperate
		/// to assign a text buffer to the active document. They resolve the problem of desynchronized / multiple events.
		/// QuickInfoSourceProvider and EditResourceTaggerProvider have the responsibility to transfer the received text buffer to the active document.
		/// At initialisation, the active document is known before the text buffer has been set, but when the user activate a new document,
		/// the activation event is raised the text buffer has been set.
		/// </remarks>
		internal EnvDTE.Document ActiveDteDocument
		{
			set
			{
				var id = value.FullName.ToLower ();
				bool created;
				this.activeDocument = this.documents.GetOrAdd (id, _ => this.CreateDocumentSource (value), out created);
				if (this.activeTextBufferPendingOwnership != null && (created || this.activeDocument.TrySetPendingTextBuffer (this.activeTextBufferPendingOwnership)))
				{
					this.activeTextBufferPendingOwnership = null;	// ownership transfered to this.activeDocument
				}
			}
		}

		internal ITextBuffer ActiveTextBuffer
		{
			set
			{
				if (this.activeTextBufferPendingOwnership != null)
				{
					throw new InvalidOperationException ("Trying to assign a text buffer while the previous one is still pending for assignment");
				}
				if (!this.activeDocument.TrySetPendingTextBuffer (value))
				{
					// ownership still pending, waiting for activation
					this.activeTextBufferPendingOwnership = value;
				}
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


		private DocumentSource CreateDocumentSource(EnvDTE.Document dteDocument)
		{
			return new DocumentSource (this.solutionProvider, dteDocument, this.activeTextBufferPendingOwnership);
		}

		private readonly ISolutionProvider solutionProvider;
		private readonly Dictionary<string, DocumentSource> documents = new Dictionary<string, DocumentSource> ();
		private DocumentSource activeDocument;
		private ITextBuffer activeTextBufferPendingOwnership;
	}
}
