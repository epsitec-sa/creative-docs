//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using NUnit.Framework;

namespace Epsitec.Common.Tests.Types
{
    [TestFixture]
    public class FormattedTextTest
    {
        [Test]
        public void CheckSplit1()
        {
            FormattedText text = new FormattedText("<i>italique</i>;;<b>gras</b>");
            FormattedText[] result = text.Split(";", System.StringSplitOptions.RemoveEmptyEntries);

            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(new FormattedText("<i>italique</i>"), result[0]);
            Assert.AreEqual(new FormattedText("<b>gras</b>"), result[1]);
        }
    }
}
