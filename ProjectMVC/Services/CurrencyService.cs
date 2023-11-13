using ProjectMVC.Models;

namespace ProjectMVC.Services
{
    public class CurrencyService 
    {
        private readonly ModelContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CurrencyService(ModelContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public IQueryable<Product>? ConvertToSelectedCurrency(IQueryable<Product>? products)
        {
            var userCurrencyCode = _httpContextAccessor.HttpContext.Session.GetString("UserCurrency") ?? "JD";
            var currency = _context.Currencies.FirstOrDefault(c => c.Currencycode == userCurrencyCode);
            
            if(currency != null){
                
                foreach (var product in products) 
                {
                    product.Price = product.Price * currency.Exchangetobase;
                }
            }

            return products;
        }

        public List<Product> ConvertToSelectedCurrency(List<Product> products)
        {
            var userCurrencyCode = _httpContextAccessor.HttpContext.Session.GetString("UserCurrency") ?? "JD";
            var currency = _context.Currencies.FirstOrDefault(c => c.Currencycode == userCurrencyCode);

            if (currency != null)
            {

                foreach (var product in products)
                {
                    product.Price = product.Price * currency.Exchangetobase;
                }
            }

            return products;
        }

        public List<Shoppingcart> ConvertToSelectedCurrency(List<Shoppingcart> cart)
		{
			var userCurrencyCode = _httpContextAccessor.HttpContext.Session.GetString("UserCurrency") ?? "JD";
			var currency = _context.Currencies.FirstOrDefault(c => c.Currencycode == userCurrencyCode);

			if (currency != null)
			{

				foreach (var product in cart)
				{
					product.Product.Price = product.Product.Price * currency.Exchangetobase;
				}
			}

			return cart;
		}

		public Product ConvertToSelectedCurrency(Product product)
        {
            var userCurrencyCode = _httpContextAccessor.HttpContext.Session.GetString("UserCurrency") ?? "JD";
            var currency = _context.Currencies.FirstOrDefault(c => c.Currencycode == userCurrencyCode);

            if (currency != null)
            {
                product.Price = (decimal)product.Price * currency.Exchangetobase;

			}

            return product;
        }

		public List<Wishlist> ConvertToSelectedCurrency(List<Wishlist> wishlist)
		{
			var userCurrencyCode = _httpContextAccessor.HttpContext.Session.GetString("UserCurrency") ?? "JD";
			var currency = _context.Currencies.FirstOrDefault(c => c.Currencycode == userCurrencyCode);

			if (currency != null)
			{
                foreach (var item in wishlist) {
                    item.Product.Price = (decimal)item.Product.Price * currency.Exchangetobase;
				}
			}

			return wishlist;
		}

		public decimal? ConvertToSelectedCurrency(decimal? totalPrice)
		{
			var userCurrencyCode = _httpContextAccessor.HttpContext.Session.GetString("UserCurrency") ?? "JD";
			var currency = _context.Currencies.FirstOrDefault(c => c.Currencycode == userCurrencyCode);
			if (currency != null)
			{
				totalPrice = (decimal)totalPrice * currency.Exchangetobase;

			}

			return totalPrice;
		}

		public string GetCurrentCurrencyCode()
        {
            return _httpContextAccessor.HttpContext.Session.GetString("UserCurrency") ?? "JD";
        }

		
	}
}
