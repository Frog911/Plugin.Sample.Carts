using System;
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
    [PipelineDisplayName("Plugin.Sample.Carts.block.DoActionPaginateCartsList")]
    public class DoActionPaginateCartsListBlock : DoActionPaginateListBlock
    {
        public DoActionPaginateCartsListBlock(CommerceCommander commander)
          : base(commander)
        {
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{Name}: The argument cannot be null.");
            if (string.IsNullOrEmpty(entityView.Name)
                || !entityView.Name.Contains("CartsList")
                || string.IsNullOrEmpty(entityView.Action)
                || !entityView.Action.Equals(context.GetPolicy<KnownCartActionsPolicy>().PaginateCartsList,
                    StringComparison.OrdinalIgnoreCase) || !Validate(entityView, context.CommerceContext))
            {
                return entityView;
            }

            var childViewName = context.GetPolicy<KnownCartViewsPolicy>().Summary;
            foreach (var cart in (await GetEntities(entityView, context)).OfType<Cart>())
            {
                var summaryView = new EntityView
                {
                    EntityId = string.Empty,
                    ItemId = cart.Id,
                    Name = childViewName
                };

                var cartIdProperty = new ViewProperty
                {
                    Name = "CartId",
                    RawValue = cart.Id,
                    IsReadOnly = true,
                    UiType = "EntityLink"
                };
                summaryView.Properties.Add(cartIdProperty);

                var dateCreatedProperty = new ViewProperty
                {
                    Name = "DateCreated",
                    RawValue = cart.DateCreated,
                    IsReadOnly = true,
                    UiType = "FullDateTime"
                };
                summaryView.Properties.Add(dateCreatedProperty);

                var adjustmentsProperty = new ViewProperty
                {
                    Name = "Adjustments",
                    RawValue = cart.Totals.AdjustmentsTotal,
                    IsReadOnly = true
                };
                summaryView.Properties.Add(adjustmentsProperty);

                var orderTotalProperty = new ViewProperty
                {
                    Name = "OrderTotal",
                    RawValue = cart.Totals.GrandTotal,
                    IsReadOnly = true
                };
                summaryView.Properties.Add(orderTotalProperty);

                var paymentsProperty = new ViewProperty
                {
                    Name = "Payments",
                    RawValue = cart.Totals.PaymentsTotal,
                    IsReadOnly = true
                };
                summaryView.Properties.Add(paymentsProperty);

                var versionProperty = new ViewProperty
                {
                    Name = "Version",
                    RawValue = cart.Version,
                    IsReadOnly = true,
                    IsHidden = true
                };
                summaryView.Properties.Add(versionProperty);

                var shopProperty = new ViewProperty
                {
                    Name = "Shop",
                    RawValue = cart.ShopName,
                    IsReadOnly = true
                };
                summaryView.Properties.Add(shopProperty);

                var component = cart.GetComponent<ContactComponent>();
                var customerProperty = new ViewProperty
                {
                    Name = "Customer",
                    RawValue = component.CustomerId,
                    IsReadOnly = true
                };
                summaryView.Properties.Add(customerProperty);

                var emailProperty = new ViewProperty
                {
                    Name = "Email",
                    RawValue = component.Email,
                    IsReadOnly = true
                };
                summaryView.Properties.Add(emailProperty);

                var currencyProperty = new ViewProperty
                {
                    Name = "Currency",
                    RawValue = component.Currency,
                    IsReadOnly = true
                };
                summaryView.Properties.Add(currencyProperty);
                entityView.ChildViews.Add(summaryView);
            }

            return entityView;
        }
    }
}