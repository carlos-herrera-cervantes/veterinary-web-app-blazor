using System;

namespace Veterinary.Domain.Config;

public static class AvatarConfig
{
    public static readonly string NoProfilePicture = Environment.GetEnvironmentVariable("NO_PROFILE_PICTURE");
}
