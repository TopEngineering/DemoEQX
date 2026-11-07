using CommunityToolkit.Mvvm.ComponentModel;
using EQX.Core.Recipe;
using EQX.UI.Controls;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using EQX.UI.Language;

namespace EQX.TemplateProject.Recipe
{
    public partial class RecipeSelector : ObservableObject
    {
        #region Privates
        private RecipeSetting recipeSetting;
        private readonly IConfiguration _configuration;

        //[Minh]
        private readonly IAlertService _alarmService;
        private readonly IAlertService _warningService;
        private readonly ILanguageService _languageService;

        //--------------------------------------------
        private string recipeFolder => _configuration.GetValue<string>("Folders:RecipeFolder") ?? "";
        #endregion

        #region Properties
        public ObservableCollection<string> ValidRecipes
        {
            get
            {
                return UpdateValidRecipes();
            }
            set
            {
                OnPropertyChanged(nameof(ValidRecipes));
            }
        }

        public RecipeSetting RecipeSetting
        {
            get { return recipeSetting; }
            set
            {
                recipeSetting = value;
                OnPropertyChanged(nameof(RecipeSetting));
            }
        }

        public RecipeList CurrentRecipe { get; private set; }
        public event Action? RecipeLoaded;
        #endregion

        #region Constructor
        public RecipeSelector(IConfiguration configuration,
            RecipeList currentRecipe,
            [FromKeyedServices("AlarmService")] IAlertService alarmService,
            [FromKeyedServices("WarningService")] IAlertService warningService,
            ILanguageService languageService)
        {
            _configuration = configuration;
            CurrentRecipe = currentRecipe;
            recipeSetting = new RecipeSetting();
            _alarmService = alarmService;
            _warningService = warningService;
            _languageService = languageService;

            currentRecipe.CommonRecipe.SelectedLanguageEvent += CommonRecipe_SelectedLanguageEvent;
        }

        private void CommonRecipe_SelectedLanguageEvent(ILanguageDefinition obj)
        {
            _languageService.SwitchLanguage(obj.Id);
            _alarmService.Update();
            _warningService.Update();
        }
        #endregion

        #region Methods
        public bool Load()
        {
            try
            {
                var settingFile = Path.Combine(recipeFolder, "RecipeSetting.json");
                var setting = JsonConvert.DeserializeObject<RecipeSetting>(File.ReadAllText(settingFile))
                    ?? throw new FormatException("RecipeSetting.json is empty.");
                var source = ReadRecipe(setting.CurrentRecipe);
                ApplyRecipe(source);
                RecipeSetting = setting;
                RecipeLoaded?.Invoke();
                return true;
            }
            catch (Exception ex)
            {
                MessageBoxEx.ShowDialog($"Cannot load recipe: {ex.Message}");
                return false;
            }
        }

        private RecipeList ReadRecipe(string name)
        {
            var folder = GetRecipeDirectory(name);
            return JsonConvert.DeserializeObject<RecipeList>(
                File.ReadAllText(Path.Combine(folder, "Recipe.json")),
                new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto })
                ?? throw new FormatException("Recipe.json is empty.");
        }

        private void ApplyRecipe(RecipeList source)
        {
            foreach (var property in typeof(RecipeList).GetProperties()
                .Where(p => typeof(IRecipe).IsAssignableFrom(p.PropertyType)))
            {
                ((IRecipe)property.GetValue(CurrentRecipe)!).Clone((IRecipe)property.GetValue(source)!);
            }
        }

        public void Copy(string selectedRecipe)
        {
            string recipeFolder = _configuration.GetValue<string>("Folders:RecipeFolder");

            string currentRecipeFolder = Path.Combine(recipeFolder, selectedRecipe);
            if (!Directory.Exists(currentRecipeFolder))
            {
                MessageBox.Show($"Recipe folder {currentRecipeFolder} not found");
                return;
            }
            if (Directory.Exists($"{currentRecipeFolder}_Copy") == false)
            {
                Directory.CreateDirectory($"{currentRecipeFolder}_Copy");

                foreach (var file in Directory.GetFiles(currentRecipeFolder))
                {
                    string fileSource = Path.Combine(currentRecipeFolder, Path.GetFileName(file));
                    string fileBackup = Path.Combine($"{currentRecipeFolder}_Copy", Path.GetFileName(file));

                    File.Copy(fileSource, fileBackup, true);
                }
            }
            else
            {
                MessageBox.Show($" Recipe folder \"$\"{currentRecipeFolder}_copy\"\" exists");
            }
        }

        public void Save()
        {
            string recipeFolder = _configuration.GetValue<string>("Folders:RecipeFolder");

            string currentRecipeFolder = Path.Combine(recipeFolder, RecipeSetting.CurrentRecipe);
            string currentRecipeFile = Path.Combine(currentRecipeFolder, "Recipe.json");
            if (Directory.Exists(currentRecipeFolder) == false)
                MessageBox.Show($" Recipe folder \"{currentRecipeFolder}\" not found");

            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            };

            string serializeStr = JsonConvert.SerializeObject(CurrentRecipe, Formatting.Indented, settings);
            File.WriteAllText(currentRecipeFile, serializeStr);
        }

        public ObservableCollection<string> UpdateValidRecipes()
        {
            ObservableCollection<string> validRecipes = new ObservableCollection<string>();
            if (!Directory.Exists(recipeFolder)) return validRecipes;
            List<string> result = Directory.GetDirectories(recipeFolder, "*", SearchOption.TopDirectoryOnly)
                .Where(d => !Path.GetFileName(d).StartsWith(".") && File.Exists(Path.Combine(d, "Recipe.json")))
                .Select(d => new DirectoryInfo(d).Name)
                .ToList();
            foreach (var model in result)
            {
                validRecipes.Add(model);
            }

            return validRecipes;
        }

        public void SetCurrentModel(string selectedRecipe)
        {
            var source = ReadRecipe(selectedRecipe);
            var backup = JsonConvert.DeserializeObject<RecipeList>(JsonConvert.SerializeObject(CurrentRecipe))!;
            var previousName = RecipeSetting.CurrentRecipe;
            var settingPath = Path.Combine(recipeFolder, "RecipeSetting.json");
            var temporaryPath = settingPath + "." + Guid.NewGuid().ToString("N") + ".tmp";
            try
            {
                ApplyRecipe(source);
                RecipeSetting.CurrentRecipe = selectedRecipe;
                File.WriteAllText(temporaryPath, JsonConvert.SerializeObject(RecipeSetting, Formatting.Indented));
                File.Move(temporaryPath, settingPath, true);
            }
            catch
            {
                RecipeSetting.CurrentRecipe = previousName;
                ApplyRecipe(backup);
                throw;
            }
            finally
            {
                if (File.Exists(temporaryPath)) File.Delete(temporaryPath);
            }
            RecipeLoaded?.Invoke();
        }
        #endregion
    }
}
