using System;
using System.Linq;
using System.Threading.Tasks;
using Plugin.Sample.Carts.Policies;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Commerce.Plugin.Pricing;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Plugin.Sample.Carts.Pipelines.Blocks
{
    [PipelineDisplayName("Plugin.Sample.Carts.block.GetCartLinesView")]
    public class GetCartLinesViewBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{Name}: The argument can not be null");
            var request = context.CommerceContext.GetObject<EntityViewArgument>();
            if (request == null
                || request.ViewName != context.GetPolicy<KnownCartViewsPolicy>().Lines
                && request.ViewName != context.GetPolicy<KnownCartViewsPolicy>().LineItemDetails
                && request.ViewName != context.GetPolicy<KnownCartViewsPolicy>().Master
                || request.ViewName == context.GetPolicy<KnownCartViewsPolicy>().LineItemDetails
                && string.IsNullOrEmpty(request.ItemId)
                || !(request.Entity is Cart))
            {
                return Task.FromResult(entityView);
            }

            var cart = request.Entity as Cart;
            EntityView entityViewToProcess;
            if (request.ViewName == context.GetPolicy<KnownCartViewsPolicy>().Master)
            {
                var linesView = new EntityView
                {
                    EntityId = cart.Id,
                    Name = "Lines"
                };
                entityView.ChildViews.Add(linesView);
                entityViewToProcess = linesView;
            }
            else
            {
                entityViewToProcess = entityView;
            }

            if (request.ViewName == context.GetPolicy<KnownCartViewsPolicy>().Lines
                || request.ViewName == context.GetPolicy<KnownCartViewsPolicy>().Master)
            {
                entityViewToProcess.UiHint = "Table";
                foreach (var line in cart.Lines)
                {
                    var lineItemDetailsView = new EntityView
                    {
                        EntityId = entityViewToProcess.EntityId,
                        ItemId = line.Id,
                        Name = context.GetPolicy<KnownCartViewsPolicy>().LineItemDetails
                    };

                    PopulateLineChildView(lineItemDetailsView, line, context);
                    entityViewToProcess.ChildViews.Add(lineItemDetailsView);
                }
            }
            else
            {
                var line = cart.Lines.FirstOrDefault(l => l.Id.Equals(request.ItemId, StringComparison.OrdinalIgnoreCase));
                PopulateLineChildView(entityViewToProcess, line, context);
            }
            return Task.FromResult(entityView);
        }

       private static void PopulateLineChildView(EntityView lineEntityView, CartLineComponent line, CommercePipelineExecutionContext context)
        {
            var policy = context.GetPolicy<LineQuantityPolicy>();
            var itemIdProperty = new ViewProperty
            {
                Name = "ItemId",
                IsHidden = true,
                IsReadOnly = true,
                RawValue = line.Id
            };
            lineEntityView.Properties.Add(itemIdProperty);

            var listPriceProperty = new ViewProperty
            {
                Name = "ListPrice",
                IsReadOnly = true,
                RawValue = line.UnitListPrice
            };
            lineEntityView.Properties.Add(listPriceProperty);

            var sellPriceProperty = new ViewProperty
            {
                Name = "SellPrice",
                IsReadOnly = true,
                RawValue = line.GetPolicy<PurchaseOptionMoneyPolicy>().SellPrice
            };
            lineEntityView.Properties.Add(sellPriceProperty);

            var quantityProperty = new ViewProperty
            {
                Name = "Quantity",
                IsReadOnly = true,
                RawValue = policy.AllowDecimal ? line.Quantity : (int)line.Quantity,
                OriginalType = policy.AllowDecimal ? typeof(Decimal).FullName : typeof(int).FullName
            };
            lineEntityView.Properties.Add(quantityProperty);

            var subtotalProperty = new ViewProperty
            {
                Name = "Subtotal",
                IsReadOnly = true,
                RawValue = line.Totals.SubTotal
            };
            lineEntityView.Properties.Add(subtotalProperty);

            var adjustmentsProperty = new ViewProperty
            {
                Name = "Adjustments",
                IsReadOnly = true,
                RawValue = line.Totals.AdjustmentsTotal
            };
            lineEntityView.Properties.Add(adjustmentsProperty);

            var lineTotalProperty = new ViewProperty
            {
                Name = "LineTotal",
                IsReadOnly = true,
                RawValue = line.Totals.GrandTotal
            };
            lineEntityView.Properties.Add(lineTotalProperty);

            var component = line.GetComponent<CartProductComponent>();
            var nameProperty = new ViewProperty
            {
                Name = "Name",
                IsReadOnly = true,
                RawValue = component.DisplayName,
                UiType = "ItemLink"
            };
            lineEntityView.Properties.Add(nameProperty);

            var sizeProperty = new ViewProperty
            {
                Name = "Size",
                IsReadOnly = true,
                RawValue = component.Size
            };
            lineEntityView.Properties.Add(sizeProperty);

            var colorProperty = new ViewProperty
            {
                Name = "Color",
                IsReadOnly = true,
                RawValue = component.Color
            };
            lineEntityView.Properties.Add(colorProperty);

            var styleProperty = new ViewProperty
            {
                Name = "Style",
                IsReadOnly = true,
                RawValue = component.Style
            };
            lineEntityView.Properties.Add(styleProperty);

            var variationProperty = new ViewProperty
            {
                Name = "Variation",
                IsReadOnly = true,
                RawValue = line.HasComponent<ItemVariationSelectedComponent>()
                    ? line.GetComponent<ItemVariationSelectedComponent>().VariationId
                    : string.Empty
            };
            lineEntityView.Properties.Add(variationProperty);
        }
    }
}
