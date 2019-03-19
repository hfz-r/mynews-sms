using System.Threading.Tasks;
using Microsoft.AspNetCore.WebHooks;
using StockManagementSystem.Api.Constants;
using StockManagementSystem.Api.Helpers;
using StockManagementSystem.Api.Services;
using StockManagementSystem.Core.Domain.Common;
using StockManagementSystem.Core.Domain.Devices;
using StockManagementSystem.Core.Domain.Items;
using StockManagementSystem.Core.Domain.Users;
using StockManagementSystem.Core.Events;
using StockManagementSystem.Core.Infrastructure;
using StockManagementSystem.Services.Events;

namespace StockManagementSystem.Api.WebHooks
{
    public class WebHookEventConsumer :
        IConsumer<EntityInsertedEvent<User>>,
        IConsumer<EntityUpdatedEvent<User>>,
        IConsumer<EntityInsertedEvent<GenericAttribute>>,
        IConsumer<EntityUpdatedEvent<GenericAttribute>>,
        IConsumer<EntityInsertedEvent<Device>>,
        IConsumer<EntityUpdatedEvent<Device>>,
        IConsumer<EntityInsertedEvent<Item>>,
        IConsumer<EntityUpdatedEvent<Item>>
    {
        private IWebHookManager _webHookManager;
        private readonly IUserApiService _userApiService;
        private readonly IDtoHelper _dtoHelper;

        public WebHookEventConsumer()
        {
            _userApiService = EngineContext.Current.Resolve<IUserApiService>();
            _dtoHelper = EngineContext.Current.Resolve<IDtoHelper>();
        }

        private IWebHookManager WebHookManager
        {
            get
            {
                if (_webHookManager == null)
                {
                    var webHookService = EngineContext.Current.Resolve<IWebHookService>();
                    _webHookManager = webHookService.GetWebHookManager();
                }

                return _webHookManager;
            }
        }

        #region Utilities

        private async Task NotifyRegisteredWebHooks<T>(T entityDto, string webHookEvent)
        {
            await WebHookManager.NotifyAllAsync(new[] { new NotificationDictionary(webHookEvent, entityDto) }, predicate: null);
        }
      
        #endregion

        public void HandleEvent(EntityInsertedEvent<User> eventMessage)
        {
            if (eventMessage.Entity.IsGuest())
                return;

            var user = _userApiService.GetUserById(eventMessage.Entity.Id);

            NotifyRegisteredWebHooks(user, WebHookNames.UsersCreated).GetAwaiter().GetResult();
        }

        public void HandleEvent(EntityUpdatedEvent<User> eventMessage)
        {
            if (eventMessage.Entity.IsGuest())
                return;

            var user = _userApiService.GetUserById(eventMessage.Entity.Id, true);

            var webHookEvent = user.Deleted == true ? WebHookNames.UsersDeleted : WebHookNames.UsersUpdated;

            NotifyRegisteredWebHooks(user, webHookEvent).GetAwaiter().GetResult();
        }

        public void HandleEvent(EntityInsertedEvent<GenericAttribute> eventMessage)
        {
            if (eventMessage.Entity.Key == UserDefaults.FirstNameAttribute ||
                eventMessage.Entity.Key == UserDefaults.LastNameAttribute)
            {
                var userDto = _userApiService.GetUserById(eventMessage.Entity.EntityId);

                NotifyRegisteredWebHooks(userDto, WebHookNames.UsersCreated).GetAwaiter().GetResult();
            }
        }

        public void HandleEvent(EntityUpdatedEvent<GenericAttribute> eventMessage)
        {
            if (eventMessage.Entity.Key == UserDefaults.FirstNameAttribute ||
                eventMessage.Entity.Key == UserDefaults.LastNameAttribute)
            {
                var userDto = _userApiService.GetUserById(eventMessage.Entity.EntityId);

                NotifyRegisteredWebHooks(userDto, WebHookNames.UsersUpdated).GetAwaiter().GetResult();
            }
        }

        public void HandleEvent(EntityInsertedEvent<Device> eventMessage)
        {
            var deviceDto = _dtoHelper.PrepareDeviceDto(eventMessage.Entity);

            NotifyRegisteredWebHooks(deviceDto, WebHookNames.DevicesCreated).GetAwaiter().GetResult();
        }

        public void HandleEvent(EntityUpdatedEvent<Device> eventMessage)
        {
            var deviceDto = _dtoHelper.PrepareDeviceDto(eventMessage.Entity);

            NotifyRegisteredWebHooks(deviceDto, WebHookNames.DevicesUpdated).GetAwaiter().GetResult();
        }

        public void HandleEvent(EntityInsertedEvent<Item> eventMessage)
        {
            throw new System.NotImplementedException();
        }

        public void HandleEvent(EntityUpdatedEvent<Item> eventMessage)
        {
            throw new System.NotImplementedException();
        }
    }
}