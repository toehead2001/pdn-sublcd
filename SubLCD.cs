// Name: SubLCD
// Submenu:
// Author: xrl & toe_head2001
// Title:
// Desc:
// Keywords:
// URL: http://www.getpaint.net/redirect/plugins.html
// Help:
void Render(Surface dst, Surface src, Rectangle rect)
{
    Rectangle selection = EnvironmentParameters.GetSelection(src.Bounds).GetBoundsInt();

    Surface selectionSurface = new Surface(selection.Width, selection.Height);
    selectionSurface.CopySurface(src, selection);

    Surface stretchedSurface = new Surface(selection.Width * 2, selection.Height);
    stretchedSurface.FitSurface(ResamplingAlgorithm.Bicubic, selectionSurface);

    ColorBgra t;
    for (int y = 0; y < stretchedSurface.Height; y++)
    {
        if (IsCancelRequested) return;
        int v = selection.Left;
        for (int x = 0; x < stretchedSurface.Width; x += 2)
        {
            if (x % 2 == 0)
            {
                t.R = stretchedSurface[x, y].R;
                t.G = stretchedSurface[x + 1, y].G;
                if (x != stretchedSurface.Width - 2)
                {
                    t.B = stretchedSurface[x + 2, y].B;
                }
                else
                {
                    t.B = (byte)((stretchedSurface[stretchedSurface.Width - 1, y].B + stretchedSurface[stretchedSurface.Width - 2, y].B) >> 1);
                }
                dst[v, y + selection.Top] = ColorBgra.FromBgr(t.B, t.G, t.R);
                v++;
            }
        }
    }
}
