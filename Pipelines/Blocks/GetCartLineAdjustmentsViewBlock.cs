using System;
using System.Linq;
using System.Threading.Tasks;
using Plugin.Sample.Carts.Policies;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Commerce.Plugin.Pricing;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Plugin.Sample.Carts.Pipelines.Blocks
{
    [PipelineDisplayName("Plugin.Sample.Carts.block.GetCartLineAdjustmentsView")]
    public class GetCartLineAdjustmentsViewBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{Name}: The argument can not be null");
            var request = context.CommerceContext.GetObject<EntityViewArgument>();
            if (request == null
                || request.ViewName != context.GetPolicy<KnownCartViewsPolicy>().Adjustments
                && request.ViewName != context.GetPolicy<KnownCartViewsPolicy>().LineItemDetails
                || request.ViewName == context.GetPolicy<KnownCartViewsPolicy>().LineItemDetails
                && string.IsNullOrEmpty(request.ItemId)
                || !(request.Entity is Cart))
            {
                return Task.FromResult(entityView);
            }

            var cart = request.Entity as Cart;
            EntityView entityViewToProcess;
            if (request.ViewName == context.GetPolicy<KnownCartViewsPolicy>().LineItemDetails)
            {
                var adjustmentsView = new EntityView
                {
                    EntityId = cart.Id,
                    Name = context.GetPolicy<KnownCartViewsPolicy>().Adjustments
                };
                entityView.ChildViews.Add(adjustmentsView);
                entityViewToProcess = adjustmentsView;
            }
            else
            {
                entityViewToProcess = entityView;
            }

            entityViewToProcess.UiHint = "Table";

            var cartLineComponent = cart.Lines.FirstOrDefault(l => l.Id.Equals(request.ItemId, StringComparison.OrdinalIgnoreCase));
            if (cartLineComponent == null)
            {
                return Task.FromResult(entityView);
            }

            foreach (var adjustment in cartLineComponent.Adjustments)
            {
                PopulateAdjustmentChildView(entityViewToProcess, adjustment, context);
            }

            return Task.FromResult(entityView);
        }

        private static void PopulateAdjustmentChildView(EntityView entityView, AwardedAdjustment adjustment, CommercePipelineExecutionContext context)
        {
            var adjustmentView = new EntityView
            {
                EntityId = entityView.EntityId,
                ItemId = adjustment.Name,
                Name = context.GetPolicy<KnownCartViewsPolicy>().Adjustment
            };

            var itemIdProperty = new ViewProperty
            {
                Name = "ItemId",
                IsHidden = true,
                IsReadOnly = true,
                RawValue = adjustment.Name
            };
            adjustmentView.Properties.Add(itemIdProperty);

            var typeProperty = new ViewProperty
            {
                Name = "Type",
                IsReadOnly = true,
                RawValue = adjustment.AdjustmentType
            };
            adjustmentView.Properties.Add(typeProperty);

            var adjustmentProperty = new ViewProperty
            {
                Name = "Adjustment",
                IsReadOnly = true,
                RawValue = adjustment.Adjustment
            };
            adjustmentView.Properties.Add(adjustmentProperty);

            var taxableProperty = new ViewProperty
            {
                Name = "Taxable",
                IsReadOnly = true,
                RawValue = adjustment.IsTaxable
            };
            adjustmentView.Properties.Add(taxableProperty);

            var includeInGrandTotalProperty5 = new ViewProperty
            {
                Name = "IncludeInGrandTotal",
                IsReadOnly = true,
                RawValue = adjustment.IncludeInGrandTotal
            };
            adjustmentView.Properties.Add(includeInGrandTotalProperty5);

            var awardingBlockProperty = new ViewProperty
            {
                Name = "AwardingBlock",
                IsReadOnly = true,
                RawValue = adjustment.AwardingBlock
            };
            adjustmentView.Properties.Add(awardingBlockProperty);
            entityView.ChildViews.Add(adjustmentView);
        }
    }
}
