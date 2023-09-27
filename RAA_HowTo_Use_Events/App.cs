#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Versioning;
using System.Windows.Markup;

#endregion

namespace RAA_HowTo_Use_Events
{
    internal class App : IExternalApplication
    {
        public string logFilePath;
        public Result OnStartup(UIControlledApplication app)
        {
            // register the event handler on startup
            app.ControlledApplication.DocumentOpened += new EventHandler
                <DocumentOpenedEventArgs>(DocOpened);

            app.ViewActivated += new EventHandler
                <ViewActivatedEventArgs>(ViewChanged);

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication a)
        {
            // remove the event handler
            a.ControlledApplication.DocumentOpened -= DocOpened;
            a.ViewActivated -= ViewChanged;

            return Result.Succeeded;
        }

        public void DocOpened(object sender, DocumentOpenedEventArgs args)
        {
            //TaskDialog.Show("Test", "I just opened a document!");

            Document doc = args.Document;

            if (doc == null) return;

            //string filename = doc.PathName;
            //string user = doc.Application.Username;
            //string revitVersion = doc.Application.VersionName;
            //string date = DateTime.Now.ToString();

            //string output = $"{revitVersion} : {user} : {filename} : {date}";
            string output = "File opened";

            OutputToLog(doc, output);

            //TaskDialog.Show("Test", output);
        }

        public void ViewChanged(object sender, ViewActivatedEventArgs args)
        {
            Document doc = args.Document;

            if (doc == null) return;

            View curView = doc.ActiveView;
            string viewName = curView.Name;
            string viewType = curView.ViewType.ToString();

            string output = $"View opened: {viewName} - {viewType}";

            OutputToLog(doc, output);

            //TaskDialog.Show("Test", output);
        }

        private void OutputToLog(Document doc, string output)
        {
            string filename = doc.PathName;
            string user = doc.Application.Username;
            string revitVersion = doc.Application.VersionName;
            string date = DateTime.Now.ToString();

            string logEntry = $"{revitVersion},{user},{filename},{date},{output}";

            // set log filename and path
            if (string.IsNullOrEmpty(logFilePath))
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string fileName = $"_{revitVersion}_{user}_{DateTime.Now.ToString("yyyy-MM-dd")}_Log File.txt";
                logFilePath = path + "\\" + fileName;
            }

            WriteToTxtFile(logFilePath, logEntry);

        }
        private void WriteToTxtFile(string filePath, string fileContents)
        {
            using (StreamWriter writer = File.AppendText(filePath))
            {
                writer.WriteLine(fileContents);
            }
        }
    }
}
