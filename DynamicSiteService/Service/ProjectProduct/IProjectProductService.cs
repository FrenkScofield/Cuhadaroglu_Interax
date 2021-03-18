using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

using System;


public interface IProjectProductService : IGenericRepo<ProjectProduct>
{
    RModel<ProjectProduct> InsertOrUpdate(ProjectProduct model);

}

