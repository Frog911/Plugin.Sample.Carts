using System;
using System.Linq;
using System.Threading.Tasks;
using Plugin.Sample.Carts.Policies;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Plugin.Sample.Carts.Pipelines.Blocks
{
    [PipelineDisplayName("Plugin.Sample.Carts.block.GetCartsDashboardView")]
    public class GetCartsDashboardViewBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly IPopulateListMetadataPipeline _populateListMetadataPipeline;

        public GetCartsDashboardViewBlock(IPopulateListMetadataPipeline populateListMetadataPipeline)
        {
            _populateListMetadataPipeline = populateListMetadataPipeline;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{Name}: The argument cannot be null.");
            var entityViewArgument = context.CommerceContext.GetObjects<EntityViewArgument>().FirstOrDefault();
            if (string.IsNullOrEmpty(entityViewArgument?.ViewName)
                || !entityViewArgument.ViewName.Equals(context.GetPolicy<KnownCartViewsPolicy>().CartsDashboard,
                    StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            entityView.DisplayName = "Carts Dashboard";

            var cartsProperty = new ViewProperty
            {
                Name = context.GetPolicy<KnownCartViewsPolicy>().Carts,
                RawValue = await GetListCount("Carts", context.CommerceContext),
                IsReadOnly = true
            };
            entityView.Properties.Add(cartsProperty);

            return entityView;
        }

        private async Task<Decimal> GetListCount(string listName, CommerceContext context)
        {
            return Convert.ToDecimal((await _populateListMetadataPipeline.Run(new ListMetadata(listName), context.GetPipelineContextOptions())).Count);
        }
    }
}
