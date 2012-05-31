using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Loader;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.Expressions
{


	/// <summary>
	/// The <c>BinaryComparison</c> class represents a comparison between two
	/// <see cref="Value"/>, such as (a = b).
	/// </summary>
	public class BinaryComparison : Comparison
	{


		/// <summary>
		/// Builds a new <c>BinaryComparison</c>.
		/// </summary>
		/// <param name="left">The <see cref="Value"/> on the left of the <see cref="BinaryComparator"/>.</param>
		/// <param name="op">The <see cref="BinaryComparator"/> used by the <c>BinaryComparison</c>.</param>
		/// <param name="right">The <see cref="Value"/> on the left of the <see cref="BinaryComparator"/>.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="left"/> is null.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="right"/> is null.</exception>
		public BinaryComparison(Value left, BinaryComparator op, Value right)
			: base ()
		{
			left.ThrowIfNull ("left");
			right.ThrowIfNull ("right");
			
			this.Left = left;
			this.Operator = op;
			this.Right = right;
		}


		/// <summary>
		/// The left side of the <c>BinaryComparison</c>.
		/// </summary>
		public Value Left
		{
			get;
			private set;
		}


		/// <summary>
		/// The <see cref="BinaryComparator"/> of the <c>BinaryComparison</c>.
		/// </summary>
		public BinaryComparator Operator
		{
			get;
			private set;
		}


		/// <summary>
		/// The right side of the <c>BinaryComparison</c>.
		/// </summary>
		public Value Right
		{
			get;
			private set;
		}


		internal override SqlFunction CreateSqlCondition(SqlFieldBuilder builder)
		{
			return new SqlFunction
			(
				EnumConverter.ToSqlFunctionCode (this.Operator),
				this.Left.CreateSqlField (builder),
				this.Right.CreateSqlField (builder)
			);
		}


		internal override void CheckFields(FieldChecker checker)
		{
			this.Left.CheckField (checker);
			this.Right.CheckField (checker);
		}


		internal override void AddEntities(HashSet<AbstractEntity> entities)
		{
			this.Left.AddEntities (entities);
			this.Right.AddEntities (entities);
		}


	}


}
