using System.Linq;
using System.Threading.Tasks;
using Plugin.Sample.Carts.Policies;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Plugin.Sample.Carts.Pipelines.Blocks
{
    [PipelineDisplayName("Plugin.Sample.Carts.block.GetCartsListView")]
    public class GetCartsListViewBlock : GetListViewBlock
    {
        public GetCartsListViewBlock(CommerceCommander commander)
          : base(commander)
        {
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{Name}: The argument cannot be null.");
            var entityViewArgument = context.CommerceContext.GetObjects<EntityViewArgument>().FirstOrDefault();

            if (string.IsNullOrEmpty(entityViewArgument?.ViewName)
                || !entityViewArgument.ViewName.Contains("CartsList"))
            {
                return entityView;
            }

            var listName = entityViewArgument.ViewName.Replace("CartsList-", string.Empty);
            await SetListMetadata(entityView, listName, context.GetPolicy<KnownCartActionsPolicy>().PaginateCartsList, context);
            var childViewName = context.GetPolicy<KnownCartViewsPolicy>().Summary;
            entityView.DisplayName = "Carts";

            foreach (var cart in (await GetEntities(entityView, listName, context)).OfType<Cart>())
            {
                var summaryView = new EntityView
                {
                    EntityId = string.Empty,
                    ItemId = cart.Id,
                    Name = childViewName
                };

                var childView = summaryView;

                var cartIdProperty = new ViewProperty
                {
                    Name = "CartId",
                    RawValue = cart.Id,
                    IsReadOnly = true,
                    UiType = "EntityLink"
                };
                childView.Properties.Add(cartIdProperty);

                var dateCreatedProperty = new ViewProperty
                {
                    Name = "DateCreated",
                    RawValue = cart.DateCreated,
                    IsReadOnly = true,
                    UiType = "FullDateTime"
                };
                childView.Properties.Add(dateCreatedProperty);

                var adjustmentsTotal = cart.Totals.AdjustmentsTotal;
                var cartTotal = cart.Totals.GrandTotal;
                var paymentTotal = cart.Totals.PaymentsTotal;

                var adjustmentsProperty = new ViewProperty
                {
                    Name = "Adjustments",
                    RawValue = adjustmentsTotal,
                    IsReadOnly = true
                };
                childView.Properties.Add(adjustmentsProperty);

                var cartTotalProperty = new ViewProperty
                {
                    Name = "CartTotal",
                    RawValue = cartTotal,
                    IsReadOnly = true
                };
                childView.Properties.Add(cartTotalProperty);

                var paymentsProperty = new ViewProperty
                {
                    Name = "Payments",
                    RawValue = paymentTotal,
                    IsReadOnly = true
                };
                childView.Properties.Add(paymentsProperty);

                var versionProperty = new ViewProperty
                {
                    Name = "Version",
                    RawValue = cart.Version,
                    IsReadOnly = true,
                    IsHidden = true
                };
                childView.Properties.Add(versionProperty);

                var shopProperty = new ViewProperty
                {
                    Name = "Shop",
                    RawValue = cart.ShopName,
                    IsReadOnly = true
                };
                childView.Properties.Add(shopProperty);

                var component = cart.GetComponent<ContactComponent>();
                var customerProperty = new ViewProperty
                {
                    Name = "Customer",
                    RawValue = component.CustomerId,
                    IsReadOnly = true
                };
                childView.Properties.Add(customerProperty);

                var emailProperty = new ViewProperty
                {
                    Name = "Email",
                    RawValue = component.Email,
                    IsReadOnly = true
                };
                childView.Properties.Add(emailProperty);

                var currencyProperty = new ViewProperty
                {
                    Name = "Currency",
                    RawValue = component.Currency,
                    IsReadOnly = true
                };
                childView.Properties.Add(currencyProperty);

                entityView.ChildViews.Add(childView);
            }
            return entityView;
        }
    }
}
