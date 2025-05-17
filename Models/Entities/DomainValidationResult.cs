using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class DomainValidationResult 
    {
        public bool IsValid => Errors.Count == 0;
        public List<string> Errors { get;} = new List<string>();
    }
}
