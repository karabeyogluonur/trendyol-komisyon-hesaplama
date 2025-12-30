using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace TKH.Integrations.Trendyol.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TrendyolOtherFinancialTransactionType
    {
        CashAdvance,
        WireTransfer,
        IncomingTransfer,
        ReturnInvoice,
        CommissionAgreementInvoice,
        PaymentOrder,
        DeductionInvoices,
        FinancialItem,
        Stoppage
    }
}
