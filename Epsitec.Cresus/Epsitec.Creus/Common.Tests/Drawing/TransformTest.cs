using NUnit.Framework;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Tests.Drawing
{
	[TestFixture]
	public class TransformTest
	{
		static TransformTest()
		{
		}
		
		[SetUp] public void Initialize()
		{
		}
		
		[Test] public void CheckCreation()
		{
			Transform t1 = Transform.Identity;
			Transform t2 = Transform.CreateTranslationTransform (20, 10);
			Transform t3 = Transform.CreateRotationDegTransform (60);
			
			Assert.IsTrue ((t1.XX == 1) && (t1.XY == 0) && (t1.YX == 0) && (t1.YY == 1) && (t1.TX == 0)  && (t1.TY == 0));
			Assert.IsTrue ((t2.XX == 1) && (t2.XY == 0) && (t2.YX == 0) && (t2.YY == 1) && (t2.TX == 20) && (t2.TY == 10));
			Assert.IsTrue ((t3.XX == t3.YY) && (t3.XY == -t3.YX) && (t3.TX == 0) && (t3.TY == 0));
		}
		
		[Test] public void CheckCompare()
		{
			Transform t1 = Transform.Identity;
			Transform t2 = Transform.Identity;
			Transform t3 = t1;
			
//-			Assert.IsTrue (t1 == t1);
			Assert.IsTrue (t1 == t2);
			Assert.IsTrue (t1 == t3);
			
			Assert.AreEqual ((t1 == null), false);
			Assert.AreEqual ((t1 != null), true);
			
			t2 = t2.RotateDeg (10);
			
			Assert.IsTrue (t1 != t2);
			Assert.AreEqual (t1.Equals (t2), false);
			Assert.AreEqual (t1.EqualsStrictly (t2), false);
			Assert.AreEqual (t1.EqualsStrictly (null), false);
			
			t2 = t2.RotateDeg (-10);
			
			Assert.IsTrue (t1 == t2);
			Assert.AreEqual (t1.Equals (t2), true);
		}
		
		[Test] public void CheckRotation()
		{
			Transform identity = Transform.Identity;
			
			Transform t1 = Transform.CreateTranslationTransform (20, 10);
			Transform t2 = Transform.CreateTranslationTransform (-20, -10);
			Transform t3 = Transform.CreateRotationDegTransform (60);
			Transform t4 = Transform.CreateRotationDegTransform (60, new Point (20, 10));
			Transform t5 = Transform.CreateRotationDegTransform (- 60);
			Transform t6 = Transform.CreateRotationDegTransform (- 60, new Point (20, 10));
			Transform t7 = Transform.CreateRotationDegTransform (90);
			Transform t8 = Transform.CreateRotationDegTransform (- 90);
			
			Transform t;
			
			t = Transform.Multiply (t3, t2);
			t = Transform.Multiply (t1, t);
			Assert.IsTrue (t.Equals (t4));
			
			t = Transform.Multiply (t3, t5);
			Assert.IsTrue (t.Equals (identity));
			
			t = Transform.Multiply (t4, t6);
			Assert.IsTrue (t.Equals (identity));
			
			t3 = Transform.CreateRotationDegTransform (-30);
			Point pt = t3.TransformDirect (new Point (1, 3));
			
			Assert.IsTrue (Transform.Equal (pt, new Point (2.366025f, 2.098076f)));

			Point pt1 = t7.TransformDirect (new Point (1, 0));
			Point pt2 = t8.TransformDirect (new Point (1, 0));
			
			Assert.IsTrue (Transform.Equal (pt1, new Point (0, 1)));
			Assert.IsTrue (Transform.Equal (pt2, new Point (0, -1)));
		}
		
		[Test] public void CheckScale()
		{
			Transform identity = Transform.Identity;
			
			Transform t1 = Transform.CreateScaleTransform (5, 8);
			Transform t2 = Transform.CreateScaleTransform (1/5.0f, 1/8.0f);
			
			Transform t;
			
			t = Transform.Multiply (t1, t2);
			Assert.IsTrue (t.Equals (identity));
		}
		
		[Test] public void CheckInversion()
		{
			Transform identity = Transform.Identity;
			
			Transform t1 = Transform.CreateTranslationTransform (20, 10);
			Transform t2 = Transform.CreateTranslationTransform (-20, -10);
			Transform t3 = Transform.CreateRotationDegTransform (60);
			Transform t4 = Transform.CreateRotationDegTransform (60, new Point (20, 10));
			Transform t5 = Transform.CreateRotationDegTransform (- 60);
			Transform t6 = Transform.CreateRotationDegTransform (- 60, new Point (20, 10));
			
			Transform t;
			
			t = Transform.Inverse (t1);
			Assert.IsTrue (t.Equals (t2));
			
			t = Transform.Inverse (t2);
			Assert.IsTrue (t.Equals (t1));
			
			t = Transform.Inverse (t3);
			Assert.IsTrue (t.Equals (t5));
			
			t = Transform.Inverse (t4);
			Assert.IsTrue (t.Equals (t6));
		}
		
		[Test] public void CheckPointTransform()
		{
			Transform identity = Transform.Identity;
			Point pt = new Point (30, 40);
			
			Transform t1 = Transform.CreateTranslationTransform (20, 10);
			Transform t2 = Transform.CreateTranslationTransform (-20, -10);
			Transform t3 = Transform.CreateRotationDegTransform (60);
			Transform t4 = Transform.CreateRotationDegTransform (60, new Point (20, 10));
			Transform t5 = Transform.CreateRotationDegTransform (- 60);
			Transform t6 = Transform.CreateRotationDegTransform (- 60, new Point (20, 10));
			
			Point result;
			
			result = t1.TransformDirect (pt);
			Assert.IsTrue ((result.X == 50) && (result.Y == 50));
			
			result = t2.TransformDirect (pt);
			Assert.IsTrue ((result.X == 10) && (result.Y == 30));
			
			result = t1.TransformInverse (pt);
			Assert.IsTrue ((result.X == 10) && (result.Y == 30));
			
			result = t2.TransformInverse (pt);
			Assert.IsTrue ((result.X == 50) && (result.Y == 50));
			
			result = t4.TransformDirect (pt);
			result = t6.TransformDirect (result);
			Assert.IsTrue (Transform.Equal (result, pt));
			
			result = t5.TransformDirect (pt);
			result = t5.TransformInverse (result);
			Assert.IsTrue (Transform.Equal (result, pt));
			
			result = t6.TransformDirect (pt);
			result = t6.TransformInverse (result);
			Assert.IsTrue (Transform.Equal (result, pt));
			
			pt = new Point (20, 10);
			result = t4.TransformDirect (pt);
			Assert.IsTrue (Transform.Equal (result, pt));
			result = t4.TransformInverse (pt);
			Assert.IsTrue (Transform.Equal (result, pt));
		}	
		
		[Test] public void CheckOps()
		{
			Transform t1 = Transform.CreateTranslationTransform (20, 10);
			Transform t2 = Transform.CreateTranslationTransform (-20, -10);
			Transform t3 = Transform.CreateRotationDegTransform (60);
			Transform t4 = Transform.CreateRotationDegTransform (60, new Point (20, 10));

			Transform t = Transform.Identity;

			t = t.Translate (20, 10);
			Assert.IsTrue (t.Equals (t1));

			t = Transform.Identity;
			t = t.Translate (-20, -10);
			Assert.IsTrue (t.Equals (t2));

			t = Transform.Identity;
			t = t.RotateDeg (60);
			Assert.IsTrue (t.Equals (t3));

			t = Transform.Identity;
			t = t.Translate (-20, -10);
			t = t.RotateDeg (60);
			t = t.Translate (20, 10);
			Assert.IsTrue (t.Equals (t4));
		}	
	}
}
