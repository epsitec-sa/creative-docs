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
		
		//	Spécial, utilisé dans une condition SqlSelect pour générer une clause JOIN
		
		JoinInner,						//	A.a, B.b -> A INNER JOIN B ON A.a = B.b
		
		//	Equivalents :
		
		CompareNotLessThan				= CompareGreaterThanOrEqual,
		CompareNotGreaterThan			= CompareLessThanOrEqual,
		CompareNotLessThanOrEqual		= CompareGreaterThan,
		CompareNotGreaterThanOrEqual	= CompareLessThan,
	}
}
