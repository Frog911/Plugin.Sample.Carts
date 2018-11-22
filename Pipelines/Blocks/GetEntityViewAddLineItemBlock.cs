using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plugin.Sample.Carts.Policies;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Plugin.Sample.Carts.Pipelines.Blocks
{
    [PipelineDisplayName("Plugin.Sample.Carts.block.GetEntityViewEditLineItem")]
    public class GetEntityViewAddLineItemBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull("The argument can not be null");
            var entityViewArgument = context.CommerceContext.GetObject<EntityViewArgument>();
            if (entityViewArgument == null
                || entityViewArgument.ViewName != context.GetPolicy<KnownCartViewsPolicy>().CartAddLineItem
                || !(entityViewArgument.Entity is Cart))
            {
                return Task.FromResult(entityView);
            }

            entityView.Name = context.GetPolicy<KnownCartViewsPolicy>().CartAddLineItem;

            var itemProperty = new ViewProperty
            {
                Name = "Item",
                IsHidden = false,
                IsReadOnly = false,
                UiType = "Product",
                RawValue = string.Empty
            };
            entityView.Properties.Add(itemProperty);

            var policy = context.GetPolicy<LineQuantityPolicy>();
            var quantityProperty = new ViewProperty
            {
                Name = "Quantity",
                RawValue = Decimal.One,
                Policies = new List<Policy>
                {
                    policy
                },
                OriginalType = policy.AllowDecimal ? typeof(Decimal).FullName : typeof(int).FullName
            };
            entityView.Properties.Add(quantityProperty);
            return Task.FromResult(entityView);
        }
    }
}