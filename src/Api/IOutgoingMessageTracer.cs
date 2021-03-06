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

namespace Dynatrace.OneAgent.Sdk.Api
{
    /// <summary>
    /// Tracer used for tracing an outgoing message.
    /// </summary>
    public interface IOutgoingMessageTracer : ITracer, IOutgoingTaggable
    {
        /// <summary>
        /// Adds optional information about a traced message: message id provided by messaging system.
        /// </summary>
        /// <param name="vendorMessageId">the messageId</param>
        void SetVendorMessageId(string vendorMessageId);

        /// <summary>
        /// Adds optional information about a traced message: correlation id used by messaging system.
        /// </summary>
        /// <param name="correlationId">correlationId</param>
        void SetCorrelationId(string correlationId);
    }
}
