using System.Threading.Tasks;
using Plugin.Sample.Carts.Policies;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Plugin.Sample.Carts.Pipelines.Blocks
{
    [PipelineDisplayName("Plugin.Sample.Carts.block.GetCartsNavigationView")]
    public class GetCartsNavigationViewBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{Name}: The argument cannot be null.");
            var newEntityView = new EntityView
            {
                Name = context.GetPolicy<KnownCartViewsPolicy>().CartsDashboard,
                ItemId = context.GetPolicy<KnownCartViewsPolicy>().CartsDashboard,
                Icon = "shopping_cart2",
                DisplayRank = 6,
                DisplayName = "Carts"
            };

            entityView.ChildViews.Add(newEntityView);

            return Task.FromResult(entityView);
        }
    }
}
