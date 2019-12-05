using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace QuickShoot.Helpers
{
    public class Config
    {
        public string ImageStorePath { get; set; }
        public SolidColorBrush SelectedBrush { get; set; }
        public bool EnableAnimations { get; set; }
        public bool EnableBlurEffect { get; set; }
        public bool EnableLiteMode { get; set; }
        public bool EnableOnlyClipboard { get; set; }
        public int ThresholdValue { get; set; }
        public double BackgroundBlurRadious { get; set; }
        public double EditorBackgroundOpacity { get; set; }
        public double ShapeShadowBlurRadius { get; set; }
        public double ShapeShadowDepth { get; set; }
        public double ShapeShadowDirection { get; set; }
        public double ShapeThickness { get; set; }
        public double ShapeShadowBlurRadiusForText { get; set; }
        public double ShapeShadowDepthForText { get; set; }
        public double ShapeFontSize { get; set; }

    }
}
