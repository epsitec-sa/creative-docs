//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System;
using System.Runtime.InteropServices;

namespace Epsitec.Common.Drawing.Agg
{
	/// <summary>
	/// The <c>Library</c> class provides the basic initialization interface
	/// for AGG. It must be called to properly set up the underlying drawing
	/// engine.
	/// </summary>
	public sealed class Library
	{
		Library()
		{
			AntiGrain.Interface.Initialise ();
		}
		
		~Library()
		{
			Library.instance = null;
			AntiGrain.Interface.ShutDown ();
		}


		/// <summary>
		/// Initializes the AGG library.
		/// </summary>
		public static void Initialize()
		{
		}

		
		public static Library		Current
		{
			get
			{
				return Library.instance;
			}
		}
		
		public static long			Cycles
		{
			get
			{
				return 0;
			}
		}
		
		public static int			CycleDelta
		{
			get
			{
				return 1;
			}
		}
		
		
		static Library				instance = new Library ();
	}
}