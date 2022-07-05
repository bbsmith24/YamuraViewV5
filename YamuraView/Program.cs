using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace YamuraView
{
    // The class that handles the creation of the application windows
    class YamuraViewAppContext : ApplicationContext
    {

        public static List<Form> appForms = new List<Form>();
        public static List<string> appFormPositions = new List<string>();

        //private AppForm1 _form1;
        //private AppForm2 _form2;
        //private Form1 _form3;

        //private Rectangle _form1Position;
        //private Rectangle _form2Position;
        //private Rectangle _form3Position;

        private FileStream _userData;

        private YamuraViewAppContext()
        {
            // Handle the ApplicationExit event to know when the application is exiting.
            Application.ApplicationExit += new EventHandler(this.OnApplicationExit);

            try
            {
                // Create a file that the application will store user specific data in.
                _userData = new FileStream(Application.UserAppDataPath + "\\appdata.txt", FileMode.OpenOrCreate);
            }
            catch (IOException e)
            {
                // Inform the user that an error occurred.
                MessageBox.Show("An error occurred while attempting to show the application." +
                                "The error is:" + e.ToString());

                // Exit the current thread instead of showing the windows.
                ExitThread();
            }

            // Create both application forms and handle the Closed event
            // to know when both forms are closed.
//            appForms.Add(new AppForm1());
//            appForms[appForms.Count - 1].Closed += new EventHandler(OnFormClosed);
//            appForms[appForms.Count - 1].Closing += new CancelEventHandler(OnFormClosing);

//            appForms.Add(new AppForm2());
//            appForms[appForms.Count - 1].Closed += new EventHandler(OnFormClosed);
//            appForms[appForms.Count - 1].Closing += new CancelEventHandler(OnFormClosing);

            appForms.Add(new YamuraViewMain());
            appForms[appForms.Count - 1].Closed += new EventHandler(OnFormClosed);
            //appForms[appForms.Count - 1].Closing += new CancelEventHandler(OnFormClosing);

            //// Get the form positions based upon the user specific data.
            //if (ReadFormDataFromFile())
            //{
            //    // If the data was read from the file, set the form
            //    // positions manually.
            //    appForms[0].StartPosition = FormStartPosition.Manual;
            //    appForms[1].StartPosition = FormStartPosition.Manual;
            //    appForms[2].StartPosition = FormStartPosition.Manual;

            //    appForms[0].Bounds = appForms[0]._formPosition;// _form1Position;
            //    appForms[1].Bounds = appForms[1]._formPosition;
            //    appForms[2].Bounds = appForms[2]._formPosition;
            //}

            // Show both forms.
            for(int idx = 0; idx < appForms.Count; idx++)
            {
                appForms[idx].Show();
            }
            //appForms[1].Show();
            //appForms[2].Show();
        }

        private void OnApplicationExit(object sender, EventArgs e)
        {
            // When the application is exiting, write the application data to the
            // user file and close it.
            WriteFormDataToFile();

            try
            {
                // Ignore any errors that might occur while closing the file handle.
                _userData.Close();
            }
            catch { }
        }

        private void OnFormClosing(object sender, CancelEventArgs e)
        {
            // When a form is closing, remember the form position so it
            // can be saved in the user data file.
            //if (sender is AppForm1)
            //    appForms[0]._formPosition = ((Form)sender).Bounds;
            //else if (sender is AppForm2)
            //    appForms[1]._formPosition = ((Form)sender).Bounds;
            //else if (sender is Form1)
            //    appForms[2]._formPosition = ((Form)sender).Bounds;
        }

        private void OnFormClosed(object sender, EventArgs e)
        {
            // When a form is closed, decrement the count of open forms.

            // When the count gets to 0, exit the app by calling
            // ExitThread().
            //RectangleConverter rectConv = new RectangleConverter();
            //string formpos = rectConv.ConvertToString(appForms[appForms.IndexOf(sender as Form)]._formPosition);

            ////appFormPositions[appForms.IndexOf(sender as SizedForm)] = formpos;
            appForms.Remove(sender as Form);
            if (appForms.Count == 0)
            {
                ExitThread();
            }
        }

        private bool WriteFormDataToFile()
        {
            //// Write the form positions to the file.
            //UTF8Encoding encoding = new UTF8Encoding();

            //RectangleConverter rectConv = new RectangleConverter();
            //string form1pos = rectConv.ConvertToString(appForms[0]._formPosition);
            //string form2pos = rectConv.ConvertToString(appForms[1]._formPosition);
            //string form3pos = rectConv.ConvertToString(appForms[2]._formPosition);

            //byte[] dataToWrite = encoding.GetBytes("~" + form1pos + "~" + form2pos);

            //try
            //{
            //    // Set the write position to the start of the file and write
            //    _userData.Seek(0, SeekOrigin.Begin);
            //    _userData.Write(dataToWrite, 0, dataToWrite.Length);
            //    _userData.Flush();

            //    _userData.SetLength(dataToWrite.Length);
            //    return true;
            //}
            //catch
            //{
            //    // An error occurred while attempting to write, return false.
            //    return false;
            //}
            return true;
        }

        private bool ReadFormDataFromFile()
        {
            //// Read the form positions from the file.
            //UTF8Encoding encoding = new UTF8Encoding();
            //string data;

            //if (_userData.Length != 0)
            //{
            //    byte[] dataToRead = new byte[_userData.Length];

            //    try
            //    {
            //        // Set the read position to the start of the file and read.
            //        _userData.Seek(0, SeekOrigin.Begin);
            //        _userData.Read(dataToRead, 0, dataToRead.Length);
            //    }
            //    catch (IOException e)
            //    {
            //        string errorInfo = e.ToString();
            //        // An error occurred while attempt to read, return false.
            //        return false;
            //    }

            //    // Parse out the data to get the window rectangles
            //    data = encoding.GetString(dataToRead);

            //    try
            //    {
            //        // Convert the string data to rectangles
            //        RectangleConverter rectConv = new RectangleConverter();
            //        string form1pos = data.Substring(1, data.IndexOf("~", 1) - 1);

            //        appForms[0]._formPosition = (Rectangle)rectConv.ConvertFromString(form1pos);

            //        string form2pos = data.Substring(data.IndexOf("~", 1) + 1);
            //        appForms[1]._formPosition = (Rectangle)rectConv.ConvertFromString(form2pos);

            //        string form3pos = data.Substring(data.IndexOf("~", 1) + 1);
            //        appForms[2]._formPosition = (Rectangle)rectConv.ConvertFromString(form3pos);

            //        return true;
            //    }
            //    catch
            //    {
            //        // Error occurred while attempting to convert the rectangle data.
            //        // Return false to use default values.
            //        return false;
            //    }
            //}
            //else
            //{
            //    // No data in the file, return false to use default values.
            //    return false;
            //}
            return false;
        }

        [STAThread]
        static void Main(string[] args)
        {

            // Create the MyApplicationContext, that derives from ApplicationContext,
            // that manages when the application should exit.

            YamuraViewAppContext context = new YamuraViewAppContext();

            // Run the application with the specific context. It will exit when
            // all forms are closed.
            Application.Run(context);
        }
    }
}