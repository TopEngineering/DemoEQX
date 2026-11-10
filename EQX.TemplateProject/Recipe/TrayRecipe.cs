using EQX.Core.Recipe;
using EQX.Core.Units;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQX.TemplateProject.Recipe
{
    public class TrayRecipe : RecipeBase
    {
        private double inputTrayXPitch;
        private double inputTrayYPitch;
        private double outputTrayXPitch;
        private double outputTrayYPitch;
        private int inputTrayColumns;
        private int inputTrayRows;
        private int outputTrayColumns;
        private int outputTrayRows;
        [SingleRecipeDescription(Description = "Input Tray X Pitch", Unit = Unit.mm, Detail = "Pitch")]
        public double InputTrayXPitch
        {
            get => inputTrayXPitch;
            set 
            {
                OnRecipeChanged(inputTrayXPitch, value);
                inputTrayXPitch = value;
            }
        }
        [SingleRecipeDescription(Description = "Input Tray Y Pitch", Unit = Unit.mm, Detail = "Pitch")]
        public double InputTrayYPitch
        {
            get => inputTrayYPitch;
            set
            {
                OnRecipeChanged(inputTrayYPitch, value);
                inputTrayYPitch = value;
            }
        }
        [SingleRecipeDescription(Description = "Output Tray X Pitch", Unit = Unit.mm, Detail = "Pitch")]
        public double OutputTrayXPitch
        {
            get => outputTrayXPitch;
            set
            {
                OnRecipeChanged(outputTrayXPitch, value);
                outputTrayXPitch = value;
            }
        }
        [SingleRecipeDescription(Description = "Output Tray Y Pitch", Unit = Unit.mm, Detail = "Pitch")]
        public double OutputTrayYPitch
        {
            get => outputTrayYPitch;
            set
            {
                OnRecipeChanged(outputTrayYPitch, value);
                outputTrayYPitch = value;
            }
        }
        [SingleRecipeDescription(Description = "Input Tray Columns", Unit = Unit.ETC, Detail = "Column")]
        public int InputTrayColumn
        {
            get => inputTrayColumns;
            set
            {
                OnRecipeChanged(inputTrayColumns, value);
                inputTrayColumns = value;
                OnPropertyChanged();
            }
        }
        [SingleRecipeDescription(Description = "Input Tray Rows", Unit = Unit.ETC, Detail = "Row")]
        public int InputTrayRow
        {
            get => inputTrayRows;
            set
            {
                OnRecipeChanged(inputTrayRows, value);
                inputTrayRows = value;
                OnPropertyChanged();
            }
        }
        [SingleRecipeDescription(Description = "Output Tray Columns", Unit = Unit.ETC, Detail = "Column")]
        public int OutputTrayColumn
        {
            get => outputTrayColumns;
            set
            {
                OnRecipeChanged(outputTrayColumns, value);
                outputTrayColumns = value;
                OnPropertyChanged();
            }
        }
        [SingleRecipeDescription(Description = "Output Tray Rows", Unit = Unit.ETC, Detail = "Row")]
        public int OutputTrayRow
        {
            get => outputTrayRows;
            set
            {
                OnRecipeChanged(outputTrayRows, value);
                outputTrayRows = value;
                OnPropertyChanged();
            }
        }
    }
}
