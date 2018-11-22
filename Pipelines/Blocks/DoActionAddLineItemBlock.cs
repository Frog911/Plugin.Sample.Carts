using System;
using System.Linq;
using System.Threading.Tasks;
using Plugin.Sample.Carts.Policies;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Framework.Pipelines;

namespace Plugin.Sample.Carts.Pipelines.Blocks
{
    [PipelineDisplayName("Plugin.Sample.Carts.block.DoActionAddLineItem")]
    public class DoActionAddLineItemBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly AddCartLineCommand _addCartLineCommand;

        public DoActionAddLineItemBlock(AddCartLineCommand addCartLineCommand)
        {
            _addCartLineCommand = addCartLineCommand;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            if (entityView == null)
            {
                return null;
            }

            if (entityView.Action != context.GetPolicy<KnownCartActionsPolicy>().CartAddLineItem)
            {
                return entityView;
            }

            var cart = context.CommerceContext.GetObject((Func<Cart, bool>)(p => p.Id.Equals(entityView.EntityId, StringComparison.OrdinalIgnoreCase)));
            if (cart == null)
            {
                return entityView;
            }

            var line = new CartLineComponent
            {
                ItemId = entityView.Properties.FirstOrDefault(p => p.Name.Equals("Item", StringComparison.OrdinalIgnoreCase))?.Value,
                Quantity = Convert.ToDecimal(entityView.Properties.FirstOrDefault(p => p.Name.Equals("Quantity", StringComparison.OrdinalIgnoreCase))?.Value)
            };

            await _addCartLineCommand.Process(context.CommerceContext, cart, line);

            return entityView;
        }
    }
}