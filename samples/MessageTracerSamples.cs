//
// Copyright 2019 Dynatrace LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using Dynatrace.OneAgent.Sdk.Api;
using Dynatrace.OneAgent.Sdk.Api.Enums;
using Dynatrace.OneAgent.Sdk.Api.Infos;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Dynatrace.OneAgent.Sdk.Sample
{
    static class MessageTracerSamples
    {
        /// <summary>
        /// Produces a message (traced with the <see cref="IOutgoingMessageTracer"/>)
        /// The Dynatrace tag is transported along with the message
        /// </summary>
        public static void ProducerExample()
        {
            string serverEndpoint = "messageserver.example.com:1234";
            string topic = "my-topic";
            IMessagingSystemInfo messagingSystemInfo = SampleApplication.OneAgentSdk
                .CreateMessagingSystemInfo(MessageSystemVendor.KAFKA, topic, MessageDestinationType.TOPIC, ChannelType.TCP_IP, serverEndpoint);

            IOutgoingMessageTracer outgoingMessageTracer = SampleApplication.OneAgentSdk.TraceOutgoingMessage(messagingSystemInfo);

            outgoingMessageTracer.Start();
            try
            {
                Message message = new Message
                {
                    CorrelationId = "my-correlation-id-1234" // optional, determined by application
                };

                // transport the Dynatrace tag along with the message to allow the outgoing message tracer to be linked
                // together with the message processing tracer on the receiving side
                message.Headers[OneAgentSdkConstants.DYNATRACE_MESSAGE_PROPERTYNAME] = outgoingMessageTracer.GetDynatraceByteTag();

                SendResult result = MyMessagingSystem.SendMessage(message);

                outgoingMessageTracer.SetCorrelationId(message.CorrelationId);    // optional
                outgoingMessageTracer.SetVendorMessageId(result.VendorMessageId); // optional
            }
            catch (Exception e)
            {
                outgoingMessageTracer.Error(e);
                // handle or rethrow
                throw e;
            }
            finally
            {
                outgoingMessageTracer.End();
            }
        }

        /// <summary>
        /// Consumes a message on a daemon thread using a blocking receive call
        /// (traced with the <see cref="IIncomingMessageReceiveTracer"/> and <see cref="IIncomingMessageProcessTracer"/>)
        /// The Dynatrace tag is transported along with the message
        /// </summary>
        public static void ConsumerDaemonExample()
        {
            string serverEndpoint = "messageserver.example.com:1234";
            string topic = "my-topic";
            IMessagingSystemInfo messagingSystemInfo = SampleApplication.OneAgentSdk
                .CreateMessagingSystemInfo(MessageSystemVendor.KAFKA, topic, MessageDestinationType.TOPIC, ChannelType.TCP_IP, serverEndpoint);

            IIncomingMessageReceiveTracer receiveTracer = SampleApplication.OneAgentSdk.TraceIncomingMessageReceive(messagingSystemInfo);

            receiveTracer.Start();
            try
            {
                // blocking call until message is available:
                ReceiveResult receiveResult = MyMessagingSystem.ReceiveMessage();
                Message message = receiveResult.Message;

                IIncomingMessageProcessTracer processTracer = SampleApplication.OneAgentSdk.TraceIncomingMessageProcess(messagingSystemInfo);

                // retrieve Dynatrace tag created using the outgoing message tracer to link both sides together:
                if (message.Headers.ContainsKey(OneAgentSdkConstants.DYNATRACE_MESSAGE_PROPERTYNAME))
                {
                    processTracer.SetDynatraceByteTag(message.Headers[OneAgentSdkConstants.DYNATRACE_MESSAGE_PROPERTYNAME]);
                }

                // start processing:
                processTracer.Start();
                processTracer.SetCorrelationId(message.CorrelationId);           // optional
                processTracer.SetVendorMessageId(receiveResult.VendorMessageId); // optional
                try
                {
                    ProcessMessage(message); // do the work ...
                }
                catch (Exception e)
                {
                    processTracer.Error(e);
                    // handle or rethrow
                    throw e;
                }
                finally
                {
                    processTracer.End();
                }
            }
            catch (Exception e)
            {
                receiveTracer.Error(e);
                // handle or rethrow
                throw e;
            }
            finally
            {
                receiveTracer.End();
            }
        }

        /// <summary>
        /// Event handler for consuming a message
        /// (traced with the <see cref="IIncomingMessageProcessTracer"/>)
        /// The Dynatrace tag is transported along with the message
        /// </summary>
        private static void OnMessageReceived(ReceiveResult receiveResult)
        {
            string serverEndpoint = "messageserver.example.com:1234";
            string topic = "my-topic";
            IMessagingSystemInfo messagingSystemInfo = SampleApplication.OneAgentSdk
                .CreateMessagingSystemInfo("MyCustomMessagingSystem", topic, MessageDestinationType.TOPIC, ChannelType.TCP_IP, serverEndpoint);

            Message message = receiveResult.Message;

            IIncomingMessageProcessTracer processTracer = SampleApplication.OneAgentSdk.TraceIncomingMessageProcess(messagingSystemInfo);

            // retrieve Dynatrace tag created using the outgoing message tracer to link both sides together:
            if (message.Headers.ContainsKey(OneAgentSdkConstants.DYNATRACE_MESSAGE_PROPERTYNAME))
            {
                processTracer.SetDynatraceByteTag(message.Headers[OneAgentSdkConstants.DYNATRACE_MESSAGE_PROPERTYNAME]);
            }

            // start processing:
            processTracer.Start();
            processTracer.SetCorrelationId(message.CorrelationId);           // optional
            processTracer.SetVendorMessageId(receiveResult.VendorMessageId); // optional
            try
            {
                ProcessMessage(message); // do the work ...
            }
            catch (Exception e)
            {
                processTracer.Error(e);
                // handle or rethrow
                throw e;
            }
            finally
            {
                processTracer.End();
            }
        }

        /// <summary>
        /// Produces a message (traced with the <see cref="IOutgoingMessageTracer"/>) and consumes it
        /// (traced with the <see cref="IIncomingMessageReceiveTracer"/> and <see cref="IIncomingMessageProcessTracer"/>)
        /// The Dynatrace tag is transported along with the message
        /// </summary>
        public static void ProducerConsumerExample()
        {
            string serverEndpoint = "messageserver.example.com:1234";
            string topic = "my-topic";
            IMessagingSystemInfo messagingSystemInfo = SampleApplication.OneAgentSdk
                .CreateMessagingSystemInfo(MessageSystemVendor.KAFKA, topic, MessageDestinationType.TOPIC, ChannelType.TCP_IP, serverEndpoint);

            IOutgoingMessageTracer outgoingTracer = SampleApplication.OneAgentSdk.TraceOutgoingMessage(messagingSystemInfo);

            outgoingTracer.Start();
            try
            {
                Message message = new Message
                {
                    CorrelationId = "my-correlation-id-1234" // optional, determined by application
                };

                // transport the Dynatrace tag along with the message to allow the outgoing message tracer to be linked
                // together with the message processing tracer on the receiving side
                message.Headers[OneAgentSdkConstants.DYNATRACE_MESSAGE_PROPERTYNAME] = outgoingTracer.GetDynatraceByteTag();

                SendResult result = MyMessagingSystem.SendMessage(message);

                outgoingTracer.SetCorrelationId(message.CorrelationId);    // optional
                outgoingTracer.SetVendorMessageId(result.VendorMessageId); // optional
            }
            catch (Exception e)
            {
                outgoingTracer.Error(e);
                // handle or rethrow
                throw e;
            }
            finally
            {
                outgoingTracer.End();
            }

            // represents server side processing
            Thread server = new Thread(() =>
            {
                IIncomingMessageReceiveTracer receiveTracer = SampleApplication.OneAgentSdk.TraceIncomingMessageReceive(messagingSystemInfo);

                receiveTracer.Start();
                try
                {
                    // blocking call until message is available:
                    ReceiveResult receiveResult = MyMessagingSystem.ReceiveMessage();
                    Message message = receiveResult.Message;

                    IIncomingMessageProcessTracer processTracer = SampleApplication.OneAgentSdk.TraceIncomingMessageProcess(messagingSystemInfo);

                    // retrieve Dynatrace tag created using the outgoing message tracer to link both sides together:
                    if (message.Headers.ContainsKey(OneAgentSdkConstants.DYNATRACE_MESSAGE_PROPERTYNAME))
                    {
                        processTracer.SetDynatraceByteTag(message.Headers[OneAgentSdkConstants.DYNATRACE_MESSAGE_PROPERTYNAME]);
                    }

                    // start processing:
                    processTracer.Start();
                    processTracer.SetCorrelationId(message.CorrelationId);           // optional
                    processTracer.SetVendorMessageId(receiveResult.VendorMessageId); // optional
                    try
                    {
                        ProcessMessage(message); // do the work ...
                    }
                    catch (Exception e)
                    {
                        processTracer.Error(e);
                        // handle or rethrow
                        throw e;
                    }
                    finally
                    {
                        processTracer.End();
                    }
                }
                catch (Exception e)
                {
                    receiveTracer.Error(e);
                    // handle or rethrow
                    throw e;
                }
                finally
                {
                    receiveTracer.End();
                }
            });
            server.Start();
            server.Join();
        }

        #region helper classes for demonstration

        class Message
        {
            public Dictionary<string, byte[]> Headers { get; } = new Dictionary<string, byte[]>();
            public string CorrelationId { get; set; }
            public object Value { get; set; }
        }

        class SendResult
        {
            public string VendorMessageId { get; set; }
        }

        class ReceiveResult
        {
            public string VendorMessageId { get; set; }
            public Message Message { get; set; }
        }

        static class MyMessagingSystem
        {
            private static Message lastSentMessage;
            public static SendResult SendMessage(Message message)
            {
                Thread.Sleep(50);
                lastSentMessage = message;
                return new SendResult
                {
                    VendorMessageId = "my-vendor-message-id-1234"
                };
            }
            public static ReceiveResult ReceiveMessage()
            {
                Thread.Sleep(100);
                return new ReceiveResult
                {
                    VendorMessageId = "my-vendor-message-id-1234",
                    Message = lastSentMessage ?? new Message()
                };
            }
        }

        private static void ProcessMessage(object content)
        {
            Thread.Sleep(300);
        }
        #endregion
    }
}
