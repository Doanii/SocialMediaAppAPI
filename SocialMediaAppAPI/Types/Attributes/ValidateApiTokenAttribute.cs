using System;

namespace SocialMediaAppAPI.Types.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ValidateApiTokenAttribute : Attribute
    {
    }
}
