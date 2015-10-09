/*
Paint.net SubLCD resize plugin by xrl - http://web.ist.utl.pt/tcf
based on:
http://www.oyhus.no/SubLCD.html

Should return an image with half the width but I didn't bored search how to do it
*/


unsafe void Render(Surface dst, Surface src, Rectangle rect)
{
    ColorBgra t;
    for (int y = 0; y < src.Height; y++)
    {
        int v = 0;
        for (int x = 0; x < src.Width - 1; x += 2)
        {
            if (x % 2 == 0)
            {
                t.R = src[x, y].R;
                t.G = src[x + 1, y].G;
                if (x != src.Width - 2)
                {
                    t.B = src[x + 2, y].B;
                }
                else
                {
                    t.B = (byte)((src[src.Width - 1, y].B + src[src.Width - 2, y].B) >> 1);
                }
                dst[v, y] = ColorBgra.FromBgr(t.B, t.G, t.R);
                v++;
            }
        }
    }
}
