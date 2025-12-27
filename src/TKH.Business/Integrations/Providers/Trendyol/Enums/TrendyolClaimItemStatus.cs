using System.Runtime.Serialization;

namespace TKH.Business.Integrations.Providers.Trendyol.Enums
{
    public enum TrendyolClaimItemStatus
    {
        [EnumMember(Value = "Created")]
        Created,

        [EnumMember(Value = "WaitingInAction")]
        WaitingInAction,

        [EnumMember(Value = "WaitingFraudCheck")]
        WaitingFraudCheck,

        [EnumMember(Value = "Accepted")]
        Accepted,

        [EnumMember(Value = "Rejected")]
        Rejected,

        [EnumMember(Value = "Cancelled")]
        Cancelled,

        [EnumMember(Value = "InAnalysis")]
        InAnalysis,

        [EnumMember(Value = "Unresolved")]
        Unresolved
    }
}
