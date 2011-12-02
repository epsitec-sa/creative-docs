using System;

using System.IO;

using System.Diagnostics;


namespace Epsitec.Cresus.WebCore.Server
{


	internal sealed class NGinxServer : IDisposable
	{


		public NGinxServer(FileInfo path)
		{
			this.path = path;

			this.Start ();
		}


		public void Dispose()
		{
			this.Stop ();
		}


		private void Start()
		{
			var startInfo = this.BuildStartInfo (this.path);

			Process.Start (startInfo);
		}


		private void Stop()
		{
			var startInfo = this.BuildStartInfo (this.path, "-s stop");

			Process.Start (startInfo);
		}


		private ProcessStartInfo BuildStartInfo(FileInfo path, string arguments = null)
		{
			return new ProcessStartInfo (path.FullName)
			{
				Arguments = arguments,
				CreateNoWindow = true,
				RedirectStandardError = true,
				RedirectStandardInput = true,
				RedirectStandardOutput = true,
				UseShellExecute = false,
				WorkingDirectory = path.Directory.FullName
			};
		}


		private readonly FileInfo path;


	}


}

