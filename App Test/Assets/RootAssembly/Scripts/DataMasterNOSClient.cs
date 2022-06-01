using System;
public interface IDataMasterNOSClient 
{
    public int SendAppointmentNotifcations(DateTime StartTime, DateTime EndTime, int repeat, string titel);
    public int SendNewDeadlineNotifications(string titel, DateTime expireTime);

}
