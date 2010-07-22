using Epsitec.Common.Support.Extensions;

namespace Epsitec.Cresus.DataLayer.Expressions
{


	/// <summary>
	/// The <c>BinaryComparisonFieldWithField</c> class represents a comparison between two
	/// <see cref="Field"/>, such as (a = b).
	/// </summary>
	public class BinaryComparisonFieldWithField : Comparison
	{


		/// <summary>
		/// Builds a new <c>BinaryComparisonFieldWithField</c>.
		/// </summary>
		/// <param name="left">The <see cref="Field"/> on the left of the <see cref="BinaryComparator"/>.</param>
		/// <param name="op">The <see cref="BinaryComparator"/> used by the <c>BinaryComparisonFieldWithField</c>.</param>
		/// <param name="left">The <see cref="Field"/> on the left of the <see cref="BinaryComparator"/>.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="left"/> is null.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="right"/> is null.</exception>
		public BinaryComparisonFieldWithField(Field left, BinaryComparator op, Field right) : base ()
		{
			left.ThrowIfNull ("left");
			right.ThrowIfNull ("right");
			
			this.Left = left;
			this.Operator = op;
			this.Right = right;
		}


		/// <summary>
		/// The left side of the <c>BinaryComparisonFieldWithField</c>.
		/// </summary>
		public Field Left
		{
			get;
			private set;
		}


		/// <summary>
		/// The <see cref="BinaryComparator"/> of the <c>BinaryComparisonFieldWithField</c>.
		/// </summary>
		public BinaryComparator Operator
		{
			get;
			private set;
		}


		/// <summary>
		/// The right side of the <c>BinaryComparisonFieldWithField</c>.
		/// </summary>
		public Field Right
		{
			get;
			private set;
		}


	}


}
