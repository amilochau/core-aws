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
 * Do not modify this file. This file is generated from the email-2010-12-01.normal.json service model.
 */


using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Amazon.Runtime;
using Amazon.SimpleEmail.Model;

namespace Amazon.SimpleEmail
{
    /// <summary>
    /// Interface for accessing SimpleEmailService
    ///
    /// Amazon Simple Email Service 
    /// <para>
    ///  This document contains reference information for the <a href="https://aws.amazon.com/ses/">Amazon
    /// Simple Email Service</a> (Amazon SES) API, version 2010-12-01. This document is best
    /// used in conjunction with the <a href="https://docs.aws.amazon.com/ses/latest/DeveloperGuide/Welcome.html">Amazon
    /// SES Developer Guide</a>. 
    /// </para>
    ///  <note> 
    /// <para>
    ///  For a list of Amazon SES endpoints to use in service requests, see <a href="https://docs.aws.amazon.com/ses/latest/DeveloperGuide/regions.html">Regions
    /// and Amazon SES</a> in the <a href="https://docs.aws.amazon.com/ses/latest/DeveloperGuide/Welcome.html">Amazon
    /// SES Developer Guide</a>.
    /// </para>
    ///  </note> 
    /// <para>
    /// This documentation contains reference information related to the following:
    /// </para>
    ///  <ul> <li> 
    /// <para>
    ///  <a href="https://docs.aws.amazon.com/ses/latest/APIReference/API_Operations.html">Amazon
    /// SES API Actions</a> 
    /// </para>
    ///  </li> <li> 
    /// <para>
    ///  <a href="https://docs.aws.amazon.com/ses/latest/APIReference/API_Types.html">Amazon
    /// SES API Data Types</a> 
    /// </para>
    ///  </li> <li> 
    /// <para>
    ///  <a href="https://docs.aws.amazon.com/ses/latest/APIReference/CommonParameters.html">Common
    /// Parameters</a> 
    /// </para>
    ///  </li> <li> 
    /// <para>
    ///  <a href="https://docs.aws.amazon.com/ses/latest/APIReference/CommonErrors.html">Common
    /// Errors</a> 
    /// </para>
    ///  </li> </ul>
    /// </summary>
    public partial interface IAmazonSimpleEmailService : IAmazonService, IDisposable
    {
        /// <summary>
        /// Composes an email message using an email template and immediately queues it for sending.
        /// 
        ///  
        /// <para>
        /// To send email using this operation, your call must meet the following requirements:
        /// </para>
        ///  <ul> <li> 
        /// <para>
        /// The call must refer to an existing email template. You can create email templates
        /// using the <a>CreateTemplate</a> operation.
        /// </para>
        ///  </li> <li> 
        /// <para>
        /// The message must be sent from a verified email address or domain.
        /// </para>
        ///  </li> <li> 
        /// <para>
        /// If your account is still in the Amazon SES sandbox, you may only send to verified
        /// addresses or domains, or to email addresses associated with the Amazon SES Mailbox
        /// Simulator. For more information, see <a href="https://docs.aws.amazon.com/ses/latest/dg/verify-addresses-and-domains.html">Verifying
        /// Email Addresses and Domains</a> in the <i>Amazon SES Developer Guide.</i> 
        /// </para>
        ///  </li> <li> 
        /// <para>
        /// The maximum message size is 10 MB.
        /// </para>
        ///  </li> <li> 
        /// <para>
        /// Calls to the <code>SendTemplatedEmail</code> operation may only include one <code>Destination</code>
        /// parameter. A destination is a set of recipients that receives the same version of
        /// the email. The <code>Destination</code> parameter can include up to 50 recipients,
        /// across the To:, CC: and BCC: fields.
        /// </para>
        ///  </li> <li> 
        /// <para>
        /// The <code>Destination</code> parameter must include at least one recipient email address.
        /// The recipient address can be a To: address, a CC: address, or a BCC: address. If a
        /// recipient email address is invalid (that is, it is not in the format <i>UserName@[SubDomain.]Domain.TopLevelDomain</i>),
        /// the entire message is rejected, even if the message contains other recipients that
        /// are valid.
        /// </para>
        ///  </li> </ul> <important> 
        /// <para>
        /// If your call to the <code>SendTemplatedEmail</code> operation includes all of the
        /// required parameters, Amazon SES accepts it and returns a Message ID. However, if Amazon
        /// SES can't render the email because the template contains errors, it doesn't send the
        /// email. Additionally, because it already accepted the message, Amazon SES doesn't return
        /// a message stating that it was unable to send the email.
        /// </para>
        ///  
        /// <para>
        /// For these reasons, we highly recommend that you set up Amazon SES to send you notifications
        /// when Rendering Failure events occur. For more information, see <a href="https://docs.aws.amazon.com/ses/latest/dg/send-personalized-email-api.html">Sending
        /// Personalized Email Using the Amazon SES API</a> in the <i>Amazon Simple Email Service
        /// Developer Guide</i>.
        /// </para>
        ///  </important>
        /// </summary>
        /// <param name="request">Container for the necessary parameters to execute the SendTemplatedEmail service method.</param>
        /// <param name="cancellationToken">
        ///     A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        /// 
        /// <returns>The response from the SendTemplatedEmail service method, as returned by SimpleEmailService.</returns>
        /// <exception cref="Amazon.SimpleEmail.Model.AccountSendingPausedException">
        /// Indicates that email sending is disabled for your entire Amazon SES account.
        /// 
        ///  
        /// <para>
        /// You can enable or disable email sending for your Amazon SES account using <a>UpdateAccountSendingEnabled</a>.
        /// </para>
        /// </exception>
        /// <exception cref="Amazon.SimpleEmail.Model.ConfigurationSetDoesNotExistException">
        /// Indicates that the configuration set does not exist.
        /// </exception>
        /// <exception cref="Amazon.SimpleEmail.Model.ConfigurationSetSendingPausedException">
        /// Indicates that email sending is disabled for the configuration set.
        /// 
        ///  
        /// <para>
        /// You can enable or disable email sending for a configuration set using <a>UpdateConfigurationSetSendingEnabled</a>.
        /// </para>
        /// </exception>
        /// <exception cref="Amazon.SimpleEmail.Model.MailFromDomainNotVerifiedException">
        /// Indicates that the message could not be sent because Amazon SES could not read the
        /// MX record required to use the specified MAIL FROM domain. For information about editing
        /// the custom MAIL FROM domain settings for an identity, see the <a href="https://docs.aws.amazon.com/ses/latest/DeveloperGuide/mail-from-edit.html">Amazon
        /// SES Developer Guide</a>.
        /// </exception>
        /// <exception cref="Amazon.SimpleEmail.Model.MessageRejectedException">
        /// Indicates that the action failed, and the message could not be sent. Check the error
        /// stack for more information about what caused the error.
        /// </exception>
        /// <exception cref="Amazon.SimpleEmail.Model.TemplateDoesNotExistException">
        /// Indicates that the Template object you specified does not exist in your Amazon SES
        /// account.
        /// </exception>
        /// <seealso href="http://docs.aws.amazon.com/goto/WebAPI/email-2010-12-01/SendTemplatedEmail">REST API Reference for SendTemplatedEmail Operation</seealso>
        Task<SendTemplatedEmailResponse> SendTemplatedEmailAsync(SendTemplatedEmailRequest request, System.Threading.CancellationToken cancellationToken);
    }
}