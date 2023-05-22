# Notifications
Qualification Project for OA

## Встановлення для Visual Studio 2022
Перед початком потріно встановити SQL Server 2017 і вище.

- Завантажити архів з проектом
- Розпакувати в потрібну директорію
- Відкрити у Visual Studio 2022

Встановити в змінних оточення пристрою ключ. Це може бути будь-який рядок, наприклад GUID:

```sh
setx KEY "a33771e8-aa9d-47bc-a49a-f0fde5933d56" /M
```

Запустити ngrok.exe, ввести команду:
```
ngrok http --host-header=localhost 60043
```

Скопіювати адресу, навпроти значення Forwarding. Наприклад, на скріншоті нижче це "https://6e4b-87-196-72-94.eu.ngrok.io"
![alt text](https://github.com/Kolyan525/Notifications/blob/NotificationsService/Ngrok.png?raw=true)

Перейти в проект Notification.Api, змінити у файлі appsettings.json ключ для бота:
```sh
"Token": "Ваш ключ"
```

Встановити потрібну назву для бази даних:
```
"DefaultConnection": "Data Source=.\\SQLEXPRESS;Initial Catalog=**НазваБД**;Integrated Security=True;MultipleActiveResultSets=True;"
```

Та в поле Url вставити раніше скопійовану адресу, вона повинна закінчуватись на "/":
```
"Url": "https://6e4b-87-196-72-94.eu.ngrok.io/"
```

Встановити вебхук для бота, в браузері ввести наступне у адресний рядок:
```
https://api.telegram.org/bot**КлючДляБота**/setwebhook?url=**ПосиланняЗngrok.exe**/api/message/update
```

У Visual Studio 2022, перейти у View -> Other Windows -> Package Manager Console. У консолі що відкрилась ввести наступну команду:
```
update-database
```

Запустити проект, натиснувши на кнопку "IIS Express"

----------------------------------------------------------
# English
# Notifications
Qualification Project for OA

## Installation for Visual Studio 2022
Before you start you need to install SQL Server 2017 or higher.

- Download the project archive
- Unpack to desired folder
- Open in Visual Studio 2022

Set the key in system variables. This can be a random string (e.g. GUID):

```sh
setx KEY "a33771e8-aa9d-47bc-a49a-f0fde5933d56" /M
```

Run ngrok.exe, and run the following command:
```
ngrok http --host-header=localhost 60043
```

Copy the address, next to "Forwarding". For example, on the screenshot below it's "https://6e4b-87-196-72-94.eu.ngrok.io"
![alt text](https://github.com/Kolyan525/Notifications/blob/NotificationsService/Ngrok.png?raw=true)

Go to Notification.Api project, change bot key in appsettings.json:
```sh
"Token": "Your key"
```

Set appropriate database name:
```
"DefaultConnection": "Data Source=.\\SQLEXPRESS;Initial Catalog=**НазваБД**;Integrated Security=True;MultipleActiveResultSets=True;"
```

And paste the previous copied Url address, it should end with "/":
```
"Url": "https://6e4b-87-196-72-94.eu.ngrok.io/"
```

Set bot webhook by entering the following into the browser's address bar:
```
https://api.telegram.org/bot**BotKey**/setwebhook?url=**ngrok.exeURL**/api/message/update
```

In Visual Studio 2022, choose View -> Other Windows -> Package Manager Console. Into the opened console enter the following command:
```
update-database
```

Start the project by clicking the "IIS Express" button
