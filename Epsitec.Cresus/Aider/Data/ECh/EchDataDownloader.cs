//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using System.Net;
using Epsitec.Common.Types;

namespace Epsitec.Aider.Data.ECh
{
	public class EchDataDownloader
	{
		public EchDataDownloader(string repositoryPath)
		{
			this.repositoryPath = repositoryPath;

			this.ftpDirectory        = new List<string> ();
			this.ftpDirectoryDetails = new Dictionary<string, long> ();
			this.downloadedFilePaths = new List<string> ();
			
			this.FtpListAvailableFiles ();
			this.FtpListAvailableFilesDetails ();
		}


		public bool								HasDownloadedFiles
		{
			get
			{
				return this.downloadedFilePaths.Count > 0;
			}
		}

		public IList<string>					DownloadedFilePaths
		{
			get
			{
				return this.downloadedFilePaths.AsReadOnly ();
			}
		}


		public void DeleteDownloadedFilesButLast()
		{
			foreach (var file in this.downloadedFilePaths.Select (x => System.IO.Path.GetFileName (x)).Reverse ().Skip (1))
			{
				this.FtpDeleteFile (file);
			}
		}


		public static EchDataDownloader Download(string dirPath, string fileName = null)
		{
			var download = new EchDataDownloader (dirPath);

			if (string.IsNullOrEmpty (fileName))
			{
				foreach (var file in download.GetMismatches ())
				{
					download.FtpDownloadFile (file);
				}
			}
			else
			{
				download.FtpDownloadFile (fileName);
			}

			return download;
		}

		
		private bool FtpDownloadFile(string fileName)
		{
			long size;

			if (this.ftpDirectoryDetails.TryGetValue (fileName, out size))
			{
				using (WebClient client = new WebClient ())
				{
					client.Proxy       = new WebProxy ();
					client.Credentials = EchDataDownloader.GetCredentials ();

					System.Console.WriteLine ("FTP: Downloading {0}, {1} MB", fileName, size/(1024*1024));

					var filePath = System.IO.Path.Combine (this.repositoryPath, fileName);
					client.DownloadFile (EchDataDownloader.GetFtpUri (fileName), filePath);
					var fileInfo = new System.IO.FileInfo (filePath);

					if (fileInfo.Length == size)
					{
						this.downloadedFilePaths.Add (filePath);

						System.Console.WriteLine ("FTP: Download successful ({0}, {1} MB)", fileName, size/(1024*1024));

						return true;
					}

					System.Console.WriteLine ("FTP: Download failed ({0}, size mismatch = {1})", fileName, fileInfo.Length - size);
				}
			}

			return false;
		}

		private IEnumerable<string> GetMismatches()
		{
			var local = System.IO.Directory.GetFiles (this.repositoryPath, "*.xml", System.IO.SearchOption.TopDirectoryOnly);
			var remote = new HashSet<string> (this.ftpDirectory);

			foreach (var file in local.Select (x => new System.IO.FileInfo (x)))
			{
				long size;

				if ((this.ftpDirectoryDetails.TryGetValue (file.Name, out size)) &&
					(size == file.Length))
				{
					remote.Remove (file.Name);
				}
			}

			return remote;
		}

		
		private void FtpListAvailableFiles()
		{
			FtpWebRequest ftpRequest = WebRequest.Create (EchDataDownloader.GetFtpUri ()) as FtpWebRequest;
			ftpRequest.Credentials = EchDataDownloader.GetCredentials ();
			ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;

			var response     = ftpRequest.GetResponse () as FtpWebResponse;
			var streamReader = new System.IO.StreamReader (response.GetResponseStream ());

			string line = streamReader.ReadLine ();

			while (!string.IsNullOrEmpty (line))
			{
				this.ftpDirectory.Add (line);
				line = streamReader.ReadLine ();
			}

			streamReader.Close ();
			response.Close ();

			this.ftpDirectory.Sort ();
		}

		private void FtpListAvailableFilesDetails()
		{
			FtpWebRequest ftpRequest = WebRequest.Create (EchDataDownloader.GetFtpUri ()) as FtpWebRequest;
			ftpRequest.Credentials = EchDataDownloader.GetCredentials ();
			ftpRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;

			var response     = ftpRequest.GetResponse () as FtpWebResponse;
			var streamReader = new System.IO.StreamReader (response.GetResponseStream ());

			string line = streamReader.ReadLine ();

			while (!string.IsNullOrEmpty (line))
			{
				var tokens = line.Split (' ');

				//	"-rw-rw----   1 zeervftp geervftp 440734119 Oct 26 21:26 eerv-2015-10-26.xml"

				if (tokens.Length >= 11)
				{
					var name = tokens.Last ();

					if (this.ftpDirectory.Contains (name))
					{
						long size;
						
						if (InvariantConverter.Convert (tokens[6], out size))
						{
							this.ftpDirectoryDetails.Add (name, size);
						}
					}
				}

				line = streamReader.ReadLine ();
			}

			streamReader.Close ();
			response.Close ();
		}

		private void FtpDeleteFile(string fileName)
		{
			FtpWebRequest ftpRequest = WebRequest.Create (EchDataDownloader.GetFtpUri (fileName)) as FtpWebRequest;
			ftpRequest.Credentials = EchDataDownloader.GetCredentials ();
			ftpRequest.Method = WebRequestMethods.Ftp.DeleteFile;

			var response = ftpRequest.GetResponse () as FtpWebResponse;
			var status   = response.StatusCode;

			response.Close ();

			if (status != FtpStatusCode.FileActionOK)
			{
				throw new System.InvalidOperationException (string.Format ("FTP delete returned {0}", status));
			}
		}

		
		private static System.Uri GetFtpUri()
		{
			return new System.Uri ("ftp://ftp.vd.ch/data/EERV/");
		}

		private static System.Uri GetFtpUri(string name)
		{
			return new System.Uri ("ftp://ftp.vd.ch/data/EERV/" + name);
		}

		private static NetworkCredential GetCredentials()
		{
			return new NetworkCredential ("zeervftp", "3erVf8tp");
		}

		
		private readonly string					repositoryPath;
		private readonly List<string>			ftpDirectory;
		private readonly Dictionary<string, long> ftpDirectoryDetails;
		private readonly List<string>			downloadedFilePaths;
	}
}
