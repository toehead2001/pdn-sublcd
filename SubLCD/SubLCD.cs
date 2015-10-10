using System;
using System.Drawing;
using System.Reflection;
using PaintDotNet;
using PaintDotNet.Effects;
using PaintDotNet.PropertySystem;

namespace SubLCDEffect
{
    public class PluginSupportInfo : IPluginSupportInfo
    {
        public string Author
        {
            get
            {
                return ((AssemblyCopyrightAttribute)base.GetType().Assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false)[0]).Copyright;
            }
        }
        public string Copyright
        {
            get
            {
                return ((AssemblyDescriptionAttribute)base.GetType().Assembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false)[0]).Description;
            }
        }

        public string DisplayName
        {
            get
            {
                return ((AssemblyProductAttribute)base.GetType().Assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false)[0]).Product;
            }
        }

        public Version Version
        {
            get
            {
                return base.GetType().Assembly.GetName().Version;
            }
        }

        public Uri WebsiteUri
        {
            get
            {
                return new Uri("http://www.getpaint.net/redirect/plugins.html");
            }
        }
    }

    [PluginSupportInfo(typeof(PluginSupportInfo), DisplayName = "SubLCD")]
    [EffectCategory(EffectCategory.Adjustment)]
    public class SubLCDEffectPlugin : PropertyBasedEffect
    {
        public static string StaticName
        {
            get
            {
                return "SubLCD";
            }
        }

        public static Image StaticIcon
        {
            get
            {
                return new Bitmap(typeof(SubLCDEffectPlugin), "SubLCD.png");
            }
        }

        public static string SubmenuName
        {
            get
            {
                return null;  // Programmer's chosen default
            }
        }

        public SubLCDEffectPlugin()
            : base(StaticName, StaticIcon, SubmenuName, EffectFlags.None)
        {
        }

        protected override PropertyCollection OnCreatePropertyCollection()
        {
            return null;
        }

        protected override void OnSetRenderInfo(PropertyBasedEffectConfigToken newToken, RenderArgs dstArgs, RenderArgs srcArgs)
        {
            Rectangle selection = EnvironmentParameters.GetSelection(srcArgs.Surface.Bounds).GetBoundsInt();

            Surface selectionSurface = new Surface(selection.Width, selection.Height);
            selectionSurface.CopySurface(srcArgs.Surface, selection);

            stretchedSurface = new Surface(selection.Width * 2, selection.Height);
            stretchedSurface.FitSurface(ResamplingAlgorithm.Bicubic, selectionSurface);
        }

        protected override unsafe void OnRender(Rectangle[] rois, int startIndex, int length)
        {
            if (length == 0) return;
            for (int i = startIndex; i < startIndex + length; ++i)
            {
                Render(DstArgs.Surface, SrcArgs.Surface, rois[i]);
            }
        }

        #region User Entered Code
        // Name: SubLCD
        // Submenu:
        // Author: xrl & toe_head2001
        // Title:
        // Desc:
        // Keywords:
        // URL: http://www.getpaint.net/redirect/plugins.html
        // Help:
        private Surface stretchedSurface;

        void Render(Surface dst, Surface src, Rectangle rect)
        {
            Rectangle selection = EnvironmentParameters.GetSelection(src.Bounds).GetBoundsInt();

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

        #endregion
    }
}