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
    [PipelineDisplayName("Plugin.Sample.Carts.block.DoActionEditLineItem")]
    public class DoActionEditLineItemBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly UpdateCartLineCommand _updateCartLineCommand;

        public DoActionEditLineItemBlock(UpdateCartLineCommand updateCartLineCommand)
        {
            _updateCartLineCommand = updateCartLineCommand;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            if (entityView == null
                || !entityView.Action.Equals(context.GetPolicy<KnownCartActionsPolicy>().CartEditLineItem,
                    StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            var cart = context.CommerceContext.GetObject((Func<Cart, bool>)(p => p.Id == entityView.EntityId));
            if (cart == null)
            {
                return entityView;
            }

            var updatedLine = cart.Lines.FirstOrDefault(p => p.Id.Equals(entityView.ItemId, StringComparison.OrdinalIgnoreCase));
            if (updatedLine == null)
            {
                await context.CommerceContext.AddMessage(context.GetPolicy<KnownResultCodes>().ValidationError, "CartLineNotFound", new object[]
                {
                    entityView.ItemId
                }, $"Cart line {entityView.ItemId} was not found");
                return entityView;
            }

            updatedLine.Quantity = Convert.ToDecimal(entityView.Properties.FirstOrDefault(p => p.Name == "Quantity")?.Value);
            await _updateCartLineCommand.Process(context.CommerceContext, cart, updatedLine);

            return entityView;
        }
    }
}