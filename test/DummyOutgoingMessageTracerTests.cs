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
using Xunit;

namespace Dynatrace.OneAgent.Sdk.Test
{
    /// <summary>
    /// See <see cref="DummyTracerTestBase{T}"/>
    /// </summary>
    public class DummyOutgoingMessageTracerTests : DummyTracerTestBase<IOutgoingMessageTracer>
    {
        readonly IMessagingSystemInfo messagingSystemInfo = OneAgentSdk
            .CreateMessagingSystemInfo("vendor", "destination", MessageDestinationType.QUEUE, ChannelType.OTHER, null);

        protected override IOutgoingMessageTracer CreateTracer() => OneAgentSdk.TraceOutgoingMessage(messagingSystemInfo);

        protected override void ExecuteTracerSpecificCalls(IOutgoingMessageTracer tracer)
        {
            tracer.SetCorrelationId("correlation-id");
            tracer.SetVendorMessageId("message-id");
            tracer.SetCorrelationId("");
            tracer.SetVendorMessageId("");
            tracer.SetCorrelationId(null);
            tracer.SetVendorMessageId(null);
        }

        [Fact]
        private void CreateInfoNullValues()
        {
            Assert.NotNull(OneAgentSdk.CreateMessagingSystemInfo(null, null, MessageDestinationType.TOPIC, ChannelType.TCP_IP, null));
        }

        [Fact]
        private void TraceNullInfo()
        {
            Assert.NotNull(OneAgentSdk.TraceOutgoingMessage(null));
        }
    }
}
