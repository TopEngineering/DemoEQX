namespace EQX.TemplateProject.Recipe
{
    public class RecipeList
    {
        public RecipeList(CommonRecipe commonRecipe, TransferRecipe transferRecipe, TrayRecipe trayRecipe)
        {
            CommonRecipe = commonRecipe;
            CommonRecipe.Name = "Common";
            TransferRecipe = transferRecipe;
            TransferRecipe.Name = "Transfer";
            TrayRecipe = trayRecipe;
            TrayRecipe.Name = "Tray";
        }

        public CommonRecipe CommonRecipe { get; }
        public TransferRecipe TransferRecipe { get; }
        public TrayRecipe TrayRecipe { get; }
    }
}
