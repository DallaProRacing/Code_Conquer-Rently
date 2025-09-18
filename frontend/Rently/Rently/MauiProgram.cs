#if ANDROID
using AndroidX.Core.View;
#endif
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;

namespace Rently
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder 
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            builder.ConfigureLifecycleEvents(events =>
            {
#if ANDROID
                events.AddAndroid(android => android.OnCreate((activity, _) =>
                {
                    var w = activity.Window;
                    w?.SetStatusBarColor(Android.Graphics.Color.Orange);
                    var ctl = WindowCompat.GetInsetsController(w, w.DecorView);
                    if (ctl is not null)
                        ctl.AppearanceLightStatusBars = true;
                }));
#endif
            });
            return builder.Build();
        }
    }
}
