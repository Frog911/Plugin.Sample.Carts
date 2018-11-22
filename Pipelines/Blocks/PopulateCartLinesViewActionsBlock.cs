using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plugin.Sample.Carts.Policies;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Framework.Pipelines;

namespace Plugin.Sample.Carts.Pipelines.Blocks
{
    [PipelineDisplayName("Plugin.Sample.Carts.block.PopulateCartLinesViewActions")]
    public class PopulateCartLinesViewActionsBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        public override Task<EntityView> Run(EntityView arg, CommercePipelineExecutionContext context)
        {
            if (string.IsNullOrEmpty(arg?.Name)
                || !arg.Name.Equals(context.GetPolicy<KnownCartViewsPolicy>().Lines, StringComparison.OrdinalIgnoreCase)
                || !string.IsNullOrEmpty(arg.Action))
            {
                return Task.FromResult(arg);
            }

            Cart entity = context.CommerceContext.GetObject<EntityViewArgument>()?.Entity as Cart;
            if (entity == null)
            {
                return Task.FromResult(arg);
            }

            List<EntityActionView> actions = arg.GetPolicy<ActionsPolicy>().Actions;
            EntityActionView addLineItemActionView =
                new EntityActionView
                {
                    Name = context.GetPolicy<KnownCartViewsPolicy>().CartAddLineItem,
                    DisplayName = "Add LineItem",
                    Description = "Adds a line item",
                    IsEnabled = true,
                    EntityView = context.GetPolicy<KnownCartViewsPolicy>().CartAddLineItem,
                    Icon = "add"
                };
            actions.Add(addLineItemActionView);


            var editLineItemActionView =
                new EntityActionView
                {
                    Name = context.GetPolicy<KnownCartViewsPolicy>().CartEditLineItem,
                    DisplayName = "Edit LineItem",
                    Description = "Edits a line item",
                    IsEnabled = true,
                    EntityView = context.GetPolicy<KnownCartViewsPolicy>().CartEditLineItem,
                    Icon = "edit"
                };
            actions.Add(editLineItemActionView);

            EntityActionView deleteLineItemActionView =
                new EntityActionView
                {
                    Name = context.GetPolicy<KnownCartViewsPolicy>().CartDeleteLineItem,
                    DisplayName = "Delete LineItem",
                    Description = "Deletes a line item",
                    IsEnabled = true,
                    RequiresConfirmation = true,
                    EntityView = string.Empty,
                    Icon = "delete"
                };
            actions.Add(deleteLineItemActionView);

            return Task.FromResult(arg);
        }
    }
}