namespace TKH.Entities.Enums
{
    public enum FinancialTransactionType
    {
        Other = 0,             // Tanımsızlar
        Sale = 1,              // Satış
        Return = 2,            // İade
        Commission = 3,        // Komisyon Giderleri
        Promotion = 5,         // İndirimler, Kuponlar (Cirodan düşenler)
        ServiceFee = 8,        // Hizmet Bedelleri, Faturalar
        Settlement = 9,        // Para transferleri (WireTransfer vb.)
        Adjustment = 10        // Manuel düzeltmeler, Ciro iptalleri vb.
    }
}
