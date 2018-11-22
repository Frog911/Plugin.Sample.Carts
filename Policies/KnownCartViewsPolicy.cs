using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Carts;

namespace Plugin.Sample.Carts.Policies
{
    public class KnownCartViewsPolicy : Policy
    {
        public KnownCartViewsPolicy()
        {
            CartsDashboard = nameof(CartsDashboard);
            Preview = nameof(Preview);
            Summary = nameof(Summary);
            Master = nameof(Master);
            Lines = nameof(Lines);
            CartEditLineItem = nameof(CartEditLineItem);
            CartAddLineItem = nameof(CartAddLineItem);
            LineItemDetails = nameof(LineItemDetails);
            Adjustments = nameof(Adjustments);
            Adjustment = nameof(Adjustment);
            Messages = nameof(Messages);
            Message = nameof(Message);
            Carts = nameof(Carts);
            CartDeleteLineItem = nameof(CartDeleteLineItem);
            DeleteCart = nameof(DeleteCart);
            AllCarts = nameof(AllCarts);
        }

        public string CartsDashboard { get; set; }

        public string Preview { get; set; }

        public string Summary { get; set; }

        public string Master { get; set; }

        public string Lines { get; set; }

        public string CartEditLineItem { get; set; }

        public string CartAddLineItem { get; set; }

        public string LineItemDetails { get; set; }

        public string Adjustments { get; set; }

        public string Adjustment { get; set; }

        public string Messages { get; set; }

        public string Message { get; set; }

        public string Carts { get; set; }

        public string CartDeleteLineItem { get; set; }

        public string DeleteCart { get; set; }

        public string AllCarts { get; set; }
    }
}