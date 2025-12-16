using ProjectHavuzu.Business.Services.Abstracts;
using ProjeHavuzu.Data.Entites;
using ProjeHavuzu.Data.Repository.Abstract;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectHavuzu.Business.Services.Concretes
{
    public class CategoryManagerService : ManagerService<Category>, ICategoryManagerService
    {
        public CategoryManagerService(IRepository<Category> repository ) : base(repository)
        {
        }
    }
}
