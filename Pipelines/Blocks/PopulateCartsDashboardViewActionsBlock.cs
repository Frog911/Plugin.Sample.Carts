using System;
using System.Linq;
using System.Threading.Tasks;
using Plugin.Sample.Carts.Policies;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.BusinessUsers;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Plugin.Sample.Carts.Pipelines.Blocks
{
    [PipelineDisplayName("Plugin.Sample.Carts.block.PopulateCartsDashboardViewActions")]
    public class PopulateCartsDashboardViewActionsBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{Name}: The argument cannot be null.");
            if (string.IsNullOrEmpty(entityView?.Name)
                || !entityView.Name.Equals(context.GetPolicy<KnownCartViewsPolicy>().CartsDashboard)
                || !string.IsNullOrEmpty(entityView.Action))
            {
                return Task.FromResult(entityView);
            }

            var entityViewArgument = context.CommerceContext.GetObjects<EntityViewArgument>().FirstOrDefault();
            if (!string.IsNullOrEmpty(entityViewArgument?.ViewName)
                && entityViewArgument.ViewName.Equals(
                    context.GetPolicy<KnownBusinessUsersViewsPolicy>().ToolsNavigation,
                    StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(entityView);
            }

            var actions = entityView.GetPolicy<ActionsPolicy>().Actions;

            var allCartsActionView = new EntityActionView
            {
                Name = context.GetPolicy<KnownCartViewsPolicy>().AllCarts,
                IsEnabled = true,
                UiHint = "RelatedList",
                EntityView = "CartsList-Carts",
                Icon = "list_style_bullets",
                DisplayName = "All"
            };
            actions.Add(allCartsActionView);
           
            return Task.FromResult(entityView);
        }
    }
}