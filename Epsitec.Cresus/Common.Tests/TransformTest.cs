using System;
using NUnit.Framework;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Tests
{
	[TestFixture]
	public class TransformTest
	{
		static TransformTest()
		{
			try { System.Diagnostics.Debug.WriteLine (""); } 
			catch { }
		}
		
		[SetUp] public void Initialise()
		{
		}
		
		[Test] public void TestCreation()
		{
			Transform t1 = new Transform ();
			Transform t2 = Transform.FromTranslation (20, 10);
			Transform t3 = Transform.FromRotation (60);
			
			Assertion.Assert ((t1.XX == 1) && (t1.XY == 0) && (t1.YX == 0) && (t1.YY == 1) && (t1.TX == 0)  && (t1.TY == 0));
			Assertion.Assert ((t2.XX == 1) && (t2.XY == 0) && (t2.YX == 0) && (t2.YY == 1) && (t2.TX == 20) && (t2.TY == 10));
			Assertion.Assert ((t3.XX == t3.YY) && (t3.XY == -t3.YX) && (t3.TX == 0) && (t3.TY == 0));
		}
		
		[Test] public void TestRotation()
		{
			Transform identity = new Transform ();
			
			Transform t1 = Transform.FromTranslation (20, 10);
			Transform t2 = Transform.FromTranslation (-20, -10);
			Transform t3 = Transform.FromRotation (60);
			Transform t4 = Transform.FromRotation (60, new System.Drawing.PointF (20, 10));
			Transform t5 = Transform.FromRotation (- 60);
			Transform t6 = Transform.FromRotation (- 60, new System.Drawing.PointF (20, 10));
			
			Transform t;
			
			t = Transform.Multiply (t3, t2);
			t = Transform.Multiply (t1, t);
			Assertion.Assert (t.Equals (t4));
			
			t = Transform.Multiply (t3, t5);
			Assertion.Assert (t.Equals (identity));
			
			t = Transform.Multiply (t4, t6);
			Assertion.Assert (t.Equals (identity));
			
			Assertion.Assert (t.EqualsStrictly (identity) == false);
			t.Round ();
			Assertion.Assert (t.EqualsStrictly (identity));
			
			t3 = Transform.FromRotation (30);
			System.Drawing.PointF pt = t3.TransformDirect (new System.Drawing.PointF (1, 3));
			
			Assertion.Assert (Transform.Equal (pt, new System.Drawing.PointF (2.366025f, 2.098076f)));
		}
		
		[Test] public void TestScale()
		{
			Transform identity = new Transform ();
			
			Transform t1 = Transform.FromScale (5, 8);
			Transform t2 = Transform.FromScale (1/5.0f, 1/8.0f);
			
			Transform t;
			
			t = Transform.Multiply (t1, t2);
			Assertion.Assert (t.Equals (identity));
		}
		
		[Test] public void TestInversion()
		{
			Transform identity = new Transform ();
			
			Transform t1 = Transform.FromTranslation (20, 10);
			Transform t2 = Transform.FromTranslation (-20, -10);
			Transform t3 = Transform.FromRotation (60);
			Transform t4 = Transform.FromRotation (60, new System.Drawing.PointF (20, 10));
			Transform t5 = Transform.FromRotation (- 60);
			Transform t6 = Transform.FromRotation (- 60, new System.Drawing.PointF (20, 10));
			
			Transform t;
			
			t = Transform.Inverse (t1);
			Assertion.Assert (t.Equals (t2));
			
			t = Transform.Inverse (t2);
			Assertion.Assert (t.Equals (t1));
			
			t = Transform.Inverse (t3);
			Assertion.Assert (t.Equals (t5));
			
			t = Transform.Inverse (t4);
			Assertion.Assert (t.Equals (t6));
		}
		
		[Test] public void TestPointTransform()
		{
			Transform identity = new Transform ();
			System.Drawing.PointF pt = new System.Drawing.PointF (30, 40);
			
			Transform t1 = Transform.FromTranslation (20, 10);
			Transform t2 = Transform.FromTranslation (-20, -10);
			Transform t3 = Transform.FromRotation (60);
			Transform t4 = Transform.FromRotation (60, new System.Drawing.PointF (20, 10));
			Transform t5 = Transform.FromRotation (- 60);
			Transform t6 = Transform.FromRotation (- 60, new System.Drawing.PointF (20, 10));
			
			System.Drawing.PointF result;
			
			result = t1.TransformDirect (pt);
			Assertion.Assert ((result.X == 50) && (result.Y == 50));
			
			result = t2.TransformDirect (pt);
			Assertion.Assert ((result.X == 10) && (result.Y == 30));
			
			result = t1.TransformInverse (pt);
			Assertion.Assert ((result.X == 10) && (result.Y == 30));
			
			result = t2.TransformInverse (pt);
			Assertion.Assert ((result.X == 50) && (result.Y == 50));
			
			result = t4.TransformDirect (pt);
			result = t6.TransformDirect (result);
			Assertion.Assert (Transform.Equal (result, pt));
			
			result = t5.TransformDirect (pt);
			result = t5.TransformInverse (result);
			Assertion.Assert (Transform.Equal (result, pt));
			
			result = t6.TransformDirect (pt);
			result = t6.TransformInverse (result);
			Assertion.Assert (Transform.Equal (result, pt));
		}	
		
		[Test] public void TestOps()
		{
			Transform t1 = Transform.FromTranslation (20, 10);
			Transform t2 = Transform.FromTranslation (-20, -10);
			Transform t3 = Transform.FromRotation (60);
			Transform t4 = Transform.FromRotation (60, new System.Drawing.PointF (20, 10));
			
			Transform t = new Transform ();
			
			t.Translate (20, 10);
			Assertion.Assert (t.Equals (t1));
			
			t.Reset ();
			t.Translate (-20, -10);
			Assertion.Assert (t.Equals (t2));
			
			t.Reset ();
			t.Rotate (60);
			Assertion.Assert (t.Equals (t3));
			
			t.Reset ();
			t.Translate (-20, -10);
			t.Rotate (60);
			t.Translate (20, 10);
			Assertion.Assert (t.Equals (t4));
		}	
	}
}
