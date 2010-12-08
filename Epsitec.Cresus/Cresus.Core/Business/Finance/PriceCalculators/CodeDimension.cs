using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.Core.Business.Finance.PriceCalculators
{


	public sealed class CodeDimension : AbstractDimension
	{
		
		
		public CodeDimension(string code, string name, IEnumerable<string> values)
			: base (code, name)
		{
			this.values = new List<string> (values);
		}


		public override IEnumerable<string> Values
		{
			get
			{
				return this.values;
			}
		}


		public override int Count
		{
			get
			{
				return this.values.Count;
			}
		}
		

		public override void Add(string value)
		{
			this.Insert (this.values.Count, value);
		}


		public void Insert(int index, string value)
		{
			this.values.Insert (index, value);

			this.DimensionTable.NotifyDimensionValueAdded (this, value);
		}


		public override void Remove(string value)
		{
			int index = this.GetIndexOf (value);

			this.values.RemoveAt (index);
		}


		public void RemoveAt(int index)
		{
			string value = this.values[index];

			this.values.RemoveAt (index);

			this.DimensionTable.NotifyDimensionValueRemoved (this, value);
		}


		public void Swap(string value1, string value2)
		{
			int index1 = this.GetIndexOf (value1);
			int index2 = this.GetIndexOf (value2);

			this.SwapAt (index1, index2);
		}


		public void SwapAt(int index1, int index2)
		{
			string value1 = this.values[index1];
			string value2 = this.values[index2];

			this.values[index1] = value2;
			this.values[index2] = value1;
		}


		public override bool Contains(string value)
		{
			return this.values.Contains (value);
		}


		public override bool IsValueRoundable(string value)
		{
			return this.Contains (value);
		}


		public override string GetRoundedValue(string value)
		{
			return value;
		}
		

		public override int GetIndexOf(string value)
		{
			return this.values.IndexOf (value);
		}


		public override string GetValueAt(int index)
		{
			return this.values[index];
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
		/// <param name="code">The code of the dimension.</param>
		/// <param name="name">The name of the dimension.</param>
		/// <param name="stringData">The serialized string data.</param>
		/// <returns>The new <see cref="CodeDimension"/>.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="code"> is <c>null</c> or empty.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="name"> is <c>null</c> or empty.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="stringData"> is <c>null</c>, empty or invalid.</exception>
		public static CodeDimension BuildCodeDimension(string code, string name, string stringData)
		{
			code.ThrowIfNullOrEmpty ("code");
			name.ThrowIfNullOrEmpty ("name");
			stringData.ThrowIfNullOrEmpty ("stringData");

			var values = stringData.Split (CodeDimension.valueSeparator);

			return new CodeDimension (code, name, values);
		}


		/// <summary>
		/// The codes that are the points of the current instance.
		/// </summary>
		private List<string> values;


		/// <summary>
		/// The separator used to separate the different codes in the serialized string data.
		/// </summary>
		private static readonly string valueSeparator = ";";


	}


}
