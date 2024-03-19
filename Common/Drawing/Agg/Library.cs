//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Drawing.Agg
{
    /// <summary>
    /// The <c>Library</c> class provides the basic initialization interface
    /// for AGG. It must be called to properly set up the underlying drawing
    /// engine.
    /// </summary>
    public sealed class Library
    {
        static Library() => AntigrainCPP.Interface.Initialise();

        /// <summary>
        /// Initializes the AGG library.
        /// </summary>
        public static void Initialize() { }
        public static void ShutDown() => AntigrainCPP.Interface.ShutDown();

        public static long Cycles     => 0;
        public static int  CycleDelta => 1;
    }
}
