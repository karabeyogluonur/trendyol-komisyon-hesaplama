using TKH.Core.Common.Exceptions;
using TKH.Core.Entities.Abstract;
using TKH.Entities.Common;
using TKH.Entities.Enums;

namespace TKH.Entities
{
    public class MarketplaceAccount : BaseEntity, IEntity
    {
        #region Properties

        public MarketplaceType MarketplaceType { get; private set; }

        public string StoreName { get; private set; } = string.Empty;
        public string ApiKey { get; private set; } = string.Empty;
        public string ApiSecretKey { get; private set; } = string.Empty;
        public string MerchantId { get; private set; } = string.Empty;

        public bool IsActive { get; private set; }

        public MarketplaceConnectionState ConnectionState { get; private set; }
        public MarketplaceSyncState SyncState { get; private set; }

        public DateTime? LastSyncStartTime { get; private set; }
        public string? LastErrorMessage { get; private set; }
        public DateTime? LastErrorDate { get; private set; }

        public virtual ICollection<Product> Products { get; private set; } = new List<Product>();
        public virtual ICollection<Order> Orders { get; private set; } = new List<Order>();
        public virtual ICollection<ShipmentTransaction> ShipmentTransactions { get; private set; } = new List<ShipmentTransaction>();
        public virtual ICollection<Claim> Claims { get; private set; } = new List<Claim>();
        public virtual ICollection<FinancialTransaction> FinancialTransactions { get; private set; } = new List<FinancialTransaction>();

        #endregion

        #region Ctor

        protected MarketplaceAccount()
        {

        }

        #endregion

        #region Factory

        public static MarketplaceAccount Create(MarketplaceType marketplaceType, string storeName, string merchantId, string apiKey, string encryptedApiSecretKey)
        {
            return new MarketplaceAccount
            {
                MarketplaceType = marketplaceType,
                StoreName = storeName,
                MerchantId = merchantId,
                ApiKey = apiKey,
                ApiSecretKey = encryptedApiSecretKey,

                IsActive = true,
                ConnectionState = MarketplaceConnectionState.Initializing,
                SyncState = MarketplaceSyncState.Queued
            };
        }

        #endregion

        #region Update

        public void UpdateGeneralInfo(string storeName, MarketplaceType marketplaceType, bool isActive)
        {
            StoreName = storeName;
            MarketplaceType = marketplaceType;
            IsActive = isActive;
        }

        public void UpdateCredentials(string apiKey, string merchantId, string? encryptedApiSecretKey)
        {
            bool credentialsChanged = ApiKey != apiKey || MerchantId != merchantId || !string.IsNullOrWhiteSpace(encryptedApiSecretKey);

            ApiKey = apiKey;
            MerchantId = merchantId;

            if (!string.IsNullOrWhiteSpace(encryptedApiSecretKey))
                ApiSecretKey = encryptedApiSecretKey;

            if (credentialsChanged)
                ResetConnection();
        }

        #endregion

        #region Sync Logic

        public bool CanStartSync(DateTime now, TimeSpan zombieThreshold)
        {
            if (!IsActive)
                return false;

            if (SyncState != MarketplaceSyncState.Syncing)
                return true;

            if (!LastSyncStartTime.HasValue)
                return true;

            return LastSyncStartTime.Value < now.Subtract(zombieThreshold);
        }

        public void MarkAsSyncing(DateTime now)
        {
            SyncState = MarketplaceSyncState.Syncing;
            LastSyncStartTime = now;

            if (ConnectionState == MarketplaceConnectionState.AuthError || ConnectionState == MarketplaceConnectionState.SystemError)
                ConnectionState = MarketplaceConnectionState.Initializing;
        }

        public void MarkSyncCompleted()
        {
            SyncState = MarketplaceSyncState.Idle;
            ConnectionState = MarketplaceConnectionState.Connected;
            ClearErrors();
        }

        public void MarkSyncFailed(string errorMessage, bool isAuthError)
        {
            SyncState = MarketplaceSyncState.Idle;
            LastErrorMessage = errorMessage;
            LastErrorDate = DateTime.UtcNow;

            ConnectionState = isAuthError ? MarketplaceConnectionState.AuthError : MarketplaceConnectionState.SystemError;
        }

        #endregion

        #region Delete Rules

        public void EnsureDeletable(string demoMerchantId)
        {
            if (MerchantId == demoMerchantId)
                throw new DomainException("Demo hesap silinemez.");

            if (SyncState == MarketplaceSyncState.Syncing)
                throw new DomainException("Veri eşitlemesi devam eden bir mağaza silinemez.");
        }

        #endregion

        #region Helpers

        private void ResetConnection()
        {
            ConnectionState = MarketplaceConnectionState.Initializing;
            SyncState = MarketplaceSyncState.Queued;
            ClearErrors();
        }

        private void ClearErrors()
        {
            LastErrorMessage = null;
            LastErrorDate = null;
        }

        #endregion
    }
}
