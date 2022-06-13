using Telegram.Bot.Types.ReplyMarkups;

namespace Notifications.BL
{
    public class TelegramButtons
    {
        public class GetEvent
        {
            public static InlineKeyboardMarkup Subscription = new(new[]
               {
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: "Підписатися", callbackData: "Subscription")
                }
            });
            public static InlineKeyboardMarkup Unsubscribe = new(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithCallbackData(text: "Відписатися", callbackData: "Unsubscribe")
                },
                new InlineKeyboardButton[]
                {
                        InlineKeyboardButton.WithCallbackData(text: "Отримати сповіщення", callbackData: "Notifications")
                }

            });
            public static InlineKeyboardMarkup ButtonsWithSubscription = new(new[]
            {
                    new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Підписатися", callbackData: "Subscription")
                    },
                    new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Завантажити ще", callbackData: "NextSearchingEvents")
                    }
                });
            public static InlineKeyboardMarkup ButtonsWithUnsubscribe = new(new[]
            {
                    new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Відписатися", callbackData: "Unsubscribe")
                    },
                    new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Отримати сповіщення", callbackData: "Notifications")
                    },
                    new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Завантажити ще", callbackData: "NextSearchingEvents")
                    }
                });
        }

        public class GetCategories
        {
            public static InlineKeyboardMarkup Subscription = new(new[]
                {
                    new InlineKeyboardButton []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Підписатися", callbackData: "SubscriptionCategory")
                    }
                });
            public static InlineKeyboardMarkup Unsubscribe = new(new[]
            {
                    new InlineKeyboardButton []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Відписатися", callbackData: "UnsubscribeCategory")
                    }
                });
            public static InlineKeyboardMarkup NextEventsWithSubscription = new(new[]
            {
                    new InlineKeyboardButton []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Наступні події", callbackData: "NextCategoryEvents")
                    },
                    new InlineKeyboardButton []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Підписатися", callbackData: "SubscriptionCategory")
                    }

                });
            public static InlineKeyboardMarkup NextEventsWithUnsubscribe = new(new[]
            {
                    new InlineKeyboardButton []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Наступні події", callbackData: "NextCategoryEvents")
                    },
                    new InlineKeyboardButton []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Відписатися", callbackData: "UnsubscribeCategory")
                    }
                });
            public static InlineKeyboardMarkup PreviousEventsWithSubscription = new(new[]
            {
                    new InlineKeyboardButton []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Попередні події", callbackData: "PreviousCategoryEvents")
                    },
                    new InlineKeyboardButton []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Підписатися", callbackData: "SubscriptionCategory")
                    }
                });
            public static InlineKeyboardMarkup PreviousEventsWithUnsubscribe = new(new[]
            {
                    new InlineKeyboardButton []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Попередні події", callbackData: "PreviousCategoryEvents")
                    },
                    new InlineKeyboardButton []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Відписатися", callbackData: "UnsubscribeCategory")
                    }
                });
            public static InlineKeyboardMarkup ButtonsWithSubscription = new(new[]
            {
                    new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Попередні події", callbackData: "PreviousCategoryEvents"),
                        InlineKeyboardButton.WithCallbackData(text: "Наступні події", callbackData: "NextCategoryEvents")
                    },
                    new InlineKeyboardButton []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Підписатися", callbackData: "SubscriptionCategory")
                    }
                });
            public static InlineKeyboardMarkup ButtonsWithUnsubscribe = new(new[]
            {
                    new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Попередні події", callbackData: "PreviousCategoryEvents"),
                        InlineKeyboardButton.WithCallbackData(text: "Наступні події", callbackData: "NextCategoryEvents"),
                    },
                    new InlineKeyboardButton []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Відписатися", callbackData: "UnsubscribeCategory")
                    }
            });


            public static InlineKeyboardMarkup NextCategories = new(new[]
            {
                    new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Завантажити ще", callbackData: "NextCategories")
                    }
            });
            public static InlineKeyboardMarkup NextCategoriesWithSubscription = new(new[]
            {
                    new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Підписатися", callbackData: "SubscriptionCategory")
                    },
                    new InlineKeyboardButton []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Завантажити ще", callbackData: "NextCategories")
                    }
            });
            public static InlineKeyboardMarkup NextCategoriesWithUnsubscribe = new(new[]
            {
                    new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Відписатися", callbackData: "UnsubscribeCategory")
                    },
                    new InlineKeyboardButton []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Завантажити ще", callbackData: "NextCategories")
                    }
            });
            public static InlineKeyboardMarkup NextCategoriesWithNextEventsWithSubscription = new(new[]
            {
                    new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Наступні події", callbackData: "NextCategoryEventsWithNextCategories"),
                    },
                    new InlineKeyboardButton []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Підписатися", callbackData: "SubscriptionCategory")
                    },
                    new InlineKeyboardButton []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Завантажити ще", callbackData: "NextCategories")
                    }
            });
            public static InlineKeyboardMarkup NextCategoriesWithNextEventsWithUnsubscribe = new(new[]
            {
                     new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Наступні події", callbackData: "NextCategoryEventsWithNextCategories"),
                    },
                    new InlineKeyboardButton []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Відписатися", callbackData: "UnsubscribeCategory")
                    },
                    new InlineKeyboardButton []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Завантажити ще", callbackData: "NextCategories")
                    }
            });
            public static InlineKeyboardMarkup NextCategoriesWithPreviousEventsWithSubscription = new(new[]
            {
                    new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Попередні події", callbackData: "PreviousCategoryEventsWithNextCategories"),
                    },
                    new InlineKeyboardButton []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Підписатися", callbackData: "SubscriptionCategory")
                    },
                    new InlineKeyboardButton []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Завантажити ще", callbackData: "NextCategories")
                    }
            });
            public static InlineKeyboardMarkup NextCategoriesWithPreviousEventsWithUnsubscribe = new(new[]
            {
                    new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Попередні події", callbackData: "PreviousCategoryEventsWithNextCategories"),
                    },
                    new InlineKeyboardButton []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Відписатися", callbackData: "UnsubscribeCategory")
                    },
                    new InlineKeyboardButton []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Завантажити ще", callbackData: "NextCategories")
                    }
            });
            public static InlineKeyboardMarkup NextCategoriesWithButtonsWithSubscription = new(new[]
            {
                    new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Попередні події", callbackData: "PreviousCategoryEventsWithNextCategories"),
                        InlineKeyboardButton.WithCallbackData(text: "Наступні події", callbackData: "NextCategoryEventsWithNextCategories"),
                    },
                    new InlineKeyboardButton []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Підписатися", callbackData: "SubscriptionCategory")
                    },
                    new InlineKeyboardButton []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Завантажити ще", callbackData: "NextCategories")
                    }
            });
            public static InlineKeyboardMarkup NextCategoriesWithButtonsWithUnsubscribe = new(new[]
            {
                    new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Попередні події", callbackData: "PreviousCategoryEventsWithNextCategories"),
                        InlineKeyboardButton.WithCallbackData(text: "Наступні події", callbackData: "NextCategoryEventsWithNextCategories"),
                    },
                    new InlineKeyboardButton []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Відписатися", callbackData: "UnsubscribeCategory")
                    },
                    new InlineKeyboardButton []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Завантажити ще", callbackData: "NextCategories")
                    }
            });
        }

        public class GetEvents
        {
            public static InlineKeyboardMarkup Detail = new(new[]
                {
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Детальніше", callbackData: "Detail")
                    }
                });
            public static InlineKeyboardMarkup Buttons = new(new[]
            {
                    new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Детальніше", callbackData: "Detail")
                    },
                    new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Завантажити ще", callbackData: "NextEvents")
                    }
            });
        }
        public class GetSubscription
        {
            public static InlineKeyboardMarkup Buttons = new(new[]
            {
                    new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithCallbackData(text: "За тиждень до початку", callbackData: "Week"),
                        InlineKeyboardButton.WithCallbackData(text: "За день до початку", callbackData: "Day")
                    },
                    new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithCallbackData(text: "За годину до початку", callbackData: "Hour"),
                        InlineKeyboardButton.WithCallbackData(text: "Обрати свій варіант", callbackData: "Other")
                    }
            });
        }
        public class GetSubscriptionEvents
        {
            public static InlineKeyboardMarkup Detail = new(new[]
                {
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Детальніше", callbackData: "Detail")
                    }
                });
            public static InlineKeyboardMarkup Buttons = new(new[]
            {
                    new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Детальніше", callbackData: "Detail")
                    },
                    new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Завантажити ще", callbackData: "NextSubEvents")
                    }
            });
        }
        public class StartCommand
        {
            public static ReplyKeyboardMarkup ReplyKeyboard = new(new[]
            {
                new KeyboardButton[] {"Список команд", "Події"},
                new KeyboardButton[] { "Подія", "Відстежувані події" },
                new KeyboardButton[] { "Категорії" }
            })
            {
                ResizeKeyboard = true
            };
        }
    }
}
