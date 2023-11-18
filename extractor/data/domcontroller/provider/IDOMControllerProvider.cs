using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace extractor.data.domcontroller.provider
{
    internal interface IDOMControllerProvider
    {
        IDOMController GetDOMController(string html);
    }
}
