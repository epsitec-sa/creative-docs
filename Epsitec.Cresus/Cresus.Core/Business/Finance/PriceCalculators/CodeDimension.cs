using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.Core.Business.Finance.PriceCalculators
{


	/// <summary>
	/// A <see cref="CodeDimension"/> is an <see cref="AbstractDimension"/> whose points are codes,
	/// i.e. simple strings such as "a", "b", "c", etc. That mean that only the defined points are
	/// defined on the dimention, and that there is no concept of nearest value for this kind of
	/// dimension.
	/// </summary>
	public sealed class CodeDimension : AbstractDimension
	{
		
		
		/// <summary>
		/// Builds a new <see cref="CodeDimension"/>.
		/// </summary>
		/// <remarks>
		/// The codes that are the points of the dimension must be alpha numeric strings.
		/// </remarks>
		/// <param name="name">The name of the dimension.</param>
		/// <param name="values">The codes that are the points of the dimension.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="values"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="values"/> is empty, contains <c>null</c> or empty or invalid elements.</exception>
		public CodeDimension(string name, IEnumerable<string> values)
			: base (name)
		{
			values.ThrowIfNull ("values");
			
			this.values = new SortedSet<string> (values);

			this.values.ThrowIf (v => !v.Any (), "values is empty.");
			this.values.ThrowIf (e => e.Any (v => string.IsNullOrEmpty (v)), "values in values cannot be null or empty.");
			this.values.ThrowIf (e => e.Any (v => !v.IsAlphaNumeric ()), "values in values must be alpha numeric.");
		}


		/// <summary>
		/// Gets the codes that are the points of this instance.
		/// </summary>
		public override IEnumerable<object> Values
		{
			get
			{
				return this.values.Cast<object> ();
			}
		}


		/// <summary>
		/// Tells whether the given value is a code that defines the points of this instance.
		/// </summary>
		/// <param name="value">The value to check.</param>
		/// <returns><c>true</c> if the given value is a code, <c>false</c> if it isn't.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="value"/> is <c>null</c>.</exception>
		public override bool IsValueDefined(object value)
		{
			value.ThrowIfNull ("value");

			return (value is string) && this.values.Contains ((string) value);
		}


		/// <summary>
		/// Tells whether there is a nearest code for the given value.
		/// </summary>
		/// <remarks>
		/// As there is no concept of nearest value for the codes, this method is strictly
		/// equivalent to <see cref="CodeDimension.IsValueDefined"/>.
		/// </remarks>
		/// <param name="value">The value to check.</param>
		/// <returns><c>true</c> if there is a nearest code for the given value is a code, <c>false</c> if there isn't.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="value"/> is <c>null</c>.</exception>
		public override bool IsNearestValueDefined(object value)
		{
			return this.IsValueDefined (value);
		}


		/// <summary>
		/// Gets the nearest code for the given value.
		/// </summary>
		/// <remarks>
		/// As there is no concept of nearest value for the codes, this method will fail if
		/// it is called with anything that is not a defined code for the current instance.
		/// </remarks>
		/// <param name="value">The value whose nearest code to get.</param>
		/// <returns>The nearest code.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="value"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">If there is no nearest code for <paramref name="value"/>.</exception>
		public override object GetNearestValue(object value)
		{
			if (!this.IsValueDefined (value))
			{
				throw new System.ArgumentException ("The given value is not defined on the current dimension.");
			}

			return value;
		}

		
		/// <summary>
		/// Gets a <see cref="System.String"/> that contains the data that is necessary to serialize
		/// the current instance and deserialize it later.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> that can be used to build a clone of the current instance.
		/// </returns>
		public override string GetStringData()
		{
			return string.Join (CodeDimension.valueSeparator, this.values);
		}


		/// <summary>
		/// Builds a new instance of <see cref="CodeDimension"/> given a name and the serialized data
		/// obtained by the <see cref="CodeDimension.GetStringData"/> method.
		/// </summary>
		/// <param name="name">The name of the dimension.</param>
		/// <param name="stringData">The serialized string data.</param>
		/// <returns>The new <see cref="CodeDimension"/>.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="name"> is <c>null</c> or empty.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="stringData"> is <c>null</c>, empty or invalid.</exception>
		public static CodeDimension BuildCodeDimension(string name, string stringData)
		{
			name.ThrowIfNullOrEmpty ("name");
			stringData.ThrowIfNullOrEmpty ("stringData");

			var values = stringData.Split (CodeDimension.valueSeparator);

			return new CodeDimension (name, values);
		}


		/// <summary>
		/// The codes that are the points of the current instance.
		/// </summary>
		private SortedSet<string> values;


		/// <summary>
		/// The separator used to separate the different codes in the serialized string data.
		/// </summary>
		private static readonly string valueSeparator = ";";


	}


}
