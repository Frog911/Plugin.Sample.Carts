using System;
using System.Linq;
using System.Threading.Tasks;
using Plugin.Sample.Carts.Policies;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Plugin.Sample.Carts.Pipelines.Blocks
{
    [PipelineDisplayName("Plugin.Sample.Carts.block.GetCartLineMessagesView")]
    public class GetCartLineMessagesViewBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{Name}: The argument can not be null");
            var request = context.CommerceContext.GetObject<EntityViewArgument>();
            if (request == null
                || request.ViewName != context.GetPolicy<KnownCartViewsPolicy>().Messages
                && request.ViewName != context.GetPolicy<KnownCartViewsPolicy>().LineItemDetails
                || request.ViewName == context.GetPolicy<KnownCartViewsPolicy>().LineItemDetails
                && string.IsNullOrEmpty(request.ItemId)
                || !(request.Entity is Cart))
            {
                return Task.FromResult(entityView);
            }

            var cart = request.Entity as Cart;
            EntityView entityViewToProcess;
            if (request.ViewName == context.GetPolicy<KnownCartViewsPolicy>().LineItemDetails)
            {
                var messagesView = new EntityView
                {
                    EntityId = cart.Id,
                    Name = context.GetPolicy<KnownCartViewsPolicy>().Messages
                };
                entityView.ChildViews.Add(messagesView);
                entityViewToProcess = messagesView;
            }
            else
            {
                entityViewToProcess = entityView;
            }
            
            entityViewToProcess.UiHint = "Table";
            MessagesComponent messages = null;

            var cartLineComponent = cart.Lines.FirstOrDefault(l => l.Id.Equals(request.ItemId, StringComparison.OrdinalIgnoreCase));
            if (cartLineComponent == null)
            {
                return Task.FromResult(entityView);
            }

            if (cartLineComponent.HasComponent<MessagesComponent>())
            {
                messages = cartLineComponent.ChildComponents.OfType<MessagesComponent>().FirstOrDefault();
            }

            if (messages == null || !messages.Messages.Any())
            {
                return Task.FromResult(entityView);
            }

            foreach (var message in messages.Messages)
            {
                PopulateMessageChildView(entityViewToProcess, message, context);
            }

            return Task.FromResult(entityView);
        }

        private static void PopulateMessageChildView(EntityView entityView, MessageModel message, CommercePipelineExecutionContext context)
        {
            var messageView = new EntityView
            {
                EntityId = entityView.EntityId,
                ItemId = message.Id,
                Name = context.GetPolicy<KnownCartViewsPolicy>().Message
            };

            var itemIdProperty = new ViewProperty
            {
                Name = "ItemId",
                IsHidden = true,
                IsReadOnly = true,
                RawValue = message.Id
            };
            messageView.Properties.Add(itemIdProperty);

            var codeProperty = new ViewProperty
            {
                Name = "Code",
                IsReadOnly = true,
                RawValue = message.Code
            };
            messageView.Properties.Add(codeProperty);

            var textProperty = new ViewProperty
            {
                Name = "Text",
                IsReadOnly = true,
                RawValue = message.Text
            };
            messageView.Properties.Add(textProperty);
            entityView.ChildViews.Add(messageView);
        }
    }
}
