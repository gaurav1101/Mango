﻿using Microsoft.AspNetCore.Authentication;
using System.Net.Http;

namespace Mango.Services.OrderAPI.Utility
{
    public class BackendApiAuthenticationHttpClientHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public BackendApiAuthenticationHttpClientHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor= httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
