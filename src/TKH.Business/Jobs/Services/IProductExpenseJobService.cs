namespace TKH.Business.Jobs.Services
{
    public interface IProductExpenseJobService
    {
        Task DispatchScheduledAllAccountsExpenseAnalysisAsync();
        void DispatchImmediateSingleAccountExpenseAnalysis(int marketplaceAccountId);
        string ContinueWithExpenseAnalysisChain(int marketplaceAccountId, string parentJobId);
    }
}
