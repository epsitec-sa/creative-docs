//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.Core.Business.Finance.PriceCalculators
{
	// TODO Comment this class.
	// Marc

	public sealed class CodeDimension : AbstractDimension
	{
		public CodeDimension(string code)
			: this (code, new List<string> ())
		{
		}


		public CodeDimension(string code, IEnumerable<string> values)
			: base (code)
		{
			values.ThrowIfNull ("values");

			this.values = new List<string> ();

			foreach (string value in values)
			{
				this.Add (value);
			}
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
			index.ThrowIf (i => i < 0 || i > this.values.Count, "Index is out of range.");

			value.ThrowIfNullOrEmpty ("value");
			this.CheckValueIsNotInDimension (value);
			
			this.values.Insert (index, value);

			if (this.DimensionTable != null)
			{
				this.DimensionTable.NotifyDimensionValueAdded (this, value);
			}
		}

		public override void Remove(string value)
		{
			value.ThrowIfNullOrEmpty ("value");
			this.CheckValueIsInDimension (value);
			
			int index = this.GetIndexOf (value);

			this.values.RemoveAt (index);
		}

		public void RemoveAt(int index)
		{
			index.ThrowIf (i => i < 0 || i >= this.values.Count, "Index is out of range.");

			string value = this.values[index];

			this.values.RemoveAt (index);

			if (this.DimensionTable != null)
			{
				this.DimensionTable.NotifyDimensionValueRemoved (this, value);
			}
		}

		public void Swap(string value1, string value2)
		{
			value1.ThrowIfNullOrEmpty ("value1");
			value2.ThrowIfNullOrEmpty ("value2");
			
			this.CheckValueIsInDimension (value1);
			this.CheckValueIsInDimension (value2);

			int index1 = this.GetIndexOf (value1);
			int index2 = this.GetIndexOf (value2);

			this.SwapAt (index1, index2);
		}

		public void SwapAt(int index1, int index2)
		{
			index1.ThrowIf (i => i < 0 || i >= this.values.Count, "Index1 is out of range.");
			index2.ThrowIf (i => i < 0 || i >= this.values.Count, "Index2 is out of range.");

			string value1 = this.values[index1];
			string value2 = this.values[index2];

			this.values[index1] = value2;
			this.values[index2] = value1;
		}

		public override bool Contains(string value)
		{
			value.ThrowIfNullOrEmpty ("value");
			
			return this.values.Contains (value);
		}

		public override bool IsValueRoundable(string value)
		{
			value.ThrowIfNullOrEmpty ("value");
			
			return this.Contains (value);
		}

		public override string GetRoundedValue(string value)
		{
			value.ThrowIfNullOrEmpty ("value");
			this.CheckValueIsInDimension (value);

			return value;
		}

		public override int GetIndexOf(string value)
		{
			value.ThrowIfNullOrEmpty ("value");
			this.CheckValueIsInDimension (value);

			return this.values.IndexOf (value);
		}

		public override string GetValueAt(int index)
		{
			index.ThrowIf (i => i < 0 || i >= this.values.Count, "Index is out of range.");

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
			return StringPacker.Pack (this.values, CodeDimension.separatorChar, CodeDimension.escapeChar);
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
		public static CodeDimension BuildCodeDimension(string code, string stringData)
		{
			code.ThrowIfNullOrEmpty ("code");
			stringData.ThrowIfNullOrEmpty ("stringData");

			var values = StringPacker.Unpack (stringData, CodeDimension.separatorChar, CodeDimension.escapeChar);

			return new CodeDimension (code, values);
		}


		private void CheckValueIsNotInDimension(string value)
		{
			value.ThrowIf (v1 => this.values.Any (v2 => v1 == v2), "value is already in this instance.");
		}

		private void CheckValueIsInDimension(string value)
		{
			value.ThrowIf (v1 => this.values.All (v2 => v1 != v2), "value is already in this instance.");
		}


		private readonly List<string> values;
		private static readonly char separatorChar = ';';
		private static readonly char escapeChar = ':';
	}
}
