**RAIDAChatNode**
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
>     "nickName": "String"
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
>    "id": "GUID",
>    "name": "String" 
>}
**_Possible response errors:_**

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
>"data": { }
>
>For other users:
>"data": {
>    "callFunction": "AddMemberInGroup",
>    "id": "GUID",
>    "name": "String" 
>}
**_Possible response errors:_**

1. User is not found
1. Group is not found
1. This user already consists in this group 
1. This group is closed
---

**_Description:_** Sending message for the server.\
**_Request to Chat service:_**

>```JSON
>{
>  "execFun": "SendMsg",
>  "data": {
>     "dialogId": "GUID",   
>     "textMsg": "String",
>     "guidMsg": "GUID",
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
>    "messages":[{
>       "guidMsg": "GUID",
>       "textMsg": "String",
>       "curFrg": "Integer",
>       "totalFrg": "Integer",
>       "sendTime": "Long",
>       "senderName": "String"
>     }]
>}
**_Possible response errors:_**
>
1. Dialog is not found

---
**_Description:_** Request for obtaining all messages from the server.\
**_Request to Chat service:_**

>```JSON
>{
>  "execFun": "GetMsg",
>  "data": { }
>}

**_Return from Chat service:_**
>```
>"data": [
>   {
>    "dialogId": "GUID",
>    "groupName": "String",
>    "messages":[{
>       "guidMsg": "GUID",
>       "textMsg": "String",
>       "curFrg": "Integer",
>       "totalFrg": "Integer",
>       "sendTime": "Long",
>       "senderName": "String"
>    }]
>   },
>   {dialogId ...}
>]

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

1. Планирую добавить функцию для изменения статуса диалога, можно будет делать приватный диалог, т.е нельзя будет добавлять новых пользователей. Почти как oneToOne только можно будет менять этот статус.
1. Придумаю в каком месте и как отправлять информацию о статсусе пользователя в чате, онлайн или оффлайн
1. Дальше сделаю для управления организациями

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
