//	Copyright � 2003-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Widgets.TextField))]

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>TextField</c> class implements a simple text field.
	/// </summary>
	public class TextField : AbstractTextField
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TextField"/> class.
		/// </summary>
		public TextField()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TextField"/> class.
		/// </summary>
		/// <param name="embedder">The embedder.</param>
		public TextField(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}
	}
}
