using CommunityToolkit.Mvvm.Input;
using EQX.Core.Common;
using EQX.Core.Sequence;
using EQX.UI.Controls;
using EQX.UI.Language;
using log4net;
using EQX.TemplateProject.Defines.Devices;
using EQX.TemplateProject.Process;
using EQX.TemplateProject.Recipe;
using EQX.TemplateProject.MVVM.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace EQX.TemplateProject.MVVM.ViewModels
{
    public class AutoViewModel : ViewModelBase
    {
        #region Constructor
        public AutoViewModel(MachineStatus machineStatus,
            INavigationService navigationService,
            Devices devices,
            RecipeSelector recipeSelector,
            UserStore userStore,
            NavigationStore navigationStore,
            ILanguageService languageService)
        {
            MachineStatus = machineStatus;
            _navigationService = navigationService;
            Devices = devices;
            RecipeSelector = recipeSelector;
            UpdateTrayLayout();
            recipeSelector.RecipeLoaded += UpdateTrayLayout;
            UserStore = userStore;
            _navigationStore = navigationStore;
            _languageService = languageService;
            MachineStatus.PropertyChanged += MachineStatusOnPropertyChanged;

            Log = LogManager.GetLogger("AutoVM");

            statusUpdateTimer = new System.Timers.Timer(100);
            statusUpdateTimer.Elapsed += StatusUpdateTimerHandler;
            statusUpdateTimer.Start();
        }
        #endregion

        public void EnableTimer()
        {
            statusUpdateTimer.Enabled = true;
        }

        public void DisableTimer()
        {
            statusUpdateTimer.Enabled = false;
        }
        private void StatusUpdateTimerHandler(object? sender, System.Timers.ElapsedEventArgs e)
        {
            if (_navigationStore.CurrentViewModel != this) return;

            Devices.Inputs.DoorSensor.RaiseValueUpdated();
            Devices.Inputs.Emergency.RaiseValueUpdated();
        }

        #region Properties
        public ObservableCollection<ILanguageDefinition> Cultures
        {
            get
            {
                ObservableCollection<ILanguageDefinition> availableLanguages = new ObservableCollection<ILanguageDefinition>();
                foreach (var item in _languageService.AvailableLanguages)
                {
                    availableLanguages.Add(item);
                }
                return availableLanguages;
            }
        }

        public MachineStatus MachineStatus { get; }
        public Devices Devices { get; }
        public RecipeSelector RecipeSelector { get; }
        public UserStore UserStore { get; }

        public string MachineRunModeDisplay => MachineStatus.MachineRunModeDisplay;
        #endregion

        public ObservableCollection<TrayCell> InputCells { get; } = new();
        public ObservableCollection<TrayCell> OutputCells { get; } = new();
        public int InputColumns => _layoutRecipe?.InputColumns > 0 ? _layoutRecipe.InputColumns : 1;
        public int OutputColumns => _layoutRecipe?.OutputColumns > 0 ? _layoutRecipe.OutputColumns : 1;
        private TransferRecipe? _layoutRecipe;

        private void UpdateTrayLayout()
        {
            if (Application.Current?.Dispatcher is { } dispatcher && !dispatcher.CheckAccess())
            {
                dispatcher.Invoke(UpdateTrayLayout);
                return;
            }
            if (_layoutRecipe != null) _layoutRecipe.PropertyChanged -= TrayRecipeChanged;
            _layoutRecipe = RecipeSelector.CurrentRecipe.TransferRecipe;
            _layoutRecipe.PropertyChanged += TrayRecipeChanged;
            InputCells.Clear();
            OutputCells.Clear();
            long inputCount = (long)_layoutRecipe.InputRows * _layoutRecipe.InputColumns;
            long outputCount = (long)_layoutRecipe.OutputRows * _layoutRecipe.OutputColumns;
            if (_layoutRecipe.InputRows > 0 && _layoutRecipe.InputColumns > 0 && inputCount <= 2500)
                for (int i = 1; i <= inputCount; i++) InputCells.Add(new TrayCell(i));
            if (_layoutRecipe.OutputRows > 0 && _layoutRecipe.OutputColumns > 0 && outputCount <= 2500)
                for (int i = 1; i <= outputCount; i++) OutputCells.Add(new TrayCell(i));
            OnPropertyChanged(nameof(InputColumns));
            OnPropertyChanged(nameof(OutputColumns));
        }

        private void TrayRecipeChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(TransferRecipe.InputRows) or nameof(TransferRecipe.InputColumns)
                or nameof(TransferRecipe.OutputRows) or nameof(TransferRecipe.OutputColumns) or null or "")
                UpdateTrayLayout();
        }

        private void MachineStatusOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MachineStatus.MachineRunMode))
            {
                OnPropertyChanged(nameof(MachineRunModeDisplay));
            }
        }

        #region Commands
        public ICommand SelectRunModeCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    var selected = RunModeDialog.ShowRunModeDialog(Enum.GetValues<EQX.TemplateProject.Process.EMachineRunMode>());
                    if (selected.HasValue)
                    {
                        MachineStatus.MachineRunMode = selected.Value;
                    }
                });
            }
        }

        public ICommand IOMonitoringNavigate
        {
            get
            {
                return new RelayCommand(() =>
                {
                    _navigationService.NavigateTo<IOMonitoringViewModel>();
                });
            }
        }

        public ICommand InitializeNavigate
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (MessageBoxEx.ShowDialog((string)Application.Current.Resources["str_AreYouSureYouWantToSetMachineReady"], (string)Application.Current.Resources["str_Confirm"]) == false)
                    {
                        return;
                    }
                    Log.Debug("Ready Button Click");
                    MachineStatus.OPCommand = EOperationCommand.Ready;
                });
            }
        }

        public ICommand InputStopCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (MachineStatus.IsInputStop == false)
                    {
                        MachineStatus.IsInputStop = true;
                        Log.Debug("ENABLE STOP INTPUT!");
                    }
                    else
                    {
                        MachineStatus.IsInputStop = false;
                        Log.Debug("DISABLE STOP INTPUT!");
                    }
                });
            }
        }

        public ICommand OutputStopCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (MachineStatus.IsOutputStop == false)
                    {
                        MachineStatus.IsOutputStop = true;
                        Log.Debug("ENABLE STOP OUTPUT!");
                    }
                    else
                    {
                        MachineStatus.IsOutputStop = false;
                        Log.Debug("DISABLE STOP OUTPUT!");
                    }
                });
            }
        }

        public ICommand OriginCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    _navigationService.NavigateTo<OriginViewModel>();
                });
            }
        }

        public ICommand StartCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    var messageKey = "str_AreYouSureYouWantToStartMachine";

                    if (MachineStatus.IsDryRunMode)
                    {
                        messageKey = "str_AreYouSureYouWantToStartMachineDryRun";
                    }
                    //else if (MachineStatus.IsByPassMode)
                    //{
                    //    messageKey = "str_AreYouSureYouWantToStartMachineByPass";
                    //}

                    if (MessageBoxEx.ShowDialog((string)Application.Current.Resources[messageKey], (string)Application.Current.Resources["str_Confirm"]) == false)
                    {
                        return;
                    }
                    Log.Debug("Start Button Click");
                    MachineStatus.OPCommand = EOperationCommand.Start;
                });
            }
        }

        public ICommand StopCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Log.Debug("Stop Button Click");
                    MachineStatus.OPCommand = EOperationCommand.Stop;
                });
            }
        }


        public ICommand DoorOpenCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    bool currentValue = Devices.Outputs.DoorOpen.Value;
                    Devices.Outputs.DoorOpen.Value = !currentValue;
                });
            }
        }

        public ICommand BuzzerOffCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Devices.Outputs.TowerBuzzer.Value = false;
                });
            }
        }
        #endregion

        #region Privates
        private readonly INavigationService _navigationService;
        private readonly NavigationStore _navigationStore;
        private readonly ILanguageService _languageService;
        System.Timers.Timer statusUpdateTimer;
        #endregion
    }
}
