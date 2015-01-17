//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Common.IO;

using System.Diagnostics;
using System.IO;


namespace Epsitec.Cresus.WebCore.Server
{
	/// <summary>
	/// This class manages the NGinx server that is embedded within this project.
	/// </summary>
	internal sealed class NGinxServer : System.IDisposable
	{
		public NGinxServer(FileInfo path)
		{
			this.path = path;

			this.Start ();

			Logger.LogToConsole ("Nginx started");
		}

		~NGinxServer()
		{
			this.Stop ();
		}


		#region IDisposable Members

		public void Dispose()
		{
			System.GC.SuppressFinalize (this);
			this.Stop ();
		}

		#endregion

		private void Start()
		{
			Process.Start (this.BuildStartInfo (this.path));
		}

		private void Stop()
		{
			if (this.stopped == false)
			{
				Process.Start (this.BuildStartInfo (this.path, "-s stop"));
				this.stopped = true;
			}
		}

		
		private ProcessStartInfo BuildStartInfo(FileInfo path, string arguments = null)
		{
			return new ProcessStartInfo (path.FullName)
			{
				Arguments              = arguments,
				CreateNoWindow         = true,
				RedirectStandardError  = true,
				RedirectStandardInput  = true,
				RedirectStandardOutput = true,
				UseShellExecute        = false,
				WorkingDirectory       = path.Directory.FullName
			};
		}

		
		private readonly FileInfo				path;
		private bool							stopped;
	}
}
