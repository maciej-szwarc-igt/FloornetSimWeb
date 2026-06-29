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
            // Provide valid, non-null defaults so getValidationIds returns a valid
            // response (IsValid == true) without manual UI configuration. This lets
            // a System-validation cashout proceed end-to-end out of the box.
            //
            // These seed values mirror the production CMS golden reference
            // (see c:\dev\config-exchange-reference.md: getValidationIdsResp
            // seedValue1=49198, seedValue2=1457631). Using the production seeds here
            // means a SecureEnhanced TITO cashout succeeds immediately after startup
            // without a manual PUT /api/tito/state.
            SeedValue1 = 49198L;
            SeedValue2 = 1457631L;
            SeedDateTime = DateTime.UtcNow;
            ValidationIds = null;
        }
    }
}
