using System.Runtime.Serialization;

namespace TKH.Integrations.Trendyol.Enums
{
    public enum TrendyolReturnReason
    {
        [EnumMember(Value = "None")]
        None = 51,

        [EnumMember(Value = "WarehouseLoss")]
        WarehouseLoss = 101,

        [EnumMember(Value = "CrossDefective")]
        CrossDefective = 151,

        [EnumMember(Value = "CustomerLostReturn")]
        CustomerLostReturn = 201,

        [EnumMember(Value = "DislikeModel")]
        DislikeModel = 251,

        [EnumMember(Value = "FaultyProductSent")]
        FaultyProductSent = 301,

        [EnumMember(Value = "WrongProductSent")]
        WrongProductSent = 351,

        [EnumMember(Value = "ChangedMind")]
        ChangedMind = 401,

        [EnumMember(Value = "Other")]
        Other = 451,

        [EnumMember(Value = "SizeTooSmall")]
        SizeTooSmall = 501,

        [EnumMember(Value = "SizeTooLarge")]
        SizeTooLarge = 551,

        [EnumMember(Value = "ProductNotAsSpecified")]
        ProductNotAsSpecified = 651,

        [EnumMember(Value = "WrongOrderPlaced")]
        WrongOrderPlaced = 701,

        [EnumMember(Value = "OtherFraudRelated")]
        OtherFraudRelated = 751,

        [EnumMember(Value = "DislikeQuality")]
        DislikeQuality = 1651,

        [EnumMember(Value = "DeliveryDelay")]
        DeliveryDelay = 1701,

        [EnumMember(Value = "PenalApproval")]
        PenalApproval = 2000,

        [EnumMember(Value = "NonPenalApproval")]
        NonPenalApproval = 2001,

        [EnumMember(Value = "TechnicalSupportRequired")]
        TechnicalSupportRequired = 2002,

        [EnumMember(Value = "UsedProductRejected")]
        UsedProductRejected = 2003,

        [EnumMember(Value = "HygienicReasonRejected")]
        HygienicReasonRejected = 2004,

        [EnumMember(Value = "NoProductionIssueFromAnalysis")]
        NoProductionIssueFromAnalysis = 2005,

        [EnumMember(Value = "AnalysisExchange")]
        AnalysisExchange = 2006,

        [EnumMember(Value = "AnalysisRepair")]
        AnalysisRepair = 2007,

        [EnumMember(Value = "MissingQuantityOrAccessory")]
        MissingQuantityOrAccessory = 2008,

        [EnumMember(Value = "NonFirmProduct")]
        NonFirmProduct = 2009,

        [EnumMember(Value = "Reshipment")]
        Reshipment = 2010,

        [EnumMember(Value = "InitiatedBySeller")]
        InitiatedBySeller = 2011,

        [EnumMember(Value = "ReturnPeriodExpired")]
        ReturnPeriodExpired = 2012,

        [EnumMember(Value = "ProductWithCustomer")]
        ProductWithCustomer = 2013,

        [EnumMember(Value = "SendForAnalysis")]
        SendForAnalysis = 2014,

        [EnumMember(Value = "UndeliverableShipment")]
        UndeliverableShipment = 2015,

        [EnumMember(Value = "BetterPriceAvailable")]
        BetterPriceAvailable = 2030,

        [EnumMember(Value = "DontLikeProduct")]
        DontLikeProduct = 2042,

        [EnumMember(Value = "MissingPartOfProduct")]
        MissingPartOfProduct = 2043
    }
}
