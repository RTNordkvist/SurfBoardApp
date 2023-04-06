using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;

namespace SurfBoardApp.ModelBinding
{
    /// <summary>
    /// Inspiration from https://stackoverflow.com/questions/39737607/aspnet-core-decimal-binding-not-working-on-non-english-culture
    /// </summary>
    public class InvariantDecimalModelBinderProvider : IModelBinderProvider
    {
        private readonly ILoggerFactory loggerFactory;

        public InvariantDecimalModelBinderProvider(ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;
        }

        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            if (!context.Metadata.IsComplexType && 
                (context.Metadata.ModelType == typeof(decimal) || context.Metadata.ModelType == typeof(decimal?) || 
                context.Metadata.ModelType == typeof(double) || context.Metadata.ModelType == typeof(double?)))
            {
                return new InvariantDecimalModelBinder(context.Metadata.ModelType, loggerFactory);
            }

            return null;
        }
    }

    public class InvariantDecimalModelBinder : IModelBinder
    {
        private readonly SimpleTypeModelBinder _baseBinder;

        public InvariantDecimalModelBinder(Type modelType, ILoggerFactory loggerFactory)
        {
            _baseBinder = new SimpleTypeModelBinder(modelType, loggerFactory);
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null) throw new ArgumentNullException(nameof(bindingContext));

            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (valueProviderResult != ValueProviderResult.None)
            {
                bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

                var valueAsString = valueProviderResult.FirstValue;

                if (bindingContext.ModelType == typeof(double) || bindingContext.ModelType == typeof(double?))
                {
                    double result;

                    // Use invariant culture
                    if (double.TryParse(valueAsString, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out result))
                    {
                        bindingContext.Result = ModelBindingResult.Success(result);
                        return Task.CompletedTask;
                    }
                } 
                else
                {
                    decimal result;

                    // Use invariant culture
                    if (decimal.TryParse(valueAsString, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out result))
                    {
                        bindingContext.Result = ModelBindingResult.Success(result);
                        return Task.CompletedTask;
                    }
                }
            }

            // If we haven't handled it, then we'll let the base SimpleTypeModelBinder handle it
            return _baseBinder.BindModelAsync(bindingContext);
        }
    }
}
