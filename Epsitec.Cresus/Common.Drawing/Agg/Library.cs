using System;
using System.Runtime.InteropServices;

namespace Epsitec.Common.Drawing.Agg
{
	public class Library : System.IDisposable
	{
		Library()
		{
			AntiGrain.Interface.Initialise ();
		}
		
		~Library()
		{
			this.Dispose (false);
		}
		
		[DllImport ("AntiGrain.Win32.dll")] internal extern static bool		AggInitialise();
		[DllImport ("AntiGrain.Win32.dll")] internal extern static void		AggShutDown();
		[DllImport ("AntiGrain.Win32.dll")] internal extern static IntPtr	AggGetVersion();
		[DllImport ("AntiGrain.Win32.dll")] internal extern static IntPtr	AggGetProductName();
		
		[DllImport ("AntiGrain.Win32.dll")] internal extern static IntPtr	AggBufferNew(int dx, int dy, int bpp);
		[DllImport ("AntiGrain.Win32.dll")] internal extern static void		AggBufferResize(IntPtr buffer, int dx, int dy, int bpp);
		[DllImport ("AntiGrain.Win32.dll")] internal extern static void		AggBufferPaint(IntPtr buffer, IntPtr hdc, int x1, int y1, int x2, int y2);
		[DllImport ("AntiGrain.Win32.dll")] internal extern static void		AggBufferPaintOffset(IntPtr buffer, IntPtr hdc, int ox, int oy, int x1, int y1, int x2, int y2);
		[DllImport ("AntiGrain.Win32.dll")] internal extern static void		AggBufferBlendOffset(IntPtr buffer, IntPtr hdc, int ox, int oy, int x1, int y1, int x2, int y2);
		[DllImport ("AntiGrain.Win32.dll")] internal extern static void		AggBufferClear(IntPtr buffer);
		[DllImport ("AntiGrain.Win32.dll")] internal extern static void		AggBufferClearRect(IntPtr buffer, int x1, int y1, int x2, int y2);
		[DllImport ("AntiGrain.Win32.dll")] internal extern static void		AggBufferDelete(IntPtr buffer);
		
		[DllImport ("AntiGrain.Win32.dll")] internal extern static IntPtr	AggRendererSolidNew(System.IntPtr buffer);
		[DllImport ("AntiGrain.Win32.dll")] internal extern static void		AggRendererSolidClear(System.IntPtr renderer, double r, double g, double b, double a);
		[DllImport ("AntiGrain.Win32.dll")] internal extern static void		AggRendererSolidColor(System.IntPtr renderer, double r, double g, double b, double a);
		[DllImport ("AntiGrain.Win32.dll")] internal extern static void		AggRendererSolidDelete(System.IntPtr renderer);
		
		
		[DllImport ("AntiGrain.Win32.dll")] internal extern static IntPtr	AggRendererImageNew(System.IntPtr buffer);
		[DllImport ("AntiGrain.Win32.dll")] internal extern static void		AggRendererImageMatrix(System.IntPtr renderer, double xx, double xy, double yx, double yy, double tx, double ty);
		[DllImport ("AntiGrain.Win32.dll")] internal extern static void		AggRendererImageSource1(System.IntPtr renderer, System.IntPtr buffer);
		[DllImport ("AntiGrain.Win32.dll")] internal extern static void		AggRendererImageSource2(System.IntPtr renderer, System.IntPtr byte_buffer, int dx, int dy, int stride);
		[DllImport ("AntiGrain.Win32.dll")] internal extern static void		AggRendererImageDelete(System.IntPtr renderer);
		
		[DllImport ("AntiGrain.Win32.dll")] internal extern static IntPtr	AggRendererGradientNew(System.IntPtr buffer);
		[DllImport ("AntiGrain.Win32.dll")] internal extern static void		AggRendererGradientDelete(System.IntPtr renderer);
		[DllImport ("AntiGrain.Win32.dll")] internal extern static void		AggRendererGradientSelect(System.IntPtr renderer, int id);
		[DllImport ("AntiGrain.Win32.dll")] internal extern static void		AggRendererGradientColor1(System.IntPtr renderer, [MarshalAs(UnmanagedType.LPArray, SizeConst=256)] double[] r,
																														  [MarshalAs(UnmanagedType.LPArray, SizeConst=256)] double[] g,
																														  [MarshalAs(UnmanagedType.LPArray, SizeConst=256)] double[] b,
																														  [MarshalAs(UnmanagedType.LPArray, SizeConst=256)] double[] a);
		[DllImport ("AntiGrain.Win32.dll")] internal extern static void		AggRendererGradientRange(System.IntPtr renderer, double r1, double r2);
		[DllImport ("AntiGrain.Win32.dll")] internal extern static void		AggRendererGradientMatrix(System.IntPtr renderer, double xx, double xy, double yx, double yy, double tx, double ty);
		
		[DllImport ("AntiGrain.Win32.dll")] internal extern static IntPtr	AggRasterizerNew();
		[DllImport ("AntiGrain.Win32.dll")] internal extern static void		AggRasterizerClear(System.IntPtr rasterizer);
		[DllImport ("AntiGrain.Win32.dll")] internal extern static void		AggRasterizerFillingRule(System.IntPtr rasterizer, int mode);
		[DllImport ("AntiGrain.Win32.dll")] internal extern static void		AggRasterizerGamma(System.IntPtr rasterizer, double gamma);
		[DllImport ("AntiGrain.Win32.dll")] internal extern static void		AggRasterizerSetTransform(System.IntPtr rasterizer, double xx, double xy, double yx, double yy, double tx, double ty);
		[DllImport ("AntiGrain.Win32.dll")] internal extern static void		AggRasterizerSetClipBox(System.IntPtr rasterizer, double x1, double y1, double x2, double y2);
		[DllImport ("AntiGrain.Win32.dll")] internal extern static void		AggRasterizerResetClipBox(System.IntPtr rasterizer);
		[DllImport ("AntiGrain.Win32.dll")] internal extern static void		AggRasterizerAddPath(System.IntPtr rasterizer, System.IntPtr path, bool curves);
		[DllImport ("AntiGrain.Win32.dll")] internal extern static void		AggRasterizerAddGlyph(System.IntPtr rasterizer, System.IntPtr face, int glyph, double x, double y, double scale);
		[DllImport ("AntiGrain.Win32.dll", CharSet=CharSet.Unicode)]
										internal extern static double	AggRasterizerAddText(System.IntPtr rasterizer, System.IntPtr face, string text, int mode, double xx, double xy, double yx, double yy, double tx, double ty);
		[DllImport ("AntiGrain.Win32.dll")] internal extern static void		AggRasterizerAddPathStroke1(System.IntPtr rasterizer, System.IntPtr path, double width, bool curves);
		[DllImport ("AntiGrain.Win32.dll")] internal extern static void		AggRasterizerAddPathStroke2(System.IntPtr rasterizer, System.IntPtr path, double width, int cap, int join, double miter_limit, bool curves);
		[DllImport ("AntiGrain.Win32.dll")] internal extern static void		AggRasterizerRenderSolid(System.IntPtr rasterizer, System.IntPtr renderer);
		[DllImport ("AntiGrain.Win32.dll")] internal extern static void		AggRasterizerRenderImage(System.IntPtr rasterizer, System.IntPtr renderer);
		[DllImport ("AntiGrain.Win32.dll")] internal extern static void		AggRasterizerRenderGradient(System.IntPtr rasterizer, System.IntPtr renderer);
		[DllImport ("AntiGrain.Win32.dll")] internal extern static void		AggRasterizerDelete(System.IntPtr rasterizer);
		
		[DllImport ("AntiGrain.Win32.dll")] internal extern static IntPtr	AggPathNew();
		[DllImport ("AntiGrain.Win32.dll")] internal extern static void		AggPathMoveTo(System.IntPtr path, double x, double y);
		[DllImport ("AntiGrain.Win32.dll")] internal extern static void		AggPathLineTo(System.IntPtr path, double x, double y);
		[DllImport ("AntiGrain.Win32.dll")] internal extern static void		AggPathCurve3(System.IntPtr path, double x_c, double y_c, double x, double y);
		[DllImport ("AntiGrain.Win32.dll")] internal extern static void		AggPathCurve4(System.IntPtr path, double x_c1, double y_c1, double x_c2, double y_c2, double x, double y);
		[DllImport ("AntiGrain.Win32.dll")] internal extern static void		AggPathClose(System.IntPtr path);
		[DllImport ("AntiGrain.Win32.dll")] internal extern static void		AggPathAddNewPath(System.IntPtr path);
		[DllImport ("AntiGrain.Win32.dll")] internal extern static void		AggPathAppendGlyph(System.IntPtr path, System.IntPtr face, int glyph, double xx, double xy, double yx, double yy, double tx, double ty, double bold);
		[DllImport ("AntiGrain.Win32.dll")] internal extern static void		AggPathAppendPath(System.IntPtr path, System.IntPtr path2, double xx, double xy, double yx, double yy, double tx, double ty, double bold);
		[DllImport ("AntiGrain.Win32.dll")] internal extern static void		AggPathComputeBounds(System.IntPtr path, out double x1, out double y1, out double x2, out double y2);
		[DllImport ("AntiGrain.Win32.dll")] internal extern static void		AggPathRemoveAll(System.IntPtr path);
		[DllImport ("AntiGrain.Win32.dll")] internal extern static int		AggPathElemCount(System.IntPtr path);
		[DllImport ("AntiGrain.Win32.dll")] internal extern static void		AggPathElemGet(System.IntPtr path, int n, int[] types, double[] x, double[] y);
		[DllImport ("AntiGrain.Win32.dll")] internal extern static void		AggPathDelete(System.IntPtr path);
		
		[DllImport ("AntiGrain.Win32.dll")] internal extern static void		AggFontInitialise();
		[DllImport ("AntiGrain.Win32.dll")] internal extern static int		AggFontGetFaceCount();
		[DllImport ("AntiGrain.Win32.dll")] internal extern static IntPtr	AggFontGetFaceByRank(int n);
		[DllImport ("AntiGrain.Win32.dll", CharSet=CharSet.Unicode)]
										internal extern static IntPtr	AggFontGetFaceByName(string family, string style, string optical);
		[DllImport ("AntiGrain.Win32.dll", CharSet=CharSet.Unicode)]
										internal extern static string	AggFontFaceGetName(System.IntPtr face, int id);
		[DllImport ("AntiGrain.Win32.dll")] internal extern static int		AggFontFaceGetGlyphIndex(System.IntPtr face, int unicode);
		[DllImport ("AntiGrain.Win32.dll")] internal extern static double	AggFontFaceGetGlyphAdvance(System.IntPtr face, int glyph);
		[DllImport ("AntiGrain.Win32.dll")] internal extern static double	AggFontFaceGetCharAdvance(System.IntPtr face, int unicode);
		[DllImport ("AntiGrain.Win32.dll", CharSet=CharSet.Unicode)]
										internal extern static double	AggFontFaceGetTextAdvance(System.IntPtr face, string text, int mode);
		[DllImport ("AntiGrain.Win32.dll")] internal extern static void		AggFontFaceGetGlyphBounds(System.IntPtr face, int glyph, out double x_min, out double y_min, out double x_max, out double y_max);
		[DllImport ("AntiGrain.Win32.dll")] internal extern static void		AggFontFaceGetCharBounds(System.IntPtr face, int unicode, out double x_min, out double y_min, out double x_max, out double y_max);
		[DllImport ("AntiGrain.Win32.dll", CharSet=CharSet.Unicode)]
										internal extern static void		AggFontFaceGetTextBounds(System.IntPtr face, string text, int mode, out double x_min, out double y_min, out double x_max, out double y_max);
		[DllImport ("AntiGrain.Win32.dll")] internal extern static double	AggFontFaceGetMetrics(System.IntPtr face, int id);
		[DllImport ("AntiGrain.Win32.dll", CharSet=CharSet.Unicode)]
										internal extern static int		AggFontFaceGetTextCharEndXArray(System.IntPtr face, string text, int mode, double[] x_array);
		[DllImport ("AntiGrain.Win32.dll", CharSet=CharSet.Unicode)]
										internal extern static IntPtr	AggFontFaceBreakNew(System.IntPtr face, string text, int mode);
		[DllImport ("AntiGrain.Win32.dll")]	internal extern static IntPtr	AggFontFaceBreakIter(System.IntPtr context, ref double width, out int n_char);
		[DllImport ("AntiGrain.Win32.dll")]	internal extern static void		AggFontFaceBreakDelete(System.IntPtr context);
		[DllImport ("AntiGrain.Win32.dll")]	internal extern static bool		AggFontFaceBreakHasMore(System.IntPtr context);
		
		[DllImport ("AntiGrain.Win32.dll", CharSet=CharSet.Unicode)]
										internal extern static void		AggFontPixelCacheFill(System.IntPtr face, string text, double scale, double ox, double oy);
		[DllImport ("AntiGrain.Win32.dll", CharSet=CharSet.Unicode)]
										internal extern static void		AggFontPixelCacheRender(System.IntPtr buffer, System.IntPtr face, string text, double scale, double ox, double oy);

		
		[DllImport ("AntiGrain.Win32.dll")] internal extern static void		AggDebugGetCycles(out System.UInt32 high, out System.UInt32 low);
		[DllImport ("AntiGrain.Win32.dll")] internal extern static int		AggDebugGetCycleDelta();
		
		
		public static long			Cycles
		{
			get
			{
				uint high, low;
				Library.AggDebugGetCycles (out high, out low);
				long cycles = (((long)high) << 32) + low;
				return cycles;
			}
		}
		
		public static int			CycleDelta
		{
			get
			{
				return Library.AggDebugGetCycleDelta ();
			}
		}
		
		public void Dispose()
		{
			this.Dispose (true);
			GC.SuppressFinalize (this);
		}
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
			}
			
			Library.instance = null;
			AntiGrain.Interface.ShutDown ();
		}
		
		public string				ProductName
		{
			get
			{
				string text;
				
				unsafe
				{
					System.IntPtr ptr = Library.AggGetProductName ();
					text = new string ((char*) ptr);
				}
				
				return text;
			}
		}
		
		public string				Version
		{
			get
			{
				string text;
				
				unsafe
				{
					System.IntPtr ptr = Library.AggGetVersion ();
					text = new string ((char*) ptr);
				}
				
				return text;
			}
		}
		
		
		public static Library		Current
		{
			get
			{
				return Library.instance;
			}
		}
		
		
		static Library				instance = new Library ();
	}
}