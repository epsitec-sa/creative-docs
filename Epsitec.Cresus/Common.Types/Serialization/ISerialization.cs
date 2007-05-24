//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Types.Serialization
{
	/// <summary>
	/// The <c>ISerialization</c> interface is used to notify objects immediately
	/// before their serialization.
	/// </summary>
	public interface ISerialization
	{
		bool NotifySerializationStarted(Serialization.Context context);
		void NotifySerializationCompleted(Serialization.Context context);
	}
}
