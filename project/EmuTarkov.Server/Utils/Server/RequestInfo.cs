using System;

public class RequestInfo
{
    public string Session;
    public Uri Uri;
    public string Body;

    public RequestInfo(Uri uri)
    {
        Session = null;
        Uri = uri;
        Body = null;
    }
}