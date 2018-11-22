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
    [PipelineDisplayName("Plugin.Sample.Carts.block.DoActionDeleteLineItem")]
    public class DoActionDeleteLineItemBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly RemoveCartLineCommand _removeCartLineCommand;

        public DoActionDeleteLineItemBlock(RemoveCartLineCommand removeCartLineCommand)
        {
            _removeCartLineCommand = removeCartLineCommand;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            if (entityView == null)
            {
                return null;
            }

            if (entityView.Action != context.GetPolicy<KnownCartActionsPolicy>().CartDeleteLineItem)
            {
                return entityView;
            }

            var cart = context.CommerceContext.GetObject((Func<Cart, bool>)(p => p.Id == entityView.EntityId));
            if (cart == null)
            {
                await context.CommerceContext.AddMessage(context.GetPolicy<KnownResultCodes>().ValidationError, "CartNotFound", new object[1]
                {
                    entityView.EntityId
                }, $"{Name}-Cart with id {entityView.EntityId} was not found.");

                return entityView;
            }
           
            var line = cart.Lines.FirstOrDefault(p => p.Id == entityView.ItemId);
            if (line == null)
            {
                await context.CommerceContext.AddMessage(context.GetPolicy<KnownResultCodes>().ValidationError, "CartLineNotFound", new object[2]
                {
                      entityView.ItemId,
                      cart.Id
                }, $"Line {entityView.ItemId} in cart {cart.Id} was not found.");
                return entityView;
            }

            await _removeCartLineCommand.Process(context.CommerceContext, cart, line);

            return entityView;
        }
    }
}