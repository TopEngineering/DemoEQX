using CommunityToolkit.Mvvm.Input;
using EQX.UI.Controls;
using System.Windows.Input;

namespace EQX.TemplateProject.MVVM.ViewModels
{
    public partial class DataViewModel
    {
        public ICommand ReloadRecipeCommand => new RelayCommand(() =>
        {
            if (!MachineStatus.IsStandByProcessMode) return;
            try
            {
                if (MessageBoxEx.ShowDialog("Reload recipe từ file? Thay đổi chưa lưu sẽ bị bỏ.") != true) return;
                if (!RecipeSelector.Load()) throw new InvalidOperationException("Không đọc được recipe. Kiểm tra file Recipe.json.");
                OnPropertyChanged(nameof(Recipes));
                LoadRecipeEvent?.Invoke();
            }
            catch (Exception ex) { MessageBoxEx.ShowDialog(ex.Message); }
        });
        public ICommand ChangeRecipeCommand => new RelayCommand(() =>
        {
            if (!MachineStatus.IsStandByProcessMode) return;
            try
            {
                if (MessageBoxEx.ShowDialog($"Change recipe sang {SelectedModel}? Thay đổi chưa lưu sẽ bị bỏ.") != true) return;
                RecipeSelector.SetCurrentModel(SelectedModel);
                OnPropertyChanged(nameof(Recipes));
                LoadRecipeEvent?.Invoke();
            }
            catch (Exception ex) { MessageBoxEx.ShowDialog(ex.Message); }
        });

        public ICommand DeleteRecipeCommand => new RelayCommand(() =>
        {
            if (!MachineStatus.IsStandByProcessMode) return;
            try
            {
                if (MessageBoxEx.ShowDialog($"Delete recipe {SelectedModel}? Recipe sẽ chuyển vào thư mục .Deleted.") != true) return;
                SelectedModel = RecipeSelector.RecipeSetting.CurrentRecipe;
            }
            catch (Exception ex) { MessageBoxEx.ShowDialog(ex.Message); }
        });
    }
}
