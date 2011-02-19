//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using System.IO;
using System.Linq;
using System.Reflection;

namespace Epsitec.App.BanquePiguet
{

	/// <summary>
	/// The <c>Tools</c> class contains some static methods that are usefull somewhere else.
	/// </summary>
	static class Tools
	{

		/// <summary>
		/// Gets a <see cref="Stream"/> for the resource given by <paramref name="fileName"/>.
		/// </summary>
		/// <param name="fileName">The name of the resource.</param>
		/// <returns>A <see cref="Stream"/> for <paramref name="fileName"/>.</returns>
		/// <exception cref="System.ArgumentException">If the resource does not exit.</exception>
		public static Stream GetResourceStream(string fileName)
		{
			string path = string.Format("{0}.{1}", Tools.resourcesLocation, fileName);

			if (!assembly.GetManifestResourceNames ().Contains (path))
			{
				string message = string.Format ("The requested resource does not exist: {0}", fileName);
				throw new System.ArgumentException (message);
			}

			return Tools.assembly.GetManifestResourceStream (path);
		}

		/// <summary>
		/// The executing assembly.
		/// </summary>
		private static readonly Assembly assembly = Assembly.GetExecutingAssembly ();

		/// <summary>
		/// The path to the resources of the program.
		/// </summary>
		private static readonly string resourcesLocation = "Epsitec.App.BanquePiguet.Resources";

	}

}
