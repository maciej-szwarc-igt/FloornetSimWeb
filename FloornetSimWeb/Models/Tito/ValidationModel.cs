using System;

namespace IGT.FloorNet.Tools.ServiceSimulator.Models.Tito
{
    public class ValidationModel : ModelBase
    {
        private long? _seedValue1;
        private long? _seedValue2;
        private DateTime? _seedDateTime;
        private string _validationIds;

        public ValidationModel()
        {
            Clear();
        }

        public long? SeedValue1
        {
            get => _seedValue1;
            set
            {
                _seedValue1 = value;
                OnPropertyChanged("SeedValue1");
            }
        }

        public long? SeedValue2
        {
            get => _seedValue2;
            set
            {
                _seedValue2 = value;
                OnPropertyChanged("SeedValue2");
            }
        }

        public DateTime? SeedDateTime
        {
            get => _seedDateTime;
            set
            {
                _seedDateTime = value;
                OnPropertyChanged("SeedDateTime");
            }
        }

        public string ValidationIds
        {
            get => _validationIds;
            set
            {
                _validationIds = value;
                OnPropertyChanged("ValidationIds");
            }
        }

        public bool IsValid => SeedValue1 != null && SeedValue2 != null && SeedDateTime != null;

        public void Clear()
        {
            SeedValue1 = null;
            SeedValue2 = null;
            SeedDateTime = DateTime.UtcNow;
            ValidationIds = null;
        }
    }
}
