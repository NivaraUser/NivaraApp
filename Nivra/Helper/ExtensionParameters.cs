using Autofac.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nivara.Web.Helper
{
    public static class ExtensionParameters
    {
        public static List<ResolvedParameter> ResolvedParameters()
        {
            List<ResolvedParameter> parameters = new List<ResolvedParameter>();
           // parameters.Add(new ResolvedParameter(
           // (pi, ctx) => pi.ParameterType == typeof(string) && pi.Name == SessionConstants.UserId,
           // (pi, ctx) => SessionHelper.UserId));
           // parameters.Add(new ResolvedParameter(
           //(pi, ctx) => pi.ParameterType == typeof(int) && pi.Name == SessionConstants.BusinessId,
           //(pi, ctx) => SessionHelper.BusinessId));
           // parameters.Add(new ResolvedParameter(
           //(pi, ctx) => pi.ParameterType == typeof(int) && pi.Name == SessionConstants.BusinessUserId,
           //(pi, ctx) => SessionHelper.BusinessUserId));
           // parameters.Add(new ResolvedParameter(
           //(pi, ctx) => pi.ParameterType == typeof(int) && pi.Name == SessionConstants.BusinessLocationId,
           //(pi, ctx) => SessionHelper.BusinessLocationId));
            return parameters;
        }
    }
}
