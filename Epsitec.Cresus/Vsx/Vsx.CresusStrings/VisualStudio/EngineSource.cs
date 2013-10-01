using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;

namespace Epsitec.VisualStudio
{
	[Export (typeof (EngineSource))]
	public class EngineSource : IDisposable
	{
		public EngineSource()
		{
			this.engine = new Engine ();
		}

		[Import]
		public Epsitec.VisualStudio.DTE			DTE
		{
			get
			{
				return this.dte;
			}
			set
			{
				this.dte = value;
				this.dte.WindowActivated += this.OnDTEWindowActivated;
			}
		}

		public Engine							Engine
		{
			get
			{
				return this.engine;
			}
		}


		internal ITextBuffer					ActiveTextBuffer
		{
			set
			{
				this.engine.SetActiveDocument (this.dte.Application.ActiveDocument, value);
			}
		}


		#region IDisposable Members

		public void Dispose()
		{
			this.dte.WindowActivated -= this.OnDTEWindowActivated;
			this.engine.Dispose ();
		}

		#endregion


		private void OnDTEWindowActivated(EnvDTE.Window GotFocus, EnvDTE.Window LostFocus)
		{
			this.engine.SetActiveDocument(this.dte.Application.ActiveDocument);
		}

		// Visual Studio DOM
		private Epsitec.VisualStudio.DTE dte;
		private Engine engine;
	}
}
