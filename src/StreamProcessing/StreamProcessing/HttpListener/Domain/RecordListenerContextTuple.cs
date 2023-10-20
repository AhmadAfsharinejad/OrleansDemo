using System.Net;
using StreamProcessing.PluginCommon.Domain;

namespace StreamProcessing.HttpListener.Domain;

internal record struct RecordListenerContextTuple(HttpListenerContext HttpListenerContext, PluginRecord Record);
