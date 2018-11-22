using System.Linq;
using System.Threading.Tasks;
using Plugin.Sample.Carts.Policies;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Commerce.Plugin.Orders;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Plugin.Sample.Carts.Pipelines.Blocks
{
    [PipelineDisplayName("Plugin.Sample.Carts.block.GetCartSummaryEntityView")]
    public class GetCartSummaryEntityViewBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{Name}: the argument can not be null.");
            var entityViewArgument = context.CommerceContext.GetObjects<EntityViewArgument>().FirstOrDefault();

            if (entityViewArgument == null
                || entityViewArgument.ViewName != context.GetPolicy<KnownCartViewsPolicy>().Summary
                && entityViewArgument.ViewName != context.GetPolicy<KnownCartViewsPolicy>().Master)
            {
                return Task.FromResult(entityView);
            }
                
            var cart = entityViewArgument.Entity as Cart;

            if (entityViewArgument.Entity == null || cart == null)
            {
                return Task.FromResult(entityView);
            }

            EntityView entityViewToProcess;
            if (entityViewArgument.ViewName == context.GetPolicy<KnownOrderViewsPolicy>().Master)
            {
                var summaryEntityView = new EntityView
                {
                    EntityId = cart.Id,
                    Name = context.GetPolicy<KnownOrderViewsPolicy>().Summary,
                    DisplayRank = 100
                };
                entityView.ChildViews.Add(summaryEntityView);
                entityViewToProcess = summaryEntityView;
            }
            else
            {
                entityViewToProcess = entityView;
            }

            var dateCreatedProperty = new ViewProperty
            {
                Name = "DateCreated",
                IsReadOnly = true,
                RawValue = cart.DateCreated,
                UiType = "FullDateTime"
            };
            entityViewToProcess.Properties.Add(dateCreatedProperty);

            var dateUpdatedProperty = new ViewProperty
            {
                Name = "DateUpdated",
                IsReadOnly = true,
                RawValue = cart.DateUpdated,
                UiType = "FullDateTime"
            };
            entityViewToProcess.Properties.Add(dateUpdatedProperty);

            var shopNameProperty = new ViewProperty
            {
                Name = "ShopName",
                IsReadOnly = true,
                RawValue = cart.ShopName
            };
            entityViewToProcess.Properties.Add(shopNameProperty);
            if (cart.HasComponent<ContactComponent>())
            {

                var customerEmailProperty = new ViewProperty
                {
                    Name = "CustomerEmail",
                    IsReadOnly = true,
                    RawValue = cart.GetComponent<ContactComponent>().Email
                };
                entityViewToProcess.Properties.Add(customerEmailProperty);
            }


            var cartSubTotalProperty = new ViewProperty
            {
                Name = "CartSubTotal",
                IsReadOnly = true,
                RawValue = cart.Totals.SubTotal
            };
            entityViewToProcess.Properties.Add(cartSubTotalProperty);

            var cartAdjustmentsTotalProperty = new ViewProperty
            {
                Name = "CartAdjustmentsTotal",
                IsReadOnly = true,
                RawValue = cart.Totals.AdjustmentsTotal
            };
            entityViewToProcess.Properties.Add(cartAdjustmentsTotalProperty);

            var cartGrandTotalProperty = new ViewProperty
            {
                Name = "CartGrandTotal",
                IsReadOnly = true,
                RawValue = cart.Totals.GrandTotal
            };
            entityViewToProcess.Properties.Add(cartGrandTotalProperty);

            var cartPaymentsTotalProperty = new ViewProperty
            {
                Name = "CartPaymentsTotal",
                IsReadOnly = true,
                RawValue = cart.Totals.PaymentsTotal
            };
            entityViewToProcess.Properties.Add(cartPaymentsTotalProperty);

            return Task.FromResult(entityView);
        }
    }
}
