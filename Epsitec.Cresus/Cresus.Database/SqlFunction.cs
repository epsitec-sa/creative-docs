//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 07/10/2003
//			 remis en chantier, DD, 19/11/2003 (ToString)

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe SqlFunction décrit toute une famille de fonctions SQL:
	/// des opérations mathématiques simples (+, -, *...), des opérations
	/// de comparaison (=, &lt;, &gt;, &lt;&gt;, IS NULL, LIKE,...), des
	/// tests d'appartenance (IN, NOT IN, BETWEEN, NOT BETWEEN, EXISTS,
	/// NOT EXISTS...)
	/// </summary>
	public class SqlFunction
	{
		public SqlFunction(SqlFunctionType type, params SqlField[] fields)
		{
			this.type = type;
			
			int count_provided  = fields.Length;
			int count_expected = this.ArgumentCount;
			
			if (count_provided != count_expected)
			{
				throw new System.ArgumentOutOfRangeException (type.ToString () + " requires " + count_expected.ToString () + " field");
			}
			
			switch (count_provided)
			{
				case 1: this.a = fields[0]; break;
				case 2: this.a = fields[0]; this.b = fields[1]; break;
				case 3: this.a = fields[0]; this.b = fields[1]; this.c = fields[2]; break;
			}
		}
		
		public SqlFunctionType			Type
		{
			get { return this.type; }
		}
		
		public int						ArgumentCount
		{
			get
			{
				switch (this.type)
				{
					case SqlFunctionType.MathAdd:
					case SqlFunctionType.MathSubstract:
					case SqlFunctionType.MathMultiply:
					case SqlFunctionType.MathDivide:
						return 2;
					
					
					case SqlFunctionType.CompareEqual:
					case SqlFunctionType.CompareNotEqual:
					case SqlFunctionType.CompareLessThan:
					case SqlFunctionType.CompareLessThanOrEqual:
					case SqlFunctionType.CompareGreaterThan:
					case SqlFunctionType.CompareGreaterThanOrEqual:
						return 2;
					
					case SqlFunctionType.CompareIsNull:
					case SqlFunctionType.CompareIsNotNull:
						return 1;
					
					case SqlFunctionType.CompareLike:
					case SqlFunctionType.CompareNotLike:
						return 2;
					
					
					case SqlFunctionType.SetIn:
					case SqlFunctionType.SetNotIn:
						return 2;
					
					case SqlFunctionType.SetBetween:
					case SqlFunctionType.SetNotBetween:
						return 3;
					
					case SqlFunctionType.SetExists:
					case SqlFunctionType.SetNotExists:
						return 1;
					
					case SqlFunctionType.LogicNot:
						return 1;

					case SqlFunctionType.LogicAnd:
					case SqlFunctionType.LogicOr:
						return 2;

					case SqlFunctionType.JoinInner:
						return 2;

					case SqlFunctionType.Substring:
						return 3;

					case SqlFunctionType.Upper:
						return 1;
					
					default:
						return 0;
				}
			}
		}
		
		public SqlField					A
		{
			get { return this.a; }
		}
		
		public SqlField					B
		{
			get { return this.b; }
		}
		
		public SqlField					C
		{
			get { return this.c; }
		}

		public override string ToString()
		{
			//	Converti la fonction en chaîne de caractère SQL standard
			//	DD:	me semble utile d'avoir cette primitive dans la classe SqlFunction
			//		si certain moteur on besoin d'une autre convertion,
			//		alors il faudrait dérivée une classe fille (ie SqlFunctionFirebird)
			switch (this.type)
			{
				case SqlFunctionType.MathAdd:
					return	this.A.ToString() + " + " + this.B.ToString();

				case SqlFunctionType.MathSubstract:
					return	this.A.ToString() + " - " + this.B.ToString();
				case SqlFunctionType.MathMultiply:
					return	this.A.ToString() + " * " + this.B.ToString();
				case SqlFunctionType.MathDivide:
					return	this.A.ToString() + " / " + this.B.ToString();
					
				case SqlFunctionType.CompareEqual:
					return	this.A.ToString() + " = " + this.B.ToString();

				case SqlFunctionType.CompareNotEqual:
					return	this.A.ToString() + " <> " + this.B.ToString();

				case SqlFunctionType.CompareLessThan:
					return	this.A.ToString() + " < " + this.B.ToString();

				case SqlFunctionType.CompareLessThanOrEqual:
					return	this.A.ToString() + " <= " + this.B.ToString();

				case SqlFunctionType.CompareGreaterThan:
					return	this.A.ToString() + " > " + this.B.ToString();

				case SqlFunctionType.CompareGreaterThanOrEqual:
					return	this.A.ToString() + " >= " + this.B.ToString();
					
				case SqlFunctionType.CompareIsNull:
					return	this.A.ToString() + " IS NULL";

				case SqlFunctionType.CompareIsNotNull:
					return	this.A.ToString() + " IS NOT NULL";
					
				case SqlFunctionType.CompareLike:
					return	this.A.ToString() + " LIKE " + this.B.ToString();
				case SqlFunctionType.CompareNotLike:
					return	this.A.ToString() + " NOT LIKE " + this.B.ToString();	// ?
					
				case SqlFunctionType.SetIn:
					return	this.A.ToString() + " IN " + this.B.ToString();

				case SqlFunctionType.SetNotIn:
					return	this.A.ToString() + " NOT IN " + this.B.ToString();
					
				case SqlFunctionType.SetBetween:
					return	this.A.ToString() + " BETWEEN " + this.B.ToString() + " AND " + this.C.ToString();

				case SqlFunctionType.SetNotBetween:
					return	this.A.ToString() + " NOT BETWEEN " + this.B.ToString() + " AND " + this.C.ToString();
					
				case SqlFunctionType.SetExists:
					return	this.A.ToString() + " EXISTS";

				case SqlFunctionType.SetNotExists:
					return	this.A.ToString() + " NOT EXISTS";
					
				case SqlFunctionType.LogicNot:
					return	"NOT " + this.A.ToString();

				case SqlFunctionType.LogicAnd:
					return	this.A.ToString() + " AND " + this.B.ToString();

				case SqlFunctionType.LogicOr:
					return	this.A.ToString() + " OR " + this.B.ToString();

				case SqlFunctionType.JoinInner:
					return	this.A.AsName + " INNER JOIN " + this.B.AsName + " ON " +
							this.A.AsQualifiedName + " = " + this.B.AsQualifiedName;

				case SqlFunctionType.Substring:
					return	"SUBSTRING(" + this.A.ToString() + " FROM " + this.B.ToString() +
							" FOR " + this.C.ToString() + ")";

				case SqlFunctionType.Upper:
					return	"UPPER(" + this.A.ToString() + ")";
					
				default:
					return	"";
			}
		}
		
		protected SqlFunctionType		type;
		protected SqlField				a, b, c;
	}
	
	public enum SqlFunctionType
	{
		Unsupported,
		
		MathAdd,						//	a + b
		MathSubstract,					//	a - b
		MathMultiply,					//	a * b
		MathDivide,						//	a / b
		
		CompareEqual,					//	a = b
		CompareNotEqual,				//	a <> b
		CompareLessThan,				//	a < b
		CompareLessThanOrEqual,			//	a <= b
		CompareGreaterThan,				//	a > b
		CompareGreaterThanOrEqual,		//	a <= b
		CompareIsNull,					//	a IS NULL
		CompareIsNotNull,				//	a NOT IS NULL
		CompareLike,					//	a LIKE b
		CompareNotLike,					//	a NOT LIKE b
		
		SetIn,							//	a IN b
		SetNotIn,						//	a NOT IN b
		SetBetween,						//	a BETWEEN b AND c
		SetNotBetween,					//	a NOT BETWEEN b AND c
		SetExists,						//	a EXISTS
		SetNotExists,					//	a NOT EXISTS
		
		LogicNot,						//	NOT a
		LogicAnd,						//	a AND b
		LogicOr,						//	a OR b
		
		Substring,						//	SUBSTRING(a FROM b FOR c)
		Upper,							//	UPPER(a)
		Cast,							//	CAST(a AS b)
		
		//	Spécial, utilisé dans une condition SqlSelect pour générer une clause JOIN
		
		JoinInner,						//	A.a, B.b -> A INNER JOIN B ON A.a = B.b
		
		//	Equivalents :
		
		CompareNotLessThan				= CompareGreaterThanOrEqual,
		CompareNotGreaterThan			= CompareLessThanOrEqual,
		CompareNotLessThanOrEqual		= CompareGreaterThan,
		CompareNotGreaterThanOrEqual	= CompareLessThan,
	}
}
