using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Notifications.Android;
using UnityEngine;
#if UNITY_IOS
using Unity.Notifications.iOS;
#endif

public class MobileNotificationManager
{
    public void SendNotification(string title, string text, int hours)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidNotification(title, text, hours);
        }
        else if(Application.platform == RuntimePlatform.IPhonePlayer)
        {
            IOSNotification(title, text, hours);
        }
    }

    private static void AndroidNotification(string title, string text, int hours)
    {
        AndroidNotificationCenter.CancelAllDisplayedNotifications();

        var channel = new AndroidNotificationChannel()
        {
            Id = "channel_id",
            Name = "Notification Channel",
            Importance = Importance.Default,
            Description = "Reminder notifications",
            EnableVibration = true,
            LockScreenVisibility = LockScreenVisibility.Public
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);

        var notification = new AndroidNotification();
        notification.Title = title;
        notification.Text = text;
        notification.FireTime = System.DateTime.Now.AddSeconds(hours);
        notification.LargeIcon = "main_icon_large";

        //send notification
        var id = AndroidNotificationCenter.SendNotification(notification, "channel_id");

        //if already scheduled, cansel it and reschedule
        if (AndroidNotificationCenter.CheckScheduledNotificationStatus(id) == NotificationStatus.Scheduled)
        {
            AndroidNotificationCenter.CancelNotification(id);
            AndroidNotificationCenter.SendNotification(notification, "channel_id");
        }
    }

    void IOSNotification(string title, string text, int hours)
    {
#if UNITY_IOS
        var timeTrigger = new iOSNotificationTimeIntervalTrigger()
        {
            TimeInterval = new TimeSpan(hours, 0, 0),
            Repeats = false
        };

        var notification = new iOSNotification()
        {
            // You can specify a custom identifier which can be used to manage the notification later.
            // If you don't provide one, a unique string will be generated automatically.
            Identifier = "_notification_01",
            Title = title,
            Body = text,
            Subtitle = "Complete quests and receive rewards!",
            ShowInForeground = true,
            ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
            CategoryIdentifier = "category_a",
            ThreadIdentifier = "thread1",
            Trigger = timeTrigger,
        };

        iOSNotificationCenter.ScheduleNotification(notification);

        iOSNotificationCenter.RemoveScheduledNotification("_notification_01");
        iOSNotificationCenter.RemoveDeliveredNotification("_notification_01");
#endif
    }
}