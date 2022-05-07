using System.Threading;
using Microsoft.Win32.TaskScheduler;

namespace win10_dwm_tool
{
    public class TaskScheduler
    {
        public static void RunTask(string cmd, string param, string account)
        {
            string taskname = "win10_dwm_tool";
            // Get the service on the local machine
            using (TaskService ts = new TaskService())
            {
                if (ts.GetTask(taskname) != null)
                {
                    ts.RootFolder.DeleteTask(taskname, false);
                }
                TaskDefinition td = ts.NewTask();
                td.RegistrationInfo.Description = taskname + " helper task";
                td.Actions.Add(new ExecAction(cmd, param, null));
                Task task = ts.RootFolder.RegisterTaskDefinition(taskname, td, TaskCreation.CreateOrUpdate, account);
                task.Run();
                int cnt = 0;
                //Wait 1 second if not stopped
                while (task.State == TaskState.Running && cnt++ < 10) Thread.Sleep(100);
                task.Stop();
                ts.RootFolder.DeleteTask(taskname, false);
            }
        }
    }
}