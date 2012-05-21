//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

namespace Epsitec.Cresus.Core.Library
{
	/// <summary>
	/// The <c>SettingsTuple</c> immutable structure stores a key/value pair used to
	/// define settings, as stored by the <see cref="SettingsCollection"/>.
	/// </summary>
	public struct SettingsTuple : System.IEquatable<SettingsTuple>
	{
		public SettingsTuple(string key, string value)
		{
			key.ThrowIfNull ("key");
			value.ThrowIfNull ("value");

			this.key   = key;
			this.value = value;
		}

		
		public string							Key
		{
			get
			{
				return this.key;
			}
		}

		public string							Value
		{
			get
			{
				return this.value;
			}
		}

		
		#region IEquatable<SettingsTuple> Members

		public bool Equals(SettingsTuple other)
		{
			return this.key == other.key
				&& this.value == other.value;
		}

		#endregion

		
		public override bool Equals(object obj)
		{
			if (obj is SettingsTuple)
			{
				return this.Equals ((SettingsTuple) obj);
			}
			else
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			return this.key.GetHashCode ()*7 ^ this.value.GetHashCode ();
		}

		public override string ToString()
		{
			return string.Concat (this.key, " : ", this.value);
		}
		
		
		private readonly string					key;
		private readonly string					value;
	}
}
