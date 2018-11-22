using System;
using System.Threading.Tasks;
using Plugin.Sample.Carts.Policies;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Framework.Pipelines;

namespace Plugin.Sample.Carts.Pipelines.Blocks
{
    [PipelineDisplayName("Plugin.Sample.Carts.block.DoActionDeleteCart")]
    public class DoActionDeleteCartBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly IDeleteEntityPipeline _deletePipeline;

        private readonly FindEntityCommand _findEntityCommand;

        public DoActionDeleteCartBlock(IDeleteEntityPipeline deletePipeline, FindEntityCommand findEntityCommand)
        {
            _deletePipeline = deletePipeline;
            _findEntityCommand = findEntityCommand;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            if (string.IsNullOrEmpty(entityView?.Action)
                || !entityView.Action.Equals(context.GetPolicy<KnownCartActionsPolicy>().DeleteCart, StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            if (string.IsNullOrEmpty(entityView.ItemId))
            {
                await context.CommerceContext.AddMessage(context.GetPolicy<KnownResultCodes>().ValidationError, "InvalidOrMissingPropertyValue", new object[]
                {
                    "ItemId"
                }, "Invalid or missing value for property 'ItemId'.");
                return entityView;
            }
            var cart = await _findEntityCommand.Process(context.CommerceContext, typeof(Cart), entityView.ItemId);
            if (cart == null)
            {
                await context.CommerceContext.AddMessage(context.GetPolicy<KnownResultCodes>().ValidationError, "EntityNotFound", new object[]
                {
                    entityView.ItemId
                }, $"Entity {entityView.ItemId} was not found.");
                return entityView;
            }

            await _deletePipeline.Run(new DeleteEntityArgument(cart.Id), context);

            return entityView;
        }
    }
}