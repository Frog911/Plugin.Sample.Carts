using System.Linq;
using System.Threading.Tasks;
using Plugin.Sample.Carts.Policies;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Framework.Pipelines;

namespace Plugin.Sample.Carts.Pipelines.Blocks
{
    [PipelineDisplayName("Plugin.Sample.Carts.block.PopulateCartLinesViewActions")]
    public class PopulateEntityViewActionsMasterBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly IFindEntitiesInListPipeline _findEntitiesInListPipeline;

        public PopulateEntityViewActionsMasterBlock(IFindEntitiesInListPipeline findEntitiesInListPipeline)
        {
            _findEntitiesInListPipeline = findEntitiesInListPipeline;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            if (entityView == null || entityView.Name != "CartsList-Carts")
            {
                return entityView;
            }

            var listName = entityView.Name.Replace("CartsList-", string.Empty);
            var findResult = await _findEntitiesInListPipeline.Run(new FindEntitiesInListArgument(typeof(Cart), listName, 0, int.MaxValue), context.ContextOptions);
            var hasCarts = findResult.List.Items != null && findResult.List.Items.Any();

            var policy = entityView.GetPolicy<ActionsPolicy>();
            var actions = policy.Actions;
            var entityActionView = new EntityActionView
            {
                Name = context.GetPolicy<KnownCartViewsPolicy>().DeleteCart,
                DisplayName = "Delete Cart",
                Description = "Delete Cart",
                IsEnabled = hasCarts ,
                RequiresConfirmation = true,
                EntityView = null,
                Icon = "delete"
            };

            actions.Add(entityActionView);

            return entityView;
        }
    }
}