using System.ComponentModel;

namespace TKH.Entities.Enums
{
    public enum ClaimStatus
    {
        [Description("Oluşturuldu")]
        Created = 1,

        [Description("Aksiyon Bekleniyor")]
        WaitingInAction = 2,

        [Description("Fraud Kontrolü")]
        WaitingFraudCheck = 3,

        [Description("Kabul Edildi")]
        Accepted = 10,

        [Description("Reddedildi")]
        Rejected = 20,

        [Description("İptal Edildi")]
        Cancelled = 30,

        [Description("Analiz Aşamasında")]
        InAnalysis = 40,

        [Description("Çözümlenemedi")]
        Unresolved = 50, // İhtilaflı

        [Description("Bilinmiyor")]
        Other = 99
    }
}
