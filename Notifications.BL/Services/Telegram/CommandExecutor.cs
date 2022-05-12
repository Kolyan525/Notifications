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

        public CommandExecutor(IServiceProvider serviceProvider, NotificationsContext context, TelegramBot telegramBot)
        {
            _commands = serviceProvider.GetServices<BaseCommand>().ToList();
            _context = context;
            _botClient = _botClient = telegramBot.GetBot().Result;
        }

        public async Task Execute(Update update)
        {
            if (update?.Message?.Chat == null && update?.CallbackQuery == null)
                return;

            if (update.Type == UpdateType.Message)
            {
                switch (update.Message?.Text)
                {
                    case "help":
                        await ExecuteCommand(CommandNames.Help, update);
                        return;
                    case "Events":
                        await ExecuteCommand(CommandNames.GetEvents, update);
                        return;
                    case "SubEvents":
                        await ExecuteCommand(CommandNames.GetSubscriptionEvents, update);
                        return;
                    case "Event":
                        await ExecuteCommand(CommandNames.Event, update);
                        return;
                }
            }

            if (update.Type == UpdateType.CallbackQuery)
            {
                if (update.CallbackQuery.Data.Contains("Subscription"))
                {
                    await ExecuteCommand(CommandNames.GetSubscription, update);
                    return;
                }
                if (update.CallbackQuery.Data.Contains("Unsubscribe"))
                {
                    await ExecuteCommand(CommandNames.GetUnsubscribe, update);
                    return;
                }
            }

            if (update.Message != null && update.Message.Text.Contains(CommandNames.StartCommand))
            {
                await ExecuteCommand(CommandNames.StartCommand, update);
                return;
            }

            if (_context.telegramEvent.Any())
            {
                var option = _context.telegramEvent.FirstOrDefault();
                if (option.EventOption == "Подія")
                {
                    await ExecuteCommand(CommandNames.GetEvent, update);
                    return;
                }
            }
            else
            {
                await _botClient.SendTextMessageAsync(update.Message.Chat.Id, "Не має такої команди!");
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
