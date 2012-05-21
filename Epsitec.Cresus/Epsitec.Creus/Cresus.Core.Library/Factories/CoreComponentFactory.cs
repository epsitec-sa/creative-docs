//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Resolvers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Factories
{
	/// <summary>
	/// The <c>CoreComponentFactory</c> base class is provided so that all derived generic classes
	/// can share common static setup code.
	/// </summary>
	public abstract class CoreComponentFactory
	{
		protected static void Setup()
		{
			//	Make sure the static constructor gets executed; the constructor contains the
			//	logic required to load any library assemblies.
		}

		
		static CoreComponentFactory()
		{
			CoreComponentFactory.DiscoverCoreLibraryAssemblies ();
		}

		private static void DiscoverCoreLibraryAssemblies()
		{
			//	The components may reside in some not yet loaded assemblies; make sure that we
			//	load them now:

			AssemblyLoader.LoadMatching ("Cresus.Core.Library.*", System.IO.SearchOption.TopDirectoryOnly, loadMode: AssemblyLoadMode.LoadOnlyEpsitecSigned);
		}

		[System.ThreadStatic]
		protected static SafeCounter		registerRecursionCount;
	}
}
