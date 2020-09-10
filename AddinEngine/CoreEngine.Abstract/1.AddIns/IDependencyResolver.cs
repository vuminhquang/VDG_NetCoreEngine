using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace AddinEngine.Abstract
{
    public interface IDependencyResolver 
    {  
        void SetUp(IDependencyRegister dependencyRegister, IConfiguration configuration);  
    } 
}
