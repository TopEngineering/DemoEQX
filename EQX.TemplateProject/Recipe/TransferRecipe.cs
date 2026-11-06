using EQX.Core.Recipe;
using EQX.Core.Units;

namespace EQX.TemplateProject.Recipe
{
    public class TransferRecipe : RecipeBase
    {
        private bool _Head2SetupConfirmed = false;
        [SingleRecipeDescription(Description = "Head2SetupConfirmed")]
        public bool Head2SetupConfirmed
        {
            get => _Head2SetupConfirmed;
            set
            {
                if (_Head2SetupConfirmed == value) return;
                OnRecipeChanged(_Head2SetupConfirmed, value);
                _Head2SetupConfirmed = value;
                OnPropertyChanged();
            }
        }
        private double _Pick2X = 0;
        [SingleRecipeDescription(Description = "Pick2X", Unit = Unit.mm)]
        public double Pick2X
        {
            get => _Pick2X;
            set
            {
                if (_Pick2X == value) return;
                OnRecipeChanged(_Pick2X, value);
                _Pick2X = value;
                OnPropertyChanged();
            }
        }
        private double _Pick2Y = 0;
        [SingleRecipeDescription(Description = "Pick2Y", Unit = Unit.mm)]
        public double Pick2Y
        {
            get => _Pick2Y;
            set
            {
                if (_Pick2Y == value) return;
                OnRecipeChanged(_Pick2Y, value);
                _Pick2Y = value;
                OnPropertyChanged();
            }
        }
        private double _Pick2Z = 0;
        [SingleRecipeDescription(Description = "Pick2Z", Unit = Unit.mm)]
        public double Pick2Z
        {
            get => _Pick2Z;
            set
            {
                if (_Pick2Z == value) return;
                OnRecipeChanged(_Pick2Z, value);
                _Pick2Z = value;
                OnPropertyChanged();
            }
        }
        private double _Place2X = 0;
        [SingleRecipeDescription(Description = "Place2X", Unit = Unit.mm)]
        public double Place2X
        {
            get => _Place2X;
            set
            {
                if (_Place2X == value) return;
                OnRecipeChanged(_Place2X, value);
                _Place2X = value;
                OnPropertyChanged();
            }
        }
        private double _Place2Y = 0;
        [SingleRecipeDescription(Description = "Place2Y", Unit = Unit.mm)]
        public double Place2Y
        {
            get => _Place2Y;
            set
            {
                if (_Place2Y == value) return;
                OnRecipeChanged(_Place2Y, value);
                _Place2Y = value;
                OnPropertyChanged();
            }
        }
        private double _Place2Z = 0;
        [SingleRecipeDescription(Description = "Place2Z", Unit = Unit.mm)]
        public double Place2Z
        {
            get => _Place2Z;
            set
            {
                if (_Place2Z == value) return;
                OnRecipeChanged(_Place2Z, value);
                _Place2Z = value;
                OnPropertyChanged();
            }
        }
        private bool _TransferSetupConfirmed = false;
        [SingleRecipeDescription(Description = "TransferSetupConfirmed")]
        public bool TransferSetupConfirmed
        {
            get => _TransferSetupConfirmed;
            set
            {
                if (_TransferSetupConfirmed == value) return;
                OnRecipeChanged(_TransferSetupConfirmed, value);
                _TransferSetupConfirmed = value;
                OnPropertyChanged();
            }
        }
        private double _PickX = 0;
        [SingleRecipeDescription(Description = "PickX", Unit = Unit.mm)]
        public double PickX
        {
            get => _PickX;
            set
            {
                if (_PickX == value) return;
                OnRecipeChanged(_PickX, value);
                _PickX = value;
                OnPropertyChanged();
            }
        }
        private double _PickY = 0;
        [SingleRecipeDescription(Description = "PickY", Unit = Unit.mm)]
        public double PickY
        {
            get => _PickY;
            set
            {
                if (_PickY == value) return;
                OnRecipeChanged(_PickY, value);
                _PickY = value;
                OnPropertyChanged();
            }
        }
        private double _PickZ = 0;
        [SingleRecipeDescription(Description = "PickZ", Unit = Unit.mm)]
        public double PickZ
        {
            get => _PickZ;
            set
            {
                if (_PickZ == value) return;
                OnRecipeChanged(_PickZ, value);
                _PickZ = value;
                OnPropertyChanged();
            }
        }
        private double _PlaceX = 0;
        [SingleRecipeDescription(Description = "PlaceX", Unit = Unit.mm)]
        public double PlaceX
        {
            get => _PlaceX;
            set
            {
                if (_PlaceX == value) return;
                OnRecipeChanged(_PlaceX, value);
                _PlaceX = value;
                OnPropertyChanged();
            }
        }
        private double _PlaceY = 0;
        [SingleRecipeDescription(Description = "PlaceY", Unit = Unit.mm)]
        public double PlaceY
        {
            get => _PlaceY;
            set
            {
                if (_PlaceY == value) return;
                OnRecipeChanged(_PlaceY, value);
                _PlaceY = value;
                OnPropertyChanged();
            }
        }
        private double _PlaceZ = 0;
        [SingleRecipeDescription(Description = "PlaceZ", Unit = Unit.mm)]
        public double PlaceZ
        {
            get => _PlaceZ;
            set
            {
                if (_PlaceZ == value) return;
                OnRecipeChanged(_PlaceZ, value);
                _PlaceZ = value;
                OnPropertyChanged();
            }
        }
        private double _TravelZ = 0;
        [SingleRecipeDescription(Description = "TravelZ", Unit = Unit.mm)]
        public double TravelZ
        {
            get => _TravelZ;
            set
            {
                if (_TravelZ == value) return;
                OnRecipeChanged(_TravelZ, value);
                _TravelZ = value;
                OnPropertyChanged();
            }
        }
        private double _TransferSpeed = 10;
        [SingleRecipeDescription(Description = "TransferSpeed", Unit = Unit.mmPerSecond)]
        public double TransferSpeed
        {
            get => _TransferSpeed;
            set
            {
                if (_TransferSpeed == value) return;
                OnRecipeChanged(_TransferSpeed, value);
                _TransferSpeed = value;
                OnPropertyChanged();
            }
        }
        private double _VacuumTimeout = 3;
        [SingleRecipeDescription(Description = "VacuumTimeout", Unit = Unit.Second)]
        public double VacuumTimeout
        {
            get => _VacuumTimeout;
            set
            {
                if (_VacuumTimeout == value) return;
                OnRecipeChanged(_VacuumTimeout, value);
                _VacuumTimeout = value;
                OnPropertyChanged();
            }
        }
        private double _BlowTime = 0.2;
        [SingleRecipeDescription(Description = "BlowTime", Unit = Unit.Second)]
        public double BlowTime
        {
            get => _BlowTime;
            set
            {
                if (_BlowTime == value) return;
                OnRecipeChanged(_BlowTime, value);
                _BlowTime = value;
                OnPropertyChanged();
            }
        }
    }
}

