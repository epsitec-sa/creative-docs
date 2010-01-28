//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Epsitec.App.BanquePiguet
{

	/// <summary>
	/// The ControlKeyComputer class contains the tools used to compute the control key of a number.
	/// It implements the modulo 10 recursive algorithm described at the section C 9.4 of the DTA
	/// specification document.
	/// </summary>
	class ControlKeyComputer
	{

		/// <summary>
		/// This bidimensional array is the table used by the algorithm to find the next report digit
		/// at each step.
		/// </summary>
		protected static int[,] table = new int[,]
		{
			{0, 9, 4, 6, 8, 2, 7, 1, 3, 5},
			{9, 4, 6, 8, 2, 7, 1, 3, 5, 0},
			{4, 6, 8, 2, 7, 1, 3, 5, 0, 9},
			{6, 8, 2, 7, 1, 3, 5, 0, 9, 4},
			{8, 2, 7, 1, 3, 5, 0, 9, 4, 6},
			{2, 7, 1, 3, 5, 0, 9, 4, 6, 8},
			{7, 1, 3, 5, 0, 9, 4, 6, 8, 2},
			{1, 3, 5, 0, 9, 4, 6, 8, 2, 7},
			{3, 5, 0, 9, 4, 6, 8, 2, 7, 1},
			{5, 0, 9, 4, 6, 8, 2, 7, 1, 3},
		};


		/// <summary>
		/// This array contains the control keys.
		/// </summary>
		protected static char[] keys = new char[]
		{
			'0', '9', '8', '7', '6', '5', '4', '3', '2', '1'
		};


		/// <summary>
		/// Computes the control key of number using the modulo 10 recursive algorithm.
		/// </summary>
		/// <param name="number">The number whose control key to compute.</param>
		/// <returns>The control key of number.</returns>
		/// <exception cref="System.ArgumentException">Thrown when the given string is empty or contains non numeric characters.</exception>
		public static char Compute(string number)
		{
			if (!Regex.IsMatch (number, @"^\d*$"))
			{
				throw new ArgumentException ("The provided string contains non numeric characters");
			}

			if (number.Length == 0)
			{
				throw new ArgumentException ("The provided string is empty");
			}

			int report = 0;

			while (number.Length > 0)
			{
				int digit = Int32.Parse (number.Substring (0, 1), CultureInfo.InvariantCulture);
				report = ControlKeyComputer.table[report, digit];
				number = number.Substring (1);
			}

			return ControlKeyComputer.keys[report];
		}

	}

}
