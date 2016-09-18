using Snowflake.Configuration;
using Snowflake.Configuration.Attributes;
using Snowflake.Plugin.EmulatorAdapter.RetroArch.Selections;

//autogenerated using generate_retroarch.py
namespace Snowflake.Plugin.EmulatorAdapter.RetroArch.Configuration.Internal
{
    /// <summary>
    /// The menu is completely disabled when running through Snowflake.
    /// We can not disable it completely (menu driver null), but we can hide it by disabling the menu toggle.
    /// 
    /// Frankly the retro arch GUI is an abomination, and any inconvienience delivered by removing it is offset
    /// by the simplicity of only exposing core settings using "Simple".
    /// </summary>
    public class MenuConfiguration : ConfigurationSection
    {
       
        [ConfigurationOption("dpi_override_enable", DisplayName = "Dpi Override Enable", Private = true)]
        public bool DpiOverrideEnable { get; set; } = true;
        //todo check max
        [ConfigurationOption("dpi_override_value", DisplayName = "Dpi Override Value", Private = true)]
        public int DpiOverrideValue { get; set; } = 200;

        [ConfigurationOption("custom_bgm_enable", DisplayName = "Custom Bgm Enable", Private = true)]
        public bool CustomBgmEnable { get; set; } = false;
        [ConfigurationOption("history_list_enable", DisplayName = "History List Enable", Private = true)]
        public bool HistoryListEnable { get; set; } = true;
        [ConfigurationOption("content_history_size", DisplayName = "Content History Size", Private = true)]
        public int ContentHistorySize { get; set; } = 100;
        [ConfigurationOption("back_as_menu_toggle_enable", DisplayName = "Back As Menu Toggle Enable", Private = true)]
        public bool BackAsMenuToggleEnable { get; set; } = true;
        [ConfigurationOption("menu_cancel_btn", DisplayName = "Menu Cancel Btn", Private = true)]
        public int MenuCancelBtn { get; set; } = 0;
        [ConfigurationOption("menu_core_enable", DisplayName = "Menu Core Enable", Private = true)]
        public bool MenuCoreEnable { get; set; } = false;
        [ConfigurationOption("menu_default_btn", DisplayName = "Menu Default Btn", Private = true)]
        public int MenuDefaultBtn { get; set; } = 3;

        [ConfigurationOption("menu_driver", DisplayName = "Menu Driver", Private = true)]
        public MenuDriver MenuDriver { get; set; } = MenuDriver.RGUI;

        [ConfigurationOption("menu_dynamic_wallpaper_enable", DisplayName = "Menu Dynamic Wallpaper Enable", Private = true)]
        public bool MenuDynamicWallpaperEnable { get; set; } = false;
        [ConfigurationOption("menu_entry_hover_color", DisplayName = "Menu Entry Hover Color", Private = true)]
        public string MenuEntryHoverColor { get; set; } = "ff64ff64";
        [ConfigurationOption("menu_entry_normal_color", DisplayName = "Menu Entry Normal Color", Private = true)]
        public string MenuEntryNormalColor { get; set; } = "ffffffff";
        [ConfigurationOption("menu_info_btn", DisplayName = "Menu Info Btn", Private = true)]
        public int MenuInfoBtn { get; set; } = 2;
        [ConfigurationOption("menu_linear_filter", DisplayName = "Menu Linear Filter", Private = true)]
        public bool MenuLinearFilter { get; set; } = true;
        [ConfigurationOption("menu_mouse_enable", DisplayName = "Menu Mouse Enable", Private = true)]
        public bool MenuMouseEnable { get; set; } = false;
        [ConfigurationOption("menu_navigation_browser_filter_supported_extensions_enable", DisplayName = "Menu Navigation Browser Filter Supported Extensions Enable", Private = true)]
        public bool MenuNavigationBrowserFilterSupportedExtensionsEnable { get; set; } = true;
        [ConfigurationOption("menu_navigation_wraparound_enable", DisplayName = "Menu Navigation Wraparound Enable", Private = true)]
        public bool MenuNavigationWraparoundEnable { get; set; } = true;
        [ConfigurationOption("menu_ok_btn", DisplayName = "Menu Ok Btn", Private = true)]
        public int MenuOkBtn { get; set; } = 8;
        [ConfigurationOption("menu_pause_libretro", DisplayName = "Menu Pause Libretro", Private = true)]
        public bool MenuPauseLibretro { get; set; } = true;
        [ConfigurationOption("menu_pointer_enable", DisplayName = "Menu Pointer Enable", Private = true)]
        public bool MenuPointerEnable { get; set; } = false;
        [ConfigurationOption("menu_scroll_down_btn", DisplayName = "Menu Scroll Down Btn", Private = true)]
        public int MenuScrollDownBtn { get; set; } = 11;
        [ConfigurationOption("menu_scroll_up_btn", DisplayName = "Menu Scroll Up Btn", Private = true)]
        public int MenuScrollUpBtn { get; set; } = 10;
        [ConfigurationOption("menu_search_btn", DisplayName = "Menu Search Btn", Private = true)]
        public int MenuSearchBtn { get; set; } = 9;
        [ConfigurationOption("menu_show_advanced_settings", DisplayName = "Menu Show Advanced Settings", Private = true)]
        public bool MenuShowAdvancedSettings { get; set; } = true;
        [ConfigurationOption("menu_throttle_framerate", DisplayName = "Menu Throttle Framerate", Private = true)]
        public bool MenuThrottleFramerate { get; set; } = true;
        [ConfigurationOption("menu_thumbnails", DisplayName = "Menu Thumbnails", Private = true)]
        public int MenuThumbnails { get; set; } = 0;
        [ConfigurationOption("menu_timedate_enable", DisplayName = "Menu Timedate Enable", Private = true)]
        public bool MenuTimedateEnable { get; set; } = true;
        [ConfigurationOption("menu_title_color", DisplayName = "Menu Title Color", Private = true)]
        public string MenuTitleColor { get; set; } = "ff64ff64";
        [ConfigurationOption("menu_wallpaper", DisplayName = "Menu Wallpaper", Private = true)]
        public string MenuWallpaper { get; set; } = "";
        [ConfigurationOption("playlist_cores", DisplayName = "Playlist Cores", Private = true)]
        public string PlaylistCores { get; set; } = "";
        [ConfigurationOption("playlist_names", DisplayName = "Playlist Names", Private = true)]
        public string PlaylistNames { get; set; } = "";

        [ConfigurationOption("rgui_show_start_screen", DisplayName = "Rgui Show Start Screen", Private = true)]
        public bool RguiShowStartScreen { get; set; } = false;

        [ConfigurationOption("sort_savefiles_enable", DisplayName = "Enable savefile sorting", Private = true)]
        public bool SortSavefilesEnable { get; set; } = false;

        [ConfigurationOption("sort_savestates_enable", DisplayName = "Sort Savestates Enable", Private = true)]
        public bool SortSavestatesEnable { get; set; } = false;
        [ConfigurationOption("suspend_screensaver_enable", DisplayName = "Suspend Screensaver Enable", Private = true)]
        public bool SuspendScreensaverEnable { get; set; } = true;

        public MenuConfiguration() : base("menu", "Menu Options")
        {

        }

    }
}