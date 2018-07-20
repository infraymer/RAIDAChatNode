**RAIDAChatNode**

Нода серверной части RAIDAChat.

---
## Конфигурирование

Все настройки для ноды прописываются в файле <i>appsettings.json</i>

Доступные найстройки:
>```JSON
>{
>    "Connection":{
>      "Addr" : "192.168.0.102",                         //Адрес используемого интерфейса (ОБЯЗАТЕЛЬНО)
>      "Port" : 49001,                                 //Порт WebSocketa (ОБЯЗАТЕЛЬНО)
>      "SSL" : {                                       //Настройки SSL сертификата. Указать можно либо по серийному номеру либо путь к файлу и пароль от него
>        "IP": "192.168.0.102",
>		 "SerialNumb": "‎17 01 ee 13 52 a5 36 99 47 c3 93 60 08 74 ed 70", //Сертификат должен храниться в: Локальный компьютер -> Доверенные корневые центры сертификации 
>		        OR
>        "PathFile": "D:\\cert.pfx",
>        "Password": "Qwe123"
>      }
>    },
>    "DataBase": {                                     //Настройки БД, если не указывать, то используется SQLite. У пользователя бд должны быть права на создание таблиц
>      "NameDB" : "SQLite",                            //Поддерживаются: SQLite || MSSQL || MySQL
>      "ConnectionString" : "Filename=RAIDAChat.db"    //Если указывается строка подключения, то аутентификаонные данные не указываюся, они запрашиваются в процессе запуска ноды
>      --------------------------------------------
>      Пример для MSSQL:
>      "NameDB" : "MSSQL",
>      "ConnectionString" : "data source=SEREGA\\SQLSERV2016;initial catalog=CloudChatPortable;persist security info=True;multipleactiveresultsets=True"
>      --------------------------------------------
>      Пример для MySQL:
>      "NameDB" : "MySQL",
>      "ConnectionString" : "server=localhost;port=3306;database=raidachat;"
>    },
>    "TransactionRollback" : 60,                       //Время, в течении которого можно отменить действие (с). По умолчанию: 1 минута. Доступные значения (0, 3600)
>    "LimitMsgForDialog" : 100,                        //Количество сообщений, хранимых для одного диалога. По умолчанию: 100 сообщений. Доступные значения(1, int.MaxValue)
>    "CleanUpTimer" : 3600,                            //Время очистки старой информации о транзакциях и сообщениях По умолчанию: 1 час. Доступные значения(60, 86400)
>    "SyncWorldTime" : 86400,                          //Период синронизации внутреннего таймеоа (Один раз в n часов). По умолчанию: 24 часа. Доступные значения(1800, 86400)
>    "TrustedServers":[                                //Список адресов доверенных серверов (ОБЯЗАТЕЛЬНО)
>      "http://192.168.0.102:49002"                    
>    ]
>}

Необязательные параметры можно не прописывать, при этом будут указаны параметры по умолчанию.

В папке <i>bin\Release\netcoreapp2.0</i> доступны скомпилированные ноды. 
Для запуска необходимо проверить настройки и выполнить файл <i>RAIDAChatNode</i>


---

Data transfer will be carried out through **_WebSocket._** The following **_template_** of a line for data transfer on the _server_ is used:
>```JSON
>{
>  "executeFunction": "fun_name",
>  "data": {
>     "Certain property set":""
>     }
>}

For transmission from the server on the _client_ to use such template:
>```JSON
>{
>  "callFunction": "fun_name_executed",
>  "success": "true_or_false",
>  "msgError": "Exception_text",  
>  "data": {
>     "Certain property set":""
>     }
>}
---

## API:

**_Description:_** Registration of the new user.\
Идентификация и обращение к польователям чата только по логину, он уникальный
**_Request to Chat service:_**

>```JSON
>{
>  "execFun": "Registration",
>  "data": {
>     "login": "String",
>     "password": "String",
>     "nickName": "String",
>     "transactionId": "GUID"
>     } 
>}
**_Return from Chat service:_**

>```
>"data": { }
**_Possible response errors:_**

1. This login already exists
---

**_Description:_** Authorization of the user in a chat.\
**_Request to Chat service:_**

>```JSON
>{
>  "execFun": "Authorization",
>  "data": {
>     "login": "String",
>     "password": "String"
>     } 
>}
**_Return from Chat service:_**

>```
>"data": {
>     "nickName": "String",
>     "login": "String",
>     "img": "String"
>}
**_Possible response errors:_**

1. Invalid login or AN.
---

**_Description:_** Creation of new group.\
Диалог может быть на несколько человек или один-на один. За это отвечает флаг oneToOne: True - может состоять только 2 человека и всё. False - любое количество людей, в такой диалог есть возможность добавлять новых участников.
Если oneToOne = True, то поле name - есть логин пользователя с которым создаётся диалог.
Если oneToOne = False, то поле name - название создаваемого диалога.
**_Request to Chat service:_**
>```JSON
>{
>  "execFun": "CreateDialog",
>  "data": {
>     "name": "String",
>     "publicId": " GUID",
>     "oneToOne": " Boolean",
>     "transactionId": " GUID"
>     }
>}

**_Return from Chat service:_**
>```
>"data": {
>     "id": "GUID",
>     "name": "String",
>     "oneToOne": "Boolean" 
>}
**_Possible response errors:_**

1. Change the publicId
1. This dialog exists
1. User is not found

---
**_Description:_** Adding of the user to group. \
**_Request to Chat service:_**

>```JSON
>{
>  "execFun": "AddMemberInGroup",
>  "data": {
>     "memberLogin": "String",     
>     "groupId": "GUID",
>     "transactionId": " GUID"
>     }
>}
**_Return from Chat service:_**
>```
>For owner:
>"data": { 
>    "itself": "Boolean", (constant - true)
>    "newUser": {
>      "login": "String",
>      "nickName": "String",
>      "photo": "String",
>      "online": "Boolean",
>    },
>    "groupId": "GUID"
>}
>
>For other users:
>"callFunction": "AddMemberInGroup",
>"data": {
>    "id": "GUID",
>    "name": "String", 
>    "oneToOne": "Boolean",
>    "members": [
>      "login": "String"
>    ],
>    "newUser": {
>      "login": "String",
>      "nickName": "String",
>      "photo": "String",
>      "online": "Boolean",
>    }
>}
**_Possible response errors:_**

1. User is not found
1. Group is not found
1. This user already consists in this group 
1. This group is closed

---
**_Description:_** Change status dialog to private or not private. \
**_Request to Chat service:_**

>```JSON
>{
>  "execFun": "SetDialogPrivate",
>  "data": { 
>     "publicId": "GUID",
>     "privated": " Boolean"
>     }
>}
**_Return from Chat service:_**
>```
>For owner:
>"data": { 
>   "dialogId": "GUID",
>   "privated": "Boolean"
>}

**_Possible response errors:_**

1. This dialog not found or you not owner
---

**_Description:_** Sending message for the server.\
**_Request to Chat service:_**

>```JSON
>{
>  "execFun": "SendMsg",
>  "data": {
>     "dialogId": "GUID",   
>     "textMsg": "String",
>     "msgId": "GUID",
>     "curFrg": "Integer",
>     "totalFrg": "Integer",
>     "transactionId": " GUID",
>     "deathDate": "Long"
>     }
>}
**_Properties:_**
- *dialogId* - Id диалога
- *textMsg* - Непосредственно текст сообщения
- *guidMsg* - Уникальный идентификатор сообщения 
- *curFrg* - Номер части сообщения
- *totalFrg* - Общее число частей на которое было разбито сообщение
- *deathDate* - Время по истечении которого сообщение будет не доступно
>
**_Return from Chat service:_**
>```
>"data": {
>    "dialogId": "GUID",
>    "groupName": "String",
>    "oneToOne": "Boolean",
>    "priavated": "Boolean",
>    "members": [
>       "login": "String"
>    ],
>    "messages":[{
>       "guidMsg": "GUID",
>       "textMsg": "String",
>       "curFrg": "Integer",
>       "totalFrg": "Integer",
>       "sendTime": "Long",
>       "senderName": "String",
>       "senderLogin": "String",
>     }]
>}
**_Possible response errors:_**
>
1. Dialog is not found

---
**_Description:_** Request for getting messages from the server.\
**_Request to Chat service:_**

>```JSON
>{
>  "execFun": "getDialogs",
>  "data": {
>    "msgCount": "Integer"
>  }
>}
OR
>```JSON
>{
>  "execFun": "getMsg",
>  "data": {
>    "dialogId": "GUID",
>    "msgCount": "Integer",
>    "offset": "Integer"
>  }
>}

**_Return from Chat service:_**
>```
>"data": [
>   {
>    "dialogId": "GUID",
>    "groupName": "String",
>    "oneToOne": "Boolean",
>    "priavated": "Boolean",
>    "members": [
>       "login": "String"
>    ],
>    "messages":[{
>       "guidMsg": "GUID",
>       "textMsg": "String",
>       "curFrg": "Integer",
>       "totalFrg": "Integer",
>       "sendTime": "Long",
>       "senderName": "String"
>       "senderLogin": "String"
>    }]
>   },
>   {dialogId ...}
>]

---
**_Description:_** Get information about login user. \
**_Request to Chat service:_**

>```JSON
>{
>  "execFun": "GetUserInfo",
>  "data": { }
>}
**_Return from Chat service:_**
>```
>"data": { 
>   "login": "String",
>   "nickName": "String",
>   "photo": "String",
>   "online": "Boolean"
>}

---
**_Description:_** Change information about user. \
**_Request to Chat service:_**

>```JSON
>{
>  "execFun": "ChangeUserInfo",
>  "data": {
>    "name":"String",
>    "photo":"String",
>    "about":"String",
>    "changePass":"Boolean",
>    "oldPass":"String",
>    "newPass":"String"
>  }
>}
**_Return from Chat service:_**
>```
>"data": { 
>   "itself": "Boolean",
>   "user": {
>     "login": "String",
>     "nickName": "String",
>     "photo": "String",
>     "online": "Boolean"
>   }
>}

---
**_Description:_** Отменить изменение в базе. Данное действие возможно в течении 60 секунд, потом транзакция блокируется. Пока не работает\
**_Request to Chat service:_**

>```JSON
>{
>  "execFun": "RollbackTransaction",
>  "data": {
>      "transactionId": "GUID"
>    }
>}

**_Return from Chat service:_**
>```
>"data": { }
**_Possible response errors:_**
>
1. This transaction is not found
1. You not owner for this transaction
1. This transaction is blocked

---
**_Description:_** Creation new organization.\
   
   **_Request to Chat service:_**
   >```JSON
   >{
   >  "execFun": "OrganizationCreate",
   >  "data": {
   >     "publicId": " GUID",
   >     "name": "String",
   >     "transactionId": " GUID"
   >     }
   >}
   
   **_Return from Chat service:_**
   >```
   >"data": {
   >    "publicId": "GUID",
   >    "name": "String" 
   >}
   **_Possible response errors:_**
   
   1. Change the publicId
   1. You a have already organization
   
   ---
   **_Description:_** Creation new member in organization.\
   
   **_Request to Chat service:_**
   >```JSON
   >{
   >  "execFun": "OrganizationAddMember",
   >  "data": {
   >     "login": " String",
   >     "password": "String",
   >     "nickName": "String",
   >     "transactionId": " GUID"
   >     }
   >}
   
   **_Return from Chat service:_**
   >```
   >"data": {
   >    "login": "String",
   >    "nickName": "String" 
   >}
   **_Possible response errors:_**
   
   1. You a not consist in organization or a not organization owner
   1. This login already exists
   
   ---
   
##EXAMPLES

Sample Request to Chat service for registration new user
>```JSON
>{
>  "execFun": "Registration",
>  "data": {
>     "login": "myLogin",
>     "password": "99ab7bf3bef54f77b35ce8b5ee8f8260",
>     "nickName": "Anonymus",
>     "transactionId": "50b411ceab24adeb0539de62100646c0"
>     } 
>}
Sample Return from Chat service is success
>```JSON
>{
>  "callFunction":"registration",
>  "success":true,
>  "msgError":"",
>  "data":{}
>}
Sample Return from Chat service is error if user login already exists
>```JSON
>{
>  "callFunction":"registration",
>  "success":false,
>  "msgError":"This login already exists",
>  "data":{}
>}

Sample Request to Chat service for get all user messages
>```JSON
>{
>  "execFun": "getMsg",
>  "data": { }
>}
Sample Return from Chat service is success
Sample Request to Chat service for get all user messages
>```JSON
>{
>  "callFunction": "getMsg",
>  "success": true,
>  "msgError": "",
>  "data":[
>      {
>      "dialogId":"18a0ca06-57de-4fb0-9cdc-86008b2a8ebe",
>      "groupName":"Group",
>      "messages":[{
>          "guidMsg":"01d8133f-a55b-80af-b66c-a63e214c97c0",
>          "textMsg":"Fragment message",
>          "curFrg":6,
>          "totalFrg":10,
>          "sendTime":1517333361,
>          "senderName":"Anon"
>        },
>        {
>          "guidMsg":"09d8133f-a55b-80af-b66c-a63e214c97c5",
>          "textMsg":"Fragment message",
>          "curFrg":6,
>          "totalFrg":10,
>          "sendTime":1517333903,
>          "senderName":"Anonimus"}]
>      },
>      {
>      "dialogId":"64a0ca06-57de-4fb0-9cdc-86008b2a8ebe",
>      "groupName":"Group2",
>      "messages":[]
>    }
>  ]
>}

####Possible errors for all request
Sample Return from Chat service is error if parameters in "data" have invalid data-type
>```JSON
>{
>  "callFunction":"registration",
>  "success":false,
>  "msgError":"Invalid sending data",
>  "data":{}
>}
Sample Return from Chat service is error if user in not authorized
>```JSON
>{
>  "callFunction":"getMsg",
>  "success":false,
>  "msgError":"You are not authorized. To continue working you need to login.",
>  "data":{}
>}
Sample Return from Chat service is error if attempt was to call a non-existent function
>```JSON
>{
>  "callFunction":"getMsgError",
>  "success":false,
>  "msgError":"Function: 'getMsgError' not found",
>  "data":{}
>}
---
Operating procedure with the server:

1. To be connected to a websocket
1. To register (If for the first time)
1. Get authorization
1. Sending any command of the appropriate format
