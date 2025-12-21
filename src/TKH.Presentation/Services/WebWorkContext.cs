using TKH.Business.Abstract;

namespace TKH.Presentation.Services
{
    public class WebWorkContext : IWorkContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string StoreCookieKey = "TKH_WorkContext_StoreId";

        public WebWorkContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? CurrentMarketplaceAccountId
        {
            get
            {
                var context = _httpContextAccessor.HttpContext;
                if (context == null) return null;

                var cookieValue = context.Request.Cookies[StoreCookieKey];

                if (int.TryParse(cookieValue, out int accountId))
                    return accountId;

                return null;
            }
            set
            {
                var context = _httpContextAccessor.HttpContext;

                if (context == null) return;

                if (value == null)
                {
                    context.Response.Cookies.Delete(StoreCookieKey);
                    return;
                }

                var options = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(30),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax
                };

                context.Response.Cookies.Append(StoreCookieKey, value.ToString(), options);
            }
        }
    }
}
