using System;
using System.Collections.Generic;
using System.Text;

namespace ProjeHavuzu.Data.Repository.Abstract
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; set; }


    }
}
