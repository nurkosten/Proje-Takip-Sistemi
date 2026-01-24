using Microsoft.AspNetCore.Http;
using ProjeHavuzu.Data.Repository.Abstract;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace ProjeHavuzu.Data.Repository.Concrete
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? UserId
        {
            get
            {
                var userIdClaim = _httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst(ClaimTypes.NameIdentifier);

                return userIdClaim != null
                    ? Guid.Parse(userIdClaim.Value)
                    : null;
            }
            set { }
        }

        
    }
}
