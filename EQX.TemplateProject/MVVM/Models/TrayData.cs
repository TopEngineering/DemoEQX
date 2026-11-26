using EQX.Core.Units;
using EQX.TemplateProject.Defines.Devices.Tray;
using EQX.TemplateProject.Recipe;

namespace EQX.TemplateProject.MVVM.Models
{
    public sealed class TrayData
    {
        public ITray<ETrayStatus> InputTray { get; } = new Tray<ETrayStatus>("Input Tray");
        public ITray<ETrayStatus> OutputTray { get; } = new Tray<ETrayStatus>("Output Tray");

        public TrayData()
        {
            InputTray.GenerateCellsByRotation();
            OutputTray.GenerateCellsByRotation();
        }

        public void UpdateLayout(TrayRecipe recipe)
        {
            ValidateSize(recipe.InputTrayRow, recipe.InputTrayColumn);
            ValidateSize(recipe.OutputTrayRow, recipe.OutputTrayColumn);
            UpdateLayout(InputTray, recipe.InputTrayRow, recipe.InputTrayColumn);
            UpdateLayout(OutputTray, recipe.OutputTrayRow, recipe.OutputTrayColumn);
        }

        private static void ValidateSize(int rows, int columns)
        {
            if (rows < 0 || columns < 0 || (long)rows * columns > 2500)
                throw new ArgumentOutOfRangeException(nameof(rows), "Tray Over Size");
        }

        private static void UpdateLayout(ITray<ETrayStatus> tray, int rows, int columns)
        {
            if (tray.Rows == rows && tray.Columns == columns) return;
            tray.Rows = rows;
            tray.Columns = columns;
            tray.GenerateCellsByRotation();
            tray.SetAllCell(ETrayStatus.Ready);
        }

        public void ResetInputTray() => InputTray.SetAllCell(ETrayStatus.Ready);
        public void ResetOutputTray() => OutputTray.SetAllCell(ETrayStatus.Ready);
    }
}
