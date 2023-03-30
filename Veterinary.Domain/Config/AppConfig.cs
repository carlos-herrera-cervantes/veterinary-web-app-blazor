using System;

namespace Veterinary.Domain.Config;

public static class AppConfig
{
    public static readonly string GatewayHost = Environment.GetEnvironmentVariable("GATEWAY_HOST");
}
