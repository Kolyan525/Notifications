using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Notifications.BL.Commands;
using Notifications.DAL.Models.Telegram;
using Notifications.DAL.Models;

namespace Notifications.BL.Services.Telegram
{
    public class CommandExecutor : ICommandExecutor
    {
        private readonly List<BaseCommand> _commands;
        private BaseCommand _lastCommand;
        private readonly NotificationsContext _context;
        private readonly TelegramBotClient _botClient;
        private readonly ICacheService cacheService;

        public CommandExecutor(IServiceProvider serviceProvider, NotificationsContext context, TelegramBot telegramBot, ICacheService cacheService)
        {
            _commands = serviceProvider.GetServices<BaseCommand>().ToList();
            _context = context;
            _botClient = _botClient = telegramBot.GetBot().Result;
            this.cacheService = cacheService;
        }

        public async Task Execute(Update update)
        {
            if (update?.Message?.Chat == null && update?.CallbackQuery == null)
                return;

            if (update.Type == UpdateType.Message)
            {
                switch (update.Message?.Text)
                {
                    case "Список команд":
                        await ExecuteCommand(CommandNames.Help, update);
                        return;
                    case "Події":
                        await ExecuteCommand(CommandNames.GetEvents, update);
                        return;
                    case "Відстежувані події":
                        await ExecuteCommand(CommandNames.GetSubscriptionEvents, update);
                        return;
                    case "Подія":
                        await ExecuteCommand(CommandNames.Event, update);
                        return;
                    case "Категорії":
                        await ExecuteCommand(CommandNames.GetCategories, update);
                        return;
                }
            }

            if (update.Type == UpdateType.CallbackQuery)
            {
                switch (update.CallbackQuery.Data)
                {
                    case "Subscription":
                        await ExecuteCommand(CommandNames.GetSubscription, update);
                        return;
                    case "Unsubscribe":
                        await ExecuteCommand(CommandNames.GetUnsubscribe, update);
                        return;
                    case "Detail":
                        await ExecuteCommand(CommandNames.GetEvent, update);
                        return;
                    case "NextEvents":
                        await ExecuteCommand(CommandNames.GetEvents, update);
                        return;
                    case "NextSearchingEvents":
                        await ExecuteCommand(CommandNames.GetEvent, update);
                        return;
                    case "NextSubEvents":
                        await ExecuteCommand(CommandNames.GetSubscriptionEvents, update);
                        return;
                    case "NextCategoryEvents":
                        await ExecuteCommand(CommandNames.GetCategories, update);
                        return;
                    case "PreviousCategoryEvents":
                        await ExecuteCommand(CommandNames.GetCategories, update);
                        return;
                    case "SubscriptionCategory":
                        await ExecuteCommand(CommandNames.GetSubscriptionForCategory, update);
                        return;
                    case "UnsubscribeCategory":
                        await ExecuteCommand(CommandNames.GetUnsubscribeFromCategory, update);
                        return;
                    case "NextCategories":
                        await ExecuteCommand(CommandNames.GetCategories, update);
                        return;
                    case "NextCategoryEventsWithNextCategories":
                        await ExecuteCommand(CommandNames.GetCategories, update);
                        return;
                    case "PreviousCategoryEventsWithNextCategories":
                        await ExecuteCommand(CommandNames.GetCategories, update);
                        return;
                    case "Week":
                        await ExecuteCommand(CommandNames.GetNotifications, update);
                        return;
                    case "Day":
                        await ExecuteCommand(CommandNames.GetNotifications, update);
                        return;
                    case "Hour":
                        await ExecuteCommand(CommandNames.GetNotifications, update);
                        return;
                    case "Other":
                        await ExecuteCommand(CommandNames.GetNotifications, update);
                        return;
                }
            }

            if (update.Message != null && update.Message.Text.Contains(CommandNames.StartCommand))
            {
                await ExecuteCommand(CommandNames.StartCommand, update);
                return;
            }

            if (_context.TelegramEvent.Any())
            {
                var option = _context.TelegramEvent.FirstOrDefault();
                if (option.EventOption == "Подія")
                {
                    await ExecuteCommand(CommandNames.GetEvent, update);
                    return;
                }
            }

            var check = cacheService.CheckActiveCacheEvent().Result;
            if (check == true)
            {
                await ExecuteCommand(CommandNames.GetNotifications, update);
                return;
            }
            else
            {
                await _botClient.SendTextMessageAsync(update.Message.Chat.Id, "Не має такої команди!" +
                    "\nЩоб дізнатися список команд введіть \"Список команд\", або виберіть цю команду у меню");
                return;
            }
        }

        private async Task ExecuteCommand(string commandName, Update update)
        {
            _lastCommand = _commands.First(x => x.Name == commandName);
            await _lastCommand.ExecuteAsync(update);
        }
    }
}
