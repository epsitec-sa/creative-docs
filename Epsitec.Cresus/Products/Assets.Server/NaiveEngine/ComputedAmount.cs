//	Copyright � 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.NaiveEngine
{
	public struct ComputedAmount
	{
		public ComputedAmount(decimal? finalAmount)
		{
			//	Initialise un montant fixe.
			this.InitialAmount  = null;
			this.ArgumentAmount = null;
			this.FinalAmount    = finalAmount;
			this.Computed       = false;
			this.Substract      = false;
			this.Rate           = false;
		}

		public ComputedAmount(decimal? initialAmount, decimal? argumentAmount, decimal? finalAmount, bool substract, bool rate)
		{
			//	Initialise un montant calcul�.
			this.InitialAmount  = initialAmount;
			this.ArgumentAmount = argumentAmount;
			this.FinalAmount    = finalAmount;
			this.Computed       = true;
			this.Substract      = substract;
			this.Rate           = rate;
		}


		public decimal? ComputeFinal(decimal? argument)
		{
			if (this.InitialAmount.HasValue)
			{
				var i = this.InitialAmount.GetValueOrDefault (0.0m);
				var a = argument.GetValueOrDefault (0.0m);

				if (this.Rate)
				{
					if (this.Substract)
					{
						return i-i*a;
					}
					else
					{
						return i+i*a;
					}
				}
				else
				{
					if (this.Substract)
					{
						return i-a;
					}
					else
					{
						return i+a;
					}
				}
			}
			else
			{
				return null;
			}
		}

		public decimal? ComputeArgument(decimal? final)
		{
			if (this.InitialAmount.HasValue)
			{
				var i = this.InitialAmount.GetValueOrDefault (0.0m);
				var f = final.GetValueOrDefault (0.0m);

				if (this.Rate)
				{
					if (i == 0.0m)
					{
						return null;
					}
					else
					{
						if (this.Substract)
						{
							return (i-f)/i;
						}
						else
						{
							return (f-i)/i;
						}
					}
				}
				else
				{
					if (this.Substract)
					{
						return i-f;
					}
					else
					{
						return f-i;
					}
				}
			}
			else
			{
				return null;
			}
		}


		public static bool operator ==(ComputedAmount a, ComputedAmount b)
		{
			return (a.InitialAmount  == b.InitialAmount)
				&& (a.ArgumentAmount == b.ArgumentAmount)
				&& (a.FinalAmount    == b.FinalAmount)
				&& (a.Computed       == b.Computed)
				&& (a.Substract      == b.Substract)
				&& (a.Rate           == b.Rate);
		}

		public static bool operator !=(ComputedAmount a, ComputedAmount b)
		{
			return (a.InitialAmount  != b.InitialAmount)
				|| (a.ArgumentAmount != b.ArgumentAmount)
				|| (a.FinalAmount    != b.FinalAmount)
				|| (a.Computed       != b.Computed)
				|| (a.Substract      != b.Substract)
				|| (a.Rate           != b.Rate);
		}


		public readonly decimal?			InitialAmount;
		public readonly decimal?			ArgumentAmount;
		public readonly decimal?			FinalAmount;
		public readonly bool				Computed;
		public readonly bool				Substract;
		public readonly bool				Rate;
	}
}
