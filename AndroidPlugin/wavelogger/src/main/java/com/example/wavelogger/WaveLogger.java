package com.example.wavelogger;

import android.util.Log;

public class WaveLogger
{
    private static final WaveLogger instance = new WaveLogger();
    private static final String LOGTAG = "LOG - > ";

    private String allSavedLogs = "";

    public WaveLogger()
    {
        Log.i(LOGTAG, "log manager started");
        this.allSavedLogs = "Started plugin";
    }

    public static WaveLogger GetInstance()
    {
        return instance;
    }

    public void SendLog(String msj)
    {
        Log.i(LOGTAG, "Send LOG: " + msj);
        this.allSavedLogs += "\n" + LOGTAG + msj;
    }

    public String GetLogs()
    {
        Log.i(LOGTAG, "GET ALL: " + this.allSavedLogs);
        return this.allSavedLogs;
    }

    public void ClearLogs()
    {
        this.allSavedLogs = "";
    }
}
