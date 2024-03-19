//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using NUnit.Framework;

namespace Epsitec.Common.Tests
{
    [TestFixture]
    public class AntiGrainTest
    {
        [Test]
        public void CanLoadAntiGrain()
        {
            AntigrainCPP.Interface.Initialise();
            AntigrainCPP.Interface.ShutDown();
        }
    }
}
