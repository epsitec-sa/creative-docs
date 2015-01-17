using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Epsitec.Cresus.Strings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Roslyn.Services;
using System.Diagnostics;
using Epsitec.Cresus.Strings.Views;
using System.Threading;

namespace Epsitec.Cresus.ResourceManagement
{
	[TestClass]
	public class ResourceSymbolMapperTest
	{
		[TestMethod]
		public void FindAll()
		{
			var mapper = ResourceSymbolMapperTest.CreateSolutionMapper (TestData.CresusGraphSolutionPath);
			var result = mapper.FindAll ();
			foreach (var resourceItem in result.SelectMany (kv => kv.Values))
			{
				Trace.WriteLine (resourceItem);
			}
		}

		[TestMethod]
		public void FindTail()
		{
			var mapper = ResourceSymbolMapperTest.CreateSolutionMapper (TestData.CresusGraphSolutionPath);
			var result = mapper.FindTail ("Dialog.Tooltip.Close", CancellationToken.None);
			foreach (var resourceItem in result.SelectMany (kv => kv.Values))
			{
				Trace.WriteLine (resourceItem);
			}
		}

		[TestMethod]
		public void FindPartial()
		{
			var mapper = ResourceSymbolMapperTest.CreateSolutionMapper (TestData.CresusGraphSolutionPath);
			var result = mapper.FindPartial ("Dialog.Tooltip", CancellationToken.None);
			foreach (var resourceItem in result.SelectMany (kv => kv.Values))
			{
				Trace.WriteLine (resourceItem);
			}
		}

		[TestMethod]
		public void OrderBySymbol()
		{
			var mapper = ResourceSymbolMapperTest.CreateSolutionMapper (TestData.CresusGraphSolutionPath);
			var result = mapper.FindPartial ("Dialog.Tooltip", CancellationToken.None);
		}

		[TestMethod]
		public void RegexEscape()
		{
			var e1 = Regex.Escape ("Dialog.Tooltip.Close");
		}


		#region Helpers

		private static ResourceSymbolMapper CreateSolutionMapper(string solutionPath)
		{
			var workspace = Workspace.LoadSolution (solutionPath);
			var resources = new SolutionResource (workspace.CurrentSolution);
			var resourceMapper = new ResourceSymbolMapper ();
			resourceMapper.VisitSolution (resources);
			return resourceMapper;
		}

		private static ResourceSymbolMapper CreateProjectMapper(string projectPath)
		{
			var resources = ProjectResource.Load (projectPath);
			var resourceMapper = new ResourceSymbolMapper ();
			resourceMapper.VisitProject (resources);
			return resourceMapper;
		}

		#endregion
	}
}
