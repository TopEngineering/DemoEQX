using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CommunityToolkit.Mvvm.Input;
using EQX.Core.Common;
using EQX.Core.InOut;
using EQX.Core.Motion;
using EQX.Core.Recipe;
using EQX.InOut;
using EQX.UI.Controls;
using Microsoft.Extensions.DependencyInjection;
using EQX.TemplateProject.Defines;
using EQX.TemplateProject.Services.Interlock;

namespace EQX.TemplateProject.MVVM.ViewModels.Manual
{
    public class ManualUnitViewModel : ViewModelBase
    {
        #region privates
        private IMotion selectedAxis;
        private double speed;
        private double position;
        #endregion
        #region properties
        public ObservableCollection<ICylinder> Cylinders { get; set; }
        public ObservableCollection<IMotion> Motions { get; set; }
        public ObservableCollection<IDInput> Inputs { get; set; }
        public ObservableCollection<IDOutput> Outputs { get; set; }
        public IRecipe Recipe { get; set; }

        public ObservableCollection<ESemiSequence> SemiAutoSequences { get; set; }
        public string Name { get; init; }
        public ImageSource Image { get; set; }

        public ManualUnitViewModel(string name)
        {
            Name = name;
        }

        private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            if (Inputs == null) return;

            foreach (var input in Inputs)
            {
                input.RaiseValueUpdated();
            }
        }
        public IMotion SelectedAxis
        {
            get => selectedAxis;
            set
            {
                if (selectedAxis == value) return;
                selectedAxis = value;
                OnPropertyChanged();
            }
        }
        public double Speed
        {
            get => speed;
            set 
            {
                if (speed == value) return;
                speed = value;
                OnPropertyChanged();
            }
        }
        public double Position
        {
            get => position;
            set
            {
                if (position == value) return;
                position = value;
                OnPropertyChanged();
            }
        }
            
        #endregion
        #region Commands
        public ICommand CylinderForwardCommand
        {
            get
            {
                return new RelayCommand<object>((o) =>
                {
                    if (o is ICylinder cylinder == false) return;

                    if (cylinder.CylinderType == ECylinderType.ForwardBackwardReverse ||
                        cylinder.CylinderType == ECylinderType.UpDownReverse ||
                        cylinder.CylinderType == ECylinderType.RightLeftReverse ||
                        cylinder.CylinderType == ECylinderType.GripUngripReverse ||
                        cylinder.CylinderType == ECylinderType.AlignUnalignReverse ||
                        cylinder.CylinderType == ECylinderType.LockUnlockReverse ||
                        cylinder.CylinderType == ECylinderType.FlipUnflipReverse ||
                        cylinder.CylinderType == ECylinderType.ClampUnclampReverse
                        )
                    {
                        cylinder.Backward();
                        return;
                    }
                    cylinder.Forward();
                });
            }
        }

        public ICommand CylinderBackwardCommand
        {
            get
            {
                return new RelayCommand<object>((o) =>
                {
                    if (o is ICylinder cylinder == false) return;

                    if (cylinder.CylinderType == ECylinderType.ForwardBackwardReverse ||
                        cylinder.CylinderType == ECylinderType.UpDownReverse ||
                        cylinder.CylinderType == ECylinderType.RightLeftReverse ||
                        cylinder.CylinderType == ECylinderType.GripUngripReverse ||
                        cylinder.CylinderType == ECylinderType.AlignUnalignReverse ||
                        cylinder.CylinderType == ECylinderType.LockUnlockReverse ||
                        cylinder.CylinderType == ECylinderType.FlipUnflipReverse ||
                        cylinder.CylinderType == ECylinderType.ClampUnclampReverse)
                    {
                        cylinder.Forward();
                        return;
                    }
                    cylinder.Backward();
                });
            }
        }

        public ICommand MovePositionTeachingCommand
        {
            get
            {
                return new RelayCommand<object>((o) =>
                {
                    if (o is SinglePositionTeaching spt == false) return;
                    string MoveTo = (string)Application.Current.Resources["str_MoveTo"];

                    string motionName = spt.SinglePosition.Motion;
                    string moveToDescription = spt.SingleRecipeDescription.Description;

                    if (MessageBoxEx.ShowDialog($"{MoveTo} {moveToDescription} ?") == true)
                    {
                        InterlockService interlockService = App.AppHost!.Services.GetRequiredService<InterlockService>();

                        // Check Interlocks beforee move
                        if (interlockService.InterlockCheck(Name, moveToDescription) == false)
                        {
                            return;
                        }
                        Motions.FirstOrDefault(m => m.Name.Contains(motionName))!.MoveAbs(spt.Value);
                    }
                });
            }
        }
        public RelayCommand<object> MotionButtonCommand
        {
            get
            {
                return new RelayCommand<object>((sender) =>
                {
                    if (selectedAxis == null) return;
                    string tag = "";
                    if (sender is Button)
                    {
                        tag = (sender as Button)?.Tag?.ToString() ?? "";
                    }
                    switch(tag)
                    {
                        case "Jog+Button":
                            selectedAxis.MoveJog(Speed, true);
                            break;
                        case "Jog-Button":
                            selectedAxis.MoveJog(Speed, false);
                            break;
                        case "Inc-Button":
                            selectedAxis.MoveInc(Position*-1, Speed);
                            break;
                        case "Inc+Button":
                            selectedAxis.MoveInc(Position, Speed);
                            break;
                        case "JogAbsButton":
                            selectedAxis.MoveAbs(Position, Speed);
                            break;
                        case "ServoOnButton":
                            selectedAxis.MotionOn();
                            break;
                        case "ServoOffButton":
                            selectedAxis.MotionOff();
                            break;
                        case "AlarmReset":
                            selectedAxis.AlarmReset();
                            break;
                        case "HomeButton":
                            selectedAxis.SearchOrigin();
                            break;
                        case "StopButton":
                            selectedAxis.Stop();
                            break;
                        default:
                            break;
                    }
                });
            }
        }
        #endregion
    }
}
