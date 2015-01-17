using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using Epsitec.Cresus.ResourceManagement;
using Epsitec.Cresus.Strings.Views;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Roslyn.Services;

namespace Epsitec.Cresus.Strings
{
	[TestClass]
	public class SolutionResourceTest
	{
		[TestMethod]
		public void Load()
		{
			var workspace = Workspace.LoadSolution (TestData.CresusGraphSolutionPath);
			var solution = new SolutionResource (workspace.CurrentSolution);
		}

		[TestMethod]
		public void TouchedFilePathes()
		{
			var workspace = Workspace.LoadSolution (TestData.CresusGraphSolutionPath);
			var solution = new SolutionResource (workspace.CurrentSolution);

			var files = new HashSet<string> ();
			var folders = new HashSet<string> ();
			foreach (var filePath in solution.TouchedFilePathes ())
			{
				files.Add (filePath);
				folders.Add (Path.GetDirectoryName (filePath));
				Trace.WriteLine (filePath);
			}

			using (Observable.Merge (folders.Select (folder => new FileSystemWatcher (folder).Watch ()))
				.Where (n => files.Contains (n.FullPath, StringComparer.OrdinalIgnoreCase))
				.Throttle(TimeSpan.FromMilliseconds(50))
				.Subscribe (n => Trace.WriteLine (n)))
			{
				Trace.WriteLine ("Modify some cresus graph resource");
				Thread.Sleep (-1);
			}
		}
	}
}
