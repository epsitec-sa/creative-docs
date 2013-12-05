//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	public struct ComputedAmount
	{
		public ComputedAmount(decimal? finalAmount)
		{
			//	Initialise un montant fixe.
			this.InitialAmount   = null;
			this.ArgumentAmount  = null;
			this.FinalAmount     = finalAmount;
			this.Computed        = false;
			this.Substract       = true;
			this.Rate            = true;
			this.ArgumentDefined = false;
		}

		public ComputedAmount(decimal initialAmount, decimal finalAmount, bool rate = false)
		{
			this.InitialAmount   = initialAmount;
			this.FinalAmount     = finalAmount;
			this.Substract       = initialAmount > finalAmount;
			this.Computed        = true;
			this.Rate            = rate;
			this.ArgumentAmount  = ComputedAmount.ComputeArgument (this.InitialAmount, finalAmount, this.Substract, this.Rate);
			this.ArgumentDefined = true;
		}

		public ComputedAmount(decimal? initialAmount, decimal? argumentAmount, decimal? finalAmount, bool substract, bool rate, bool argumentDefined)
		{
			//	Initialise un montant calculé.
			this.InitialAmount   = initialAmount;
			this.ArgumentAmount  = argumentAmount;
			this.FinalAmount     = finalAmount;
			this.Computed        = true;
			this.Substract       = substract;
			this.Rate            = rate;
			this.ArgumentDefined = argumentDefined;
		}

		public ComputedAmount(decimal initial, ComputedAmount current)
		{
			if (current.ArgumentDefined)
			{
				this.InitialAmount   = initial;
				this.ArgumentAmount  = current.ArgumentAmount;
				this.FinalAmount     = ComputedAmount.ComputeFinal (initial, current.ArgumentAmount, current.Substract, current.Rate);
				this.Computed        = true;
				this.Substract       = current.Substract;
				this.Rate            = current.Rate;
				this.ArgumentDefined = true;
			}
			else
			{
				var a = ComputedAmount.ComputeArgument (initial, current.FinalAmount, current.Substract, current.Rate).Value;

				if (a < 0.0m)
				{
					this.InitialAmount   = initial;
					this.ArgumentAmount  = -a;
					this.FinalAmount     = current.FinalAmount;
					this.Computed        = true;
					this.Substract       = !current.Substract;
					this.Rate            = current.Rate;
					this.ArgumentDefined = false;
				}
				else
				{
					this.InitialAmount   = initial;
					this.ArgumentAmount  = a;
					this.FinalAmount     = current.FinalAmount;
					this.Computed        = true;
					this.Substract       = current.Substract;
					this.Rate            = current.Rate;
					this.ArgumentDefined = false;
				}
			}
		}

		public ComputedAmount(ComputedAmount model, decimal ratio)
		{
			if (model.InitialAmount.HasValue)
			{
				this.InitialAmount = model.InitialAmount * ratio;
			}
			else
			{
				this.InitialAmount = model.InitialAmount;
			}

			if (model.ArgumentAmount.HasValue && !model.Rate)
			{
				this.ArgumentAmount = model.ArgumentAmount * ratio;
			}
			else
			{
				this.ArgumentAmount = model.ArgumentAmount;
			}

			if (model.FinalAmount.HasValue)
			{
				this.FinalAmount = model.FinalAmount * ratio;
			}
			else
			{
				this.FinalAmount = model.FinalAmount;
			}

			this.Computed        = model.Computed;
			this.Substract       = model.Substract;
			this.Rate            = model.Rate;
			this.ArgumentDefined = model.ArgumentDefined;
		}


		public decimal? ComputeFinal(decimal? argument)
		{
			if (!this.InitialAmount.HasValue && !argument.HasValue && this.FinalAmount.HasValue)
			{
				return this.FinalAmount;
			}
			else
			{
				return ComputedAmount.ComputeFinal (this.InitialAmount, argument, this.Substract, this.Rate);
			}
		}

		public static decimal? ComputeFinal(decimal? initial, decimal? argument, bool substract, bool rate)
		{
			if (initial.HasValue)
			{
				var i = initial.GetValueOrDefault (0.0m);
				var a = argument.GetValueOrDefault (0.0m);

				if (rate)
				{
					if (substract)
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
					if (substract)
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
			if (!this.InitialAmount.HasValue && !final.HasValue && this.ArgumentAmount.HasValue)
			{
				return this.ArgumentAmount;
			}
			else
			{
				return ComputedAmount.ComputeArgument (this.InitialAmount, final, this.Substract, this.Rate);
			}
		}

		public static decimal? ComputeArgument(decimal? initial, decimal? final, bool substract, bool rate)
		{
			if (initial.HasValue)
			{
				var i = initial.GetValueOrDefault (0.0m);
				var f = final.GetValueOrDefault (0.0m);

				if (rate)
				{
					if (i == 0.0m)
					{
						return null;
					}
					else
					{
						if (substract)
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
					if (substract)
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
			return (a.InitialAmount   == b.InitialAmount)
				&& (a.ArgumentAmount  == b.ArgumentAmount)
				&& (a.FinalAmount     == b.FinalAmount)
				&& (a.Computed        == b.Computed)
				&& (a.Substract       == b.Substract)
				&& (a.Rate            == b.Rate)
				&& (a.ArgumentDefined == b.ArgumentDefined);
		}

		public static bool operator !=(ComputedAmount a, ComputedAmount b)
		{
			return (a.InitialAmount   != b.InitialAmount)
				|| (a.ArgumentAmount  != b.ArgumentAmount)
				|| (a.FinalAmount     != b.FinalAmount)
				|| (a.Computed        != b.Computed)
				|| (a.Substract       != b.Substract)
				|| (a.Rate            != b.Rate)
				|| (a.ArgumentDefined != b.ArgumentDefined);
		}


		public readonly decimal?				InitialAmount;
		public readonly decimal?				ArgumentAmount;
		public readonly decimal?				FinalAmount;
		public readonly bool					Computed;
		public readonly bool					Substract;
		public readonly bool					Rate;
		public readonly bool					ArgumentDefined;
	}
}
