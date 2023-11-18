using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using extractor.files.serialiser;

namespace extractor.files.serialiser.provider
{
    internal class SerialiserProvider<T> : ISerialiserProvider<T>
    {
        public ISerialiser<T> GetSerialiser(Uri path)
        {
            return new JsonSerialiser<T>(path);
        }
    }
}
