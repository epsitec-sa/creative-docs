//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>UrlNavigator</c> class provides support for launching an external
	/// browser to navigate to URLs.
	/// </summary>
	public static class UrlNavigator
	{
		/// <summary>
		/// Opens the specified URL in the navigator.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <returns><c>true</c> if the navigator was launched successfully; otherwise, <c>false</c>.</returns>
		public static bool OpenUrl(string url)
		{

			try
			{
				System.Diagnostics.Process.Start (url);
				return true;
			}
			catch
			{
				try
				{
					System.Diagnostics.Process.Start ("iexplore.exe", url);
					return true;
				}
				catch
				{
				}
			}

			return false;
		}
	}
}
