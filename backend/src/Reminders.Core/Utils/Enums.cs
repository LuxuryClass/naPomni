namespace Reminders.Core.Utils;

public enum RepeatUnit
{
    Day,
    Week,
    Month,
    Year
}

public enum DeliveryStatus
{
    Pending,
    Sending,
    Delivered,
    Failed
}

public enum ChannelType
{
    Telegram
}

public enum MemberRole
{
    Member,
    Admin
}
