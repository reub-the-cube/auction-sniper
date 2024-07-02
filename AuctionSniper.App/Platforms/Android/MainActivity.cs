﻿using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;

namespace AuctionSniper.App
{
    [Register("com.madetechbookclub.auctionsniper.MainActivity")]
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
    }
}
