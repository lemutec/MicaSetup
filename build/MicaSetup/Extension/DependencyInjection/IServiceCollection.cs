using System.Collections;
using System.Collections.Generic;

namespace MicaSetup.Extension.DependencyInjection;

public interface IServiceCollection : IList<ServiceDescriptor>, IEnumerable<ServiceDescriptor>, IList, IEnumerable
{
}
