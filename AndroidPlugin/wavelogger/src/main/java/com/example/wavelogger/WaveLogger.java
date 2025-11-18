package com.example.wavelogger;

import android.app.Activity;
import android.app.AlertDialog;
import android.content.Context;
import android.content.DialogInterface;

import java.io.File;
import java.io.FileWriter;
import java.io.IOException;
import java.util.Scanner;

public class WaveLogger
{

    private static final String FileName = "WaveLogs.txt";

    private Context context;
    private Activity activity;

    private long startTime;

    public WaveLogger()
    {

    }

    public void SendLogs(String log)
    {
       if(context == null)
       {
           android.util.Log.e("WaveLogger", "Context is null - You can not save the log");
           return;
       }

       try
       {
           File file = new File(context.getFilesDir(), FileName);
           FileWriter writer = new FileWriter(file, true);
           writer.append(log).append("\n");
           writer.close();
       }
       catch (IOException e)
       {
           e.printStackTrace();
       }
    }

    public String GetLogs()
    {
        StringBuilder logs = new StringBuilder();

        try
        {
            File file = new File(context.getFilesDir(), FileName);

            if(!file.exists())
                return "No file with " + FileName + " exists.";

            Scanner scanner = new Scanner(file);

            while (scanner.hasNextLine())
                logs.append(scanner.nextLine()).append("\n");

            scanner.close();
        }
        catch (IOException e)
        {
            e.printStackTrace();
        }

        return logs.toString();
    }

    public void ClarLogs()
    {
        if(activity == null || context == null)
        {
            android.util.Log.e("WaveLogger", "Activity/Context are null");
            return;
        }
        activity.runOnUiThread
                (
                        new Runnable()
                        {
                            @Override
                            public void run()
                            {
                                AlertDialog.Builder builder = new AlertDialog.Builder(activity);

                                builder.setTitle("Confirm Delete").setMessage("Are you sure?").setPositiveButton
                                        (
                                        "Yes", new DialogInterface.OnClickListener()
                                        {
                                            @Override
                                            public void onClick(DialogInterface dialog, int which)
                                            {
                                             File file = new File(context.getFilesDir(), FileName);
                                             if(file.exists())
                                             {
                                                 boolean delete = file.delete();
                                                 android.util.Log.d("WaveLogger", "Logs deleted: " + delete);
                                             }
                                             else
                                                 android.util.Log.d("WaveLogger", "Logs did not exist.");

                                             dialog.dismiss();
                                            }
                                        }

                                ).setNegativeButton
                                        (
                                                "No", new DialogInterface.OnClickListener()
                                                {
                                                    @Override
                                                    public void onClick(DialogInterface dialog, int which)
                                                    {
                                                        dialog.dismiss();
                                                    }
                                                }
                                        ).setCancelable(false);

                            }
                        }
                );
    }


}
