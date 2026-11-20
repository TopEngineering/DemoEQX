using EQX.Core.Recipe;
using EQX.Core.Units;

namespace EQX.TemplateProject.Recipe
{
    public class TransferRecipe : RecipeBase
    {
        private double xAxisReadyPosition;
        private double yAxisReadyPosition;
        private double zAxisReadyPosition;
        private double xAxisPickPosition;
        private double yAxisPickPosition;
        private double zAxisPickPosition;
        private double xAxisPlacePosition;
        private double yAxisPlacePosition;
        private double zAxisPlacePosition;
        private double xAxisInspectPosition1;
        private double yAxisInspectPosition1;
        private double zAxisInspectPosition1;
        private double xAxisInspectPosition2;
        private double yAxisInspectPosition2;
        private double zAxisInspectPosition2;
        private double picker2OffsetPick;
        private double picker2OffsetPlace;


        [SingleRecipeDescription(Description = "X Axis Ready Position", Unit = Unit.mm)]
        [SinglePositionTeaching(Motion = "XAxis")]
        public double XAxisReadyPosition
        {
            get => xAxisReadyPosition;
            set
            {
                OnRecipeChanged(xAxisReadyPosition, value);
                xAxisReadyPosition = value;
            }
        }
        [SingleRecipeDescription(Description = "Y Axis Ready Position", Unit = Unit.mm)]
        [SinglePositionTeaching(Motion = "YAxis")]
        public double YAxisReadyPosition
        {
            get => yAxisReadyPosition;
            set
            {
                OnRecipeChanged(yAxisReadyPosition, value);
                yAxisReadyPosition = value;
            }
        }
        [SingleRecipeDescription(Description = "Z Axis Ready Position", Unit = Unit.mm)]
        [SinglePositionTeaching(Motion = "ZAxis")]
        public double ZAxisReadyPosition
        {
            get => zAxisReadyPosition;
            set
            {
                OnRecipeChanged(zAxisReadyPosition, value);
                zAxisReadyPosition = value;
            }
        }
        [SingleRecipeDescription(Description = "X Axis Pick Position", Unit = Unit.mm)]
        [SinglePositionTeaching(Motion = "XAxis")]
        public double XAxisPickPosition
        {
            get => xAxisPickPosition;
            set
            {
                OnRecipeChanged(xAxisPickPosition, value);
                xAxisPickPosition = value;
            }
        }
        [SingleRecipeDescription(Description = "Y Axis Pick Position", Unit = Unit.mm)]
        [SinglePositionTeaching(Motion = "YAxis")]
        public double YAxisPickPosition
        {
            get => yAxisPickPosition;
            set
            {
                OnRecipeChanged(yAxisPickPosition, value);
                yAxisPickPosition = value;
            }
        }
        [SingleRecipeDescription(Description = "Z Axis Pick Position", Unit = Unit.mm)]
        [SinglePositionTeaching(Motion = "ZAxis")]
        public double ZAxisPickPosition
        {
            get => zAxisPickPosition;
            set
            {
                OnRecipeChanged(zAxisPickPosition, value);
                zAxisPickPosition = value;
            }
        }
        [SingleRecipeDescription(Description = "X Axis Place Position", Unit = Unit.mm)]
        [SinglePositionTeaching(Motion = "XAxis")]
        public double XAxisPlacePosition
        {
            get => xAxisPlacePosition;
            set 
            {
                OnRecipeChanged(xAxisPlacePosition, value);
                xAxisPlacePosition = value;
            }
        }
        [SingleRecipeDescription(Description = "Y Axis Place Position", Unit = Unit.mm)]
        [SinglePositionTeaching(Motion = "YAxis")]
        public double YAxisPlacePosition
        {
            get => yAxisPlacePosition;
            set
            {
                OnRecipeChanged(yAxisPlacePosition, value);
                yAxisPlacePosition = value;
            }
        }
        [SingleRecipeDescription(Description = "Z Axis Place Position", Unit = Unit.mm)]
        [SinglePositionTeaching(Motion = "ZAxis")]
        public double ZAxisPlacePosition
        {
            get => zAxisPlacePosition;
            set
            {
                OnRecipeChanged(zAxisPlacePosition, value);
                zAxisPlacePosition = value;
            }
        }
        [SingleRecipeDescription(Description = "X Axis Inspect Position 1", Unit = Unit.mm)]
        [SinglePositionTeaching(Motion = "XAxis")]
        public double XAxisInspectPosition1
        {
            get => xAxisInspectPosition1;
            set
            {
                OnRecipeChanged(xAxisInspectPosition1, value);
                xAxisInspectPosition1 = value;
            }
        }
        [SingleRecipeDescription(Description = "Y Axis Inspect Position 1", Unit = Unit.mm)]
        [SinglePositionTeaching(Motion = "YAxis")]
        public double YAxisInspectionPosition1
        {
            get => yAxisInspectPosition1;
            set
            {
                OnRecipeChanged(yAxisInspectPosition1, value);
                yAxisInspectPosition1 = value;
            }
        }
        [SingleRecipeDescription(Description = "Z Axis Inspect Position 1", Unit = Unit.mm)]
        [SinglePositionTeaching(Motion = "ZAxis")]
        public double ZAxisInspectionPosition1
        {
            get => zAxisInspectPosition1;
            set
            {
                OnRecipeChanged(zAxisInspectPosition1, value);
                zAxisInspectPosition1 = value;
            }
        }
        [SingleRecipeDescription(Description = "X Axis Inspect Position 2", Unit = Unit.mm)]
        [SinglePositionTeaching(Motion = "XAxis")]
        public double XAxisInspectPosition2
        {
            get => xAxisInspectPosition2;
            set
            {
                OnRecipeChanged(xAxisInspectPosition2, value);
                xAxisInspectPosition2 = value;
            }
        }
        [SingleRecipeDescription(Description = "Y Axis Inspect Position 2", Unit = Unit.mm)]
        [SinglePositionTeaching(Motion = "YAxis")]
        public double YAxisInspectionPosition2
        {
            get => yAxisInspectPosition2;
            set
            {
                OnRecipeChanged(yAxisInspectPosition2, value);
                yAxisInspectPosition2 = value;
            }
        }
        [SingleRecipeDescription(Description = "Z Axis Inspect Position 2", Unit = Unit.mm)]
        [SinglePositionTeaching(Motion = "ZAxis")]
        public double ZAxisInspectionPosition2
        {
            get => zAxisInspectPosition2;
            set
            {
                OnRecipeChanged(zAxisInspectPosition2, value);
                zAxisInspectPosition2 = value;
            }
        }
        [SingleRecipeDescription(Description = "Picker 2 Offset Pick", Unit = Unit.mm)]
        public double Picker2OffsetPick
        {
            get => picker2OffsetPick;
            set
            {
                OnRecipeChanged(picker2OffsetPick, value);
                picker2OffsetPick = value;
            }
        }
        [SingleRecipeDescription(Description = "Picker 2 Offset Place", Unit = Unit.mm)]
        public double Picker2OffsetPlace
        {
            get => picker2OffsetPlace;
            set
            {
                OnRecipeChanged(picker2OffsetPlace, value);
                picker2OffsetPlace = value;
            }
        }
    }
}

