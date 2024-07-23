/*
This file is part of CreativeDocs.

Copyright © 2003-2024, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland

CreativeDocs is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

CreativeDocs is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using Epsitec.Common.Support;
using NUnit.Framework;

namespace Epsitec.Common.Tests.Support
{
    [TestFixture]
    public class VersionCheckerTest
    {
        [Test]
        [Ignore("Disabled (does not build)")]
        public void Check01Version()
        {
            /*
            string url = "http://www.creativedocs.net/update/check?software={0}&version={1}";

            VersionChecker checker = VersionChecker.CheckUpdate(
                typeof(VersionCheckerTest).Assembly,
                url,
                "Support.Tests"
            );

            System.Console.Out.Write("StartCheck");

            //	Pour tester si une version plus récente de CreativeDocs existe, il faudrait
            //	utiliser l'URL de base suivante :
            //
            //	"http://www.creativedocs.net/update/check?software=CreativeDocs&version="


            checker.VersionInformationChanged += sender =>
            {
                this.asyncInfoReady = true;
            };

            Assert.IsFalse(checker.IsReady);
            Assert.IsFalse(checker.IsCheckSuccessful);

            //	Attend que la réponse nous soit parvenue :

            while (checker.IsReady == false)
            {
                System.Console.Out.Write(".");
                System.Threading.Thread.Sleep(100);
            }

            System.Console.Out.WriteLine();

            Assert.IsTrue(checker.IsReady);
            Assert.IsTrue(checker.IsCheckSuccessful);

            System.Console.Out.WriteLine("Current version: {0}", checker.CurrentVersion);
            System.Console.Out.WriteLine(
                "Newer version:   {0}, {1}",
                checker.NewerVersion,
                checker.FoundNewerVersion ? "newer" : "not newer"
            );
            System.Console.Out.WriteLine("URL:             {0}", checker.NewerVersionUrl);

            System.Windows.Forms.Application.DoEvents();

            Assert.IsTrue(this.asyncInfoReady, "VersionInformationChanged event did not fire");
            */
        }

        private bool asyncInfoReady;
    }
}
