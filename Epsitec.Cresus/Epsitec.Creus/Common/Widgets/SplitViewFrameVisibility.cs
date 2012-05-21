//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>SplitViewFrameVisibility</c> structure defines the visibility of its frames.
	/// </summary>
	public struct SplitViewFrameVisibility : System.IEquatable<SplitViewFrameVisibility>
	{
		public SplitViewFrameVisibility(bool frame1, bool frame2)
		{
			this.frame1 = frame1;
			this.frame2 = frame2;
		}

		public bool								Frame1
		{
			get
			{
				return this.frame1;
			}
		}

		public bool								Frame2
		{
			get
			{
				return this.frame2;
			}
		}


		public static bool operator ==(SplitViewFrameVisibility a, SplitViewFrameVisibility b)
		{
			return a.Equals (b);
		}

		public static bool operator !=(SplitViewFrameVisibility a, SplitViewFrameVisibility b)
		{
			return ! a.Equals (b);
		}

		public override bool Equals(object obj)
		{
			if (obj is SplitViewFrameVisibility)
			{
				return this.Equals ((SplitViewFrameVisibility) obj);
			}
			else
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			return (this.frame1 ? 0 : 0x01)
				+  (this.frame2 ? 0 : 0x02);
		}

		#region IEquatable<SplitViewFrameVisibility> Members

		public bool Equals(SplitViewFrameVisibility other)
		{
			return this.frame1 == other.frame1
				&& this.frame2 == other.frame2;
		}

		#endregion

		private readonly bool frame1;
		private readonly bool frame2;
	}
}
