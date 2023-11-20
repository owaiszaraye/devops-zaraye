using Zaraye.Core.Domain.Localization;
using System;

namespace Zaraye.Core.Domain.Common
{
    public partial class Jaiza : BaseEntity, ILocalizedEntity
    {
        public string Prediction { get; set; }
        public int predictionPictureId { get; set; }
        public bool predictionPublished { get; set; }
        public string Recommendation { get; set; }
        public int RecommendationPictureId { get; set; }
        public bool RecommendationPublished { get; set; }
        public decimal Rate { get; set; }
        public decimal DollarRate { get; set; }
        public bool RatePublished { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public int CreatedBy { get; set; }
        public bool Published { get; set; }
        public string Type { get; set; }
        public int UnitId { get; set; }
    }
}