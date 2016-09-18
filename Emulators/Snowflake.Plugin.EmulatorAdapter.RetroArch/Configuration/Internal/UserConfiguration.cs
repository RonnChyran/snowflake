using Snowflake.Configuration;
using Snowflake.Configuration.Attributes;

//autogenerated using generate_retroarch.py
namespace Snowflake.Plugin.EmulatorAdapter.RetroArch.Configuration.Internal
{
    public class UserConfiguration : ConfigurationSection
    {
       //todo check max
       [ConfigurationOption("user_language", DisplayName = "User Language", Private = true)]
       public int UserLanguage { get; set; } = 0;

       public UserConfiguration() : base ("user", "User Options")
       {

       }
       
     }
}