//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Splash;

using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Epsitec.Cresus.App
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[System.STAThread]
		static void Main()
		{
			Application.EnableVisualStyles ();
			Application.SetCompatibleTextRenderingDefault (false);
			
			var args = System.Environment.GetCommandLineArgs ();

			try
			{
				Program.ExecuteCoreProgram (args);
			}
			catch (System.Exception ex)
			{
				System.Windows.Forms.MessageBox.Show (ex.Message, "Crésus", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private static void ExecuteCoreProgram(string[] args)
		{
			if (args.Length == 1)
			{
				var probe = Program.GetDefaultPackageFileInfo ();

				if (probe.Exists)
				{
					Program.ProcessPackageFile (probe);
					return;
				}

				throw new System.Exception ("No arguments specified.");
			}

			var fileInfo = new System.IO.FileInfo (args[1]);
			
			if (fileInfo.Exists)
			{
				Program.ProcessFile (fileInfo);
			}
			else
			{
				Program.ProcessArguments (args);
			}
		}

		private static void ProcessArguments(string[] args)
		{
			switch (args[1])
			{
				case "-build":
					if (args.Length == 4)
					{
						Program.ProcessBuildRequest (args[2], args[3]);
						return;
					}
					break;
			}

			//	Don't know what to do with the command line...

			throw new System.Exception ("Invalid command line.\n\n" + System.Environment.CommandLine);
		}

		private static void ProcessFile(System.IO.FileInfo fileInfo)
		{
			if (fileInfo.Extension.ToLowerInvariant () == ".txt")
			{
				var inputFile  = fileInfo.FullName;
				var outputFile = System.IO.Path.GetFileNameWithoutExtension (inputFile) + ".cresus";

				Program.ProcessBuildRequest (inputFile, outputFile);
			}
			else
			{
				Program.ProcessPackageFile (fileInfo);
			}
		}

		private static void ProcessPackageFile(System.IO.FileInfo fileInfo)
		{
			var fileStream  = fileInfo.OpenRead ();
			var chunkReader = new ChunkReader (fileStream);
			var tempRoot    = Program.GetTempPath ();

			foreach (var chunk in chunkReader.GetChunks ())
			{
				var guid = chunk.Guid;

				if (guid == WellKnownChunks.DbAcess)
				{
				}
				else if (guid == WellKnownChunks.SoftName)
				{
					Program.softName = chunk.Name;
				}
				else if (guid == WellKnownChunks.SoftSplash)
				{
					Program.splash = new SplashScreen (chunk.Data);
				}
				else if (guid == WellKnownChunks.SoftArgs)
				{
					Program.softArgs = chunk.Name.Split ('\n');
				}
				else if (guid == WellKnownChunks.SoftFiles)
				{
					System.Diagnostics.Debug.WriteLine (string.Format ("{0} : {1}, {2} bytes", chunk.Guid, chunk.Name, chunk.Data.Length));

					var filePath = System.IO.Path.Combine (tempRoot, chunk.Name);
					var fileDir  = System.IO.Path.GetDirectoryName (filePath);
					var fileName = System.IO.Path.GetFileNameWithoutExtension (filePath).ToLowerInvariant ();

					Program.chunks[fileName] = chunk;

					if (System.IO.Directory.Exists (fileDir) == false)
					{
						System.IO.Directory.CreateDirectory (fileDir);
					}

					System.IO.File.WriteAllBytes (filePath, chunk.Data);
				}
			}

			Program.ExecutePackage ();

			System.IO.Directory.Delete (Program.GetTempPath (), true);
			
			Program.splash.NotifyIsRunning ();
		}

		private static void ExecutePackage()
		{
			if (Program.softArgs != null)
			{
				var info = new System.Diagnostics.ProcessStartInfo ()
				{
					Arguments = Program.softArgs.Skip (1).FirstOrDefault (),
					ErrorDialog = false,
					FileName = System.IO.Path.Combine (Program.GetTempPath (), Program.softArgs.First ()),
					WorkingDirectory = System.IO.Directory.GetCurrentDirectory (),
					UseShellExecute = false,
				};

				var process = System.Diagnostics.Process.Start (info);

				if (process != null)
				{
					process.WaitForInputIdle ();

					while (process.HasExited == false)
					{
						process.Refresh ();

						if (process.MainWindowHandle != System.IntPtr.Zero)
						{
							break;
						}

						System.Threading.Thread.Sleep (100);
					}

					Program.splash.NotifyIsRunning ();
					process.WaitForExit ();
				}
			}
		}

		private static void ProcessBuildRequest(string inputPath, string outputPath)
		{
			var root   = System.IO.Path.GetDirectoryName (System.IO.Path.GetFullPath (inputPath));
			var source = System.IO.File.ReadAllLines (inputPath, System.Text.Encoding.Default);
			var output = System.IO.File.OpenWrite (Program.GetRootedPath (root, outputPath));

			var writer    = new ChunkWriter (output);
			var processor = new ChunkSourceProcessor (source);

			while (processor.EndReached == false)
			{
				var guid = System.Guid.Parse (processor.GetNextLine ("G:"));
				var text = processor.GetNextLine ("T:");
				var path = processor.GetNextLine ("P:");
				var dir  = processor.GetNextLine ("D:");

				if ((string.IsNullOrEmpty (dir) == false) &&
					(string.IsNullOrEmpty (text)))
				{
					var files = new List<FileInfo> ();

					while ((string.IsNullOrEmpty (dir) == false)
						|| (string.IsNullOrEmpty (path) == false))
					{
						if (string.IsNullOrEmpty (dir) == false)
						{
							files.AddRange (dir.Split ('\n').SelectMany (x => Program.GetDirEntries (root, x)));
						}
						if (string.IsNullOrEmpty (path) == false)
						{
							files.AddRange (path.Split ('\n').Select (x => Program.GetFileEntry (root, x)));
						}
						
						dir  = processor.GetNextLine ("D:");
						path = processor.GetNextLine ("F:");
					}

					var positives = new HashSet<string> ();
					var negatives = new HashSet<string> (files.Where (x => x.IsNegative).Select (x => x.FullPath));

					foreach (var file in files)
					{
						if (positives.Add (file.FullPath))
						{
							if (negatives.Contains (file.FullPath) == false)
							{
								Program.ProcessBuildRequestAddChunk (root, writer, processor, guid, file.RelativePath, file.FullPath);
							}
						}
					}
				}
				else
				{
					Program.ProcessBuildRequestAddChunk (root, writer, processor, guid, text, path);
				}
			}

			output.SetLength (output.Position);
			output.Close ();
		}

		class FileInfo
		{
			public FileInfo(string fullPath, string relativePath, bool negative)
			{
				this.FullPath = fullPath;
				this.RelativePath = relativePath;
				this.IsNegative = negative;
			}

			public readonly string FullPath;
			public readonly string RelativePath;
			public readonly bool IsNegative;
		}

		private static FileInfo GetFileEntry(string root, string fileSpec)
		{
			bool negative = false;

			if (fileSpec.StartsWith ("-"))
			{
				negative = true;
				fileSpec = fileSpec.Substring (1);
			}
			else if (fileSpec.StartsWith ("+"))
			{
				fileSpec = fileSpec.Substring (1);
			}

			string[] args = fileSpec.Split ('|');

			string fileDir = args[0];
			string fileRel = args[1];

			fileSpec = Program.GetRootedPath (root, fileDir);

			return new FileInfo (System.IO.Path.Combine (fileSpec, fileRel), fileRel, negative);
		}

		private static IEnumerable<FileInfo> GetDirEntries(string root, string dir)
		{
			bool negative = false;

			if (dir.StartsWith ("-"))
			{
				negative = true;
				dir      = dir.Substring (1);
			}

			var pos = dir.IndexOf ('|');
			var filter = dir.Split ('|').Skip (1);
			
			dir = Program.GetRootedPath (root, pos < 0 ? dir : dir.Substring (0, pos));

			var dirInfo = new System.IO.DirectoryInfo (dir);
			var dirEntries = dirInfo.EnumerateFiles ("*", System.IO.SearchOption.AllDirectories);
			IEnumerable<string> names;

			if (filter.Any ())
			{
				var extHash = new HashSet<string> (filter.Select (x => x.ToLowerInvariant ()));
				names = dirEntries.Where (x => extHash.Contains (x.Extension.ToLowerInvariant ())).Select (x => x.FullName);
			}
			else
			{
				names = dirEntries.Select (x => x.FullName);
			}

			return names.Select (x => new FileInfo (x, x.Substring (dir.Length + 1), negative));
		}

		private static void ProcessBuildRequestAddChunk(string root, ChunkWriter writer, ChunkSourceProcessor processor, System.Guid guid, string text, string path)
		{
			if (string.IsNullOrEmpty (path))
			{
				writer.Add (new Chunk (guid, text));
			}
			else
			{
				path = Program.GetRootedPath (root, path);

				if (System.IO.File.Exists (path))
				{
					writer.Add (new Chunk (guid, text, System.IO.File.ReadAllBytes (path)));
				}
				else
				{
					throw new System.Exception (string.Format ("{0}: file '{1}' not found", processor.LineNumber, path));
				}
			}
		}

		private static System.IO.FileInfo GetDefaultPackageFileInfo()
		{
			var file = System.Reflection.Assembly.GetExecutingAssembly ().Location;
			var dir  = System.IO.Path.GetDirectoryName (file);
			var name = System.IO.Path.GetFileNameWithoutExtension (file).ToLowerInvariant ();
			
			return new System.IO.FileInfo (System.IO.Path.Combine (dir, "default." + name));
		}

		private static string GetPackageName()
		{
			var file = System.Reflection.Assembly.GetExecutingAssembly ().Location;
			var dir  = System.IO.Path.GetDirectoryName (file);
			
			return System.IO.Path.GetFileNameWithoutExtension (file).ToLowerInvariant ();
		}
		
		private static string GetRootedPath(string root, string path)
		{
			if (System.IO.Path.IsPathRooted (path))
			{
				return System.IO.Path.GetFullPath (path);
			}
			else
			{
				return System.IO.Path.GetFullPath (System.IO.Path.Combine (root, path));
			}
		}

		private static string GetTempPath()
		{
			var pid = System.Diagnostics.Process.GetCurrentProcess ().Id;
			var dir = string.Format ("{0}-{1}", Program.GetPackageName (), pid);
			
			return System.IO.Path.Combine (System.IO.Path.GetTempPath (), dir);
		}

		private static string GetCodeBase(string assemblyName)
		{
			var path = System.IO.Path.Combine (Program.GetTempPath (), assemblyName);

			var pathExe = path + ".exe";
			var pathDll = path + ".dll";

			if (System.IO.File.Exists (pathExe))
			{
				return Program.PathToUri (pathExe);
			}
			else if (System.IO.File.Exists (pathDll))
			{
				return Program.PathToUri (pathDll);
			}
			else
			{
				return null;
			}
		}

		private static string PathToUri(string path)
		{
			return "file:///" + (path.Replace (System.IO.Path.DirectorySeparatorChar, '/').Replace ("//", "/"));
		}

		private static string softName;
		private static string[] softArgs;
		private static Dictionary<string, Chunk> chunks = new Dictionary<string, Chunk> ();
		private static SplashScreen splash;
	}
}