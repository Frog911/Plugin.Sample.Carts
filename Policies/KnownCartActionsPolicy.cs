using Sitecore.Commerce.Core;

namespace Plugin.Sample.Carts.Policies
{
    public class KnownCartActionsPolicy : Policy
    {
        public KnownCartActionsPolicy()
        {
            DeleteCart = nameof(DeleteCart);
            CartEditLineItem = nameof(CartEditLineItem);
            CartDeleteLineItem = nameof(CartDeleteLineItem);
            CartAddLineItem = nameof(CartAddLineItem);
            PaginateCartsList = nameof(PaginateCartsList);
        }

        public string DeleteCart { get; set; }

        public string CartEditLineItem { get; set; }

        public string CartDeleteLineItem { get; set; }

        public string CartAddLineItem { get; set; }

        public string PaginateCartsList { get; set; }
    }
}