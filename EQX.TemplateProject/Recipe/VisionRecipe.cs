using EQX.Core.Recipe;
using EQX.Core.Units;

namespace EQX.TemplateProject.Recipe
{
    public class VisionRecipe : RecipeBase
    {
        private double underCameraExposureTime = 5000;
        private int underVisionLightLevel = 100;
        private double underVisionPixelSize = 5;
        private int retryCount = 2;
        private uint lightComport = 3;
        [SingleRecipeDescription(Description = "Under Camera Exposure Time", Unit = Unit.MicroSecond)]
        [SingleRecipeMinMax(Min = 0, Max = 30000)]
        public double UnderCamera_ExposureTime
        {
            get => underCameraExposureTime;
            set
            {
                OnRecipeChanged(underCameraExposureTime, value);
                underCameraExposureTime = value;
            }
        }

        [SingleRecipeDescription(Description = "Under Vision Light Level", Unit = "level")]
        [SingleRecipeMinMax(Min = 0, Max = 255)]
        public int UnderVision_LightLevel
        {
            get => underVisionLightLevel;
            set
            {
                OnRecipeChanged(underVisionLightLevel, value);
                underVisionLightLevel = value;
            }
        }

        [SingleRecipeDescription(Description = "Under Vision Pixel Size", Unit = Unit.Micrometer)]
        [SingleRecipeMinMax(Min = 0, Max = 50)]
        public double UnderVision_PixelSize
        {
            get => underVisionPixelSize;
            set
            {
                OnRecipeChanged(underVisionPixelSize, value);
                underVisionPixelSize = value;
            }
        }

        [SingleRecipeDescription(Description = "Vision Retry Count", Unit = "times")]
        [SingleRecipeMinMax(Min = 0, Max = 10)]
        public int RetryCount
        {
            get => retryCount;
            set
            {
                OnRecipeChanged(retryCount, value);
                retryCount = value;
            }
        }

        [SingleRecipeDescription(Description = "Light Controller COM Port", Unit = "COM")]
        [SingleRecipeMinMax(Min = 0, Max = 20)]
        public uint LightComport
        {
            get => lightComport;
            set
            {
                OnRecipeChanged(lightComport, value);
                lightComport = value;
            }
        }

    }
}
