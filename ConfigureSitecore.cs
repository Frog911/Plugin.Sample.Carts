using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Plugin.Sample.Carts.Pipelines.Blocks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.BusinessUsers;
using Sitecore.Framework.Configuration;
using Sitecore.Framework.Pipelines;
using Sitecore.Framework.Pipelines.Definitions.Extensions;

namespace Plugin.Sample.Carts
{
    public class ConfigureSitecore : IConfigureSitecore
    {
        public void ConfigureServices(IServiceCollection services)
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            services.RegisterAllPipelineBlocks(executingAssembly);
            services.Sitecore().Pipelines(config =>
            {
                string section = "main";
                int order = 1000;
                SitecorePipelinesConfigBuilder pipelinesConfigBuilder2 = config.ConfigurePipeline<IPopulateEntityViewActionsPipeline>(d => d
                    .Add<PopulateEntityViewActionsMasterBlock>()
                    .Add<PopulateCartLinesViewActionsBlock>().
                    Add<PopulateCartsDashboardViewActionsBlock>(), section, order);
                SitecorePipelinesConfigBuilder pipelinesConfigBuilder3 = pipelinesConfigBuilder2.ConfigurePipeline<IGetEntityViewPipeline>(d => d
                    .Add<GetCartLinesViewBlock>().After<PopulateEntityVersionBlock>()
                    .Add<GetCartPreviewEntityViewBlock>().After<PopulateEntityVersionBlock>()
                    .Add<GetEntityViewEditLineItemBlock>().After<PopulateEntityVersionBlock>()
                    .Add<GetCartSummaryEntityViewBlock>().After<PopulateEntityVersionBlock>()
                    .Add<GetCartAdjustmentsViewBlock>().After<PopulateEntityVersionBlock>()
                    .Add<GetCartMessagesViewBlock>().After<PopulateEntityVersionBlock>()
                    .Add<GetEntityViewAddLineItemBlock>().After<PopulateEntityVersionBlock>()
                    .Add<GetCartsDashboardViewBlock>().After<PopulateEntityVersionBlock>()
                    .Add<GetCartsListViewBlock>().After<PopulateEntityVersionBlock>()
                    .Add<GetCartLineAdjustmentsViewBlock>().After<PopulateEntityVersionBlock>()
                    .Add<GetCartLineMessagesViewBlock>().After<PopulateEntityVersionBlock>(), section, order);
                SitecorePipelinesConfigBuilder pipelinesConfigBuilder4 = pipelinesConfigBuilder3.ConfigurePipeline<IDoActionPipeline>(c => c
                    .Add<DoActionEditLineItemBlock>().After<ValidateEntityVersionBlock>()
                    .Add<DoActionDeleteLineItemBlock>().After<ValidateEntityVersionBlock>()
                    .Add<DoActionAddLineItemBlock>().After<ValidateEntityVersionBlock>()
                    .Add<DoActionPaginateCartsListBlock>().After<ValidateEntityVersionBlock>()
                    .Add<DoActionDeleteCartBlock>().After<ValidateEntityVersionBlock>()
                    , section
                    , order);
                pipelinesConfigBuilder4.ConfigurePipeline<IBizFxNavigationPipeline>(c => c
                    .Add<GetCartsNavigationViewBlock>()
                    .After<GetNavigationViewBlock>(), section, order);
            });
            services.RegisterAllCommands(executingAssembly);
        }
    }
}