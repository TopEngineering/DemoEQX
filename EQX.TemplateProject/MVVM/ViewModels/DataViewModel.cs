using CommunityToolkit.Mvvm.Input;
using EQX.Core.Common;
using EQX.Core.Motion;
using EQX.Core.Recipe;
using EQX.Motion;
using EQX.UI.Controls;
using EQX.TemplateProject.Recipe;
using EQX.TemplateProject.Defines;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using Newtonsoft.Json;
using System.IO;
using Microsoft.Extensions.Configuration;
using log4net;
using EQX.TemplateProject.Process;
using EQX.UI.Language;

namespace EQX.TemplateProject.MVVM.ViewModels
{
    public partial class DataViewModel : ViewModelBase
    {
        private string selectedModel;
        private RecipeBase _selectedRecipe;
        private readonly Motions _motions;
        private readonly IConfiguration _configuration;
        public DataViewModel(RecipeSelector recipeSelector,
            Motions motions,
            IConfiguration configuration,
            MachineStatus machineStatus,
            ILanguageService languageService)
        {
            RecipeSelector = recipeSelector;
            _motions = motions;
            _configuration = configuration;
            MachineStatus = machineStatus;
            _languageService = languageService;
            Log = LogManager.GetLogger("Data");

            RecipeSelector.CurrentRecipe.CommonRecipe.RecipeChanged += RecipeChanged_Handler;
        }

        private readonly ILanguageService _languageService;
        public ObservableCollection<ILanguageDefinition> Cultures
        {
            get
            {
                return new ObservableCollection<ILanguageDefinition>(_languageService.AvailableLanguages);
            }
        }
        public ObservableCollection<IMotion> AllMotions
        {
            get
            {
                List<IMotion> motions = new List<IMotion>();
                motions.AddRange(_motions.All);

                return new ObservableCollection<IMotion>(motions);
            }
        }

        public RecipeSelector RecipeSelector { get; }
        public TransferRecipe TransferOptions => RecipeSelector.CurrentRecipe.TransferRecipe;

        public string SelectedModel
        {
            get { return selectedModel; }
            set
            {
                selectedModel = value;
                OnPropertyChanged();
            }
        }

        public event Action LoadRecipeEvent;

        public ObservableCollection<RecipeBase> Recipes
        {
            get
            {
                var recipeProps = RecipeSelector.CurrentRecipe.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => typeof(RecipeBase).IsAssignableFrom(p.PropertyType))
                .ToList();

                var recipeObjects = recipeProps
                .Select(p => p.GetValue(RecipeSelector.CurrentRecipe) as RecipeBase)
                .Where(r => r != null)
                .ToList();

                return new ObservableCollection<RecipeBase>(recipeObjects);
            }
        }

        public RecipeBase SelectedRecipe
        {
            get
            {
                return _selectedRecipe;
            }
            set
            {
                _selectedRecipe = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsEnableSelectLanguage));
            }
        }

        public bool IsEnableSelectLanguage => SelectedRecipe is CommonRecipe;
        public MachineStatus MachineStatus { get; }

        public RecipeBase CurrentRecipe => SelectedRecipe ?? RecipeSelector.CurrentRecipe.CommonRecipe;

        public ICommand SaveRecipeCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (MessageBoxEx.ShowDialog((string)Application.Current.Resources["str_SaveAllData"]) == true)
                    {
                        if (!MachineStatus.IsStandByProcessMode) return;
                        RecipeSelector.Save();
                        OnPropertyChanged(nameof(Recipes));
                    }
                });
            }
        }
        public ICommand RefreshRecipeCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    RecipeSelector.ValidRecipes = RecipeSelector.UpdateValidRecipes();
                    LoadRecipeEvent?.Invoke();
                });
            }
        }
        public ICommand CopyRecipeCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    string CopyRecipe = (string)Application.Current.Resources["str_CopyRecipe"];
                    if (MessageBoxEx.ShowDialog($"{CopyRecipe} {SelectedModel} ? ") == true)
                    {
                        RecipeSelector.Copy(SelectedModel);
                        RecipeSelector.ValidRecipes = RecipeSelector.UpdateValidRecipes();
                        LoadRecipeEvent?.Invoke();
                    }
                });
            }
        }

        public ICommand SaveMotionConfigCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    try
                    {
                        var result = MessageBoxEx.ShowDialog("Do you want to save the motion configurations?", true, "Confirm Save");

                        if (result == true)
                        {
                            SaveMotionConfigurations();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBoxEx.ShowDialog($"Error saving motion configurations: {ex.Message}");
                    }
                });
            }
        }

        private void SaveMotionConfigurations()
        {
            var ajinConfigPath = _configuration["Files:MotionAjinParaConfigFile"];
            if (!string.IsNullOrEmpty(ajinConfigPath))
            {
                var existingAjinParams = JsonConvert.DeserializeObject<List<MotionAjinParameter>>(
                    File.ReadAllText(ajinConfigPath)) ?? new List<MotionAjinParameter>();

                for (int i = 0; i < _motions.AjinMotions.All.Count && i < existingAjinParams.Count; i++)
                {
                    existingAjinParams[i].Velocity = _motions.AjinMotions.All[i].Parameter.Velocity;
                    existingAjinParams[i].Acceleration = _motions.AjinMotions.All[i].Parameter.Acceleration;
                    existingAjinParams[i].Deceleration = _motions.AjinMotions.All[i].Parameter.Deceleration;
                }

                var ajinJson = JsonConvert.SerializeObject(existingAjinParams, Formatting.Indented);
                File.WriteAllText(ajinConfigPath, ajinJson);
            }
        }

        private void RecipeChanged_Handler(object? oldValue, object? newValue, string? propertyName = null,
            int parameterIndex = -1, PropertyInfo? propertyInfo = null)
        {
            Log.Info($"{propertyName} value updated : {oldValue} -> {newValue}");
        }
    }
}
