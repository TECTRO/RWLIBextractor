using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace extractor.files.serialiser.provider
{
    internal interface ISerialiserProvider<T>
    {
        ISerialiser<T> GetSerialiser(Uri path);
    }
}
