using CommunityToolkit.Mvvm.ComponentModel;

namespace EQX.TemplateProject.MVVM.Models
{
    public enum TrayCellState { Pending, Working, OK, Failed }

    public sealed class TrayCell : ObservableObject
    {
        public TrayCell(int number) => Number = number;
        public int Number { get; }
        private TrayCellState _state;
        public TrayCellState State
        {
            get => _state;
            set => SetProperty(ref _state, value);
        }
    }
}
