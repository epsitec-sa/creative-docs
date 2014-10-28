//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Data
{
	public struct ComputedAmount : System.IEquatable<ComputedAmount>
	{
		public ComputedAmount(decimal? finalAmount)
		{
			//	Initialise un montant fixe.
			this.InitialAmount   = null;
			this.ArgumentAmount  = null;
			this.FinalAmount     = finalAmount;
			this.Computed        = false;
			this.Subtract        = true;
			this.Rate            = true;
			this.ArgumentDefined = false;
		}

		public ComputedAmount(decimal initialAmount, decimal finalAmount, bool rate = false)
		{
			this.InitialAmount   = initialAmount;
			this.FinalAmount     = finalAmount;
			this.Subtract        = initialAmount > finalAmount;
			this.Computed        = true;
			this.Rate            = rate;
			this.ArgumentAmount  = ComputedAmount.ComputeArgument (this.InitialAmount, finalAmount, this.Subtract, this.Rate);
			this.ArgumentDefined = true;
		}

		public ComputedAmount(decimal? initialAmount, decimal? argumentAmount, decimal? finalAmount, bool subtract, bool rate, bool argumentDefined)
		{
			//	Initialise un montant calculé.
			this.InitialAmount   = initialAmount;
			this.ArgumentAmount  = argumentAmount;
			this.FinalAmount     = finalAmount;
			this.Computed        = true;
			this.Subtract        = subtract;
			this.Rate            = rate;
			this.ArgumentDefined = argumentDefined;
		}

		public ComputedAmount(decimal? initial, ComputedAmount current)
		{
			if (initial.HasValue && current.ArgumentDefined)
			{
				this.InitialAmount   = initial;
				this.ArgumentAmount  = current.ArgumentAmount;
				this.FinalAmount     = ComputedAmount.ComputeFinal (initial, current.ArgumentAmount, current.Subtract, current.Rate);
				this.Computed        = true;
				this.Subtract        = current.Subtract;
				this.Rate            = current.Rate;
				this.ArgumentDefined = true;
			}
			else
			{
				var a = ComputedAmount.ComputeArgument (initial, current.FinalAmount, current.Subtract, current.Rate);

				if (a.HasValue)
				{
					if (a < 0.0m)
					{
						this.InitialAmount   = initial;
						this.ArgumentAmount  = -a;
						this.FinalAmount     = current.FinalAmount;
						this.Computed        = true;
						this.Subtract        = !current.Subtract;
						this.Rate            = current.Rate;
						this.ArgumentDefined = false;
					}
					else
					{
						this.InitialAmount   = initial;
						this.ArgumentAmount  = a;
						this.FinalAmount     = current.FinalAmount;
						this.Computed        = true;
						this.Subtract        = current.Subtract;
						this.Rate            = current.Rate;
						this.ArgumentDefined = false;
					}
				}
				else
				{
					this.InitialAmount   = initial;
					this.ArgumentAmount  = null;
					this.FinalAmount     = current.FinalAmount;
					this.Computed        = true;
					this.Subtract        = current.Subtract;
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
			this.Subtract        = model.Subtract;
			this.Rate            = model.Rate;
			this.ArgumentDefined = model.ArgumentDefined;
		}

		public ComputedAmount(ComputedAmount a, ComputedAmount b)
		{
			this.InitialAmount   = null;
			this.ArgumentAmount  = null;
			this.FinalAmount     = a.FinalAmount + b.FinalAmount;
			this.Computed        = false;
			this.Subtract        = true;
			this.Rate            = true;
			this.ArgumentDefined = false;
		}


		public decimal? ComputeFinal(decimal? argument)
		{
			if (!this.InitialAmount.HasValue && !argument.HasValue && this.FinalAmount.HasValue)
			{
				return this.FinalAmount;
			}
			else
			{
				return ComputedAmount.ComputeFinal (this.InitialAmount, argument, this.Subtract, this.Rate);
			}
		}

		public static decimal? ComputeFinal(decimal? initial, decimal? argument, bool subtract, bool rate)
		{
			if (initial.HasValue)
			{
				var i = initial.GetValueOrDefault (0.0m);
				var a = argument.GetValueOrDefault (0.0m);

				if (rate)
				{
					if (subtract)
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
					if (subtract)
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
				return ComputedAmount.ComputeArgument (this.InitialAmount, final, this.Subtract, this.Rate);
			}
		}

		public static decimal? ComputeArgument(decimal? initial, decimal? final, bool subtract, bool rate)
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
						if (subtract)
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
					if (subtract)
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


		#region IEquatable<ComputedAmount> Members
		public bool Equals(ComputedAmount other)
		{
			return this.InitialAmount   == other.InitialAmount
				&& this.ArgumentAmount  == other.ArgumentAmount
				&& this.FinalAmount     == other.FinalAmount
				&& this.Computed        == other.Computed
				&& this.Subtract        == other.Subtract
				&& this.Rate            == other.Rate
				&& this.ArgumentDefined == other.ArgumentDefined;
		}
		#endregion

		public override bool Equals(object obj)
		{
			if (obj is ComputedAmount)
			{
				return this.Equals ((ComputedAmount) obj);
			}
			else
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			return this.InitialAmount.GetHashCode ()
				 ^ this.ArgumentAmount.GetHashCode ()
				 ^ this.FinalAmount.GetHashCode ()
				 ^ this.Computed.GetHashCode ()
				 ^ this.Subtract.GetHashCode ()
				 ^ this.Rate.GetHashCode ()
				 ^ this.ArgumentDefined.GetHashCode ();
		}

		public static bool operator ==(ComputedAmount a, ComputedAmount b)
		{
			return object.Equals (a, b);
		}

		public static bool operator !=(ComputedAmount a, ComputedAmount b)
		{
			return !(a == b);
		}


		public void Serialize(System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement ("ComputedAmount");

			if (this.InitialAmount.HasValue)
			{
				writer.WriteElementString ("InitialAmount", this.InitialAmount.Value.ToString (System.Globalization.CultureInfo.InvariantCulture));
			}

			if (this.ArgumentAmount.HasValue)
			{
				writer.WriteElementString ("ArgumentAmount", this.ArgumentAmount.Value.ToString (System.Globalization.CultureInfo.InvariantCulture));
			}

			if (this.FinalAmount.HasValue)
			{
				writer.WriteElementString ("FinalAmount", this.FinalAmount.Value.ToString (System.Globalization.CultureInfo.InvariantCulture));
			}

			writer.WriteElementString ("Computed",        this.Computed.ToString        (System.Globalization.CultureInfo.InvariantCulture));
			writer.WriteElementString ("Subtract",        this.Subtract.ToString        (System.Globalization.CultureInfo.InvariantCulture));
			writer.WriteElementString ("Rate",            this.Rate.ToString            (System.Globalization.CultureInfo.InvariantCulture));
			writer.WriteElementString ("ArgumentDefined", this.ArgumentDefined.ToString (System.Globalization.CultureInfo.InvariantCulture));

			writer.WriteEndElement ();
		}


		public readonly decimal?				InitialAmount;
		public readonly decimal?				ArgumentAmount;
		public readonly decimal?				FinalAmount;
		public readonly bool					Computed;
		public readonly bool					Subtract;
		public readonly bool					Rate;
		public readonly bool					ArgumentDefined;
	}
}
