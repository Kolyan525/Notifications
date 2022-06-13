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
"DefaultConnection": "Data Source=.\\SQLEXPRESS;Initial Catalog=НазваБД;Integrated Security=True;MultipleActiveResultSets=True;"
```

Та в поле Url вставити раніше скопійовану адресу, вона повинна закінчуватись на "/":
```
"Url": "https://6e4b-87-196-72-94.eu.ngrok.io/"
```

У Visual Studio 2022, перейти у View -> Other Windows -> Package Manager Console. У консолі що відкрилась ввести наступну команду:
```
update-database
```

Запустити проект, натиснувши на кнопку "IIS Express"
