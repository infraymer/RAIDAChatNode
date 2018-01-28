using NetSocket.Sockets;
using RAIDAChatNode.DTO;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System;
using RAIDAChatNode.Reflections;

namespace RAIDAChatNode.SocketService
{
    public class NodeSocket : SocketServiceBase
    {

        private List<AuthSocketInfo> mClients = new List<AuthSocketInfo>();

        public override void OnClientClosed(IClient client)
        {
            if (mClients.Any(it => it.client == client))
            {
                mClients.Remove(mClients.First(it => it.client == client));
            }
            //base.OnClientClosed(client);
        }

        
        /// <summary>
        /// Received and validation input message
        /// </summary>
        /// <param name="fromClient">Is who send message</param>
        /// <param name="message">Input message</param>
        public override void OnMessageReceived(IClient fromClient, string message)
        {
            // base.OnMessageReceived(fromClient, message);

            InputSocketDataInfo socketMessage;

            try
            {
                socketMessage = JsonConvert.DeserializeObject<InputSocketDataInfo>(message);
            }
            catch
            {
                OutputSocketMessage outputSocket = new OutputSocketMessage("DeserializeObject error:",
                                                                            false,
                                                                            $"Input message: '{message}' is not valid",
                                                                            new { });

                Console.WriteLine($"{JsonConvert.SerializeObject(outputSocket)}");
                SendMessage(fromClient, outputSocket);
                return;
            }

            NextProcessing(fromClient, socketMessage);
        }


        private void NextProcessing(IClient fromClient, InputSocketDataInfo inputObject)
        {
            OutputSocketMessage outputSocket;

            Type findClass = Type.GetType($"RAIDAChatNode.Reflections.{inputObject.execFun}", false, true);
            if (findClass != null)
            {
                Console.WriteLine($"{findClass.ToString()} ; {(typeof(Authorization)).ToString()}");

                if (findClass.Equals(typeof(Authorization)))
                {
                    AuthSocketInfo info = new Authorization().Execute(inputObject.data);
                    if (info.auth)
                    {
                        info.client = fromClient;
                        mClients.Add(info);
                        outputSocket = new OutputSocketMessage(inputObject.execFun, true, "", new { info.nickName });

                        Console.WriteLine($"{JsonConvert.SerializeObject(outputSocket)}");
                        SendMessage(fromClient, outputSocket);
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Invalid login or password");
                        outputSocket = new OutputSocketMessage(inputObject.execFun,
                                                                false,
                                                                "Invalid login or password",
                                                                new { } );
                        SendMessage(fromClient, outputSocket);
                        return;
                    }
                }
                else {
                    if (mClients.Any(it => it.client == fromClient) || findClass.Equals(typeof(Registration)))
                    {
                        IReflectionActions execClass = (IReflectionActions)Activator.CreateInstance(findClass);
                        AuthSocketInfo currentUser = mClients.FirstOrDefault(it => it.client == fromClient);
                        OutputSocketMessageWithUsers response = execClass.Execute(inputObject.data, currentUser?.login);

                        Console.WriteLine($"{JsonConvert.SerializeObject(response.msgForOwner)}");
                        SendMessage(fromClient, response.msgForOwner);

                        if (response.msgForOwner.success)
                        {
                            Action<AuthSocketInfo> action = delegate (AuthSocketInfo s) { SendMessage(s.client, response.msgForOther); };
                            mClients.Where(it => response.forUserLogin.Equals(it.login)).ToList().ForEach(action);
                        }  
                    }
                    else
                    {
                        Console.WriteLine("You are not authorized. To continue working you need to login.");

                        outputSocket = new OutputSocketMessage(inputObject.execFun,
                                                                false,
                                                                "You are not authorized. To continue working you need to login.",
                                                                new { } );
                        SendMessage(fromClient, outputSocket);
                        return;
                    }
                }
            }
            else
            {
                Console.WriteLine($"Function: '{inputObject.execFun}' not found");

                outputSocket = new OutputSocketMessage(inputObject.execFun,
                                    false,
                                    $"Function: '{inputObject.execFun}' not found", 
                                    new { }
                                );
                SendMessage(fromClient, outputSocket);
                return;
            }

        }

        //Recommended to use
        private async void SendMessage(IClient toClient, OutputSocketMessage message) {
            await SendClientAsync(toClient, JsonConvert.SerializeObject(message));
        }

        private async void SendMessage(IClient toClient, object message)
        {
            await SendClientAsync(toClient, JsonConvert.SerializeObject(message));
        }


        //public override void OnClientInitialized(IClient client)
        //{

        //    long sec = DateTimeOffset.Now.ToUnixTimeSeconds();

        //    Console.WriteLine(sec.ToString());
        //    Console.WriteLine(DateTimeOffset.FromUnixTimeSeconds(sec).ToString("dd.MM.yyyy HH:mm:ss"));
        //    Console.WriteLine(DateTimeOffset.FromUnixTimeSeconds(sec*100).ToString("dd.MM.yyyy HH:mm:ss"));
        //    //base.OnClientInitialized(client);
        //}
        //public override void OnMessageSent(IClient toClient, string message, IClient fromClient)
        //{
        //    base.OnMessageSent(toClient, message, fromClient);
        //}
    }
}
