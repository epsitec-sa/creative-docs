namespace Epsitec.Common.Drawing
{
	/// <summary>
	/// Summary description for IPaintPort.
	/// </summary>
	public interface IPaintPort
	{
		double		LineWidth		{ get; set; }
		JoinStyle	LineJoin		{ get; set; }
		CapStyle	LineCap			{ get; set; }
		double		LineMiterLimit	{ get; set; }
		Color		Color			{ get; set; }
		
		void SetClippingRectangle(Rectangle rect);
		void SetClippingRectangle(Point p, Size s);
		void SetClippingRectangle(double x, double y, double width, double height);
		Rectangle SaveClippingRectangle();
		void RestoreClippingRectangle(Rectangle rect);
		void ResetClippingRectangle();
		bool TestForEmptyClippingRectangle();
		
		void Align(ref double x, ref double y);
		
		Transform SaveTransform();
		double GetTransformZoom();
		void RestoreTransform(Transform transform);
		void ScaleTransform(double sx, double sy, double cx, double cy);
		void RotateTransform(double angle, double cx, double cy);
		void TranslateTransform(double ox, double oy);
		
		void PaintOutline(Path path);
		void PaintSurface(Path path);
		
		double PaintText(double x, double y, string text, Font font, double size);
		double PaintText(double x, double y, string text, Font font, double size, Font.ClassInfo[] infos);
		
		void PaintImage(Image bitmap, Rectangle fill);
		void PaintImage(Image bitmap, double fill_x, double fill_y, double fill_width, double fill_height);
		void PaintImage(Image bitmap, Rectangle fill, Point image_origin);
		void PaintImage(Image bitmap, Rectangle fill, Rectangle image_rect);
		void PaintImage(Image bitmap, double fill_x, double fill_y, double fill_width, double fill_height, double image_origin_x, double image_origin_y);
		void PaintImage(Image bitmap, double fill_x, double fill_y, double fill_width, double fill_height, double image_origin_x, double image_origin_y, double image_width, double image_height);
	}
}
