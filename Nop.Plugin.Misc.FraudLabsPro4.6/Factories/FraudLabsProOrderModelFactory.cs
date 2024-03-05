﻿using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Misc.FraudLabsPro.Models.Order;
using Nop.Plugin.Misc.FraudLabsPro.Services;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Web.Areas.Admin.Models.Orders;

namespace Nop.Plugin.Misc.FraudLabsPro.Factories
{
    /// <summary>
    /// Represents FraudLabsPro order model factory
    /// </summary>
    public class FraudLabsProOrderModelFactory
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public FraudLabsProOrderModelFactory(
            ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            IWorkContext workContext)
        {
            _customerService = customerService;
            _genericAttributeService = genericAttributeService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare FraudLabsPro order model
        /// </summary>
        /// <param name="orderModel">Order model</param>
        /// <param name="order">Order</param>
        /// <returns>FraudLabsPro Order model</returns>
        public async Task<FraudLabsProOrderModel> PrepareOrderModelAsync(OrderModel orderModel, Order order)
        {
            if (orderModel == null)
                throw new ArgumentNullException(nameof(orderModel));

            if (order == null)
                throw new ArgumentNullException(nameof(order));

            //prepare model
            var model = new FraudLabsProOrderModel
            {
                Id = orderModel.Id,
                UserOrderID = order.Id.ToString(),
                HideBlock = await _genericAttributeService.GetAttributeAsync<bool>(await _workContext.GetCurrentCustomerAsync(), FraudLabsProDefaults.HideBlockAttribute)
            };

            var stringResponse = await _genericAttributeService.GetAttributeAsync<string>(order, FraudLabsProDefaults.OrderResultAttribute);

            if (!string.IsNullOrEmpty(stringResponse))
            {
                var response = JObject.Parse(stringResponse);
                var orderResultModel = response.ToObject<FraudLabsProOrderModel>();

                orderResultModel.Id = orderModel.Id;
                orderResultModel.IPAddress = (await _customerService.GetCustomerByIdAsync(order.CustomerId))?.LastIpAddress;
                orderResultModel.IPCountry = ISO3166.FromCountryCode(orderResultModel.IPCountry)?.Name ?? "-";
                orderResultModel.FraudLabsProOriginalStatus = await _genericAttributeService.GetAttributeAsync<string>(order, FraudLabsProDefaults.OrderStatusAttribute) ?? string.Empty;
                model = orderResultModel;
            }

            return model;
        }

        #endregion
    }
}