//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing.Agg;

using NUnit.Framework;

namespace Epsitec.Common.Tests
{
    [TestFixture]
    public class AntiGrainTest
    {
        [Test]
        public void CanLoadAntiGrain()
        {
            Library.Initialize();
            Library.ShutDown();
        }
    }
}
