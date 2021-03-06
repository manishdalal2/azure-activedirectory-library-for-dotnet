﻿// ------------------------------------------------------------------------------
// 
// Copyright (c) Microsoft Corporation.
// All rights reserved.
// 
// This code is licensed under the MIT License.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files(the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions :
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// 
// ------------------------------------------------------------------------------

using System.Globalization;
using Microsoft.Identity.Core;
using Microsoft.Identity.Core.Cache;
using Microsoft.Identity.Core.Helpers;
using Microsoft.Identity.Core.OAuth2;
using Microsoft.Identity.Json.Linq;

namespace Microsoft.Identity.Core.Cache
{
    internal class MsalIdTokenCacheItem : MsalCredentialCacheItemBase
    {
        internal MsalIdTokenCacheItem()
        {
            CredentialType = StorageJsonValues.CredentialTypeIdToken;
        }

        internal MsalIdTokenCacheItem(
            string environment,
            string clientId,
            MsalTokenResponse response,
            string tenantId)
            : this(
                environment,
                clientId,
                response.IdToken,
                response.ClientInfo,
                tenantId)
        {
        }

        internal MsalIdTokenCacheItem(
            string environment,
            string clientId,
            string secret,
            string rawClientInfo,
            string tenantId)
            : this()
        {
            Environment = environment;
            TenantId = tenantId;
            ClientId = clientId;
            Secret = secret;
            RawClientInfo = rawClientInfo;

            InitUserIdentifier();
        }

        internal string TenantId { get; set; }

        internal string Authority =>
            string.Format(CultureInfo.InvariantCulture, "https://{0}/{1}/", Environment, TenantId ?? "common");


        internal MsalIdTokenCacheKey GetKey()
        {
            return new MsalIdTokenCacheKey(Environment, TenantId, HomeAccountId, ClientId);
        }

        internal static MsalIdTokenCacheItem FromJsonString(string json)
        {
            return FromJObject(JObject.Parse(json));
        }

        internal static MsalIdTokenCacheItem FromJObject(JObject j)
        {
            var item = new MsalIdTokenCacheItem
            {
                TenantId = JsonUtils.ExtractExistingOrEmptyString(j, StorageJsonKeys.Realm),
            };

            item.PopulateFieldsFromJObject(j);

            return item;
        }

        internal override JObject ToJObject()
        {
            var json = base.ToJObject();
            json[StorageJsonKeys.Realm] = TenantId;
            return json;
        }

        internal string ToJsonString()
        {
            return ToJObject()
                .ToString();
        }
    }
}