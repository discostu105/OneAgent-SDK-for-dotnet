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

#pragma warning disable CS1591
namespace Dynatrace.OneAgent.Sdk.Api.Enums
{
    /// <summary>
    /// Encapsulates all well-known messaging systems.
    /// <see cref="IOneAgentSdk.CreateMessagingSystemInfo"/>
    /// Using these constants ensures that services captured by OneAgentSDK are handled the same way as traced via built-in sensors.
    /// </summary>
    public static class MessageSystemVendor
    {
        public static string HORNETQ => "HornetQ";
        public static string ACTIVE_MQ => "ActiveMQ";
        public static string RABBIT_MQ => "RabbitMQ";
        public static string ARTEMIS => "Artemis";
        public static string WEBSPHERE => "WebSphere";
        public static string MQSERIES_JMS => "MQSeries JMS";
        public static string MQSERIES => "MQSeries";
        public static string TIBCO => "Tibco";
        public static string KAFKA => "Apache Kafka";
    }
}
#pragma warning restore CS1591
