using Epsitec.Common.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;


namespace Epsitec.Common.Tests.Vs.Text
{


	[TestClass]
	public class UnitTestJaroWinkler
	{


		[TestMethod]
		public void TestBounds()
		{
			this.CheckBounds (JaroWinkler.MaxValue, null, null);
			this.CheckBounds (JaroWinkler.MaxValue, "", null);
			this.CheckBounds (JaroWinkler.MaxValue, null, "");
			this.CheckBounds (JaroWinkler.MaxValue, "", "");
			this.CheckBounds (JaroWinkler.MinValue, null, "a");
			this.CheckBounds (JaroWinkler.MinValue, "a", null);
			this.CheckBounds (JaroWinkler.MaxValue, "abc", "abc");
			this.CheckBounds (JaroWinkler.MaxValue, "ab", "ab");
			this.CheckBounds (JaroWinkler.MaxValue, "a", "a");
			this.CheckBounds (JaroWinkler.MaxValue, "abcd", "abcd");
			this.CheckBounds (JaroWinkler.MinValue, "abc", "def");
		}


		[TestMethod]
		public void TestJaroWinkler()
		{
			this.CheckJaroWinkler (0.840, "DWAYNE", "DUANE");
			this.CheckJaroWinkler (0.961, "MARTHA", "MARHTA");
			this.CheckJaroWinkler (0.813, "DIXON", "DICKSONX");
			this.CheckJaroWinkler (0.982, "SHACKLEFORD", "SHACKELFORD");
			this.CheckJaroWinkler (0.896, "DUNNINGHAM", "CUNNIGHAM");
			this.CheckJaroWinkler (0.956, "NICHLESON", "NICHULSON");
			this.CheckJaroWinkler (0.832, "JONES", "JOHNSON");
			this.CheckJaroWinkler (0.933, "MASSEY", "MASSIE");
			this.CheckJaroWinkler (0.922, "ABROMS", "ABRAMS");
			this.CheckJaroWinkler (0.722, "HARDIN", "MARTINEZ");
			this.CheckJaroWinkler (0.467, "ITMAN", "SMITH");
			this.CheckJaroWinkler (0.926, "JERALDINE", "GERALDINE");
			this.CheckJaroWinkler (0.921, "MICHELLE", "MICHAEL");
			this.CheckJaroWinkler (0.933, "JULIES", "JULIUS");
			this.CheckJaroWinkler (0.880, "TANYA", "TONYA");
			this.CheckJaroWinkler (0.805, "SEAN", "SUSAN");
			this.CheckJaroWinkler (0.933, "JON", "JOHN");
			this.CheckJaroWinkler (0.950, "JONATHON", "JONATHAN");
			this.CheckJaroWinkler (0.910, "EVERYBODY", "EVERY");
			this.CheckJaroWinkler (0.900, "LAMPLEY", "CAMPLEY");
			this.CheckJaroWinkler (0.730, "CRATE", "TRACE");
			this.CheckJaroWinkler (0.800, "JON", "JAN");
			this.CheckJaroWinkler (1.000, "PHYSIC", "PHYSIC");
			this.CheckJaroWinkler (0.933, "PHYSIC", "PHYSICIAN");
			this.CheckJaroWinkler (0.933, "PHYSIC", "PHYSICIST");
			this.CheckJaroWinkler (0.850, "PHYSIC", "PHYSIOLOGIST");
			this.CheckJaroWinkler (0.780, "PHYSIC", "PSYCHIATRI");
			this.CheckJaroWinkler (0.744, "PHYSIC", "PSYCHIC");
			this.CheckJaroWinkler (0.715, "PHYSIC", "PSYCHONEUROS");
			this.CheckJaroWinkler (0.678, "PHYSIC", "PSYCHOPHARMACOLOG");
			this.CheckJaroWinkler (0.696, "PHYSIC", "PSYCHOS");
			this.CheckJaroWinkler (0.825, "PHYSIC", "PSYCHOSI");
			this.CheckJaroWinkler (0.455, "PHYSIC", "APPLE");
			this.CheckJaroWinkler (0.000, "PHYSIC", "BANANA");
			this.CheckJaroWinkler (0.933, "PHYSICIAN", "PHYSIC");
			this.CheckJaroWinkler (1.000, "PHYSICIAN", "PHYSICIAN");
			this.CheckJaroWinkler (0.911, "PHYSICIAN", "PHYSICIST");
			this.CheckJaroWinkler (0.833, "PHYSICIAN", "PHYSIOLOGIST");
			this.CheckJaroWinkler (0.794, "PHYSICIAN", "PSYCHIATRI");
			this.CheckJaroWinkler (0.757, "PHYSICIAN", "PSYCHIC");
			this.CheckJaroWinkler (0.700, "PHYSICIAN", "PSYCHONEUROS");
			this.CheckJaroWinkler (0.655, "PHYSICIAN", "PSYCHOPHARMACOLOG");
			this.CheckJaroWinkler (0.720, "PHYSICIAN", "PSYCHOS");
			this.CheckJaroWinkler (0.725, "PHYSICIAN", "PSYCHOSI");
			this.CheckJaroWinkler (0.437, "PHYSICIAN", "APPLE");
			this.CheckJaroWinkler (0.425, "PHYSICIAN", "BANANA");
			this.CheckJaroWinkler (0.933, "PHYSICIST", "PHYSIC");
			this.CheckJaroWinkler (0.911, "PHYSICIST", "PHYSICIAN");
			this.CheckJaroWinkler (1.000, "PHYSICIST", "PHYSICIST");
			this.CheckJaroWinkler (0.911, "PHYSICIST", "PHYSIOLOGIST");
			this.CheckJaroWinkler (0.794, "PHYSICIST", "PSYCHIATRI");
			this.CheckJaroWinkler (0.757, "PHYSICIST", "PSYCHIC");
			this.CheckJaroWinkler (0.700, "PHYSICIST", "PSYCHONEUROS");
			this.CheckJaroWinkler (0.594, "PHYSICIST", "PSYCHOPHARMACOLOG");
			this.CheckJaroWinkler (0.807, "PHYSICIST", "PSYCHOS");
			this.CheckJaroWinkler (0.810, "PHYSICIST", "PSYCHOSI");
			this.CheckJaroWinkler (0.437, "PHYSICIST", "APPLE");
			this.CheckJaroWinkler (0.000, "PHYSICIST", "BANANA");
			this.CheckJaroWinkler (0.850, "PHYSIOLOGIST", "PHYSIC");
			this.CheckJaroWinkler (0.833, "PHYSIOLOGIST", "PHYSICIAN");
			this.CheckJaroWinkler (0.911, "PHYSIOLOGIST", "PHYSICIST");
			this.CheckJaroWinkler (1.000, "PHYSIOLOGIST", "PHYSIOLOGIST");
			this.CheckJaroWinkler (0.699, "PHYSIOLOGIST", "PSYCHIATRI");
			this.CheckJaroWinkler (0.679, "PHYSIOLOGIST", "PSYCHIC");
			this.CheckJaroWinkler (0.707, "PHYSIOLOGIST", "PSYCHONEUROS");
			this.CheckJaroWinkler (0.605, "PHYSIOLOGIST", "PSYCHOPHARMACOLOG");
			this.CheckJaroWinkler (0.757, "PHYSIOLOGIST", "PSYCHOS");
			this.CheckJaroWinkler (0.751, "PHYSIOLOGIST", "PSYCHOSI");
			this.CheckJaroWinkler (0.522, "PHYSIOLOGIST", "APPLE");
			this.CheckJaroWinkler (0.000, "PHYSIOLOGIST", "BANANA");
			this.CheckJaroWinkler (0.780, "PSYCHIATRI", "PHYSIC");
			this.CheckJaroWinkler (0.794, "PSYCHIATRI", "PHYSICIAN");
			this.CheckJaroWinkler (0.794, "PSYCHIATRI", "PHYSICIST");
			this.CheckJaroWinkler (0.699, "PSYCHIATRI", "PHYSIOLOGIST");
			this.CheckJaroWinkler (1.000, "PSYCHIATRI", "PSYCHIATRI");
			this.CheckJaroWinkler (0.891, "PSYCHIATRI", "PSYCHIC");
			this.CheckJaroWinkler (0.820, "PSYCHIATRI", "PSYCHONEUROS");
			this.CheckJaroWinkler (0.822, "PSYCHIATRI", "PSYCHOPHARMACOLOG");
			this.CheckJaroWinkler (0.842, "PSYCHIATRI", "PSYCHOS");
			this.CheckJaroWinkler (0.870, "PSYCHIATRI", "PSYCHOSI");
			this.CheckJaroWinkler (0.433, "PSYCHIATRI", "APPLE");
			this.CheckJaroWinkler (0.422, "PSYCHIATRI", "BANANA");
			this.CheckJaroWinkler (0.744, "PSYCHIC", "PHYSIC");
			this.CheckJaroWinkler (0.757, "PSYCHIC", "PHYSICIAN");
			this.CheckJaroWinkler (0.757, "PSYCHIC", "PHYSICIST");
			this.CheckJaroWinkler (0.679, "PSYCHIC", "PHYSIOLOGIST");
			this.CheckJaroWinkler (0.891, "PSYCHIC", "PSYCHIATRI");
			this.CheckJaroWinkler (1.000, "PSYCHIC", "PSYCHIC");
			this.CheckJaroWinkler (0.826, "PSYCHIC", "PSYCHONEUROS");
			this.CheckJaroWinkler (0.842, "PSYCHIC", "PSYCHOPHARMACOLOG");
			this.CheckJaroWinkler (0.885, "PSYCHIC", "PSYCHOS");
			this.CheckJaroWinkler (0.921, "PSYCHIC", "PSYCHOSI");
			this.CheckJaroWinkler (0.447, "PSYCHIC", "APPLE");
			this.CheckJaroWinkler (0.000, "PSYCHIC", "BANANA");
			this.CheckJaroWinkler (0.715, "PSYCHONEUROS", "PHYSIC");
			this.CheckJaroWinkler (0.700, "PSYCHONEUROS", "PHYSICIAN");
			this.CheckJaroWinkler (0.700, "PSYCHONEUROS", "PHYSICIST");
			this.CheckJaroWinkler (0.707, "PSYCHONEUROS", "PHYSIOLOGIST");
			this.CheckJaroWinkler (0.820, "PSYCHONEUROS", "PSYCHIATRI");
			this.CheckJaroWinkler (0.826, "PSYCHONEUROS", "PSYCHIC");
			this.CheckJaroWinkler (1.000, "PSYCHONEUROS", "PSYCHONEUROS");
			this.CheckJaroWinkler (0.827, "PSYCHONEUROS", "PSYCHOPHARMACOLOG");
			this.CheckJaroWinkler (0.916, "PSYCHONEUROS", "PSYCHOS");
			this.CheckJaroWinkler (0.891, "PSYCHONEUROS", "PSYCHOSI");
			this.CheckJaroWinkler (0.522, "PSYCHONEUROS", "APPLE");
			this.CheckJaroWinkler (0.416, "PSYCHONEUROS", "BANANA");
			this.CheckJaroWinkler (0.678, "PSYCHOPHARMACOLOG", "PHYSIC");
			this.CheckJaroWinkler (0.655, "PSYCHOPHARMACOLOG", "PHYSICIAN");
			this.CheckJaroWinkler (0.594, "PSYCHOPHARMACOLOG", "PHYSICIST");
			this.CheckJaroWinkler (0.605, "PSYCHOPHARMACOLOG", "PHYSIOLOGIST");
			this.CheckJaroWinkler (0.822, "PSYCHOPHARMACOLOG", "PSYCHIATRI");
			this.CheckJaroWinkler (0.842, "PSYCHOPHARMACOLOG", "PSYCHIC");
			this.CheckJaroWinkler (0.827, "PSYCHOPHARMACOLOG", "PSYCHONEUROS");
			this.CheckJaroWinkler (1.000, "PSYCHOPHARMACOLOG", "PSYCHOPHARMACOLOG");
			this.CheckJaroWinkler (0.842, "PSYCHOPHARMACOLOG", "PSYCHOS");
			this.CheckJaroWinkler (0.820, "PSYCHOPHARMACOLOG", "PSYCHOSI");
			this.CheckJaroWinkler (0.505, "PSYCHOPHARMACOLOG", "APPLE");
			this.CheckJaroWinkler (0.483, "PSYCHOPHARMACOLOG", "BANANA");
			this.CheckJaroWinkler (0.696, "PSYCHOS", "PHYSIC");
			this.CheckJaroWinkler (0.720, "PSYCHOS", "PHYSICIAN");
			this.CheckJaroWinkler (0.807, "PSYCHOS", "PHYSICIST");
			this.CheckJaroWinkler (0.757, "PSYCHOS", "PHYSIOLOGIST");
			this.CheckJaroWinkler (0.842, "PSYCHOS", "PSYCHIATRI");
			this.CheckJaroWinkler (0.885, "PSYCHOS", "PSYCHIC");
			this.CheckJaroWinkler (0.916, "PSYCHOS", "PSYCHONEUROS");
			this.CheckJaroWinkler (0.842, "PSYCHOS", "PSYCHOPHARMACOLOG");
			this.CheckJaroWinkler (1.000, "PSYCHOS", "PSYCHOS");
			this.CheckJaroWinkler (0.975, "PSYCHOS", "PSYCHOSI");
			this.CheckJaroWinkler (0.447, "PSYCHOS", "APPLE");
			this.CheckJaroWinkler (0.000, "PSYCHOS", "BANANA");
			this.CheckJaroWinkler (0.825, "PSYCHOSI", "PHYSIC");
			this.CheckJaroWinkler (0.725, "PSYCHOSI", "PHYSICIAN");
			this.CheckJaroWinkler (0.810, "PSYCHOSI", "PHYSICIST");
			this.CheckJaroWinkler (0.751, "PSYCHOSI", "PHYSIOLOGIST");
			this.CheckJaroWinkler (0.870, "PSYCHOSI", "PSYCHIATRI");
			this.CheckJaroWinkler (0.921, "PSYCHOSI", "PSYCHIC");
			this.CheckJaroWinkler (0.891, "PSYCHOSI", "PSYCHONEUROS");
			this.CheckJaroWinkler (0.820, "PSYCHOSI", "PSYCHOPHARMACOLOG");
			this.CheckJaroWinkler (0.975, "PSYCHOSI", "PSYCHOS");
			this.CheckJaroWinkler (1.000, "PSYCHOSI", "PSYCHOSI");
			this.CheckJaroWinkler (0.441, "PSYCHOSI", "APPLE");
			this.CheckJaroWinkler (0.000, "PSYCHOSI", "BANANA");
			this.CheckJaroWinkler (0.455, "APPLE", "PHYSIC");
			this.CheckJaroWinkler (0.437, "APPLE", "PHYSICIAN");
			this.CheckJaroWinkler (0.437, "APPLE", "PHYSICIST");
			this.CheckJaroWinkler (0.522, "APPLE", "PHYSIOLOGIST");
			this.CheckJaroWinkler (0.433, "APPLE", "PSYCHIATRI");
			this.CheckJaroWinkler (0.447, "APPLE", "PSYCHIC");
			this.CheckJaroWinkler (0.522, "APPLE", "PSYCHONEUROS");
			this.CheckJaroWinkler (0.505, "APPLE", "PSYCHOPHARMACOLOG");
			this.CheckJaroWinkler (0.447, "APPLE", "PSYCHOS");
			this.CheckJaroWinkler (0.441, "APPLE", "PSYCHOSI");
			this.CheckJaroWinkler (1.000, "APPLE", "APPLE");
			this.CheckJaroWinkler (0.455, "APPLE", "BANANA");
			this.CheckJaroWinkler (0.000, "BANANA", "PHYSIC");
			this.CheckJaroWinkler (0.425, "BANANA", "PHYSICIAN");
			this.CheckJaroWinkler (0.000, "BANANA", "PHYSICIST");
			this.CheckJaroWinkler (0.000, "BANANA", "PHYSIOLOGIST");
			this.CheckJaroWinkler (0.422, "BANANA", "PSYCHIATRI");
			this.CheckJaroWinkler (0.000, "BANANA", "PSYCHIC");
			this.CheckJaroWinkler (0.416, "BANANA", "PSYCHONEUROS");
			this.CheckJaroWinkler (0.483, "BANANA", "PSYCHOPHARMACOLOG");
			this.CheckJaroWinkler (0.000, "BANANA", "PSYCHOS");
			this.CheckJaroWinkler (0.000, "BANANA", "PSYCHOSI");
			this.CheckJaroWinkler (0.455, "BANANA", "APPLE");
			this.CheckJaroWinkler (1.000, "BANANA", "BANANA");
		}


		[TestMethod]
		public void TestJaro()
		{
			this.CheckJaro (0.896, "DUNNINGHAM", "CUNNIGHAM");
			this.CheckJaro (0.925, "NICHLESON", "NICHULSON");
			this.CheckJaro (0.790, "JONES", "JOHNSON");
			this.CheckJaro (0.888, "MASSEY", "MASSIE");
			this.CheckJaro (0.888, "ABROMS", "ABRAMS");
			this.CheckJaro (0.722, "HARDIN", "MARTINEZ");
			this.CheckJaro (0.466, "ITMAN", "SMITH");
			this.CheckJaro (0.925, "JERALDINE", "GERALDINE");
			this.CheckJaro (0.944, "MARHTA", "MARTHA");
			this.CheckJaro (0.869, "MICHELLE", "MICHAEL");
			this.CheckJaro (0.888, "JULIES", "JULIUS");
			this.CheckJaro (0.866, "TANYA", "TONYA");
			this.CheckJaro (0.822, "DWAYNE", "DUANE");
			this.CheckJaro (0.783, "SEAN", "SUSAN");
			this.CheckJaro (0.916, "JON", "JOHN");
			this.CheckJaro (0.777, "JON", "JAN");
			this.CheckJaro (1.000, "PHYSIC", "PHYSIC");
			this.CheckJaro (0.888, "PHYSIC", "PHYSICIAN");
			this.CheckJaro (0.888, "PHYSIC", "PHYSICIST");
			this.CheckJaro (0.750, "PHYSIC", "PHYSIOLOGIST");
			this.CheckJaro (0.755, "PHYSIC", "PSYCHIATRI");
			this.CheckJaro (0.715, "PHYSIC", "PSYCHIC");
			this.CheckJaro (0.683, "PHYSIC", "PSYCHONEUROS");
			this.CheckJaro (0.642, "PHYSIC", "PSYCHOPHARMACOLOG");
			this.CheckJaro (0.662, "PHYSIC", "PSYCHOS");
			this.CheckJaro (0.805, "PHYSIC", "PSYCHOSI");
			this.CheckJaro (0.455, "PHYSIC", "APPLE");
			this.CheckJaro (0.000, "PHYSIC", "BANANA");
			this.CheckJaro (0.888, "PHYSICIAN", "PHYSIC");
			this.CheckJaro (1.000, "PHYSICIAN", "PHYSICIAN");
			this.CheckJaro (0.851, "PHYSICIAN", "PHYSICIST");
			this.CheckJaro (0.722, "PHYSICIAN", "PHYSIOLOGIST");
			this.CheckJaro (0.771, "PHYSICIAN", "PSYCHIATRI");
			this.CheckJaro (0.730, "PHYSICIAN", "PSYCHIC");
			this.CheckJaro (0.666, "PHYSICIAN", "PSYCHONEUROS");
			this.CheckJaro (0.617, "PHYSICIAN", "PSYCHOPHARMACOLOG");
			this.CheckJaro (0.689, "PHYSICIAN", "PSYCHOS");
			this.CheckJaro (0.694, "PHYSICIAN", "PSYCHOSI");
			this.CheckJaro (0.437, "PHYSICIAN", "APPLE");
			this.CheckJaro (0.425, "PHYSICIAN", "BANANA");
			this.CheckJaro (0.888, "PHYSICIST", "PHYSIC");
			this.CheckJaro (0.851, "PHYSICIST", "PHYSICIAN");
			this.CheckJaro (1.000, "PHYSICIST", "PHYSICIST");
			this.CheckJaro (0.851, "PHYSICIST", "PHYSIOLOGIST");
			this.CheckJaro (0.771, "PHYSICIST", "PSYCHIATRI");
			this.CheckJaro (0.730, "PHYSICIST", "PSYCHIC");
			this.CheckJaro (0.666, "PHYSICIST", "PSYCHONEUROS");
			this.CheckJaro (0.549, "PHYSICIST", "PSYCHOPHARMACOLOG");
			this.CheckJaro (0.785, "PHYSICIST", "PSYCHOS");
			this.CheckJaro (0.789, "PHYSICIST", "PSYCHOSI");
			this.CheckJaro (0.437, "PHYSICIST", "APPLE");
			this.CheckJaro (0.000, "PHYSICIST", "BANANA");
			this.CheckJaro (0.750, "PHYSIOLOGIST", "PHYSIC");
			this.CheckJaro (0.722, "PHYSIOLOGIST", "PHYSICIAN");
			this.CheckJaro (0.851, "PHYSIOLOGIST", "PHYSICIST");
			this.CheckJaro (1.000, "PHYSIOLOGIST", "PHYSIOLOGIST");
			this.CheckJaro (0.665, "PHYSIOLOGIST", "PSYCHIATRI");
			this.CheckJaro (0.643, "PHYSIOLOGIST", "PSYCHIC");
			this.CheckJaro (0.674, "PHYSIOLOGIST", "PSYCHONEUROS");
			this.CheckJaro (0.562, "PHYSIOLOGIST", "PSYCHOPHARMACOLOG");
			this.CheckJaro (0.730, "PHYSIOLOGIST", "PSYCHOS");
			this.CheckJaro (0.724, "PHYSIOLOGIST", "PSYCHOSI");
			this.CheckJaro (0.522, "PHYSIOLOGIST", "APPLE");
			this.CheckJaro (0.000, "PHYSIOLOGIST", "BANANA");
			this.CheckJaro (0.755, "PSYCHIATRI", "PHYSIC");
			this.CheckJaro (0.771, "PSYCHIATRI", "PHYSICIAN");
			this.CheckJaro (0.771, "PSYCHIATRI", "PHYSICIST");
			this.CheckJaro (0.665, "PSYCHIATRI", "PHYSIOLOGIST");
			this.CheckJaro (1.000, "PSYCHIATRI", "PSYCHIATRI");
			this.CheckJaro (0.819, "PSYCHIATRI", "PSYCHIC");
			this.CheckJaro (0.700, "PSYCHIATRI", "PSYCHONEUROS");
			this.CheckJaro (0.703, "PSYCHIATRI", "PSYCHOPHARMACOLOG");
			this.CheckJaro (0.738, "PSYCHIATRI", "PSYCHOS");
			this.CheckJaro (0.783, "PSYCHIATRI", "PSYCHOSI");
			this.CheckJaro (0.433, "PSYCHIATRI", "APPLE");
			this.CheckJaro (0.422, "PSYCHIATRI", "BANANA");
			this.CheckJaro (0.715, "PSYCHIC", "PHYSIC");
			this.CheckJaro (0.730, "PSYCHIC", "PHYSICIAN");
			this.CheckJaro (0.730, "PSYCHIC", "PHYSICIST");
			this.CheckJaro (0.643, "PSYCHIC", "PHYSIOLOGIST");
			this.CheckJaro (0.819, "PSYCHIC", "PSYCHIATRI");
			this.CheckJaro (1.000, "PSYCHIC", "PSYCHIC");
			this.CheckJaro (0.710, "PSYCHIC", "PSYCHONEUROS");
			this.CheckJaro (0.736, "PSYCHIC", "PSYCHOPHARMACOLOG");
			this.CheckJaro (0.809, "PSYCHIC", "PSYCHOS");
			this.CheckJaro (0.869, "PSYCHIC", "PSYCHOSI");
			this.CheckJaro (0.447, "PSYCHIC", "APPLE");
			this.CheckJaro (0.000, "PSYCHIC", "BANANA");
			this.CheckJaro (0.683, "PSYCHONEUROS", "PHYSIC");
			this.CheckJaro (0.666, "PSYCHONEUROS", "PHYSICIAN");
			this.CheckJaro (0.666, "PSYCHONEUROS", "PHYSICIST");
			this.CheckJaro (0.674, "PSYCHONEUROS", "PHYSIOLOGIST");
			this.CheckJaro (0.700, "PSYCHONEUROS", "PSYCHIATRI");
			this.CheckJaro (0.710, "PSYCHONEUROS", "PSYCHIC");
			this.CheckJaro (1.000, "PSYCHONEUROS", "PSYCHONEUROS");
			this.CheckJaro (0.712, "PSYCHONEUROS", "PSYCHOPHARMACOLOG");
			this.CheckJaro (0.861, "PSYCHONEUROS", "PSYCHOS");
			this.CheckJaro (0.819, "PSYCHONEUROS", "PSYCHOSI");
			this.CheckJaro (0.522, "PSYCHONEUROS", "APPLE");
			this.CheckJaro (0.416, "PSYCHONEUROS", "BANANA");
			this.CheckJaro (0.642, "PSYCHOPHARMACOLOG", "PHYSIC");
			this.CheckJaro (0.617, "PSYCHOPHARMACOLOG", "PHYSICIAN");
			this.CheckJaro (0.549, "PSYCHOPHARMACOLOG", "PHYSICIST");
			this.CheckJaro (0.562, "PSYCHOPHARMACOLOG", "PHYSIOLOGIST");
			this.CheckJaro (0.703, "PSYCHOPHARMACOLOG", "PSYCHIATRI");
			this.CheckJaro (0.736, "PSYCHOPHARMACOLOG", "PSYCHIC");
			this.CheckJaro (0.712, "PSYCHOPHARMACOLOG", "PSYCHONEUROS");
			this.CheckJaro (1.000, "PSYCHOPHARMACOLOG", "PSYCHOPHARMACOLOG");
			this.CheckJaro (0.736, "PSYCHOPHARMACOLOG", "PSYCHOS");
			this.CheckJaro (0.700, "PSYCHOPHARMACOLOG", "PSYCHOSI");
			this.CheckJaro (0.505, "PSYCHOPHARMACOLOG", "APPLE");
			this.CheckJaro (0.483, "PSYCHOPHARMACOLOG", "BANANA");
			this.CheckJaro (0.662, "PSYCHOS", "PHYSIC");
			this.CheckJaro (0.689, "PSYCHOS", "PHYSICIAN");
			this.CheckJaro (0.785, "PSYCHOS", "PHYSICIST");
			this.CheckJaro (0.730, "PSYCHOS", "PHYSIOLOGIST");
			this.CheckJaro (0.738, "PSYCHOS", "PSYCHIATRI");
			this.CheckJaro (0.809, "PSYCHOS", "PSYCHIC");
			this.CheckJaro (0.861, "PSYCHOS", "PSYCHONEUROS");
			this.CheckJaro (0.736, "PSYCHOS", "PSYCHOPHARMACOLOG");
			this.CheckJaro (1.000, "PSYCHOS", "PSYCHOS");
			this.CheckJaro (0.958, "PSYCHOS", "PSYCHOSI");
			this.CheckJaro (0.447, "PSYCHOS", "APPLE");
			this.CheckJaro (0.000, "PSYCHOS", "BANANA");
			this.CheckJaro (0.805, "PSYCHOSI", "PHYSIC");
			this.CheckJaro (0.694, "PSYCHOSI", "PHYSICIAN");
			this.CheckJaro (0.789, "PSYCHOSI", "PHYSICIST");
			this.CheckJaro (0.724, "PSYCHOSI", "PHYSIOLOGIST");
			this.CheckJaro (0.783, "PSYCHOSI", "PSYCHIATRI");
			this.CheckJaro (0.869, "PSYCHOSI", "PSYCHIC");
			this.CheckJaro (0.819, "PSYCHOSI", "PSYCHONEUROS");
			this.CheckJaro (0.700, "PSYCHOSI", "PSYCHOPHARMACOLOG");
			this.CheckJaro (0.958, "PSYCHOSI", "PSYCHOS");
			this.CheckJaro (1.000, "PSYCHOSI", "PSYCHOSI");
			this.CheckJaro (0.441, "PSYCHOSI", "APPLE");
			this.CheckJaro (0.000, "PSYCHOSI", "BANANA");
			this.CheckJaro (0.455, "APPLE", "PHYSIC");
			this.CheckJaro (0.437, "APPLE", "PHYSICIAN");
			this.CheckJaro (0.437, "APPLE", "PHYSICIST");
			this.CheckJaro (0.522, "APPLE", "PHYSIOLOGIST");
			this.CheckJaro (0.433, "APPLE", "PSYCHIATRI");
			this.CheckJaro (0.447, "APPLE", "PSYCHIC");
			this.CheckJaro (0.522, "APPLE", "PSYCHONEUROS");
			this.CheckJaro (0.505, "APPLE", "PSYCHOPHARMACOLOG");
			this.CheckJaro (0.447, "APPLE", "PSYCHOS");
			this.CheckJaro (0.441, "APPLE", "PSYCHOSI");
			this.CheckJaro (1.000, "APPLE", "APPLE");
			this.CheckJaro (0.455, "APPLE", "BANANA");
			this.CheckJaro (0.000, "BANANA", "PHYSIC");
			this.CheckJaro (0.425, "BANANA", "PHYSICIAN");
			this.CheckJaro (0.000, "BANANA", "PHYSICIST");
			this.CheckJaro (0.000, "BANANA", "PHYSIOLOGIST");
			this.CheckJaro (0.422, "BANANA", "PSYCHIATRI");
			this.CheckJaro (0.000, "BANANA", "PSYCHIC");
			this.CheckJaro (0.416, "BANANA", "PSYCHONEUROS");
			this.CheckJaro (0.483, "BANANA", "PSYCHOPHARMACOLOG");
			this.CheckJaro (0.000, "BANANA", "PSYCHOS");
			this.CheckJaro (0.000, "BANANA", "PSYCHOSI");
			this.CheckJaro (0.455, "BANANA", "APPLE");
			this.CheckJaro (1.000, "BANANA", "BANANA");
		}


		public void CheckBounds(double expectedDistance, string s1, string s2)
		{
			Assert.AreEqual (expectedDistance, JaroWinkler.ComputeJaroDistance (s1, s2));
			Assert.AreEqual (expectedDistance, JaroWinkler.ComputeJaroWinklerDistance (s1, s2));
		}


		public void CheckJaro(double expectedDistance, string s1, string s2)
		{
			this.Check (expectedDistance, s1, s2, JaroWinkler.ComputeJaroDistance);
		}


		public void CheckJaroWinkler(double expectedDistance, string s1, string s2)
		{
			this.Check (expectedDistance, s1, s2, JaroWinkler.ComputeJaroWinklerDistance);
		}


		public void Check(double expectedDistance, string s1, string s2, Func<string, string, double> function)
		{
			var distance1 = function (s1, s2);
			var distance2 = function (s1, s2);

			Assert.AreEqual (distance1, distance2);
			Assert.IsTrue (distance1 >= JaroWinkler.MinValue);
			Assert.IsTrue (distance1 <= JaroWinkler.MaxValue);
			Assert.IsTrue (System.Math.Abs (expectedDistance - distance1) < 0.01);
		}


	}


}
