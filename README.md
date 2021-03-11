# Password Manager
## Русский
### Требования
-  [.NET Framework 4.7](https://www.microsoft.com/ru-RU/download/details.aspx?id=55167)
### Зависимости
**Библиотеки идут вместе с программой**
-  [Newtonsoft.Json v12.0.3](https://www.nuget.org/packages/Newtonsoft.Json/)
-  [SergeyRegistryExtension v1.0.0.0](https://github.com/Hlebushek2020/SergeyRegistryExtension)
### Описание
**Не требует установки! Распаковать архив и использовать**  
**Скриншоты можно просмотреть в папке "DATA/Screen" проекта.**

Хранит ваши пароли и прочие учетные данные в зашифрованном виде, длина пароля (ключа) может достигать 32 символов. Для программы можно настроить уникальный дизайн. Имеется управление горячими клавишами. Так же программа помимо стандартных языков (EN, RU) поддерживает другие (внедряются пользователем).

**Ваши личные данные не передаются третьим лицам!**
### Горячие клавиши
#### Главное окно
Клавиша|Описание
---|---
Ctrl + E|Открыть редактор
Esc|Закрыть программу при этом выдать подтверждение закрытия, если есть несохраненные изменения
#### Окно редактора
Клавиша|Описание
---|---
Ctrl + S|Сохранить
Ctrl + O|Открыть
Ctrl + A|Добавить раздел
Ctrl + E|Редактировать раздел (или двойной клик по элементу)
Ctrl + D, Delete|Удалить раздел
Esc|Закрыть окно
#### Окно редактора раздела
Клавиша|Описание
---|---
Ctrl + P|Сгенерировать пароль (доступно при редактировании или добавлении записи)
Ctrl + A|Добавить запись
Ctrl + E|Редактировать запись (или двойной клик по элементу)
Ctrl + D, Delete|Удалить запись
Esc|Закрыть окно и запросить сохранение изменений при необходимости
#### Окно шифрования / дешифрования
Клавиша|Описание
---|---
Enter|Продолжить
Ctrl + P|Сгенерировать пароль (доступно при шифровании)
Esc|Закрыть окно
#### Окно настроек
Клавиша|Описание
---|---
Ctrl + R|Сбросить настройки до настроек по умолчанию
Esc|Закрыть окно и запросить сохранение изменений с их применением при необходимости
### Скрытые настройки
Файл с настройками приложения можно найти по пути: "Документы\SergeyGovorunov\PasswordManager\settings.json".  
**Если настройки по умолчанию не изменялись, то файла не будет!**
Настройка|Описание
---|---
ClearProgramResourceFolder|Удаление ненужных файлов из папки с ресурсами при запуске программы. Значение: true (Да) или false (Нет)
MenuBackgroundMouseOver|Цвет кнопки меню при наведении. Шестнадцатеричное представление цвета в формате ARGB.
MenuBackgroundPressed|Цвет кнопки меню при нажатии. Шестнадцатеричное представление цвета в формате ARGB.


## English
### Requirements
-  [.NET Framework 4.7](https://www.microsoft.com/ru-RU/download/details.aspx?id=55167)
### Dependencies
**Libraries come with the program**
-  [Newtonsoft.Json v12.0.3](https://www.nuget.org/packages/Newtonsoft.Json/)
-  [SergeyRegistryExtension v1.0.0.0](https://github.com/Hlebushek2020/SergeyRegistryExtension)
### Description
**No installation required! Unpack the archive and use**  
**Screenshots can be viewed in the project's "DATA/Screen" folder.**

Stores your passwords and other credentials in encrypted form, the length of the password (key) can be up to 32 characters. A completely unique design can be customized for the program. There is a hotkey control. In addition to standard languages (EN, RU), the program also supports others (implemented by the user).

**Your personal data is not passed on to third parties!**
### Hotkeys
#### Main window
Key|Description
---|---
Ctrl + E|Open editor
Esc|Close the program and issue a confirmation of closing if there are unsaved changes
#### Editor window
Key|Description
---|---
Ctrl + S|Save
Ctrl + O|Open
Ctrl + A|Add section
Ctrl + E|Edit section (or double-click on an item)
Ctrl + D, Delete|Delete section
Esc|Close a window
#### Section editor window
Key|Description
---|---
Ctrl + P|Generate password (available when editing or adding an entry)
Ctrl + A|Add record
Ctrl + E|Edit record (or double-click on an item)
Ctrl + D, Delete|Delete record
Esc|Close the window and ask to save changes if necessary
#### Encryption / Decryption Window
Key|Description
---|---
Enter|Continue
Ctrl + P|Generate password (available with encryption)
Esc|Close a window
#### Settings window
Key|Description
---|---
Ctrl + R|Reset settings to default settings
Esc|Close the window and ask to save changes and apply them if necessary
### Hidden settings
You can find the file with the app settings on the path: "Documents\SergeyGovorunov\PasswordManager\settings.json".  
**If you have not changed the default settings, the file will not be.**
Setting|Description
---|---
ClearProgramResourceFolder|Removing unnecessary files from the folder with resources when starting the program. Value: true (Yes) or false (No)
MenuBackgroundMouseOver|The color of the menu button when you hover. Hexadecimal representation of color in ARGB format.
MenuBackgroundPressed|The color of the menu button when pressed. Hexadecimal representation of color in ARGB format.