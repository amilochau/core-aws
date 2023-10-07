/*
 * Copyright Amazon.com, Inc. or its affiliates. All Rights Reserved.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License").
 * You may not use this file except in compliance with the License.
 * A copy of the License is located at
 * 
 *  http://aws.amazon.com/apache2.0
 * 
 * or in the "license" file accompanying this file. This file is distributed
 * on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
 * express or implied. See the License for the specific language governing
 * permissions and limitations under the License.
 */

/*
 * Do not modify this file. This file is generated from the sesv2-2019-09-27.normal.json service model.
 */


using System;
using System.Threading;
using System.Threading.Tasks;

using Amazon.Runtime;
using Amazon.SimpleEmailV2.Model;

namespace Amazon.SimpleEmailV2
{
    /// <summary>
    /// Interface for accessing SimpleEmailServiceV2
    ///
    /// Amazon SES API v2 
    /// <para>
    ///  <a href="http://aws.amazon.com/ses">Amazon SES</a> is an Amazon Web Services service
    /// that you can use to send email messages to your customers.
    /// </para>
    ///  
    /// <para>
    /// If you're new to Amazon SES API v2, you might find it helpful to review the <a href="https://docs.aws.amazon.com/ses/latest/DeveloperGuide/">Amazon
    /// Simple Email Service Developer Guide</a>. The <i>Amazon SES Developer Guide</i> provides
    /// information and code samples that demonstrate how to use Amazon SES API v2 features
    /// programmatically.
    /// </para>
    /// </summary>
    public partial interface IAmazonSimpleEmailServiceV2 : IAmazonService, IDisposable
    {
        /// <summary>
        /// Sends an email message. You can use the Amazon SES API v2 to send the following types
        /// of messages:
        /// 
        ///  <ul> <li> 
        /// <para>
        ///  <b>Simple</b> – A standard email message. When you create this type of message, you
        /// specify the sender, the recipient, and the message body, and Amazon SES assembles
        /// the message for you.
        /// </para>
        ///  </li> <li> 
        /// <para>
        ///  <b>Raw</b> – A raw, MIME-formatted email message. When you send this type of email,
        /// you have to specify all of the message headers, as well as the message body. You can
        /// use this message type to send messages that contain attachments. The message that
        /// you specify has to be a valid MIME message.
        /// </para>
        ///  </li> <li> 
        /// <para>
        ///  <b>Templated</b> – A message that contains personalization tags. When you send this
        /// type of email, Amazon SES API v2 automatically replaces the tags with values that
        /// you specify.
        /// </para>
        ///  </li> </ul>
        /// </summary>
        /// <param name="request">Container for the necessary parameters to execute the SendEmail service method.</param>
        /// <param name="cancellationToken">
        ///     A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        /// 
        /// <returns>The response from the SendEmail service method, as returned by SimpleEmailServiceV2.</returns>
        /// <exception cref="Amazon.SimpleEmailV2.Model.AccountSuspendedException">
        /// The message can't be sent because the account's ability to send email has been permanently
        /// restricted.
        /// </exception>
        /// <exception cref="Amazon.SimpleEmailV2.Model.BadRequestException">
        /// The input you provided is invalid.
        /// </exception>
        /// <exception cref="Amazon.SimpleEmailV2.Model.LimitExceededException">
        /// There are too many instances of the specified resource type.
        /// </exception>
        /// <exception cref="Amazon.SimpleEmailV2.Model.MailFromDomainNotVerifiedException">
        /// The message can't be sent because the sending domain isn't verified.
        /// </exception>
        /// <exception cref="Amazon.SimpleEmailV2.Model.MessageRejectedException">
        /// The message can't be sent because it contains invalid content.
        /// </exception>
        /// <exception cref="Amazon.SimpleEmailV2.Model.NotFoundException">
        /// The resource you attempted to access doesn't exist.
        /// </exception>
        /// <exception cref="Amazon.SimpleEmailV2.Model.SendingPausedException">
        /// The message can't be sent because the account's ability to send email is currently
        /// paused.
        /// </exception>
        /// <exception cref="Amazon.SimpleEmailV2.Model.TooManyRequestsException">
        /// Too many requests have been made to the operation.
        /// </exception>
        /// <seealso href="http://docs.aws.amazon.com/goto/WebAPI/sesv2-2019-09-27/SendEmail">REST API Reference for SendEmail Operation</seealso>
        Task<SendEmailResponse> SendEmailAsync(SendEmailRequest request, CancellationToken cancellationToken);
    }
}