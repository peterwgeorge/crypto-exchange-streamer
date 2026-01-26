using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Credentials.Types;
public interface IKey
{
    public string GetName();

    public string GetKey();
}
