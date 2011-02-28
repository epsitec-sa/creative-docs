//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;


namespace Epsitec.Cresus.Core.Business.Finance.PriceCalculators
{
	// TODO Comment this class.
	// Marc


	public sealed class NumericDimension : AbstractDimension
	{

		
		public NumericDimension(string code, RoundingMode roundingMode)
			: this (code, roundingMode, new List<decimal> ())
		{
		}


		public NumericDimension(string code, RoundingMode roundingMode, IEnumerable<decimal> values)
			: base (code)
		{
			values.ThrowIfNull ("values");
			
			this.values = new SortedSet<decimal> ();
			this.RoundingMode = roundingMode;

			foreach (decimal value in values)
			{
				this.AddDecimal (value);
			}
		}


		public override IEnumerable<string> Values
		{
			get
			{
				return this.values.Select (v => InvariantConverter.ConvertToString (v));
			}
		}


		public IEnumerable<decimal> DecimalValues
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


		public RoundingMode RoundingMode
		{
			get;
			set;
		}
		

		public override void Add(string value)
		{
			decimal d = NumericDimension.Convert (value);

			this.AddDecimal (d);
		}


		public void AddDecimal(decimal value)
		{
			this.CheckValueIsNotInDimension (value);

			this.values.Add (value);

			string s = NumericDimension.Convert (value);

			if (this.DimensionTable != null)
			{
				this.DimensionTable.NotifyDimensionValueAdded (this, s);
			}
		}


		public override void Remove(string value)
		{
			decimal d = NumericDimension.Convert (value);

			this.RemoveDecimal (d);
		}


		public void RemoveDecimal(decimal value)
		{
			this.CheckValueIsInDimension (value);

			this.values.Remove (value);

			string s = NumericDimension.Convert (value);

			if (this.DimensionTable != null)
			{
				this.DimensionTable.NotifyDimensionValueRemoved (this, s);
			}
		}


		public override bool Contains(string value)
		{
			decimal d = NumericDimension.Convert (value);

			return this.ContainsDecimal (d);
		}


		public bool ContainsDecimal(decimal value)
		{
			return this.values.Contains (value);
		}

		
		public override bool IsValueRoundable(string value)
		{
			decimal d = NumericDimension.Convert (value);

			return this.IsDecimalValueRoundable (d);
		}


		public bool IsDecimalValueRoundable(decimal value)
		{
			switch (this.RoundingMode)
			{
				case RoundingMode.None:
					return this.ContainsDecimal (value);

				case RoundingMode.Down:
				case RoundingMode.Nearest:
				case RoundingMode.Up:
					return (value >= this.values.Min) && (value <= this.values.Max);

				default:
					throw new System.NotImplementedException ();
			}
		}
		

		public override string GetRoundedValue(string value)
		{
			decimal d1 = NumericDimension.Convert (value);
			decimal d2 = this.GetRoundedDecimalValue (d1);

			string s = NumericDimension.Convert (d2);

			return s;
		}


		public decimal GetRoundedDecimalValue(decimal value)
		{
			value.ThrowIf (v => !this.IsDecimalValueRoundable (v), "value is not roundable.");

			if (this.values.Contains (value))
			{
				return value;
			}
			else
			{
				return this.GetNearestValue (value);
			}
		}


		public override int GetIndexOf(string value)
		{
			decimal d = NumericDimension.Convert (value);

			return this.GetIndexOfDecimal (d);
		}


		public int GetIndexOfDecimal(decimal value)
		{
			this.CheckValueIsInDimension (value);

			return this.values.IndexOf (value);
		}


		public override string GetValueAt(int index)
		{
			decimal d = this.values.ElementAt (index);

			string s = NumericDimension.Convert (d);

			return s;
		}


		public decimal GetDecimalValueAt(int index)
		{
			index.ThrowIf (i => i < 0 || i >= this.values.Count, "index is out of range.");

			return this.values.ElementAt (index);
		}

		
		/// <summary>
		/// Rounds the given value to the nearest point in this instance.
		/// </summary>
		/// <param name="value">The value whose nearest value to get.</param>
		/// <returns>The nearest value to get.</returns>
		private decimal GetNearestValue(decimal value)
		{
			switch (this.RoundingMode)
			{
				case RoundingMode.None:
					return value;

				case RoundingMode.Down:
					return this.GetNearestLowerValue (value);

				case RoundingMode.Nearest:
					return this.GetNearestClosestValue (value);

				case RoundingMode.Up:
					return this.GetNearestUpperValue (value);

				default:
					throw new System.NotImplementedException ();
			}
		}


		/// <summary>
		/// Rounds the given value to the nearest point in this instance with the "down" Rounding
		/// strategy.
		/// </summary>
		/// <param name="value">The value whose nearest value to get.</param>
		/// <returns>The nearest value to get.</returns>
		private decimal GetNearestLowerValue(decimal value)
		{
			return this.values.Reverse ().First (e => e < value);
		}


		/// <summary>
		/// Rounds the given value to the nearest point in this instance with the "up" Rounding
		/// strategy.
		/// </summary>
		/// <param name="value">The value whose nearest value to get.</param>
		/// <returns>The nearest value to get.</returns>
		private decimal GetNearestUpperValue(decimal value)
		{
			return this.values.First (e => e > value);
		}


		/// <summary>
		/// Rounds the given value to the nearest point in this instance with the "closest" Rounding
		/// strategy.
		/// </summary>
		/// <param name="value">The value whose nearest value to get.</param>
		/// <returns>The nearest value to get.</returns>
		private decimal GetNearestClosestValue(decimal value)
		{
			decimal nearestLower = this.GetNearestLowerValue (value);
			decimal nearestUpper = this.GetNearestUpperValue (value);

			if (nearestUpper - value <= value - nearestLower)
			{
				return nearestUpper;
			}
			else
			{
				return nearestLower;
			}
		}


		/// <summary>
		/// Gets a <see cref="System.String"/> that contains the data that is necessary to serialize
		/// the current instance and deserialize it later.
		/// </summary>
		/// <returns> A <see cref="System.String"/> that can be used to build a clone of the current instance.</returns>
		public override string GetStringData()
		{
			string mode = System.Enum.GetName (typeof (RoundingMode), this.RoundingMode);

			var values = this.values
				.Select (v => InvariantConverter.ConvertToString (v));

			return mode + NumericDimension.valueSeparator + string.Join (NumericDimension.valueSeparator, values);
		}


		/// <summary>
		/// Builds a new instance of <see cref="NumericDimension"/> given a name and the serialized data
		/// obtained by the <see cref="NumericDimension.GetStringData"/> method.
		/// </summary>
		/// <param name="code">The code of the dimension.</param>
		/// <param name="name">The name of the dimension.</param>
		/// <param name="stringData">The serialized string data.</param>
		/// <returns>The new <see cref="NumericDimension"/>.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="code"> is <c>null</c> or empty.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="name"> is <c>null</c> or empty.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="stringData"> is <c>null</c>, empty or invalid.</exception>
		public static NumericDimension BuildNumericDimension(string code, string stringData)
		{
			stringData.ThrowIfNullOrEmpty ("stringData");

			var splittedData = stringData.Split (NumericDimension.valueSeparator).ToList ();

			var values = splittedData.Skip (1).Select (v => InvariantConverter.ConvertFromString<decimal> (v));
			var mode = InvariantConverter.ToEnum<RoundingMode> (splittedData[0]);

			return new NumericDimension (code, mode, values);
		}


		private static decimal Convert(string value)
		{
			value.ThrowIfNullOrEmpty ("value");

			try
			{
				return InvariantConverter.ConvertFromString<decimal> (value);
			}
			catch (System.Exception e)
			{
				throw new System.ArgumentException ("invalid value", e);
			}
		}


		private static string Convert(decimal value)
		{
			return InvariantConverter.ConvertToString (value);
		}


		private void CheckValueIsNotInDimension(decimal value)
		{
			value.ThrowIf (v => this.values.Contains (v), "value is already in this instance.");
		}


		private void CheckValueIsInDimension(decimal value)
		{
			value.ThrowIf (v => !this.values.Contains (v), "value is not in this instance.");
		}


		private readonly SortedSet<decimal> values;
		private static readonly string valueSeparator = ";";
	}
}
