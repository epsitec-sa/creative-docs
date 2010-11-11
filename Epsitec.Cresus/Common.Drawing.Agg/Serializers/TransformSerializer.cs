//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Daniel ROUX

using System.Collections.Generic;
using System;
namespace Epsitec.Common.Drawing.Serializers
{
	public class TransformSerializer : AbstractSerializer
	{
		public string Serialize(Transform transform)
		{
			var buffer = new System.Text.StringBuilder ();

			buffer.Append (this.Serialize (transform.XX));
			buffer.Append (" ");
			buffer.Append (this.Serialize (transform.XY));
			buffer.Append (" ");
			buffer.Append (this.Serialize (transform.YX));
			buffer.Append (" ");
			buffer.Append (this.Serialize (transform.YY));
			buffer.Append (" ");
			buffer.Append (this.Serialize (transform.TX));
			buffer.Append (" ");
			buffer.Append (this.Serialize (transform.TY));

			return buffer.ToString ();
		}


		public static Transform FromDeserialize(string value)
		{
			if (!string.IsNullOrWhiteSpace (value))
			{
				var list = value.Split (' ');

				if (list.Length == 6)
				{
					double xx, xy, yx, yy, tx, ty;

					if (double.TryParse (list[0], out xx))
					{
						if (double.TryParse (list[1], out xy))
						{
							if (double.TryParse (list[2], out yx))
							{
								if (double.TryParse (list[3], out yy))
								{
									if (double.TryParse (list[4], out tx))
									{
										if (double.TryParse (list[5], out ty))
										{
											return new Transform (xx, xy, yx, yy, tx, ty);
										}
									}
								}
							}
						}
					}
				}
			}

			return Transform.Identity;
		}
	}
}
