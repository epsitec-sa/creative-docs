//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets.Helpers
{
	/// <summary>
	/// La classe FontPreviewer permet de gérer les échantillons de caractères
	/// stockés dans le cache des fontes OpenType.
	/// </summary>
	public sealed class FontPreviewer
	{
		static FontPreviewer()
		{
			OpenType.FontIdentity.Serializing += new Epsitec.Common.OpenType.FontIdentityCallback (FontPreviewer.HandleFontIdentitySerializing);
		}
		
		
		public static void Initialize()
		{
			Drawing.Font.Initialize ();
		}
		
		
		public static Drawing.Path GetPath(OpenType.FontIdentity fid, double ox, double oy, double size)
		{
			if (fid != null)
			{
				if (fid.AssociatedBlob1.Length == 0)
				{
					try
					{
						FontPreviewer.RefreshBlob1 (fid);
					}
					catch (System.Exception ex)
					{
						System.Diagnostics.Debug.WriteLine ("RefreshBlob1 failed : " + ex.Message);
					}
				}
				if (fid.AssociatedBlob1.Length > 0)
				{
					Drawing.Path path = new Drawing.Path ();
					Drawing.Path copy = new Drawing.Path ();
					
					path.SetBlobOfElements (fid.AssociatedBlob1);
					copy.Append (path, size, 0, 0, size, ox, oy, size);
					
					path.Dispose ();
					
					return copy;
				}
			}
			
			return null;
		}
		
		public static Drawing.Path GetPathAbc(OpenType.FontIdentity fid, double ox, double oy, double size)
		{
			if (fid != null)
			{
				if (fid.AssociatedBlob2.Length == 0)
				{
					try
					{
						FontPreviewer.RefreshBlob2 (fid);
					}
					catch (System.Exception ex)
					{
						System.Diagnostics.Debug.WriteLine ("RefreshBlob2 failed : " + ex.Message);
					}
				}
				if (fid.AssociatedBlob2.Length > 0)
				{
					Drawing.Path path = new Drawing.Path ();
					Drawing.Path copy = new Drawing.Path ();
					
					path.SetBlobOfElements (fid.AssociatedBlob2);
					copy.Append (path, size, 0, 0, size, ox, oy, size);
					
					path.Dispose ();
					
					return copy;
				}
			}
			
			return null;
		}
		
		
		private static void HandleFontIdentitySerializing(OpenType.FontIdentity fid)
		{
			Drawing.Font font = Drawing.Font.GetFont (fid);

			if (font != null)
			{
				bool loaded = font.IsOpenTypeFontLoaded;

				if (fid.AssociatedBlob1.Length == 0)
				{
					FontPreviewer.RefreshBlob1 (fid);
				}
				if (fid.AssociatedBlob2.Length == 0)
				{
					FontPreviewer.RefreshBlob2 (fid);
				}

				if (!loaded)
				{
					font.ClearOpenTypeFont ();
				}
			}
		}
		
		private static void RefreshBlob1(OpenType.FontIdentity fid)
		{
			Drawing.Font font = Drawing.Font.GetFont (fid);
			
			if (font != null)
			{
				using (Drawing.Path path = new Drawing.Path ())
				{
					double x = 0;
					double y = 0;
					
					if (fid.IsSymbolFont)
					{
						for (ushort i = 2; i < 100; i++)
						{
							double advance = font.GetGlyphAdvance (i);
							
							if (advance > 0)
							{
								Drawing.Path temp = new Drawing.Path ();
								
								temp.Append (font, i, x, y, 1);
								
								//	Vérifie si le glyphe dessine quelque chose; si ce n'est
								//	pas le cas, on passe simplement au suivant sans avancer.
								
								if ((temp.IsEmpty) ||
									(temp.GetBlobOfElements ().Length < 20))
								{
									temp.Dispose ();
									continue;
								}
								
								temp.Dispose ();
								
								if (x + advance > 5)
								{
									break;
								}
								
								path.Append (font, i, x, y, 1);
								
								x += advance;
							}
						}
					}
					else
					{
						string sample = "AaBbYyZz";
						
						for (int i = 0; i < sample.Length; i++)
						{
							ushort glyph   = font.GetGlyphIndex (sample[i]);
							double advance = font.GetGlyphAdvance (glyph);
							
							path.Append (font, glyph, x, y, 1);
							
							x += advance;
						}
					}
					
					fid.AssociatedBlob1 = path.GetBlobOfElements ();
				}
			}
		}
		
		private static void RefreshBlob2(OpenType.FontIdentity fid)
		{
			Drawing.Font font = Drawing.Font.GetFont (fid);
			
			if (font != null)
			{
				using (Drawing.Path path = new Drawing.Path ())
				{
					double x = 0;
					double y = 0;
					
					if (fid.IsSymbolFont)
					{
						for (ushort i = 2; i < 100; i++)
						{
							double advance = font.GetGlyphAdvance (i);
							
							if (advance > 0)
							{
								Drawing.Path temp = new Drawing.Path ();
								
								temp.Append (font, i, x, y, 1);
								
								//	Vérifie si le glyphe dessine quelque chose; si ce n'est
								//	pas le cas, on passe simplement au suivant sans avancer.
								
								if ((temp.IsEmpty) ||
									(temp.GetBlobOfElements ().Length < 20))
								{
									temp.Dispose ();
									continue;
								}
								
								temp.Dispose ();
								
								if (x + advance > 2.0)
								{
									break;
								}
								
								path.Append (font, i, x, y, 1);
								
								x += advance;
							}
						}
					}
					else
					{
						string sample = "Abc";
						
						for (int i = 0; i < sample.Length; i++)
						{
							ushort glyph   = font.GetGlyphIndex (sample[i]);
							double advance = font.GetGlyphAdvance (glyph);
							
							path.Append (font, glyph, x, y, 1);
							
							x += advance;
						}
					}
					
					fid.AssociatedBlob2 = path.GetBlobOfElements ();
				}
			}
		}
	}
}
