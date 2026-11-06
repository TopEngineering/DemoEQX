namespace EQX.TemplateProject.Recipe
{
    public class RecipeList
    {
        public RecipeList(CommonRecipe commonRecipe, TransferRecipe? transferRecipe = null)
        {
            CommonRecipe = commonRecipe;
            CommonRecipe.Name = "Common";
            TransferRecipe = transferRecipe ?? new TransferRecipe();
            TransferRecipe.Name = "Transfer";
        }

        public CommonRecipe CommonRecipe { get; }
        public TransferRecipe TransferRecipe { get; }
    }
}
