using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary;

namespace Milieu.WebServer.HelperMethods
{
    public static class ModelStateHelperMethods
    {
        public static string GetAggregateErrors(ValueEnumerable values)
        {
            string aggregateErrors = default(string);
            foreach (var modelState in values)
            {
                foreach (var error in modelState.Errors)
                {
                    aggregateErrors += error.ErrorMessage + Environment.NewLine;
                }
            }
            return aggregateErrors;
        }
    }
}
