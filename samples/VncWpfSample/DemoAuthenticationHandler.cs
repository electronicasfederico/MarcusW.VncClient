using MarcusW.VncClient;
using MarcusW.VncClient.Protocol.SecurityTypes;
using MarcusW.VncClient.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VncWpfSample
{
// Manejador de autenticación que retorna la contraseña
    class DemoAuthenticationHandler : IAuthenticationHandler
    {
        public async Task<TInput> ProvideAuthenticationInputAsync<TInput>(
           RfbConnection connection,
           ISecurityType securityType,
           IAuthenticationInputRequest<TInput> request)
           where TInput : class, IAuthenticationInput
        {
            if (typeof(TInput) == typeof(PasswordAuthenticationInput))
            {
                string password = "464564645"; // Reemplaza con la contraseña correcta
                return (TInput)(object)new PasswordAuthenticationInput(password);
            }
            throw new NotSupportedException("El tipo de autenticación solicitado no es soportado.");
        }
    }
}
