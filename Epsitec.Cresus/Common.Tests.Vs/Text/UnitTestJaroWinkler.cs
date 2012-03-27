using Microsoft.VisualStudio.TestTools.UnitTesting;
using Epsitec.Common.Text;


namespace Epsitec.Common.Tests.Vs.Text
{


	[TestClass]
	public class UnitTestJaroWinkler
	{
		
		
		[TestMethod]
		public void TestMethod1()
		{
			Assert.AreEqual (1.0, JaroWinkler.ComputeJaroWinklerDistance (null, null));
			Assert.AreEqual (1.0, JaroWinkler.ComputeJaroWinklerDistance ("", null));
			Assert.AreEqual (1.0, JaroWinkler.ComputeJaroWinklerDistance (null, ""));
			Assert.AreEqual (1.0, JaroWinkler.ComputeJaroWinklerDistance ("", ""));
			Assert.AreEqual (1.0, JaroWinkler.ComputeJaroWinklerDistance ("abc", "abc"));
			Assert.AreEqual (1.0, JaroWinkler.ComputeJaroWinklerDistance ("ab", "ab"));
			Assert.AreEqual (1.0, JaroWinkler.ComputeJaroWinklerDistance ("a", "a"));
			Assert.AreEqual (1.0, JaroWinkler.ComputeJaroWinklerDistance ("abcd", "abcd"));
			Assert.AreEqual (0.0, JaroWinkler.ComputeJaroWinklerDistance ("abc", "def"));


			this.AreRoughlyEqual (0.840, JaroWinkler.ComputeJaroWinklerDistance ("DWAYNE", "DUANE"));
			this.AreRoughlyEqual (0.961, JaroWinkler.ComputeJaroWinklerDistance ("MARTHA", "MARHTA"));
			this.AreRoughlyEqual (0.813, JaroWinkler.ComputeJaroWinklerDistance ("DIXON", "DICKSONX"));
			this.AreRoughlyEqual (0.982, JaroWinkler.ComputeJaroWinklerDistance ("SHACKLEFORD", "SHACKELFORD"));
			this.AreRoughlyEqual (0.896, JaroWinkler.ComputeJaroWinklerDistance ("DUNNINGHAM", "CUNNIGHAM"));
			this.AreRoughlyEqual (0.956, JaroWinkler.ComputeJaroWinklerDistance ("NICHLESON", "NICHULSON"));
			this.AreRoughlyEqual (0.832, JaroWinkler.ComputeJaroWinklerDistance ("JONES", "JOHNSON"));
			this.AreRoughlyEqual (0.933, JaroWinkler.ComputeJaroWinklerDistance ("MASSEY", "MASSIE"));
			this.AreRoughlyEqual (0.922, JaroWinkler.ComputeJaroWinklerDistance ("ABROMS", "ABRAMS"));
			this.AreRoughlyEqual (0.722, JaroWinkler.ComputeJaroWinklerDistance ("HARDIN", "MARTINEZ"));
			this.AreRoughlyEqual (0.467, JaroWinkler.ComputeJaroWinklerDistance ("ITMAN", "SMITH"));
			this.AreRoughlyEqual (0.926, JaroWinkler.ComputeJaroWinklerDistance ("JERALDINE", "GERALDINE"));
			this.AreRoughlyEqual (0.921, JaroWinkler.ComputeJaroWinklerDistance ("MICHELLE", "MICHAEL"));
			this.AreRoughlyEqual (0.933, JaroWinkler.ComputeJaroWinklerDistance ("JULIES", "JULIUS"));
			this.AreRoughlyEqual (0.880, JaroWinkler.ComputeJaroWinklerDistance ("TANYA", "TONYA"));
			this.AreRoughlyEqual (0.805, JaroWinkler.ComputeJaroWinklerDistance ("SEAN", "SUSAN"));
			this.AreRoughlyEqual (0.933, JaroWinkler.ComputeJaroWinklerDistance ("JON", "JOHN"));
		}


		public void AreRoughlyEqual(double d1, double d2)
		{
			Assert.IsTrue (System.Math.Abs (d1 -d2) < 0.01);
		}


	}


}
