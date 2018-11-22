using System.Threading.Tasks;
using Plugin.Sample.Carts.Policies;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Plugin.Sample.Carts.Pipelines.Blocks
{
    [PipelineDisplayName("Plugin.Sample.Carts.block.GetCartPreviewEntityView")]
    public class GetCartPreviewEntityViewBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull("The argument can not be null");
            EntityViewArgument entityViewArgument = context.CommerceContext.GetObject<EntityViewArgument>();
            if (entityViewArgument == null
                || entityViewArgument.ViewName != context.GetPolicy<KnownCartViewsPolicy>().Preview
                || entityViewArgument.Entity == null)
            {
                return Task.FromResult(entityView);
            }

            if (entityViewArgument.Entity is Cart)
            {
                var entity = entityViewArgument.Entity as Cart;
                var cartIdProperty = new ViewProperty
                {
                    Name = "CartId",
                    RawValue = entity.Id
                };
                entityView.Properties.Add(cartIdProperty);

                var dateCreatedProperty = new ViewProperty
                {
                    Name = "DateCreated",
                    RawValue = entity.DateCreated
                };
                entityView.Properties.Add(dateCreatedProperty);

                var shopNameProperty = new ViewProperty
                {
                    Name = "ShopName",
                    RawValue = entity.ShopName
                };
                entityView.Properties.Add(shopNameProperty);

                var cartTotalProperty = new ViewProperty
                {
                    Name = "CartTotal",
                    RawValue = entity.Totals.GrandTotal
                };
                entityView.Properties.Add(cartTotalProperty);

                var customerEmailProperty = new ViewProperty
                {
                    Name = "CustomerEmail",
                    RawValue = entity.GetComponent<ContactComponent>().Email
                };
                entityView.Properties.Add(customerEmailProperty);
            }
            return Task.FromResult(entityView);
        }
    }
}
