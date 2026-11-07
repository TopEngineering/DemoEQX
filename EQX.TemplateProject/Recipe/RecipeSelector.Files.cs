using System.IO;

namespace EQX.TemplateProject.Recipe
{
    public partial class RecipeSelector
    {
        public string GetRecipeDirectory(string name)
        {
            if (string.IsNullOrWhiteSpace(name) || name != Path.GetFileName(name) ||
                name.StartsWith(".") || name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                throw new ArgumentException("Tên recipe không hợp lệ");
            var root = Path.GetFullPath(recipeFolder);
            var path = Path.GetFullPath(Path.Combine(root, name));
            if (!string.Equals(Path.GetDirectoryName(path), root.TrimEnd(Path.DirectorySeparatorChar),
                StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Recipe phải nằm trực tiếp trong thư mục recipe");
            if ((File.GetAttributes(path) & FileAttributes.ReparsePoint) != 0)
                throw new InvalidOperationException("Không thao tác recipe qua đường dẫn liên kết");
            return path;
        }

        public void DeleteRecipe(string name)
        {
            if (string.Equals(name, RecipeSetting.CurrentRecipe, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Không thể xóa recipe đang sử dụng. Hãy Change sang recipe khác");
            var source = GetRecipeDirectory(name);
            var archive = Path.Combine(Path.GetFullPath(recipeFolder), ".Deleted");
            Directory.CreateDirectory(archive);
            if ((File.GetAttributes(archive) & FileAttributes.ReparsePoint) != 0)
                throw new InvalidOperationException("Thư mục lưu recipe đã xóa không hợp lệ");
            var destination = Path.Combine(archive, name + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_" + Guid.NewGuid().ToString("N"));
            Directory.Move(source, destination);
            ValidRecipes = UpdateValidRecipes();
        }
    }
}
