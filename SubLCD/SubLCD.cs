using System;
using System.Drawing;
using System.Reflection;
using PaintDotNet;
using PaintDotNet.Effects;
using PaintDotNet.PropertySystem;
using System.Collections.Generic;

namespace SubLCDEffect
{
    public class PluginSupportInfo : IPluginSupportInfo
    {
        public string Author => ((AssemblyCopyrightAttribute)base.GetType().Assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false)[0]).Copyright;
        public string Copyright => ((AssemblyDescriptionAttribute)base.GetType().Assembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false)[0]).Description;
        public string DisplayName => ((AssemblyProductAttribute)base.GetType().Assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false)[0]).Product;
        public Version Version => base.GetType().Assembly.GetName().Version;
        public Uri WebsiteUri => new Uri("http://www.getpaint.net/redirect/plugins.html");
    }

    [PluginSupportInfo(typeof(PluginSupportInfo), DisplayName = "SubLCD")]
    [EffectCategory(EffectCategory.Adjustment)]
    public class SubLCDEffectPlugin : PropertyBasedEffect
    {
        private Surface processedSurface;
        private static readonly Image StaticIcon = new Bitmap(typeof(SubLCDEffectPlugin), "SubLCD.png");

        public SubLCDEffectPlugin()
            : base("SubLCD", StaticIcon, null, EffectFlags.None)
        {
        }

        protected override PropertyCollection OnCreatePropertyCollection()
        {
            List<Property> props = new List<Property>();

            return new PropertyCollection(props);
        }

        protected override void OnSetRenderInfo(PropertyBasedEffectConfigToken newToken, RenderArgs dstArgs, RenderArgs srcArgs)
        {
            Rectangle selection = EnvironmentParameters.GetSelection(srcArgs.Surface.Bounds).GetBoundsInt();

            Surface selectionSurface = new Surface(selection.Width, selection.Height);
            selectionSurface.CopySurface(srcArgs.Surface, selection);

            Surface stretchedSurface = new Surface(selection.Width * 2, selection.Height);
            stretchedSurface.FitSurface(ResamplingAlgorithm.Bicubic, selectionSurface);

            processedSurface = new Surface(srcArgs.Surface.Size);

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
                        processedSurface[v, y + selection.Top] = ColorBgra.FromBgr(t.B, t.G, t.R);
                        v++;
                    }
                }
            }


            base.OnSetRenderInfo(newToken, dstArgs, srcArgs);
        }

        protected override void OnRender(Rectangle[] renderRects, int startIndex, int length)
        {
            if (length == 0) return;

            DstArgs.Surface.CopySurface(processedSurface, renderRects, startIndex, length);
        }
    }
}