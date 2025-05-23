using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core.Events;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Models;
using NopStation.Plugin.Misc.Core.Controllers;
using NopStation.Plugin.Misc.Core.Models.Api;

namespace NopStation.Plugin.Misc.Core.Filters;

public sealed class PublishModelEventsAttribute : TypeFilterAttribute
{
    #region Ctor

    /// <summary>
    /// Create instance of the filter attribute
    /// </summary>
    /// <param name="ignore">Whether to ignore the execution of filter actions</param>
    public PublishModelEventsAttribute(bool ignore = false) : base(typeof(PublishModelEventsFilter))
    {
        IgnoreFilter = ignore;
        Arguments = new object[] { ignore };
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether to ignore the execution of filter actions
    /// </summary>
    public bool IgnoreFilter { get; }

    #endregion

    #region Nested filter

    /// <summary>
    /// Represents filter that publish ModelReceived event before the action executes, after model binding is complete
    /// and publish ModelPrepared event after the action executes, before the action result
    /// </summary>
    private class PublishModelEventsFilter : IAsyncActionFilter, IAsyncResultFilter
    {
        #region Fields

        private readonly bool _ignoreFilter;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        public PublishModelEventsFilter(bool ignoreFilter,
            IEventPublisher eventPublisher)
        {
            _ignoreFilter = ignoreFilter;
            _eventPublisher = eventPublisher;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Whether to ignore this filter
        /// </summary>
        /// <param name="context">A context for action filters</param>
        /// <returns>Result</returns>
        protected virtual bool IgnoreFilter(FilterContext context)
        {
            //check whether this filter has been overridden for the Action
            var actionFilter = context.ActionDescriptor.FilterDescriptors
                .Where(filterDescriptor => filterDescriptor.Scope == FilterScope.Action)
                .Select(filterDescriptor => filterDescriptor.Filter)
                .OfType<PublishModelEventsAttribute>()
                .FirstOrDefault();

            return actionFilter?.IgnoreFilter ?? _ignoreFilter;
        }

        /// <summary>
        /// Publish model prepared event
        /// </summary>
        /// <param name="model">Model</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task PublishModelPreparedEventAsync(object model)
        {
            if (model == null || !model.GetType().IsGenericType || model.GetType().GetGenericTypeDefinition() != typeof(GenericResponseModel<>))
                return;

            var responseModel = model.GetType().GetProperty("Data").GetValue(model);

            //we publish the ModelPrepared event for all models as the BaseNopModel, 
            //so you need to implement IConsumer<ModelPrepared<BaseNopModel>> interface to handle this event
            if (responseModel is BaseNopModel baseNopModel)
                await _eventPublisher.ModelPreparedAsync(baseNopModel);

            //we publish the ModelPrepared event for collection as the IEnumerable<BaseNopModel>, 
            //so you need to implement IConsumer<ModelPrepared<IEnumerable<BaseNopModel>>> interface to handle this event
            if (responseModel is IEnumerable<BaseNopModel> baseNopModelCollection)
                await _eventPublisher.ModelPreparedAsync(baseNopModelCollection);
        }

        /// <summary>
        /// Called asynchronously before the action, after model binding is complete.
        /// </summary>
        /// <param name="context">A context for action filters</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task PublishModelReceivedEventAsync(ActionExecutingContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            //only in POST requests
            if (!context.HttpContext.Request.Method.Equals(WebRequestMethods.Http.Post, StringComparison.InvariantCultureIgnoreCase))
                return;

            if (IgnoreFilter(context))
                return;

            //model received event
            foreach (var q in context.ActionArguments.Values)
            {
                if (q == null || !q.GetType().IsGenericType || q.GetType().GetGenericTypeDefinition() != typeof(BaseQueryModel<>))
                    continue;

                var queryModel = q.GetType().GetProperty("Data").GetValue(q);

                //we publish the ModelReceived event for all models as the BaseNopModel, 
                //so you need to implement IConsumer<ModelReceived<BaseNopModel>> interface to handle this event

                if (queryModel is BaseNopModel baseNopModel)
                    await _eventPublisher.ModelReceivedAsync(baseNopModel, context.ModelState);
            }
        }

        /// <summary>
        /// Called asynchronously before the action, after model binding is complete.
        /// </summary>
        /// <param name="context">A context for action filters</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task PublishModelPreparedEventAsync(ActionExecutingContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (IgnoreFilter(context))
                return;

            //model prepared event
            if (context.Controller is NopStationApiController controller)
                await PublishModelPreparedEventAsync(controller.ViewData.Model);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called asynchronously before the action, after model binding is complete.
        /// </summary>
        /// <param name="context">A context for action filters</param>
        /// <param name="next">A delegate invoked to execute the next action filter or the action itself</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            await PublishModelReceivedEventAsync(context);
            if (context.Result == null)
                await next();
            await PublishModelPreparedEventAsync(context);
        }

        /// <summary>Called asynchronously before the action result.</summary>
        /// <param name="context">A context for action filters</param>
        /// <param name="next">A delegate invoked to execute the next action filter or the action itself</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (IgnoreFilter(context))
                return;

            //model prepared event
            if (context.Result is ObjectResult result)
                await PublishModelPreparedEventAsync(result.Value);

            await next();
        }

        #endregion
    }

    #endregion
}