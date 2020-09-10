using System;
using System.Collections.Generic;
using System.Text;

namespace AddinEngine.Abstract
{
    public interface IConfigurationResolver 
    {  
        void SetUp(IConfigurationRegister configurationRegister);  
    } 
}
