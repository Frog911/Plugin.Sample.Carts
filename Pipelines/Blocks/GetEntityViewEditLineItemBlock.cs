using System;
using System.Collections.Generic;
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
    [PipelineDisplayName("Plugin.Sample.Carts.block.GetEntityViewEditLineItem")]
    public class GetEntityViewEditLineItemBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull("The argument can not be null");
            var entityViewArgument = context.CommerceContext.GetObject<EntityViewArgument>();
            if (entityViewArgument == null
                || entityViewArgument.ViewName != context.GetPolicy<KnownCartViewsPolicy>().CartEditLineItem
                || !(entityViewArgument.Entity is Cart))
            {
                return Task.FromResult(entityView);
            }

            var cart = entityViewArgument.Entity as Cart;
            entityView.Name = context.GetPolicy<KnownCartViewsPolicy>().CartEditLineItem;
            if (string.IsNullOrEmpty(entityView.ItemId))
            {
                return Task.FromResult(entityView);
            }

            var cartLineComponent = cart.Lines.FirstOrDefault(p => p.Id == entityView.ItemId);
            if (cartLineComponent == null)
            {
                return Task.FromResult(entityView);
            }

            var parentIdProperty = new ViewProperty
            {
                Name = "ParentId",
                RawValue = cart.Id,
                IsHidden = true,
                IsReadOnly = true
            };
            entityView.Properties.Add(parentIdProperty);

            var itemIdProperty = new ViewProperty
            {
                Name = "ItemId",
                RawValue = entityView.ItemId,
                IsHidden = true,
                IsReadOnly = true
            };
            entityView.Properties.Add(itemIdProperty);

            var policy = context.GetPolicy<LineQuantityPolicy>();
            var quantityProperty = new ViewProperty
            {
                Name = "Quantity",
                RawValue = policy.AllowDecimal ? cartLineComponent.Quantity : (int)cartLineComponent.Quantity,
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