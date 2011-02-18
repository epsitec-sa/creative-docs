//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Daniel ROUX

using System.Collections.Generic;
using System;

namespace Epsitec.Common.Drawing.Serializers
{
	public class PathSerializer : AbstractSerializer
	{
		public PathSerializer(int resolution = 2)
			: base (resolution)
		{
		}

		public string Serialize(Path path)
		{
			if (path.IsEmpty)
			{
				return "";
			}

			PathElement[] elements;
			Point[] points;
			path.GetElements (out elements, out points);

			var buffer = new System.Text.StringBuilder ();

			Point p1 = Point.Zero;
			Point p2 = Point.Zero;
			Point p3 = Point.Zero;
			bool addSpace = false;
			int i = 0;
			while (i < elements.Length)
			{
				switch (elements[i] & PathElement.MaskCommand)
				{
					case PathElement.MoveTo:
						p1 = points[i++];

						if (addSpace)
						{
							buffer.Append (" ");
						}

						buffer.Append ("M ");
						buffer.Append (this.Serialize (p1));
						addSpace = true;
						break;

					case PathElement.LineTo:
						p1 = points[i++];

						if (addSpace)
						{
							buffer.Append (" ");
						}

						buffer.Append ("L ");
						buffer.Append (this.Serialize (p1));
						addSpace = true;
						break;

					case PathElement.Curve3:
						p1 = points[i++];
						p2 = points[i++];

						if (addSpace)
						{
							buffer.Append (" ");
						}

						buffer.Append ("Q ");
						buffer.Append (this.Serialize (p1));
						buffer.Append (" ");
						buffer.Append (this.Serialize (p2));
						addSpace = true;
						break;

					case PathElement.Curve4:
						p1 = points[i++];
						p2 = points[i++];
						p3 = points[i++];

						if (addSpace)
						{
							buffer.Append (" ");
						}

						buffer.Append ("C ");
						buffer.Append (this.Serialize (p1));
						buffer.Append (" ");
						buffer.Append (this.Serialize (p2));
						buffer.Append (" ");
						buffer.Append (this.Serialize (p3));
						addSpace = true;
						break;

					default:
						if ((elements[i] & PathElement.FlagClose) != 0)
						{
							if (addSpace)
							{
								buffer.Append (" ");
							}

							buffer.Append ("Z");
							addSpace = true;
						}
						i++;
						break;
				}
			}

			return buffer.ToString ();
		}


		public static Path Parse(string value)
		{
			var path = new Path ();

			if (!string.IsNullOrEmpty (value))
			{
				var list = value.Split (' ');

				int i = 0;
				while (i < list.Length)
				{
					switch (list[i++])
					{
						case "M":
							path.MoveTo (PathSerializer.GetPoint (list, ref i));
							break;

						case "L":
							path.LineTo (PathSerializer.GetPoint (list, ref i));
							break;

						case "Q":
							path.CurveTo (PathSerializer.GetPoint (list, ref i),
										  PathSerializer.GetPoint (list, ref i));
							break;

						case "C":
							path.CurveTo (PathSerializer.GetPoint (list, ref i),
										  PathSerializer.GetPoint (list, ref i),
										  PathSerializer.GetPoint (list, ref i));
							break;

						case "Z":
							path.Close ();
							break;

						default:
							throw new System.ArgumentException ("Invalid serialized path");
					}
				}
			}

			return path;
		}

		private static Point GetPoint(string[] list, ref int i)
		{
			double x, y;

			if (double.TryParse (list[i++], out x))
			{
				if (double.TryParse (list[i++], out y))
				{
					return new Point (x, y);
				}
			}

			return Point.Zero;
		}
	}
}
