using EQX.Core.Recipe;
using EQX.Core.Units;
using EQX.UI.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQX.TemplateProject.Recipe
{
    public class CommonRecipe : RecipeBase
    {
        private double cylinderMoveTimeout;
        private double motionOriginTimeout;
        private double motionMoveTimeout;
        private double vacDelay;
        private double purgeDelay;
        private double pickPlaceTimeOut;
        private int logSaveDay = 30;
        public event Action<ILanguageDefinition> SelectedLanguageEvent;
        public event Action SelectedLanguageLoadAllRecipe;
        [SingleRecipeDescription(Description = "Cylinder Move Timeout", Unit = Unit.Second)]
        public double CylinderMoveTimeout
        {
            get { return cylinderMoveTimeout; }
            set
            {
                if (cylinderMoveTimeout == value) return;

                OnRecipeChanged(cylinderMoveTimeout, value);
                cylinderMoveTimeout = value;
            }
        }

        [SingleRecipeDescription(Description = "Motion Origin Timeout", Unit = Unit.Second)]
        public double MotionOriginTimeout
        {
            get { return motionOriginTimeout; }
            set
            {
                if (motionOriginTimeout == value) return;

                OnRecipeChanged(motionOriginTimeout, value);
                motionOriginTimeout = value;
            }
        }

        [SingleRecipeDescription(Description = "Motion Move Timeout", Unit = Unit.Second)]
        public double MotionMoveTimeOut
        {
            get { return motionMoveTimeout; }
            set
            {
                if (motionMoveTimeout == value) return;

                OnRecipeChanged(motionMoveTimeout, value);
                motionMoveTimeout = value;
            }
        }

        [SingleRecipeDescription(Description = "Vacuum Delay", Unit = Unit.Second)]
        public double VacDelay
        {
            get { return vacDelay; }
            set
            {
                if (vacDelay == value) return;

                OnRecipeChanged(vacDelay, value);
                vacDelay = value;
            }
        }
        [SingleRecipeDescription(Description = "Purge Delay", Unit = Unit.Second)]
        private double PurgeDelay
        {
            get => purgeDelay;
            set
            {
                OnRecipeChanged(purgeDelay, value);
                purgeDelay = value;
            }
        }
        [SingleRecipeDescription(Description = "Pick Place Timeout", Unit = Unit.Second)]
        private double PickPlaceTimeOut
        {
            get => pickPlaceTimeOut;
            set
            {
                OnRecipeChanged(pickPlaceTimeOut, value);
                pickPlaceTimeOut = value;
            }
        }
        private double materialInputTimeout = 1800.0;

        [SingleRecipeDescription(Description = "Material Input Timeout",
            Detail = "Time Machine Not Have Material To Stop", Unit = Unit.Second)]
        public double MaterialInputTimeout
        {
            get { return materialInputTimeout; }
            set
            {
                OnRecipeChanged(materialInputTimeout, value);
                materialInputTimeout = value;
            }
        }


        [SingleRecipeDescription(Description = "Log Save Day")]
        [SingleRecipeMinMax(Max = 100, Min = 5)]
        public int LogSaveDay
        {
            get { return logSaveDay; }
            set 
            {
                OnRecipeChanged(logSaveDay, value);
                logSaveDay = value; 
            }
        }

        [SingleRecipeDescription(
            Description = "Skip Cylinder 1",
            Detail = "Check to Skip Cylinder 1")]
        public bool SkipCylinder1
        {
            get { return skipCylinder1; }
            set
            {
                if (skipCylinder1 == value) return;

                OnRecipeChanged(skipCylinder1, value);
                skipCylinder1 = value;
            }
        }

        [SingleRecipeDescription(
            Description = "Skip Cylinder 2",
            Detail = "Check to Skip Cylinder 2")]
        public bool SkipCylinder2
        {
            get { return skipCylinder2; }
            set
            {
                if (skipCylinder2 == value) return;

                OnRecipeChanged(skipCylinder2, value);
                skipCylinder2 = value;
            }
        }
        [SingleRecipeDescription(Description = "Skip Vacuum Check")]
        public bool SkipVacuumCheck
        {
            get => skipVacuumCheck;
            set
            {
                if (skipVacuumCheck == value) return;
                OnRecipeChanged(skipVacuumCheck, value);
                skipVacuumCheck = value;
            }
        }
        public ILanguageDefinition SelectedLanguage
        {
            get => selectLanguage;
            set
            {
                if (selectLanguage == value) return;
                OnRecipeChanged(selectLanguage, value);
                selectLanguage = value;
                OnPropertyChanged(nameof(SelectedLanguage));
                SelectedLanguageEvent?.Invoke(SelectedLanguage);
                SelectedLanguageLoadAllRecipe?.Invoke();
            }
        }
        #region Privates
        private bool skipCylinder1;
        private bool skipCylinder2;
        private bool skipVacuumCheck;
        private ILanguageDefinition selectLanguage;
        #endregion
    }
}
